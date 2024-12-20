using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Streetcode.BLL.Exceptions
{
    public class InternalServerErrorException : HttpException
    {
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public override string Message => "Internal server error has occurred";
    }
}
