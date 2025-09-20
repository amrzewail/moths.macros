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
        public static Variable Member(string member) => default;
        public static T Member<T>(string member) => default;
    }
}