using System;
using UnityEngine;

namespace Moths.Macros
{
    /// <summary>
    /// Marks a class as a macro definition and associates it with replacement arguments.
    /// </summary>
    /// <remarks>
    /// Use this attribute to declare a macro and pass one or more string arguments,
    /// which can represent variable names, type names, or other replacement values.
    /// <br/>
    /// - To define a type name, prefix the argument with <b>"type:"</b> and mark the macro class as <c>partial</c>.<br/>
    /// - Use <see cref="Macro.Expression"/> to embed raw source code that will be injected at generation time.<br/>
    /// - Nested macro classes with type definitions are not supported.<br/>
    /// - Any code inside of <b>#region Ignore</b> will not be included in the generation
    /// </remarks>
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