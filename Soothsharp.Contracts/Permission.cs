﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soothsharp.Contracts
{
    /// <summary>
    /// Represents a fractional permission that defines the level of access to an address on the heap. Immutable.
    /// </summary>
    /// <remarks>
    /// This class is functional for representing rational numbers in C# for as long as you don't use the special permissions
    /// (<see cref="FromLocation{T}(T)"/> and <see cref="Wildcard"/>). However, this is only for debugging - do not use this class 
    /// to represent rational numbers in your code aside from contracts.
    /// </remarks>
    public class Permission
    {
#pragma warning disable CS0414
        private SpecialPermission special = SpecialPermission.NotSpecial;
#pragma warning restore CS0414
        private readonly int numerator;
        private readonly int denominator;

        /// <summary>
        /// Creates a fractional permission.
        /// </summary>
        /// <param name="numerator">The numerator of the fraction.</param>
        /// <param name="denominator">The denominator of the fraction.</param>
        public static Permission Create(int numerator, int denominator)
        {
            return new Permission(numerator, denominator);
        }

        private Permission(int numerator, int denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;
        }
        /// <summary>
        /// Gets the full write permission (1/1).
        /// </summary>
        public static Permission Write => new Permission(1, 1);

        /// <summary>
        /// Gets the wildcard permission (unspecified non-zero permission).
        /// </summary>
        public static Permission Wildcard => new Permission(0, 0)
        {
            special = SpecialPermission.Wildcard
        };

        /// <summary>
        /// Gets a half permission (1/2).
        /// </summary>
        public static Permission Half => new Permission(1, 2);

        /// <summary>
        /// Gets the zero permission (0/1).
        /// </summary>
        public static Permission None => new Permission(0, 1);

        /// <summary>
        /// Retrieves the current permission we have at this point to the given field or predicate.
        /// </summary>
        /// <param name="fieldOrPredicateCall">The field or predicate call we may have access to.</param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Global
        public static Permission FromLocation<T>(T fieldOrPredicateCall)
        {
            return new Permission(0, 0)
            {
                special = SpecialPermission.RetrievedFromLocation
            };
        }

        /// <summary>
        /// Implements the operator + for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator +(Permission one, Permission two)
        {
            return new Permission(one.numerator * two.denominator + one.denominator * two.numerator, one.denominator * two.denominator);
        }
        /// <summary>
        /// Implements the operator - for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator -(Permission one, Permission two)
        {
            return new Permission(one.numerator * two.denominator - one.denominator * two.numerator, one.denominator * two.denominator);
        }
        /// <summary>
        /// Implements the operator * for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator *(Permission one, Permission two)
        {
            return new Permission(one.numerator * two.numerator, one.denominator * two.denominator);
        }
        /// <summary>
        /// Implements the operator * for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator *(int one, Permission two)
        {
            return new Permission(one * two.numerator, two.denominator);
        }
        /// <summary>
        /// Implements the operator * for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator *(Permission one, int two)
        {
            return new Permission(one.numerator * two, one.denominator);
        }


        /// <summary>
        /// Implements the operator + for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator +(Permission one, int two)
        {
            return one + new Permission(two, 1);
        }
        /// <summary>
        /// Implements the operator + for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator +(int one, Permission two)
        {
            return new Permission(one, 1) + two;
        }
        /// <summary>
        /// Implements the operator - for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator -(Permission one, int two)
        {
            return one - new Permission(two, 1);
        }
        /// <summary>
        /// Implements the operator - for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static Permission operator -(int one, Permission two)
        {
            return new Permission(one, 1) * two;
        }

        /// <summary>
        /// Implements the operator &lt; for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static bool operator <(Permission one, Permission two)
        {
            return one.numerator * two.denominator < one.denominator * two.numerator;
        }
        /// <summary>
        /// Implements the operator &gt; for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static bool operator >(Permission one, Permission two)
        {
            return one.numerator * two.denominator > one.denominator * two.numerator;
        }
        /// <summary>
        /// Implements the operator &lt;= for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static bool operator <=(Permission one, Permission two)
        {
            return one.numerator * two.denominator <= one.denominator * two.numerator;
        }

        /// <summary>
        /// Implements the operator &gt;= for permissions.
        /// </summary>
        /// <param name="one">The first operand.</param>
        /// <param name="two">The second operand.</param>
        public static bool operator >=(Permission one, Permission two)
        {
            return one.numerator * two.denominator >= one.denominator * two.numerator;
        }

        /*
         * Note: The division operator "/" is not defined, on purpose, as it does not exist in Silver.
         * It only exists there to create a new Perm out of two integers, which is done here by the constructor.
         */

        private enum SpecialPermission
        {
            NotSpecial,
            Wildcard,
            RetrievedFromLocation
        }
    }
}
