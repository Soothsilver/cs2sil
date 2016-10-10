﻿using Microsoft.CodeAnalysis;
using Sharpsilver.Translation.Trees.Silver;
using System.Collections.Generic;
using System.Linq;

namespace Sharpsilver.Translation.Trees
{
    class CommonUtils
    {
        public static TranslationResult GetHighlevelSequence(IEnumerable<TranslationResult> results, SyntaxNode parent = null)
        {
            TranslationResult result = new TranslationResult();
            HighlevelSequenceSilvernode sequence = new HighlevelSequenceSilvernode(parent);
            foreach(var incoming in results)
            {
                result.Errors.AddRange(incoming.Errors);
                if (incoming.Silvernode != null)
                {
                    sequence.List.Add(incoming.Silvernode);
                }
            }
            result.Silvernode = sequence;
            return result;
        }

        internal static IEnumerable<Error> CombineErrors(params TranslationResult[] results)
        {
           return results.SelectMany((result) => result.Errors);
        }
    }
}
