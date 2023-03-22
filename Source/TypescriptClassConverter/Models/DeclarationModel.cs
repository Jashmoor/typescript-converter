using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TypescriptClassConverter.Collections;
using TypescriptClassConverter.Converter;

namespace TypescriptClassConverter.Models
{

    internal class NamespaceDeclaration
    {
        private readonly StringBuilder _Builder;

        internal NamespaceDeclaration(string @namespace = "ApiModels", StringBuilder writer = null)
        {
            Namespace = @namespace;
            _Builder = writer ?? new();
            Classes = new List<InterfaceDefinition>();
            Enums = new List<EnumDefinition>();
        }

        public string Namespace { get; set; }
        public IList<InterfaceDefinition> Classes { get; set; }
        public IList<EnumDefinition> Enums { get; set; }



        public override string ToString()
        {
            CreateFile();
            string file = _Builder.ToString();
            _Builder.Clear();
            return file;
        }


        private void CreateFile()
        {
            _Builder.AppendFormat(@"export namespace {0} {{", Namespace);
            _Builder.AppendLine();
            WriteEnums(Enums);
            _Builder.AppendLine();
            WriteInterfaces(Classes);
            _Builder.Append("}");
        }

        private void WriteInterfaces(IEnumerable<InterfaceDefinition> definitions)
        {
            string indent = new string('\t', 1);
            foreach (var definition in definitions)
            {
                _Builder.AppendFormat(@"{0}{1} {{", indent, definition.Declaration);
                _Builder.AppendLine();
                WriteProperties(definition.Properties);
                _Builder.AppendFormat(@"{0}}}", indent);
                _Builder.AppendLine();
            }
        }

        private void WriteProperties(IEnumerable<MemberDefinition> properties)
        {
            string indent = new string('\t', 2);
            foreach (var property in properties)
            {
                _Builder.AppendFormat(@"{0}{1} : {2};", indent, property.Name, property.Type);
                _Builder.Append("\n");
            }
        }

        private void WriteEnums(IEnumerable<EnumDefinition> enums)
        {
            string indent = new string('\t', 1);
            foreach (var @enum in enums)
            {
                _Builder.AppendFormat(@"{0}export enum {1} {{", indent, @enum.Type.Name);
                _Builder.Append("\n");
                foreach(var info in Enum.GetValues(@enum.Type))
                {
                    _Builder.AppendFormat(@"{0}{0}{1} = {2},", indent, Enum.GetName(@enum.Type, info), info.GetHashCode());
                    _Builder.Append("\n");
                }
                _Builder.AppendFormat(@"{0}}}", indent);
                _Builder.Append("\n");
            }
        }
    }

}
