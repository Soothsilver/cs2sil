﻿using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    public class EmptySilvernode : Silvernode
    {
        public EmptySilvernode(SyntaxNode node) : base(node)
        {

        }

        public override string ToString()
        {
            return "";
        }
    }
}
