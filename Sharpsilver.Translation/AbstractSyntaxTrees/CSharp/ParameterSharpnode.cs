﻿using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Sharpsilver.Translation.Exceptions;
using Sharpsilver.Translation.AbstractSyntaxTrees.Silver;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Sharpsilver.Translation.Translators;

namespace Sharpsilver.Translation.AbstractSyntaxTrees.CSharp
{
    public class ParameterSharpnode : Sharpnode
    {
        public ParameterSyntax ParameterSyntax;
        public TypeSharpnode Type;
        public string Identifier;

        public ParameterSharpnode(ParameterSyntax parameterSyntax) : base(parameterSyntax)
        {
            this.ParameterSyntax = parameterSyntax;
            this.Type = new TypeSharpnode(parameterSyntax.Type);
            this.Identifier = parameterSyntax.Identifier.Text;
        }

        public override TranslationResult Translate(TranslationContext context)
        {
            throw new TranslationNotSupportedException("ParameterSharpnode");
        }

        public TranslationResult Translate(TranslationContext context, IParameterSymbol symbol)
        {
            Error err;
            ParameterSilvernode ps = new ParameterSilvernode(Identifier,
                new TypeSilvernode(Type.TypeSyntax, TypeTranslator.TranslateType(symbol.Type, Type.TypeSyntax, out err)), OriginalNode);
            var errlist = new List<Error>();
            if (err != null) errlist.Add(err);
            return TranslationResult.FromSilvernode(ps, errlist);
        }
    }
}