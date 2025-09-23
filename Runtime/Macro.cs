using System;
using UnityEngine;

namespace Moths.Macros
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MacroAttribute : Attribute 
    {
        public MacroAttribute(params string[] args) { }
    }

    public static class Macro
    {
        public struct Params<T> { }

        public static Variable Expression(string exp) => default;
        public static T Expression<T>(string exp) => default;
    }
}