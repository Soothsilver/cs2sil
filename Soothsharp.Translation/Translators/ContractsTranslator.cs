﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Soothsharp.Contracts;
using Soothsharp.Translation.Trees.Silver;

namespace Soothsharp.Translation
{
    /// <summary>
    /// Groups functionality related to translating the class <see cref="Contract"/>. 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ContractsTranslator
    {
        // Contracts class
        private const string ContractsNamespace = nameof(Soothsharp) + "." + nameof(Contracts) + ".";
        private const string ContractsClass = ContractsNamespace + nameof(Contract) + ".";
        public const string ContractEnsures = ContractsClass + nameof(Contract.Ensures);
        public const string ContractRequires = ContractsClass + nameof(Contract.Requires);
        public const string ContractInvariant = ContractsClass + nameof(Contract.Invariant);
        public const string ContractAcc = ContractsClass + nameof(Contract.Acc);
        public const string ContractAccArray = ContractsClass + nameof(Contract.AccArray);
        public const string ContractAssert = ContractsClass + nameof(Contract.Assert);
        public const string ContractAssume = ContractsClass + nameof(Contract.Assume);
        public const string ContractInhale = ContractsClass + nameof(Contract.Inhale);
        public const string ContractExhale = ContractsClass + nameof(Contract.Exhale);
        private const string ContractIntResult = ContractsClass + nameof(Contract.IntegerResult);
        private const string ContractTruth = ContractsClass + nameof(Contract.Truth);
        public const string Result = ContractsClass + nameof(Contract.Result);
        public const string Fold = ContractsClass + nameof(Contract.Fold);
        public const string Unfold = ContractsClass + nameof(Contract.Unfold);
        public const string Folding = ContractsClass + nameof(Contract.Folding);
        public const string Unfolding = ContractsClass + nameof(Contract.Unfolding);
        public const string Old = ContractsClass + nameof(Contract.Old);
        public const string ForAll = ContractsClass + nameof(Contract.ForAll);
        public const string Exists = ContractsClass + nameof(Contract.Exists);

        // Static extensions
        public const string Implication = "System.Boolean." + nameof(StaticExtension.Implies);
        public const string Equivalence = "System.Boolean." + nameof(StaticExtension.EquivalentTo);

        // Permission class
        public const string PermissionType = ContractsNamespace + nameof(Permission);
        private const string PermissionTypeDot = PermissionType + ".";
        private const string PermissionWrite = PermissionTypeDot + nameof(Permission.Write);
        private const string PermissionHalf = PermissionTypeDot + nameof(Permission.Half);
        private const string PermissionNone = PermissionTypeDot + nameof(Permission.None);
        private const string PermissionWildcard = PermissionTypeDot + nameof(Permission.Wildcard);
        public const string PermissionCreate = PermissionTypeDot + nameof(Permission.Create);
        public const string PermissionFromLocation = PermissionTypeDot + nameof(Permission.FromLocation);

        // Attributes
        public const string PredicateAttribute = ContractsNamespace + nameof(Contracts.PredicateAttribute);
        public const string PureAttribute = ContractsNamespace + nameof(Contracts.PureAttribute);
        public const string VerifiedAttribute = ContractsNamespace + nameof(Contracts.VerifiedAttribute);
        public const string UnverifiedAttribute = ContractsNamespace + nameof(Contracts.UnverifiedAttribute);
        public const string AbstractAttribute = ContractsNamespace + nameof(Contracts.AbstractAttribute);
        public const string SignatureOnlyAttribute = ContractsNamespace + nameof(Contracts.SignatureOnlyAttribute);

        /// <summary>
        /// If the symbol corresponds to a known member of the <see cref="Contract"/> class, it is translated
        /// into Viper, otherwise null is returned. 
        /// </summary>
        /// <param name="symbol">The C# symbol of a possible property of Contract.</param>
        /// <param name="originalNode">The Roslyn node that is being translated.</param>
        /// <param name="context">The context.</param>
        public TranslationResult TranslateIdentifierAsContract(ISymbol symbol, SyntaxNode originalNode, TranslationContext context)
        {
            string silvertext = null;
            switch(symbol.GetQualifiedName())
            {
                case ContractTruth:
                    silvertext = "true";
                    break;
                case ContractIntResult:
                    silvertext = context.IsFunctionOrPredicateBlock ? "result" : Constants.SilverReturnVariableName;
                    break;
                case PermissionNone:
                    silvertext = "none";
                    break;
                case PermissionHalf:
                    silvertext = "1/2";
                    break;
                case PermissionWildcard:
                    silvertext = "wildcard";
                    break;
                case PermissionWrite:
                    silvertext = "write";
                    break;
            }
            if (silvertext != null) { 
            return TranslationResult.FromSilvernode(
                                new TextSilvernode(silvertext, originalNode)
                                );
            }
            return null;
        }

        public static bool IsMethodPureOrPredicate(IMethodSymbol theMethod)
        {
            var attributes = theMethod.GetAttributes();
            return attributes.Any(
                attr => attr.AttributeClass.GetQualifiedName() == PureAttribute ||
                        attr.AttributeClass.GetQualifiedName() == PredicateAttribute);
        }
    }
}
