﻿using Microsoft.CodeAnalysis;

namespace Sharpsilver.Translation.Trees.Silver
{
    internal class IdentifierSilvernode : Silvernode
    {
        private SyntaxToken identifierToken;
        private IdentifierDeclaration silverIdentifier;
        private IdentifierReference silverIdentifierReference;

        public IdentifierSilvernode(Identifier identifier) : base(null)
        {
            if (identifier is IdentifierDeclaration)
                silverIdentifier = identifier as IdentifierDeclaration;
            else
                silverIdentifierReference = identifier as IdentifierReference;
        }

        public IdentifierSilvernode(SyntaxToken identifierToken, IdentifierDeclaration silverIdentifier) : base(null)
        {
            this.identifierToken = identifierToken;
            this.silverIdentifier = silverIdentifier;
        }
        public IdentifierSilvernode(SyntaxToken identifierToken, IdentifierReference silverIdentifier) : base(null)
        {
            this.identifierToken = identifierToken;
            this.silverIdentifierReference = silverIdentifier;
        }

        public override string ToString()
        {
            if (silverIdentifier != null)
                return silverIdentifier.ToString();
            else
                return silverIdentifierReference.ToString();
        }
    }
}