﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation.Trees.Silver
{
    /// <summary>
    /// This silvernode consists of several smaller silvernodes. 
    /// </summary>
    /// <seealso cref="Silvernode" />
    public abstract class ComplexSilvernode : Silvernode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexSilvernode"/> class.
        /// </summary>
        /// <param name="node">The original C# node, if any.</param>
        protected ComplexSilvernode(SyntaxNode node) : base(node)
        {
            
        }

        /// <summary>
        /// Gets the silvernodes that make up this complex silvernode. A complex silvernode does not generate any text on its own, all of its 
        /// Silver text must be formed by its children.
        /// </summary>
        protected abstract override IEnumerable<Silvernode> Children { get; }
        /// <summary>
        /// Joins the <c>ToString()</c> results of all children together.
        /// </summary>
        public sealed override string ToString()
        {
            return String.Join("", this.Children);
        }
    }
}
