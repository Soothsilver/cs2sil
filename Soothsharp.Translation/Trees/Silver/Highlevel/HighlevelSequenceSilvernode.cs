﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    /// <summary>
    /// Represents a sequence of Silver code regions, separated by a newline character. This is meant to be used at the root level only; certainly
    /// within functions and methods this should not be used.
    /// </summary>
    /// <seealso cref="ComplexSilvernode" />
    public class HighlevelSequenceSilvernode : ComplexSilvernode
    {
        /// <summary>
        /// Contains silvernodes that this silvernode contains.
        /// </summary>
        public List<Silvernode> List;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlevelSequenceSilvernode"/> class.
        /// </summary>
        /// <param name="originalNode">The C# original node, or null.</param>
        /// <param name="topLevelSilvernodes">Silvernodes that make up this silvernode.</param>
        public HighlevelSequenceSilvernode(
            SyntaxNode originalNode, 
            params Silvernode[] topLevelSilvernodes)
            : base(originalNode)
        {
            this.List = new List<Silvernode>(topLevelSilvernodes);
        }

        protected override IEnumerable<Silvernode> Children
        {
            get {
                for (int i = 0; i < this.List.Count; i++)
                {
                    yield return this.List[i];
                    if (i != this.List.Count - 1)
                    {
                        yield return new NewlineSilvernode();
                    }
                }
            }
        }

        protected override void OptimizePre()
        {
            this.List.RemoveAll(silvernode => silvernode.ToString() == "");

        }
    }
}
