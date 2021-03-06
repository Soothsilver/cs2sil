﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    public class VarStatementSilvernode : StatementSilvernode
    {
        private Identifier identifier;
        private SilverType type;

        public VarStatementSilvernode(Identifier identifier, SilverType type, SyntaxNode originalNode) : base(originalNode)
        {
            this.identifier = identifier;
            this.type = type;
        }

        protected override IEnumerable<Silvernode> Children
        {
            get
            {
                yield return "var ";
                yield return this.identifier.ToString();
                yield return " : ";
                yield return this.type.ToString();
            }
        }
    }
}