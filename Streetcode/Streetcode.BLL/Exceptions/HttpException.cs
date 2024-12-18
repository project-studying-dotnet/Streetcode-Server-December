using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Streetcode.BLL.Exceptions
{
    public abstract class HttpException : Exception
    {
        public abstract int StatusCode { get; }
    }
}
