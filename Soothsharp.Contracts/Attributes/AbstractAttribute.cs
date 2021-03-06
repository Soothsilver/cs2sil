﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soothsharp.Contracts
{
    /// <summary>
    /// The annotated method should be translated as abstract in Viper. The body of this method
    /// will be ignored - just put anything in there that will satisfy the typing system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AbstractAttribute : Attribute
    {
    }
}
