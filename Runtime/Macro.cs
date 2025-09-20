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
        public static Arg Arg(string arg) => default;
    }
}