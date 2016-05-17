﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpsilver.Translation.AbstractSyntaxTrees.Silver;
using System.Collections.Generic;
using System.Linq;
using Sharpsilver.Translation;

namespace Sharpsilver.Translation.AbstractSyntaxTrees.CSharp
{
    internal class BlockSharpnode : Sharpnode
    {
        public BlockSyntax BlockSyntax;
        public List<StatementSharpnode> Statements = new List<StatementSharpnode>();

        public BlockSharpnode(BlockSyntax originalNode) : base(originalNode)
        {
            BlockSyntax = originalNode;
            Statements = originalNode.Statements.Select(stmt => RoslynToSharpnode.MapStatement(stmt)).ToList();
        }

        public override TranslationResult Translate(TranslationContext context)
        {
            List<Silvernode> statements = new List<Silvernode>();
            List<VerificationConditionSilvernode> verificationConditions = new List<VerificationConditionSilvernode>();
            List<Error> diagnostics = new List<Error>();
            foreach(var statement in Statements)
            {
                var statementResult = statement.Translate(context);
                if (statementResult.Silvernode != null)
                {
                    if (statementResult.Silvernode.IsVerificationCondition())
                    {
                        verificationConditions.Add(statementResult.Silvernode as VerificationConditionSilvernode);
                        // TODO trigger warning if father is not method
                    }
                    else
                    {
                        statements.Add(statementResult.Silvernode);
                    }
                }
                diagnostics.AddRange(statementResult.Errors);
            }
            BlockSilvernode block = new BlockSilvernode(BlockSyntax, statements);
            TranslationResult r = new TranslationResult();
            r.Silvernode = block;
            verificationConditions.Sort();
            r.VerificationConditions = verificationConditions;
            r.Errors = diagnostics;
            return r;
        }
    }
}