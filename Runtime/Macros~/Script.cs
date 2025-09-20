using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Moths.Macros
{
    internal enum Protection
    {
        Public,
        Private,
        Protected,
        Internal,
        None,
    };

    internal enum Modifier
    {
        None,
        Partial
    };

    internal enum Binding
    {
        Member,
        Static,
        Const,
    };

    internal enum Mutability
    {
        Mutable,
        Readonly
    };

    internal static class Extensions
    {
        internal static string GetString(this Protection protection)
        {
            return protection switch { Protection.Public => "public", Protection.Private => "private", Protection.Protected => "protected", Protection.Internal => "internal", Protection.None => "", _ => "" };
        }

        internal static string GetString(this Binding binding) => binding switch { Binding.Member => "", Binding.Static => "static", Binding.Const => "const", _ => "" };
        internal static string GetString(this Mutability mutability) => mutability switch { Mutability.Mutable => "", Mutability.Readonly => "readonly", _ => "" };
        internal static string GetString(this Modifier modifier) => modifier switch { Modifier.None => "", Modifier.Partial => "partial", _ => "" };
    }

    internal struct Body
    {
        private StringBuilder _text;
        public Body(string prefix)
        {
            _text = new StringBuilder(prefix);
            _text.Append("{\n\n\t");
        }

        private void AddField(Protection protection, Binding binding, Mutability mutability, string type, string name, object value)
        {
            _text.Append(protection.GetString());
            _text.Append(" ");
            _text.Append(binding.GetString());
            if (binding != Binding.Member) _text.Append(" ");
            if (binding != Binding.Const)
            {
                _text.Append(mutability.GetString());
                if (mutability != Mutability.Mutable) _text.Append(" ");
            }
            _text.Append(type);
            _text.Append(" ");
            _text.Append(name);
            if (value != null)
            {
                _text.Append(" = ");
                if (value is string)
                {
                    _text.Append($"\"{value}\"");
                }
                else if (value is float?)
                {
                    _text.Append($"{value}f");
                }
                else if (value is FieldName)
                {
                    _text.Append($"{value}");
                }
                else
                {
                    _text.Append(value);
                }
            }
            _text.Append(";\n\t");
        }

        public string Code => _text.ToString() + "\n}";

        public void AddLine(string line) => _text.AppendLine(line);

        public void AddOpeningBrace() => _text.Append("{\n");
        public void AddClosingBrace() => _text.Append("}\n");
        public void AddNewLine() => _text.Append("\n\t");
        public void AddComment(string comment) => _text.Append($"// {comment}\n\t");

        public void AddConstant(Protection protection, string name, string value) => AddField(protection, Binding.Const, Mutability.Mutable, "string", name, value);
        public void AddConstant(Protection protection, string name, float? value) => AddField(protection, Binding.Const, Mutability.Mutable, "float", name, value);
        public void AddConstant(Protection protection, string name, int? value) => AddField(protection, Binding.Const, Mutability.Mutable, "int", name, value);

        public void AddVariable(Protection protection, string type, string name, object value) => AddField(protection, Binding.Member, Mutability.Mutable, type, name, value);
        public void AddVariable(Protection protection, string name, string value) => AddVariable(protection, "string", name, value);
        public void AddVariable(Protection protection, string name, int? value) => AddVariable(protection, "int", name, value);
        public void AddVariable(Protection protection, string name, float? value) => AddVariable(protection, "float", name, value);

        public void AddStaticReadonly(Protection protection, string type, string name, object value) => AddField(protection, Binding.Static, Mutability.Readonly, type, name, value);
        public void AddStaticReadonly(Protection protection, string name, string value) => AddStaticReadonly(protection, "string", name, value);
        public void AddStaticReadonly(Protection protection, string name, float? value) => AddStaticReadonly(protection, "float", name, value);
        public void AddStaticReadonly(Protection protection, string name, int? value) => AddStaticReadonly(protection, "int", name, value);

        public void AddClass(Protection protection, Binding binding, Modifier modifier, Mutability mutability, string name)
        {
            _text.Append(protection.GetString());
            _text.Append(" ");
            _text.Append(binding.GetString());
            if (binding != Binding.Member) _text.Append(" ");
            _text.Append(modifier.GetString());
            if (modifier != Modifier.None) _text.Append(" ");
            _text.Append(mutability.GetString());
            if (mutability != Mutability.Mutable) _text.Append(" ");
            _text.AppendLine($"class {name}");
        }

        public void AddBody(Body body) => _text.Append($"{body.Code.Replace("\n", "\n\t")}\n\t");
    }

    internal struct FieldName
    {
        public string value;
        public static implicit operator FieldName(string value) => new FieldName { value = value };
        public override string ToString() => value;
    }

    internal struct Script
    {
        private StringBuilder _text;
        private Body _body;

        public Body Body => _body;
        public string Code => _text.ToString() + _body.Code;

        public Script(string text)
        {
            _text = new StringBuilder();
            _body = new Body(text);
        }

        public void Save(string scriptPath)
        {
            File.WriteAllText(scriptPath, this);
        }

        public static implicit operator string(Script script) => script.Code;

        public void AddLine(string line) => _text.AppendLine(line);

        public void AddUsing(string namespaceName) => _text.AppendLine($"using {namespaceName};");

        public void AddClass(Protection protection, Binding binding, Modifier modifier, Mutability mutability, string name, string inheritance = "")
        {
            _text.Append(protection.GetString());
            _text.Append(" ");
            _text.Append(binding.GetString());
            if (binding != Binding.Member) _text.Append(" ");
            _text.Append(modifier.GetString());
            if (modifier != Modifier.None) _text.Append(" ");
            _text.Append(mutability.GetString());
            if (mutability != Mutability.Mutable) _text.Append(" ");
            _text.Append($"class {name}");
            if (!string.IsNullOrEmpty(inheritance)) _text.Append($" : {inheritance}");
            _text.AppendLine("");
        }
    }
}
