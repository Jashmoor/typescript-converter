using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypescriptClassConverter.FileWriter
{
    public class CodeWriter
    {
        private readonly StringBuilder _Writer;

        protected CodeWriter(StringBuilder writer = null)
            => _Writer = writer ?? new StringBuilder();

        public void Reset()
        {
            _Writer.Clear();
        }

        public override string ToString()
            => _Writer.ToString();

        public void Indent(string text,int level = 0)
        {
            string indent = level == 0
                ? ""
                : new string('\t', level);
            _Writer.AppendFormat(@"{0}{1}\n",indent, text);
        }

        public void WriteProperties(string text)
        {

        }

        public void Write(string line)
        {
            _Writer.Append(line);
        }
    }
}
