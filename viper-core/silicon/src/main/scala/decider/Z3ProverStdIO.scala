/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

package viper.silicon.decider

import java.io.{PrintWriter, BufferedWriter, InputStreamReader, BufferedReader, OutputStreamWriter}
import java.nio.file.{Path, Paths}
import org.slf4s.Logging
import org.apache.commons.io.FileUtils
import viper.silicon.{Config, Map, toMap}
import viper.silicon.common.config.Version
import viper.silicon.interfaces.decider.{Prover, Sat, Unsat, Unknown}
import viper.silicon.reporting.{Bookkeeper, Z3InteractionFailed}
import viper.silicon.state.IdentifierFactory
import viper.silicon.state.terms._

/* TODO: Pass a logger, don't open an own file to log to. */
class Z3ProverStdIO(config: Config,
                    bookkeeper: Bookkeeper,
                    identifierFactory: IdentifierFactory)
    extends Prover
       with Logging {

  private var pushPopScopeDepth = 0
  private var lastTimeout: Int = -1
//  private var isLoggingCommentsEnabled: Boolean = true
  private var logFile: PrintWriter = _
  private var z3: Process = _
  private var input: BufferedReader = _
  private var output: PrintWriter = _
  /* private */ var z3Path: Path = _
  private var logPath: Path = _

  /* private */ val termConverter = new TermToSMTLib2Converter(bookkeeper)
  import termConverter._

  def z3Version(): Version = {
    val versionPattern = """\(?\s*:version\s+"(.*?)"\)?""".r
    var line = ""

    writeLine("(get-info :version)")

    line = input.readLine()
    logComment(line)

    line match {
      case versionPattern(v) => Version(v)
      case _ => throw new Z3InteractionFailed(s"Unexpected output of Z3 while getting version: $line")
    }
  }

  def start() {
    pushPopScopeDepth = 0
    lastTimeout = -1
    logPath = config.z3LogFile
    logFile = viper.silver.utility.Common.PrintWriter(logPath.toFile)
    z3Path = Paths.get(config.z3Exe)
    termConverter.start()
    z3 = createZ3Instance()
    input = new BufferedReader(new InputStreamReader(z3.getInputStream))
    output = new PrintWriter(new BufferedWriter(new OutputStreamWriter(z3.getOutputStream)), true)
  }

  /* Note: This is just a hack to get the input file name to the prover */
  def proverRunStarts() {
    logComment(s"Input file is ${config.inputFile.getOrElse("<unknown>")}")
  }

  private def createZ3Instance() = {
    log.info(s"Starting Z3 at $z3Path")

    val userProvidedZ3Args: Array[String] = config.z3Args.get match {
      case None =>
        Array()

      case Some(args) =>
        log.info(s"Additional command-line arguments are $args")
        args.split(' ').map(_.trim)
    }

    val builder = new ProcessBuilder(z3Path.toFile.getPath +: "-smt2" +: "-in" +: userProvidedZ3Args :_*)
    builder.redirectErrorStream(true)

    val process = builder.start()

    Runtime.getRuntime.addShutdownHook(new Thread {
      override def run() {
        process.destroy()
      }
    })

    process
  }

  def reset() {
    stop()
    start()
  }

  def stop() {
    this.synchronized {
      logFile.flush()
      output.flush()

      logFile.close()
      input.close()
      output.close()

      z3.destroy()
//      z3.waitFor() /* Makes the current thread wait until the process has been shut down */

      termConverter.stop()

      val currentLogPath = config.z3LogFile
      if (logPath != currentLogPath) {
        /* This is a hack to make it possible to name the SMTLIB logfile after
         * the input file that was verified. Currently, Silicon starts Z3 before
         * the input file name is known, which is partially due to our crappy
         * and complicated way of how command-line arguments are parsed and
         * how Silver programs are passed to verifiers.
         */

        FileUtils.moveFile(logPath.toFile, currentLogPath.toFile)
      }
    }
  }

  def push(n: Int = 1) {
    pushPopScopeDepth += n
    val cmd = (if (n == 1) "(push)" else "(push " + n + ")") + " ; " + pushPopScopeDepth
    writeLine(cmd)
    readSuccess()
  }

  def pop(n: Int = 1) {
    val cmd = (if (n == 1) "(pop)" else "(pop " + n + ")") + " ; " + pushPopScopeDepth
    pushPopScopeDepth -= n
    writeLine(cmd)
    readSuccess()
  }

  def emit(content: String) {
    writeLine(content)
    readSuccess()
  }

  private val quantificationLogger = bookkeeper.logfiles("quantification-problems")

  def assume(term: Term) = {
    /* Detect certain simple problems with quantifiers.
     * Note that the current checks don't take in account whether or not a
     * quantification occurs in positive or negative position.
     */
    term.deepCollect{case q: Quantification => q}.foreach(q => {
      val problems = viper.silicon.state.utils.detectQuantificationProblems(q)

      if (problems.nonEmpty) {
        quantificationLogger.println(s"\n\n${q.toString(true)}")
        quantificationLogger.println("Problems:")
        problems.foreach(p => quantificationLogger.println(s"  $p"))
      }
    })

    assume(convert(term))
  }

  def assume(term: String) {
    bookkeeper.assumptionCounter += 1

    writeLine("(assert " + term + ")")
    readSuccess()
  }

  def assert(goal: Term, timeout: Option[Int] = None) = assert(convert(goal), timeout)

  def assert(goal: String, timeout: Option[Int]) = {
    bookkeeper.assertionCounter += 1

    setTimeout(timeout)

    val (result, duration) = config.assertionMode() match {
      case Config.AssertionMode.SoftConstraints => assertUsingSoftConstraints(goal)
      case Config.AssertionMode.PushPop => assertUsingPushPop(goal)
    }

    logComment(s"${viper.silicon.common.format.formatMillisReadably(duration)}")
    logComment("(get-info :all-statistics)")

    result
  }

  private def assertUsingPushPop(goal: String): (Boolean, Long) = {
    push()

    writeLine("(assert (not " + goal + "))")
    readSuccess()

    val startTime = System.currentTimeMillis()
    writeLine("(check-sat)")
    val result = readUnsat()
    val endTime = System.currentTimeMillis()

    pop()

    (result, endTime - startTime)
  }

  private def assertUsingSoftConstraints(goal: String): (Boolean, Long) = {
    val guard = fresh("grd", Nil, sorts.Bool)

    writeLine(s"(assert (implies $guard (not $goal)))")
    readSuccess()

    val startTime = System.currentTimeMillis()
    writeLine(s"(check-sat $guard)")
    val result = readUnsat()
    val endTime = System.currentTimeMillis()

    (result, endTime - startTime)
  }

  def check(timeout: Option[Int] = None) = {
    setTimeout(timeout)

    writeLine("(check-sat)")

    readLine() match {
      case "sat" => Sat
      case "unsat" => Unsat
      case "unknown" => Unknown
    }
  }

  private def setTimeout(timeout: Option[Int]) {
    val effectiveTimeout = timeout.getOrElse(config.z3Timeout)

    /* [2015-07-27 Malte] Setting the timeout unnecessarily often seems to
     * worsen performance, if only a bit. For the current test suite of
     * 199 Silver files, the total verification time increased from 60s
     * to 70s if 'set-option' is emitted every time.
     */
    if (lastTimeout != effectiveTimeout) {
      lastTimeout = effectiveTimeout

      writeLine(s"(set-option :timeout $effectiveTimeout)")
      readSuccess()
    }
  }

  def statistics(): Map[String, String]= {
    var repeat = true
    var line = ""
    var stats = scala.collection.immutable.SortedMap[String, String]()
    val entryPattern = """\(?\s*:([A-za-z\-]+)\s+((?:\d+\.)?\d+)\)?""".r

    writeLine("(get-info :all-statistics)")

    do {
      line = input.readLine()
      logComment(line)

      /* Check that the first line starts with "(:". */
      if (line.isEmpty && !line.startsWith("(:"))
        throw new Z3InteractionFailed(s"Unexpected output of Z3 while reading statistics: $line")

      line match {
        case entryPattern(entryName, entryNumber) =>
          stats = stats + (entryName -> entryNumber)
        case _ =>
      }

      repeat = !line.endsWith(")")
    } while (repeat)

    toMap(stats)
  }

  def logComment(str: String) = {
    val sanitisedStr =
      str.replaceAll("\r", "")
         .replaceAll("\n", "\n; ")

    logToFile("; " + sanitisedStr)
  }

  def fresh(name: String, argSorts: Seq[Sort], resultSort: Sort) = {
    val id = identifierFactory.fresh(name)
    val fun = Fun(id, argSorts, resultSort)
    val decl = FunctionDecl(fun)

    emit(convert(decl))

    fun
  }

  def declare(decl: Decl) {
    val str = convert(decl)
    emit(str)
  }

  def resetAssertionCounter() { bookkeeper.assertionCounter = 0 }
  def resetAssumptionCounter() { bookkeeper.assumptionCounter = 0 }

  def resetCounters() {
    resetAssertionCounter()
    resetAssumptionCounter()
  }

  /* TODO: Handle multi-line output, e.g. multiple error messages. */

  private def readSuccess() {
    val answer = readLine()

    if (answer != "success")
      throw new Z3InteractionFailed(s"Unexpected output of Z3. Expected 'success' but found: $answer")
  }

  private def readUnsat(): Boolean = readLine() match {
    case "unsat" => true
    case "sat" => false
    case "unknown" => false

    case result =>
      throw new Z3InteractionFailed(s"Unexpected output of Z3 while trying to refute an assertion: $result")
  }

  private def readLine(): String = {
    var repeat = true
    var result = ""

    while (repeat) {
      result = input.readLine()
      if (result.toLowerCase != "success") logComment(result)

      val warning = result.startsWith("WARNING")
      if (warning) log.info(s"Z3: $result")

      repeat = warning
    }

    result
  }

  private def logToFile(str: String) {
    logFile.println(str)
  }

  private def writeLine(out: String) = {
    logToFile(out)
    output.println(out)
  }
}
