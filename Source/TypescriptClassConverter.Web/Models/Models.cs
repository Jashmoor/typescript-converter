using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TypescriptClassConverter.Web.Models
{
    public interface IRequest
    {
    }

    public interface IRequest<T>
    {
        public T Body { get; set; }
    }

    public enum Status : int
    {
        Good,
        Ok,
        Bad
    }

    public class TestRequest : IRequest<string>
    {
        public Status Status { get; set; }
        public string Body { get; set; }
        public int? Id { get; set; }
    }

    public class Base<T>
    {
        public T Test { get; set; }
    }

    public class TestResponse : Base<int>
    {
        public string ResponseId { get; set; }
        public DateTime ResponseDate { get; set; }
    }

    public class TestOtherRequest : TestRequest
    {
        public bool Working { get; set; }
        public object Object { get; set; }
    }

}
