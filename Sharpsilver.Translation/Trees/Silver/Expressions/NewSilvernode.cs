﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Sharpsilver.Translation.Trees.Silver
{
    class NewStarSilvernode : ExpressionSilvernode
    {
        public NewStarSilvernode(
            SyntaxNode originalNode) : base(originalNode, SilverType.Ref)
        {

        }

        public override string ToString()
        {
            return "new(*)";
        }
    }
}