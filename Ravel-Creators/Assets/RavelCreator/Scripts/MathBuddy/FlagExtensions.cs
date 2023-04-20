using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MathBuddy.Flags
{
    /// <summary>
    /// Extentions class for enum flag behaviours.
    /// </summary>
    public static class FlagExtensions
    {
        public enum CheckType
        {
            All,
            Any
        }
        
        public enum FlagPositive
        {
            Contains,
            DoesNotContains
        }
    }
}