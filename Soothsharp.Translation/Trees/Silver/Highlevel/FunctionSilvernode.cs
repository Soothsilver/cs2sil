﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    internal class FunctionSilvernode : SubroutineSilvernode
    {
        public FunctionSilvernode(SyntaxNode methodDeclarationSyntax,
            IdentifierSilvernode identifierSilvernode, 
            List<ParameterSilvernode> parameters,
            string returnName,
            TypeSilvernode returnType, 
            List<VerificationConditionSilvernode> verificationConditions,
            BlockSilvernode block)
            : base(methodDeclarationSyntax,
                  identifierSilvernode,
                  parameters,
                  returnName,
                  returnType,
                  verificationConditions,
                  block)
        {
        }
        public override IEnumerable<Silvernode> Children
        {
            get
            {
                var children = new List<Silvernode>();
                children.Add("function ");
                children.Add(Identifier);
                children.Add(" (");
                children.AddRange(Parameters.WithSeparator<Silvernode>(new TextSilvernode(", ")));
                children.Add(")");
                children.Add(" : ");
                children.Add(ReturnType);
                
                AddVerificationConditions(children);
                AddBlock(children);
                return children;
            }
        }

      
    }
}