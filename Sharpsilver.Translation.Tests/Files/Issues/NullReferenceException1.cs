﻿using Sharpsilver.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sharpsilTesting
{
    [Verified]
    static class VerifiedClass
    {
        class A
        {

        }
        public static Int32 FindMinimum(int a, int b, int c)
        {
            Contract.Requires(true);
            Contract.Ensures(false);
            Contract.Ensures(a == Contract.IntegerResult == (a > b && a > c));

            float f = 2.0f;


            switch (true)
            {
                default:
                    break;
            }

            return 8;

        }
    }
    [Unverified]
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
