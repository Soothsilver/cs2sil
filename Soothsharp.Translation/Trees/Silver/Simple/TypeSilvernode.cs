﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Soothsharp.Translation.Trees.Silver
{
    internal class TypeSilvernode : Silvernode
    {
        private SilverType silverType;

        public TypeSilvernode(TypeSyntax typeSyntax, SilverType silverType) : base(typeSyntax)
        {
            this.silverType = silverType;
        }

        public override string ToString()
        {
            return TypeTranslator.SilverTypeToString(silverType);
        }

        public bool RepresentsVoid()
        {
            return silverType == SilverType.Void;
        }
    }
}