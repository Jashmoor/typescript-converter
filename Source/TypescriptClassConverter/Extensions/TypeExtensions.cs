using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypescriptClassConverter.Models;

namespace TypescriptClassConverter.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsClosedDictionaryType(this Type type)
            => type.IsConstructedGenericType 
               && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                   type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

        public static bool IsClosedEnumerableType(this Type type)
            => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);


        //public static MemberDefinition Define(this Type type)
        //{
        //    if(type.IsClass)
        //}
    }
}
