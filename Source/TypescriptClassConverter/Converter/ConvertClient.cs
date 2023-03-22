using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TypescriptClassConverter.Collections;
using TypescriptClassConverter.Models;

namespace TypescriptClassConverter.Converter
{
    public static class ConvertClient
    {
        public static void GenerateModels<T>(string path, Assembly assembly)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            var test = new TypeExtractor<ControllerBase>();
            test.ExtractTypes(assembly);

            IEnumerable<Type> types = TypestoConvert<T>(assembly);
            NamespaceDeclaration @namespace = new NamespaceDeclaration();
            foreach (var type in types)
            {
                CreateDefinition(type, @namespace);
            }
            string declarationPath = Path.Combine(path, $"{@namespace.Namespace}.ts");

            string directoryPath = Path.GetDirectoryName(declarationPath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            File.WriteAllText(declarationPath, @namespace.ToString());
            File.WriteAllText($"{Path.Combine(path, "index.ts")}", $"export * from './{@namespace.Namespace}';");
        }

        private static Type[] TypestoConvert<T>(Assembly assembly)
        {
            ISet<Type> attributes = new HashSet<Type>()
            {
                typeof(Microsoft.AspNetCore.Mvc.HttpGetAttribute),
                typeof(Microsoft.AspNetCore.Mvc.HttpPostAttribute)
            };

            var controllers = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)));

            var actionMethods = controllers.SelectMany(c =>
                c.GetMethods().Where(m =>
                    m.IsPublic && m.GetCustomAttributes().Any(a => attributes.Contains(a.GetType()))));

            var types = actionMethods.SelectMany(m =>
                new Type[1] { m.ReturnType }.Concat(m.GetParameters().Select(p => p.ParameterType)));

            return types.Select(ReplaceByGenericArgument)
                .Where(t => !t.IsPrimitive && !PrimitivesExcludeList.Contains(t))
                .Distinct()
                .ToArray();
        }

        private static Type ReplaceByGenericArgument(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (!type.IsConstructedGenericType)
                return type;

            var genericArgument = type.GenericTypeArguments.First();

            var isTask = type.GetGenericTypeDefinition() == typeof(Task<>);
            var isActionResult = type.GetGenericTypeDefinition() == typeof(Microsoft.AspNetCore.Mvc.ActionResult<>);
            var isEnumerable = typeof(IEnumerable<>).MakeGenericType(genericArgument).IsAssignableFrom(type);

            if (!isTask && !isActionResult && !isEnumerable)
                throw new InvalidOperationException();

            if (genericArgument.IsConstructedGenericType)
                return ReplaceByGenericArgument(genericArgument);

            return genericArgument;
        }


        private static Type[] PrimitivesExcludeList =
        {
            typeof(object),
            typeof(string),
            typeof(decimal),
            typeof(void),
        };

        private static InterfaceDefinition CreateDefinition(Type type, NamespaceDeclaration declaration)
        {
            if (type == null)
                return null;

            if (type.IsEnum)
            {
                CreateEnumDefinition(type, declaration);
                return null;
            }

            if (type.IsGenericParameter)
                return null;

            if (Type.GetTypeCode(type) != TypeCode.Object)
                return null;

            if (type.FullName == "System.Object")
                return null;
            if (type.FullName == null)
            {
                var i = type;
                return null; 
            }
            if (type.IsConstructedGenericType)
            {
                foreach (var argument in type.GetGenericArguments())
                    CreateDefinition(argument, declaration);
                type = type.GetGenericTypeDefinition();
            }

            if (declaration.Classes.FirstOrDefault(c => c.ClassType == type) is InterfaceDefinition found)
                return found;

            var interfaceRefs = GetInterfaces(type, declaration);
            string baseReference = null;
            if (type.IsClass)
            {
                if (CreateDefinition(type.BaseType, declaration) != null)
                    baseReference = CreateTypeReference(type.BaseType, declaration);
            }

            string converted;
            string typeName;

            if (!type.IsGenericType)
                converted = typeName = Rename(type.Name);
            else
            {
                var genericPrms = type.GetGenericArguments().Select(g =>
                {
                    var constraints = g.GetGenericParameterConstraints()
                        .Where(c => CreateDefinition(c, declaration) != null)
                        .Select(c => CreateTypeReference(c, declaration))
                        .ToList();

                    return constraints.Any() ? $"{g.Name} extends {string.Join(" & ", constraints)}" : g.Name;
                });

                converted = Rename(StripGenericFromName(type.Name));
                typeName = $"{converted}<{string.Join(", ", genericPrms)}>";
            }

            typeName = $"export interface {typeName}";

            if (!string.IsNullOrEmpty(baseReference))
                interfaceRefs.Insert(0, baseReference);

            if (interfaceRefs.Any())
            {
                typeName = $"{typeName} extends {string.Join(", ", interfaceRefs)}";
            }

            var definition = new InterfaceDefinition(type, converted, typeName);
            definition.AddProperties(GetMembers(type, declaration));
            declaration.Classes.Add(definition);
            return definition;


            string Rename(string currentName)
            {
                var checkName = currentName;
                var i = 1;
                while (declaration.Classes.Any(td => td.Name == currentName))
                {
                    checkName = $"{currentName}${i++}";
                }
                return checkName;
            }
        }

        private static EnumDefinition CreateEnumDefinition(Type type, NamespaceDeclaration declaration)
        {
            var found = declaration.Enums.FirstOrDefault(x => x.Type == type);
            if (found != null)
                return found;

            string members = $"\"{string.Join("\" | \"", Enum.GetNames(type))}\"";

            var def = new EnumDefinition(type, members);
            declaration.Enums.Add(def);
            return def;

        }


        private static IEnumerable<MemberDefinition> GetMembers(Type type, NamespaceDeclaration context)
        {
            var memberDefs = type.GetFields(ConverterConstants.BindingFlags)
                .Select(f =>
                {
                    var fieldType = f.FieldType;
                    var nullable = false;
                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        fieldType = fieldType.GetGenericArguments()[0];
                        nullable = true;
                    }

                    return new MemberDefinition(f.Name, CreateTypeReference(fieldType, context), nullable);
                })
                .ToList();

            memberDefs.AddRange(
                type.GetProperties(ConverterConstants.BindingFlags)
                    .Select(p =>
                    {
                        var propertyType = p.PropertyType;
                        var nullable = false;
                        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            // choose the generic parameter, rather than the nullable
                            propertyType = propertyType.GetGenericArguments()[0];
                            nullable = true;
                        }
                        return new MemberDefinition(p.Name, CreateTypeReference(propertyType, context), nullable);
                    })
            );

            return memberDefs;
        }

        private static List<string> GetInterfaces(Type type, NamespaceDeclaration declaration)
        {
            var interfaces = type.GetInterfaces().ToList();
            return interfaces
                .Except(type.BaseType?.GetInterfaces() ?? Enumerable.Empty<Type>())
                .Except(interfaces.SelectMany(i => i.GetInterfaces())) // get only implemented by this type
                .Where(i => CreateDefinition(i, declaration) != null)
                .Select(i => CreateTypeReference(i, declaration))
                .ToList();
        }
        private static string StripGenericFromName(string name) => name.Substring(0, name.IndexOf('`'));
        private static string CreateTypeReference(Type type, NamespaceDeclaration declaration)
        {
            if (type.IsGenericParameter)
                return type.Name;

            if (type.IsEnum)
            {
                var @enum = CreateEnumDefinition(type, declaration);
                return @enum != null ? type.Name : "any";
            }

            if (Type.GetTypeCode(type) != TypeCode.Object)
                return ConverterConstants.Primitives[type];

            Type dictionaryType;
            if (IsClosedDictionaryType(type))
                dictionaryType = type;
            else
                dictionaryType = type.GetInterfaces().FirstOrDefault(IsClosedDictionaryType);

            if (dictionaryType != null)
            {
                var keyType = dictionaryType.GetGenericArguments().ElementAt(0);
                var valueType = dictionaryType.GetGenericArguments().ElementAt(1);
                return $"Record<{CreateTypeReference(keyType, declaration)}, {CreateTypeReference(valueType, declaration)}>";
            }

            Type enumerable;
            if (IsClosedEnumerableType(type))
                enumerable = type;
            else
                enumerable = type.GetInterfaces().FirstOrDefault(IsClosedEnumerableType);

            if (enumerable != null)
                return $"Array<{CreateTypeReference(enumerable.GetGenericArguments().First(), declaration)}>";

            var typeDef = CreateDefinition(type, declaration);
            if (typeDef == null)
                return "any";

            var typeName = typeDef.Name;
            if (type.IsGenericType)
            {
                var genericPrms = type.GetGenericArguments().Select(t => CreateTypeReference(t, declaration));
                return $"{typeName}<{string.Join(", ", genericPrms)}>";
            }

            return typeName;


            bool IsClosedEnumerableType(Type check)
                => check.IsConstructedGenericType && check.GetGenericTypeDefinition() == typeof(IEnumerable<>);

            bool IsClosedDictionaryType(Type check) =>
                check.IsConstructedGenericType && (check.GetGenericTypeDefinition() == typeof(IDictionary<,>) || check.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

        }
    }
}
