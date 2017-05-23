﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation.Trees.CSharp.Invocation
{
    class InvocationVerificationCondition : InvocationTranslation
    {
        private readonly string _methodName;

        public InvocationVerificationCondition(string methodName)
        {
            this._methodName = methodName;
        }

        public override void Run(List<ExpressionSharpnode> arguments, SyntaxNode originalNode, TranslationContext context)
        {
            // TODO more checks
            var conditionResult = arguments[0].Translate(context.ChangePurityContext(PurityContext.PurityNotRequired));
            Silvernode result;
            switch (this._methodName)
            {
                case ContractsTranslator.ContractEnsures:
                    result = new EnsuresSilvernode(conditionResult.Silvernode, originalNode);
                    break;
                case ContractsTranslator.ContractRequires:
                    result = new RequiresSilvernode(conditionResult.Silvernode, originalNode);
                    break;
                case ContractsTranslator.ContractInvariant:
                    result = new InvariantSilvernode(conditionResult.Silvernode, originalNode);
                    break;
                default:
                    throw new System.Exception("This kind of verification condition does not exist.");
            }
            this.Silvernode = result;
            this.Errors.AddRange(conditionResult.Errors);
        }
    }
}