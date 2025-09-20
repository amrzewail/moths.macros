using UnityEngine;

namespace Moths.Macros
{    
    public struct Variable
    {
        // Equality
        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(Variable a, Variable b) => false;
        public static bool operator !=(Variable a, Variable b) => false;

        // Comparison
        public static bool operator <(Variable a, Variable b) => false;
        public static bool operator >(Variable a, Variable b) => false;
        public static bool operator <=(Variable a, Variable b) => false;
        public static bool operator >=(Variable a, Variable b) => false;

        // Arithmetic
        public static Variable operator +(Variable a, Variable b) => default;
        public static Variable operator -(Variable a, Variable b) => default;
        public static Variable operator *(Variable a, Variable b) => default;
        public static Variable operator /(Variable a, Variable b) => default;
        public static Variable operator %(Variable a, Variable b) => default;

        // Unary arithmetic
        public static Variable operator +(Variable a) => default;
        public static Variable operator -(Variable a) => default;
        public static Variable operator ++(Variable a) => default;
        public static Variable operator --(Variable a) => default;

        // Bitwise
        public static Variable operator &(Variable a, Variable b) => default;
        public static Variable operator |(Variable a, Variable b) => default;
        public static Variable operator ^(Variable a, Variable b) => default;
        public static Variable operator ~(Variable a) => default;
        public static Variable operator <<(Variable a, int shift) => default;
        public static Variable operator >>(Variable a, int shift) => default;

        // Logical (true/false operators are required for use in conditionals)
        public static bool operator true(Variable a) => false;
        public static bool operator false(Variable a) => false;

        // === Implicit conversions to all built-in value types ===
        public static implicit operator bool(Variable value) => default;
        public static implicit operator byte(Variable value) => default;
        public static implicit operator sbyte(Variable value) => default;
        public static implicit operator char(Variable value) => default;
        public static implicit operator short(Variable value) => default;
        public static implicit operator ushort(Variable value) => default;
        public static implicit operator int(Variable value) => default;
        public static implicit operator uint(Variable value) => default;
        public static implicit operator long(Variable value) => default;
        public static implicit operator ulong(Variable value) => default;
        public static implicit operator float(Variable value) => default;
        public static implicit operator double(Variable value) => default;
        public static implicit operator decimal(Variable value) => default;
        public static implicit operator string(Variable value) => default;
        public static implicit operator Object(Variable value) => default;

        // === Implicit conversions to UnityEngine basic types ===
        public static implicit operator Vector2(Variable value) => default;
        public static implicit operator Vector3(Variable value) => default;
        public static implicit operator Vector4(Variable value) => default;
        public static implicit operator Quaternion(Variable value) => default;
        public static implicit operator Color(Variable value) => default;
        public static implicit operator Color32(Variable value) => default;
        public static implicit operator Rect(Variable value) => default;
        public static implicit operator Bounds(Variable value) => default;
        public static implicit operator Plane(Variable value) => default;
        public static implicit operator Ray(Variable value) => default;
        public static implicit operator LayerMask(Variable value) => default;
        public static implicit operator Matrix4x4(Variable value) => default;

        public Variable Invoke(params object[] args) => default;
        public Variable Call(params object[] args) => default;
    }
}