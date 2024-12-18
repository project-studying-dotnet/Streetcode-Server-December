using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Cmp;

namespace Streetcode.BLL.Exceptions
{
    public class ForbiddenAccessException : HttpException
    {
        public override int StatusCode => StatusCodes.Status403Forbidden;
        public override string Message => "Forbidden access";
    }
}
