﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation.Trees.CSharp.Expressions
{
    public class SimpleAssignmentExpressionSharpnode : ExpressionSharpnode
    {
        private ExpressionSharpnode Left;
        private ExpressionSharpnode Right;

        public SimpleAssignmentExpressionSharpnode(AssignmentExpressionSyntax syntax) : base(syntax)
        {
            this.Left = RoslynToSharpnode.MapExpression(syntax.Left);
            this.Right = RoslynToSharpnode.MapExpression(syntax.Right);
        }

        public override TranslationResult Translate(TranslationContext context)
        {
            var left = this.Left.Translate(new TranslationContext(context)
            {
                LValueNeeded = true
            }); 
            var right = this.Right.Translate(context.ChangePurityContext(PurityContext.Purifiable));

            List<Error> errors = CommonUtils.CombineErrors(left, right).ToList();

            StatementsSequenceSilvernode sequence;
            if (left.ArraysContainer != null)
            {
                errors.AddRange(left.ArraysContainer.Errors);
                errors.AddRange(left.ArraysIndex.Errors);
                var arrayWrite = context.Process.ArraysTranslator.ArrayWrite(this.OriginalNode, left.ArraysContainer.Silvernode,
                    left.ArraysIndex.Silvernode, right.Silvernode);
                sequence = new StatementsSequenceSilvernode(null,
                    right.PrependTheseSilvernodes.Concat(new[] {arrayWrite}).ToArray());
            }
            else
            {
                var assignment = new AssignmentSilvernode(left.Silvernode, right.Silvernode, this.OriginalNode);
                sequence = new StatementsSequenceSilvernode(null,
                    right.PrependTheseSilvernodes.Concat(new[] {assignment}).ToArray());
            }
            return TranslationResult.FromSilvernode(sequence, errors);
        }
    }
}