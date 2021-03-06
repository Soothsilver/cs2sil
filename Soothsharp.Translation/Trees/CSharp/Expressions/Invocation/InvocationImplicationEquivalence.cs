﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Soothsharp.Contracts;
using Soothsharp.Translation.Trees.CSharp.Invocation;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation.Trees.CSharp.Invocation
{
    /// <summary>
    /// Translates <see cref="Soothsharp.Contracts.StaticExtension.Implies(bool, bool)"/>
    /// and <see cref="StaticExtension.EquivalentTo(bool, bool)"/>.  
    /// </summary>
    /// <seealso cref="Soothsharp.Translation.Trees.CSharp.Invocation.InvocationTranslation" />
    class InvocationImplicationEquivalence : InvocationTranslation
    {
        private readonly string _operator;
        private readonly ExpressionSyntax MethodGroup;

        public InvocationImplicationEquivalence(string @operator, ExpressionSyntax methodGroup)
        {
            this._operator = @operator; // either "==>" or "<==>"
            this.MethodGroup = methodGroup;
        }

        public override void Run(List<ExpressionSharpnode> arguments, SyntaxNode originalNode, TranslationContext context)
        {
            if (this.MethodGroup is MemberAccessExpressionSyntax)
            {
                MemberAccessExpressionSyntax memberAccess = (MemberAccessExpressionSyntax) this.MethodGroup;
                var leftExpression = RoslynToSharpnode.MapExpression(memberAccess.Expression);
                // There are some Viper purity requirements here, but they are checked by the verifier,
                // so we don't need to check them.
                var leftExpressionResult = leftExpression.Translate(context);
                var rightExpressionResult = arguments[0].Translate(context);
                Silvernode implies = new BinaryExpressionSilvernode(
                    leftExpressionResult.Silvernode, this._operator,
                    rightExpressionResult.Silvernode,
                    originalNode
                    );

                this.Errors.AddRange(leftExpressionResult.Errors);
                this.Errors.AddRange(rightExpressionResult.Errors);
                this.Prependors.AddRange(leftExpressionResult.PrependTheseSilvernodes);
                this.Prependors.AddRange(rightExpressionResult.PrependTheseSilvernodes);
                this.Silvernode = implies;
            }
            else
            {
                this.Errors.Add(new Error(Diagnostics.SSIL110_InvalidSyntax, originalNode,
                    "member access expression expected"));
                this.Silvernode = new TextSilvernode(Constants.SilverErrorString, originalNode);
            }
        }
    }
}