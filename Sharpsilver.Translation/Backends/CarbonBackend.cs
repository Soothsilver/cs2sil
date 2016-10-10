﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sharpsilver.Translation.Trees.Silver;

namespace Sharpsilver.Translation.BackendInterface
{
    /// <summary>
    /// Connects to the "carbon" backend verifier.
    /// </summary>
    public class CarbonBackend : IBackend
    {       
        public VerificationResult Verify(Silvernode silvernode)
        {
            string silvercode = silvernode.ToString();
            string filename = Path.GetTempFileName();
            File.WriteAllText(filename, silvercode);
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                var enviromentPath = Environment.GetEnvironmentVariable("PATH");
                Debug.Assert(enviromentPath != null, "enviromentPath != null");
                var paths = enviromentPath.Split(';');
                var exePath = paths
                    .Select(x => Path.Combine(x, "carbon.bat"))
                    .FirstOrDefault(File.Exists) ?? "carbon.bat";
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = "\"" +  filename + "\"";
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                VerificationResult r = new VerificationResult();
                r.OriginalOutput = output;
                r.Errors = BackendUtils.ConvertErrorMessages(output, silvernode);
                return r;
            }
            catch (Exception)
            {
               return VerificationResult.Error(new Error(Diagnostics.SSIL201_BackendNotFound, null, "carbon.bat"));
            }
        }
    }
}
