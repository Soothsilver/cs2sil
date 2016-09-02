﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharpsilver.Translation.Trees.Silver;

namespace Sharpsilver.Translation
{
    public class TranslationProcessResult
    {
        public Silvernode Silvernode;
        public bool WasTranslationSuccessful
        {
            get
            {
                return Errors.All(err => err.Diagnostic.Severity != DiagnosticSeverity.Error);
            }
        }
        public List<Error> Errors { get; private set; } 
        public TranslationProcessResult(Silvernode silvernode, List<Error> errors)
        {
            Silvernode = silvernode;
            Errors = errors;
        }
    }
}