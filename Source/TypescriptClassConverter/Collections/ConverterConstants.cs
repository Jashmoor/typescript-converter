using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptClassConverter.Collections
{
    public static class ConverterConstants
    {

        public static BindingFlags BindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

        public static IImmutableDictionary<Type, string> Primitives =>
            new Dictionary<Type, string>()
            {
                [typeof(string)] = "string",
                [typeof(char)] = "string",
                [typeof(Nullable<char>)] = "string | null",
                [typeof(byte)] = "number",
                [typeof(Nullable<byte>)] = "number | null",
                [typeof(sbyte)] = "number",
                [typeof(Nullable<sbyte>)] = "number | null",
                [typeof(short)] = "number",
                [typeof(Nullable<short>)] = "number | null",
                [typeof(ushort)] = "number",
                [typeof(int)] = "number",
                [typeof(Nullable<int>)] = "number | null",
                [typeof(uint)] = "number",
                [typeof(long)] = "number",
                [typeof(ulong)] = "number",
                [typeof(float)] = "number",
                [typeof(double)] = "number",
                [typeof(decimal)] = "number",
                [typeof(bool)] = "boolean",
                [typeof(object)] = "any",
                [typeof(void)] = "void",
                [typeof(DateTime)] = "string | Date",
                [typeof(Nullable<DateTime>)] = "string | Date | null",
            }.ToImmutableDictionary();
    }
}
