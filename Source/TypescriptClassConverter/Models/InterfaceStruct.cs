using System;
using System.Collections.Generic;
using System.Linq;

namespace TypescriptClassConverter.Models
{

    internal class InterfaceDefinition
    {
        public InterfaceDefinition(
            Type type,
            string name,
            string declaration
            )
        {
            ClassType = type;
            Name = name;
            Declaration = declaration;
            Properties = new List<MemberDefinition>();
        }

        public void AddProperties(IEnumerable<MemberDefinition> types)
        {
            foreach (var type in types)
                Properties.Add(type);
        }

        public string Declaration { get; }
        public Type ClassType { get; }
        public string Name { get; }
        public IList<MemberDefinition> Properties { get; }
    }

    internal class MemberDefinition
    {
        public MemberDefinition(
            string name,
            string type,
            bool nullable = false,
            IEnumerable<string> decorators = null
            )
        {
            Name = name;
            Type = type;
            Nullable = nullable;
            Decorators = ((decorators?.ToList()) ?? new List<string>()).AsReadOnly();
        }

        public string Name { get; }
        public string Type { get; }
        public bool Nullable { get; }
        public IReadOnlyList<string> Decorators { get; }
    }

    internal class EnumDefinition
    {
        public EnumDefinition(
            Type type,
            string fields
        )
        {
            Type = type;
            Fields = fields;
        }

        public Type Type { get; set; }
        public string Fields { get; set; }

    }

    internal struct Definition<T1, T2>
        where T1 : class
        where T2 : class
    {
        public Definition(T1 @object)
        {
            Value1 = @object;
            Value2 = default(T2);
        }

        public Definition(T2 @object)
        {
            Value1 = default(T1);
            Value2 = @object;
        }

        public T1 Value1 { get; }
        public T2 Value2 { get; }

        public static implicit operator T1(Definition<T1, T2> definition)
            => definition.Value1;

        public static implicit operator T2(Definition<T1, T2> definition)
            => definition.Value2;

        public override string ToString()
        {
            if (Value1 != default)
                return Value1.ToString();
            if (Value2 != default)
                return Value2.ToString();

            return null;
        }
    }
}
