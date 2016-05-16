﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpsilver.Translation.AbstractSyntaxTrees.CSharp
{
    public abstract class Sharpnode
    {
        /// <summary>
        /// Gets the Roslyn syntax node that corresponds to this sharpnode.
        /// </summary>
        public SyntaxNode OriginalNode { get; private set; }

        protected Sharpnode(SyntaxNode originalNode)
        {
            OriginalNode = originalNode;
        }

        public abstract TranslationResult Translate(TranslationContext context);
    }
}
