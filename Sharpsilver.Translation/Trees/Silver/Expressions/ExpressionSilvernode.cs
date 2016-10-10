﻿using Microsoft.CodeAnalysis;

namespace Sharpsilver.Translation.Trees.Silver
{
    abstract class ExpressionSilvernode : Silvernode
    {
        public SilverType Type { get; private set; }

        protected ExpressionSilvernode(SyntaxNode node, SilverType type) : base(node)
        {
            Type = type;
        }
    }
}
