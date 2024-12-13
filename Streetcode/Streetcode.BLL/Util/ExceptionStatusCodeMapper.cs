using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Streetcode.BLL.Exceptions;

namespace Streetcode.BLL.Util
{
    public class ExceptionStatusCodeMapper
    {
        public static int MapToStatusCode(Exception exception)
        {
            return exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                Exceptions.UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ForbiddenAccessException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
