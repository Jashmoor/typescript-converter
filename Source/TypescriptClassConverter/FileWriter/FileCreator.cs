using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TypescriptClassConverter.Models;

namespace TypescriptClassConverter.FileWriter
{
    internal class FileCreator
    {
        private readonly StringBuilder _Builder;
        private readonly TypeCollector _Collector;
        private readonly string _Path;
        private readonly int _Indent;

        internal FileCreator(string path, TypeCollector collector, int indent = 4, StringBuilder builder = null)
        {
            _Collector = collector;
            _Path = $"{path}.{_Collector.Method}";
            _Builder = builder ?? new();
            _Indent = indent;
        }

        public void CreateFile()
        {
            if (Directory.Exists(_Path))
                Directory.Delete(_Path, true);

            File.WriteAllText(_Path, TextBuilder());
        }

        private string Indent(int level = 1)
            => new('\t', (_Indent * level));

        private string TextBuilder()
        {
            _Builder.AppendLine($"export namespace {_Path} {{");
            _Builder.Append("\n");
            WriteEnums();
            _Builder.Append("\n");
            WriteInterfaces();
            _Builder.Append("\n");
            _Builder.Append("}");
            return _Builder.ToString();
        }

        private void WriteEnums()
        {
            foreach (Type @enum in _Collector.Enums)
            {
                _Builder.AppendFormat(@"{0}export enum {1} {{ \n", Indent(2), @enum.Name);
                foreach (string member in Enum.GetNames(@enum))
                {
                    _Builder.AppendFormat(@"{0}{1} = {2}", Indent(3), member, member.GetHashCode());
                }
                _Builder.Append("\n");
            }
        }

        private void WriteInterfaces()
        {

        }
    }
}
