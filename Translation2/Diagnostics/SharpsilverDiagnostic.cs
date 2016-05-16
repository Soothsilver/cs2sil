﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpsilver.Translation
{
    public class SharpsilverDiagnostic
    {
        private SharpsilverDiagnostic(string errorCode, string caption, string details, DiagnosticSeverity severity)
        {
            this.ErrorCode = errorCode;
            this.Caption = caption;
            this.Details = details;
            this.Severity = severity;
        }

        public string ErrorCode { get; }
        public string Caption { get; }
        public string Details { get; }
        public DiagnosticSeverity Severity { get; }


        public static SharpsilverDiagnostic Create(
            string errorCode, 
            string caption, 
            string details, DiagnosticSeverity severity)
        {
            SharpsilverDiagnostic sd = new SharpsilverDiagnostic(errorCode, caption, details, severity);
            return sd;
        }
        
    }
    public class Diagnostics
    {
        public static SharpsilverDiagnostic SSIL101_UnknownNode =
            SharpsilverDiagnostic.Create(
                "SSIL101",
                "The Sharpsilver translator does not support elements of the syntax kind '{0}'.",
                "A syntax node of this kind cannot be translated by the Sharpsilver translator because the feature it provides is unavailable in Silver, or because it is difficult to translate. If you can use a less advanced construct, please do so.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL102_UnexpectedNode =
            SharpsilverDiagnostic.Create(
                "SSIL102",
                "An element of the syntax kind '{0}' is not expected at this code location.",
                "While the Sharpsilver translator might otherwise be able to handle this kind of C# nodes, this is not a place where it is able to do so. There may be an error in your C# syntax (check compiler errors) or you may be using C# features that the translator does not understand.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL103_ExceptionConstructingCSharp =
            SharpsilverDiagnostic.Create(
                "SSIL103",
                "An exception ('{0}') occured during the construction of the C# abstract syntax tree.",
                "While this is an internal error of the translator, it mostly occurs when there is a C# syntax or semantic error in your code. Try to fix any other compiler errors and maybe this issue will be resolved.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL104_ExceptionConstructingSilver =
            SharpsilverDiagnostic.Create(
                "SSIL104",
                "An exception ('{0}') occured during the construction of the Silver abstract syntax tree.",
                "While this is an internal error of the translator, it mostly occurs when there is a C# syntax or semantic error in your code. Try to fix any other compiler errors and maybe this issue will be resolved.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL105_FeatureNotYetSupported =
            SharpsilverDiagnostic.Create(
                "SSIL105",
                "This feature ({0}) is not yet supported.",
                "As the C#-to-Silver translation project is developed, we plan to allow this feature to be used in verifiable C# class files. For now, however, it is unsupported and won't work.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL106_TypeNotSupported =
            SharpsilverDiagnostic.Create(
                "SSIL106",
                "The type {0} is not supported in Silver.",
                "The Silver language can only use a 32-bit integer, a boolean and reference objects. Other value types besides these three cannot be translated.",
                DiagnosticSeverity.Error);
        public static SharpsilverDiagnostic SSIL107_ThisExpressionCannotBeStatement =
            SharpsilverDiagnostic.Create(
                "SSIL106",
                "This expression cannot form an expression statement in Silver.",
                "The Silver language does not support this expression as a standalone expression in a statement, even if C# supported it.",
                DiagnosticSeverity.Error);

        public static SharpsilverDiagnostic SSIL301_InternalLocalizedError =
            SharpsilverDiagnostic.Create(
                "SSIL301",
                "The transcompiler encountered an internal error ({0}) while parsing this.",
                "Try to remove the infringing C# code fragment. You may be forced to make do without that C# feature. You can also submit this as a bug report as this error should never be displayed to the user normally.",
                DiagnosticSeverity.Error);

        public static SharpsilverDiagnostic SSIL302_InternalError =
            SharpsilverDiagnostic.Create(
                "SSIL302",
                "The transcompiler encountered an internal error ({0}).",
                "Try to undo your most recent change to the code as it may be triggering this error. You may be forced to make do without that C# feature. You can also submit this as a bug report as this error should never be displayed to the user normally.",
                DiagnosticSeverity.Error);

        public static IEnumerable<SharpsilverDiagnostic> GetAllDiagnostics()
        {
            Type t = typeof(Diagnostics);
            var fs = t.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach(var f in fs)
            {
                object diagnostic = f.GetValue(null);
                yield return diagnostic as SharpsilverDiagnostic;
            }
        }
    }
}
