using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptClassConverter.Models
{
    internal class TypeCollector
    {
        private readonly IList<Type> _Types;
        private readonly IList<Type> _Enums;

        internal TypeCollector(string method)
        {
            Method = method;
            _Types = new List<Type>();
            _Enums = new List<Type>();
        }

        internal TypeCollector(string method, IEnumerable<Type> types) 
            : this(method)
        {
            foreach (Type type in types)
            {
                if (type.IsEnum)
                {
                    AddEnum(type);
                    continue;
                }
                Add(type);
            }
        }

        public string Method { get; init; }
        public List<Type> Interfaces => _Types.ToList();
        public List<Type> Enums => _Enums.ToList();

        public void Add(Type type)
            => _Types.Add(type);

        public void AddEnum(Type type)
            => _Enums.Add(type);
    }
}
