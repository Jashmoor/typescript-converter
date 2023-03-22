using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TypescriptClassConverter.Models
{
    internal class TypeExtractor<T>
        where T : ControllerBase
    {
        private readonly IDictionary<string, TypeCollector> _Collection;
        private readonly ITypeCache _Cache;

        private readonly Func<Type, bool>
            _ControllerFilter = x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(T));

        internal TypeExtractor()
        {
            _Cache = new TypeCache();
            _Collection = new Dictionary<string, TypeCollector>();
        }

        public void ExtractTypes(Assembly assembly)
        {
            IEnumerable<Type> controllers = ExtractControllers();
            foreach ((Type type, string method) in ControllerMethods)
            {
                _Collection[method] = new TypeCollector($"{type.Name}.{method}", assembly.GetTypes());
            }
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(_Collection));


            string RouteTemplate(ICustomAttributeProvider provider)
            {
                var attr = provider.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault();
                return ((RouteAttribute)attr)?.Name;
            }

            IEnumerable<Type> ExtractControllers()
                => assembly.GetTypes().Where(_ControllerFilter);

            IEnumerable<Type> AttributeFilter(Type type)
                => controllers.SelectMany(c =>
                    c.GetMethods().Where(m =>
                        m.IsPublic && m.GetCustomAttributes().Any(a => a.GetType() == type)))
                    .SelectMany(m =>
                        new Type[1] { m.ReturnType }.Concat(m.GetParameters().Select(p => p.ParameterType)));

        }

        private readonly (Type type, string method)[] ControllerMethods =
        {
            (typeof(HttpGetAttribute), "GET"),
            (typeof(HttpPostAttribute), "POST"),
            (typeof(HttpDeleteAttribute), "DELETE"),
            (typeof(HttpPatchAttribute), "PATCH"),
            (typeof(HttpPutAttribute), "PUT"),
            (typeof(HttpHeadAttribute), "HEAD"),
            (typeof(HttpOptionsAttribute), "OPTIONS")
        };
    }
}
