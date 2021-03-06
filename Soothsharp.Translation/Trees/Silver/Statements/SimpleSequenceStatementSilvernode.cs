﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    public class SimpleSequenceStatementSilvernode : StatementSilvernode
    {
        public List<Silvernode> List;

        public SimpleSequenceStatementSilvernode(SyntaxNode originalNode, params Silvernode[] topLevelSilvernodes)
            : base(originalNode)
        {
            this.List = new List<Silvernode>(topLevelSilvernodes);
        }

        protected override IEnumerable<Silvernode> Children
        {
            get { return this.List.SelectMany(s => new[] { s }); }
        }
    }
}
