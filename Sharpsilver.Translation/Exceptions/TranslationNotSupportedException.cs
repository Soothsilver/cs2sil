﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharpsilver.Translation.AbstractSyntaxTrees.CSharp;

namespace Sharpsilver.Translation.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a <see cref="Sharpnode"/>'s Translate method is called for a node that should never be translated.
    /// </summary>
    class TranslationNotSupportedException : Exception
    {
        public TranslationNotSupportedException(string nodeType) : base($"Nodes of type {nodeType} cannot be translated.")
        {
        }
    }
}