using UnityEngine;

namespace Moths.Macros
{
    public struct Arg
    {
        // Equality
        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(Arg a, Arg b) => false;
        public static bool operator !=(Arg a, Arg b) => false;

        // Comparison
        public static bool operator <(Arg a, Arg b) => false;
        public static bool operator >(Arg a, Arg b) => false;
        public static bool operator <=(Arg a, Arg b) => false;
        public static bool operator >=(Arg a, Arg b) => false;

        // Arithmetic
        public static Arg operator +(Arg a, Arg b) => default;
        public static Arg operator -(Arg a, Arg b) => default;
        public static Arg operator *(Arg a, Arg b) => default;
        public static Arg operator /(Arg a, Arg b) => default;
        public static Arg operator %(Arg a, Arg b) => default;

        // Unary arithmetic
        public static Arg operator +(Arg a) => default;
        public static Arg operator -(Arg a) => default;
        public static Arg operator ++(Arg a) => default;
        public static Arg operator --(Arg a) => default;

        // Bitwise
        public static Arg operator &(Arg a, Arg b) => default;
        public static Arg operator |(Arg a, Arg b) => default;
        public static Arg operator ^(Arg a, Arg b) => default;
        public static Arg operator ~(Arg a) => default;
        public static Arg operator <<(Arg a, int shift) => default;
        public static Arg operator >>(Arg a, int shift) => default;

        // Logical (true/false operators are required for use in conditionals)
        public static bool operator true(Arg a) => false;
        public static bool operator false(Arg a) => false;

        // === Implicit conversions to all built-in value types ===
        public static implicit operator bool(Arg value) => default;
        public static implicit operator byte(Arg value) => default;
        public static implicit operator sbyte(Arg value) => default;
        public static implicit operator char(Arg value) => default;
        public static implicit operator short(Arg value) => default;
        public static implicit operator ushort(Arg value) => default;
        public static implicit operator int(Arg value) => default;
        public static implicit operator uint(Arg value) => default;
        public static implicit operator long(Arg value) => default;
        public static implicit operator ulong(Arg value) => default;
        public static implicit operator float(Arg value) => default;
        public static implicit operator double(Arg value) => default;
        public static implicit operator decimal(Arg value) => default;
        public static implicit operator string(Arg value) => default;
        public static implicit operator Object(Arg value) => default;

        // === Implicit conversions to UnityEngine basic types ===
        public static implicit operator Vector2(Arg value) => default;
        public static implicit operator Vector3(Arg value) => default;
        public static implicit operator Vector4(Arg value) => default;
        public static implicit operator Quaternion(Arg value) => default;
        public static implicit operator Color(Arg value) => default;
        public static implicit operator Color32(Arg value) => default;
        public static implicit operator Rect(Arg value) => default;
        public static implicit operator Bounds(Arg value) => default;
        public static implicit operator Plane(Arg value) => default;
        public static implicit operator Ray(Arg value) => default;
        public static implicit operator LayerMask(Arg value) => default;
        public static implicit operator Matrix4x4(Arg value) => default;

        public Arg Call() => default;
        public Arg Call(Arg arg1) => default;
        public Arg Call(Arg arg1, Arg arg2) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5, Arg arg6) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5, Arg arg6, Arg arg7) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5, Arg arg6, Arg arg7, Arg arg8) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5, Arg arg6, Arg arg7, Arg arg8, Arg arg9) => default;
        public Arg Call(Arg arg1, Arg arg2, Arg arg3, Arg arg4, Arg arg5, Arg arg6, Arg arg7, Arg arg8, Arg arg9, Arg arg10) => default;
    }
}