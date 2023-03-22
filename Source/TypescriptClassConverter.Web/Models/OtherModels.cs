using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TypescriptClassConverter.Web.Models
{
    public interface IEmptyInterface<out T> { }


    public class EmptyRequest : IEmptyInterface<EmptyResponse>
    {
        public string Name { get; set; }
        public int? Id { get; set; }
    }

    public class EmptyResponse
    {
        public (string String, int Int) Tuple { get; set; }
    }
}
