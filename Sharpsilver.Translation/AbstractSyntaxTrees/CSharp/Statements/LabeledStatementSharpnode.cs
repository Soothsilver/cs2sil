﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpsilver.Translation.AbstractSyntaxTrees.Silver;
using Microsoft.CodeAnalysis.CSharp;

namespace Sharpsilver.Translation.AbstractSyntaxTrees.CSharp.Statements
{
    class LabeledStatementSharpnode : StatementSharpnode
    {
        public StatementSharpnode Statement;
        public LabeledStatementSyntax Self;
        public LabeledStatementSharpnode(LabeledStatementSyntax stmt) : base(stmt)
        {
            Self = stmt;
            Statement = RoslynToSharpnode.MapStatement(stmt.Statement);
        }

        public override TranslationResult Translate(TranslationContext context)
        {
            var symbol = context.Semantics.GetDeclaredSymbol(Self);
            var identifier = context.Process.IdentifierTranslator.RegisterAndGetIdentifier(symbol);
            var statementResult = Statement.Translate(context);
            SequenceSilvernode seq = new SequenceSilvernode(OriginalNode, 
                new LabelSilvernode(identifier, OriginalNode),
                statementResult.Silvernode
                );
            return TranslationResult.FromSilvernode(seq, statementResult.Errors);
        }
    }
}