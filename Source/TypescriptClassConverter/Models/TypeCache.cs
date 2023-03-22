using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptClassConverter.Models
{
    public interface ITypeCache : IEnumerable<Type>
    {
        bool Add(Type type);
        bool Contains(Type type);
    }

    internal class TypeCache : ITypeCache
    {
        private readonly ISet<Type> _Types;

        public TypeCache()
        {
            _Types = new HashSet<Type>();
        }

        public bool Add(Type type)
           => _Types.Add(type);

        public bool Contains(Type type)
           => _Types.Contains(type);

        public IEnumerator<Type> GetEnumerator()
            => _Types.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
