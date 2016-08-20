﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
            string filename = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(filename, silvercode);
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                var enviromentPath = System.Environment.GetEnvironmentVariable("PATH");
                Debug.Assert(enviromentPath != null, "enviromentPath != null");
                var paths = enviromentPath.Split(';');
                var exePath = paths
                    .Select(x => Path.Combine(x, "carbon.bat"))
                    .FirstOrDefault(File.Exists);
                if (exePath == null) exePath = "carbon.bat";
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