﻿using System;

namespace Sharpsilver.Contracts
{
    /// <summary>
    /// Sets the name of the annotated class, method or field in the generated Silver code. It is the user's responsibility to ensure that the chosen name won't collide with any other name specified by this attribute elsewhere or with a name generated automatically by the translator. 
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Method |
        AttributeTargets.Field)]
    public class SilvernameAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the identifier the annotated item will have in generated Silver code.
        /// </summary>
        private string SilverIdentifier { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SilvernameAttribute"/> class.
        /// </summary>
        /// <param name="silverIdentifier">The identifier the annotated item will have in generated Silver code.</param>
        public SilvernameAttribute(string silverIdentifier)
        {
            SilverIdentifier = silverIdentifier;
        }
    }
}