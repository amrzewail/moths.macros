using System;
using System.Collections.Generic;
using System.Text;

namespace Moths.Macros
{
    public struct ArgStructTemplate
    {
        private string _namespaces;
        private string _macro;
        private string _name;
        public ArgStructTemplate(string namespaces, string macro, string name)
        {
            _namespaces = namespaces;
            _macro = macro;
            _name = name;
        }

        public static implicit operator string(ArgStructTemplate template)
        {
            return @$"
using UnityEngine;
using Moths.Macros;

{(string.IsNullOrEmpty(template._namespaces) ? "" : $"namespace {template._namespaces} {{")} 
    public partial class {template._macro} 
    {{
        public struct {template._name} {{

// Equality
        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==({template._name} a, {template._name} b) => false;
        public static bool operator !=({template._name} a, {template._name} b) => false;

        // Comparison
        public static bool operator <({template._name} a, {template._name} b) => false;
        public static bool operator >({template._name} a, {template._name} b) => false;
        public static bool operator <=({template._name} a, {template._name} b) => false;
        public static bool operator >=({template._name} a, {template._name} b) => false;

        // Arithmetic
        public static {template._name} operator +({template._name} a, {template._name} b) => default;
        public static {template._name} operator -({template._name} a, {template._name} b) => default;
        public static {template._name} operator *({template._name} a, {template._name} b) => default;
        public static {template._name} operator /({template._name} a, {template._name} b) => default;
        public static {template._name} operator %({template._name} a, {template._name} b) => default;

        // Unary arithmetic
        public static {template._name} operator +({template._name} a) => default;
        public static {template._name} operator -({template._name} a) => default;
        public static {template._name} operator ++({template._name} a) => default;
        public static {template._name} operator --({template._name} a) => default;

        // Bitwise
        public static {template._name} operator &({template._name} a, {template._name} b) => default;
        public static {template._name} operator |({template._name} a, {template._name} b) => default;
        public static {template._name} operator ^({template._name} a, {template._name} b) => default;
        public static {template._name} operator ~({template._name} a) => default;
        public static {template._name} operator <<({template._name} a, int shift) => default;
        public static {template._name} operator >>({template._name} a, int shift) => default;

        // Logical (true/false operators are required for use in conditionals)
        public static bool operator true({template._name} a) => false;
        public static bool operator false({template._name} a) => false;

        // === Implicit conversions to all built-in value types ===
        public static implicit operator bool({template._name} value) => default;
        public static implicit operator byte({template._name} value) => default;
        public static implicit operator sbyte({template._name} value) => default;
        public static implicit operator char({template._name} value) => default;
        public static implicit operator short({template._name} value) => default;
        public static implicit operator ushort({template._name} value) => default;
        public static implicit operator int({template._name} value) => default;
        public static implicit operator uint({template._name} value) => default;
        public static implicit operator long({template._name} value) => default;
        public static implicit operator ulong({template._name} value) => default;
        public static implicit operator float({template._name} value) => default;
        public static implicit operator double({template._name} value) => default;
        public static implicit operator decimal({template._name} value) => default;
        public static implicit operator string({template._name} value) => default;
        public static implicit operator Object({template._name} value) => default;

        // === Implicit conversions to UnityEngine basic types ===
        public static implicit operator Vector2({template._name} value) => default;
        public static implicit operator Vector3({template._name} value) => default;
        public static implicit operator Vector4({template._name} value) => default;
        public static implicit operator Quaternion({template._name} value) => default;
        public static implicit operator Color({template._name} value) => default;
        public static implicit operator Color32({template._name} value) => default;
        public static implicit operator Rect({template._name} value) => default;
        public static implicit operator Bounds({template._name} value) => default;
        public static implicit operator Plane({template._name} value) => default;
        public static implicit operator Ray({template._name} value) => default;
        public static implicit operator LayerMask({template._name} value) => default;
        public static implicit operator Matrix4x4({template._name} value) => default;

        public Variable Invoke(params object[] args) => default;

        }}
    }}
{(string.IsNullOrEmpty(template._namespaces) ? "" : $"}}")}";
        }
    }
}
