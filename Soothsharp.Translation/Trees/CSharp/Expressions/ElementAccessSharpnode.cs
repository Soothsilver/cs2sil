﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Soothsharp.Translation.Translators;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation.Trees.CSharp
{
    class ElementAccessSharpnode : ExpressionSharpnode
    {
        private ElementAccessExpressionSyntax eaes;
        private ExpressionSharpnode Container;
        private ExpressionSharpnode Index;
        public ElementAccessSharpnode(ElementAccessExpressionSyntax syntax) : base(syntax)
        {
            this.eaes = syntax;
            this.Container = RoslynToSharpnode.MapExpression(syntax.Expression);
            // Only single-dimensional arrays are supported.
            this.Index = RoslynToSharpnode.MapExpression(syntax.ArgumentList.Arguments[0].Expression);

        }

        public override TranslationResult Translate(TranslationContext context)
        {
            // see thesis for details

            SymbolInfo symbolInfo = context.Semantics.GetSymbolInfo(this.eaes);
            ISymbol symbol = symbolInfo.Symbol;
            string accessorName = symbol?.GetQualifiedName();
            var errors = new List<Error>();
            var container = this.Container.Translate(context);
            var index = this.Index.Translate(context.ChangePurityContext(PurityContext.Purifiable));
            errors.AddRange(container.Errors);
            errors.AddRange(index.Errors);
            if (accessorName == SeqTranslator.SeqAccess)
            {
                return TranslationResult.FromSilvernode(
                    new SimpleSequenceSilvernode(this.OriginalNode,
                        container.Silvernode,
                        "[",
                        index.Silvernode,
                        "]"
                      ), errors
                    ).AndPrepend(container.PrependTheseSilvernodes.Concat(index.PrependTheseSilvernodes));
            }
            else
            {
                var typeInfo = context.Semantics.GetTypeInfo(Container.OriginalNode);
                var t = typeInfo.Type;
                if (t.Kind == SymbolKind.ArrayType)
                {
                    // Let's assume that this is an array read.
                    // If this is an array write, the parent will usee ArraysContainer and ArraysIndex instead
                    var readsilvernode = context.Process.ArraysTranslator.ArrayRead(this.OriginalNode, container.Silvernode,
                        index.Silvernode); 
                    TranslationResult read = TranslationResult.FromSilvernode(readsilvernode, errors).AndPrepend(container.PrependTheseSilvernodes.Concat(index.PrependTheseSilvernodes));
                    read.ArraysContainer = container;
                    read.ArraysIndex = index;
                    return read; 
                }
                else
                {
                    return TranslationResult.Error(this.OriginalNode,
                        Diagnostics.SSIL128_IndexersAreOnlyForSeqsAndArrays);
                }
            }
        }
    }
}
