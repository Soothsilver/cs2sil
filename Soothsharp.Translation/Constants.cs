﻿// ReSharper disable InconsistentNaming

namespace Soothsharp.Translation
{
    /// <summary>
    /// Contains constant names used in Silver.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The number of spaces to add to the Viper code for each indent level within methods.
        /// </summary>
        public const int SpacesPerIndentLevel = 4;

        /// <summary>
        /// In Silver, methods' return values have names that one used to refer to the return value
        /// inside the method. C# methods don't have this, so this is the name that is always given to 
        /// the return value. The identifier translator guarantees name safety.
        /// </summary>
        public const string SilverReturnVariableName = "res";
        /// <summary>
        /// This text is used instead of abstract syntax nodes that could not be translated. This text
        /// must be syntactically incorrect in Silver in any context (by using an illegal character).
        /// </summary>
        public const string SilverErrorString = "!ERROR!";
        /// <summary>
        /// Name of the label that is used in Silver for the end of the method (this is used when translating
        /// return statements).
        /// </summary>
        public const string SilverMethodEndLabel = "end";

        /// <summary>
        /// Name of the "silver type" for the superclass of all classes, System.Object.
        /// </summary>
        public const string SilverSystemObject = "System_Object";
        /// <summary>
        /// The identifier that represents the receiver object in Silver
        /// </summary>
        public const string SilverThis = "this";
        /// <summary>
        /// Tag for the default constructor or the initializer created by C# compiler.
        /// </summary>
        public const string InitializerTag = "init";
        /// <summary>
        /// Tag for a user-defined constructor.
        /// </summary>
        public const string ConstructorTag = "ctor";
    }
}
