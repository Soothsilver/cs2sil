﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soothsharp.Contracts;
using static Soothsharp.Contracts.Contract;

namespace Soothsharp.Translation.Tests.Syntax.Arrays
{
    static class BasicTest
    {
        public static void ModifyArray(int[] array)
        {
            Contract.Requires(AccArray(array));
            Contract.Requires(array.Length >= 2);

            int f = array[0];
            int s = array[1];
            array[0] = s;
            array[1] = f;
            Contracts.Contract.Assert(f == s);
        }

        public static void Main()
        {
            int[] array = null;
            int read = array[2];
            array[3] = 42;

        }
    }
}
