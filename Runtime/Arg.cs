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
    }
}