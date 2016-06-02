﻿using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpsilver.Translation;
using System.Collections.Generic;

namespace Sharpsilver.Translation.AbstractSyntaxTrees.CSharp.Expressions
{
    public class SimpleAssignmentExpressionSharpnode : ExpressionSharpnode
    {
        public ExpressionSharpnode Left;
        public ExpressionSharpnode Right;

        public SimpleAssignmentExpressionSharpnode(AssignmentExpressionSyntax syntax) : base(syntax)
        {
            Left = RoslynToSharpnode.MapExpression(syntax.Left);
            Right = RoslynToSharpnode.MapExpression(syntax.Right);
        }

        public override TranslationResult Translate(TranslationContext context)
        {
            var left = Left.Translate(context);
            var right = Right.Translate(context);
            IEnumerable<Error> errors = CommonUtils.CombineErrors(left, right);
            return TranslationResult.FromSilvernode(new BinaryExpressionSilvernode(left.Silvernode, ":=", right.Silvernode, OriginalNode), errors);
        }
    }
}