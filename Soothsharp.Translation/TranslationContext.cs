﻿using Microsoft.CodeAnalysis;

namespace Soothsharp.Translation
{
    /// <summary>
    /// A context modifies how should C# code be translated into Silver. A context determines, for example,
    /// whether we requires pure expressions without side-effects.
    /// </summary>
    public class TranslationContext
    {
        /// <summary>
        /// Gets the instance of this transcompilation process.
        /// </summary>
        public TranslationProcess Process { get; }
        /// <summary>
        /// Indicates whether only pure expressions are allowed in this context.
        /// </summary>
        public PurityContext PurityContext { get; set; } = PurityContext.PurityNotRequired;
        /// <summary>
        /// Indicates whether classes and method with no [Verified] or [Unverified] attribute should be verified.
        /// </summary>
        // ReSharper disable once RedundantDefaultMemberInitializer
        public bool VerifyUnmarkedItems { get; private set; } = false;
        /// <summary>
        /// Gets a value indicating whether method bodies should be ignored (except for contracts) and translated
        /// as abstract Viper subroutines.
        /// </summary>
        public bool MarkEverythingAbstract { get; private set; }
        /// <summary>
        /// Gets the semantic model of the C# compilation.
        /// </summary>
        public SemanticModel Semantics { get; }
        /// <summary>
        /// Gets or sets a value indicating whether we are within a function or a predicate. This influences what identifier should be used for the return value.
        /// </summary>
        public bool IsFunctionOrPredicateBlock { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the result of the translation must be an l-value for the 
        /// translation to make sense. This is a relic of earlier code that is now unused.
        /// </summary>
        public bool LValueNeeded { get; set; }

        /// <summary>
        /// Creates a new translation context as a copy of a previous one.
        /// </summary>
        /// <param name="copyFrom">The context that should be copied.</param>
        public TranslationContext(TranslationContext copyFrom)
        {
            this.PurityContext = copyFrom.PurityContext;
            this.Process = copyFrom.Process;
            this.Semantics = copyFrom.Semantics;
            this.IsFunctionOrPredicateBlock = copyFrom.IsFunctionOrPredicateBlock;
            this.VerifyUnmarkedItems = copyFrom.VerifyUnmarkedItems;
            this.LValueNeeded = copyFrom.LValueNeeded;
            this.MarkEverythingAbstract = copyFrom.MarkEverythingAbstract;
        }

        private TranslationContext(TranslationProcess process, SemanticModel semantics)
        {
            this.Process = process;
            this.Semantics = semantics;
        }
        public static TranslationContext StartNew(TranslationProcess translationProcess, SemanticModel semantics, bool verifyUnmarkedItems, CompilationUnitVerificationStyle style)
        {
            return new TranslationContext(translationProcess, semantics)
            {
                VerifyUnmarkedItems = verifyUnmarkedItems,
                MarkEverythingAbstract = style == CompilationUnitVerificationStyle.ContractsOnly
            };
        }

        /// <summary>
        /// Creates a context copy that is pure in addition to the properties of the copied context.
        /// </summary>
        /// <returns>A copy of the context that is marked pure.</returns>
        public TranslationContext ChangePurityContext(PurityContext newPurityContext)
        {
            return new TranslationContext(this)
            {
                PurityContext = newPurityContext
            };
        }
    }
    /// <summary>
    /// Some sharpnodes can only be translated in impure context. The <see cref="PurityContext"/> determines what should be done
    /// if they're encountered elsewhere. 
    /// </summary>
    public enum PurityContext
    {
        /// <summary>
        /// Sharpnodes that result in impure silvernodes may translate normally.
        /// </summary>
        PurityNotRequired,
        /// <summary>
        /// Sharpnodes must result in pure silvernodes, but prepatory impure silvernodes may be stored with the <see cref="TranslationResult"/>'s prepend nodes.
        /// </summary>
        Purifiable,
        /// <summary>
        /// Sharpnodes must result in pure silvernodes. If they can't do that, an error should be triggered.
        /// </summary>
        PureOrFail
    }
}