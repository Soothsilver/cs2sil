﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Soothsharp.Translation.Trees.CSharp
{
    class UnexpectedSharpnode : Sharpnode
    {
        public UnexpectedSharpnode(SyntaxNode node) : base(node)
        {
        }

        public override TranslationResult Translate(TranslationContext translationContext)
        {
            return TranslationResult.Error(this.OriginalNode, Diagnostics.SSIL102_UnexpectedNode, this.OriginalNode.Kind());
        }
    }
}
