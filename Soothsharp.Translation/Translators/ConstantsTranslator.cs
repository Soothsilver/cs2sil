﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation
{
    /// <summary>
    /// Groups functionality that translates C# constants into Viper text.
    /// </summary>
    public class ConstantsTranslator
    {
        /// <summary>
        /// Translates a constant, given as an identifier or a member access, to Viper text.
        /// </summary>
        /// <param name="symbol">The Roslyn symbol that represents the constant.</param>
        /// <param name="syntaxNode">The Roslyn node that reads the constant.</param>
        /// <returns></returns>
        public TranslationResult TranslateIdentifierAsConstant(ISymbol symbol, SyntaxNode syntaxNode)
        {
            if (symbol is IFieldSymbol)
            {
                IFieldSymbol field = (IFieldSymbol) symbol;
                if (field.IsConst && field.HasConstantValue)
                {
                    return
                        TranslationResult.FromSilvernode(new TextSilvernode(ConstantToString(field.ConstantValue), syntaxNode));
                }
            }
            return null;
        }

        private static string ConstantToString(object constantValue)
        {
            // Only int, bool and null is supported (not floats, doubles or decimals)
            if (constantValue is int)
            {
                return constantValue.ToString();
            }
            else if (constantValue is bool)
            {
                if ((bool) constantValue) return "true";
                else return "false";
            }
            else if (constantValue == null)
            {
                return "null";
            }
            else
            {
                return Constants.SilverErrorString;
            }
        }
    }
}
