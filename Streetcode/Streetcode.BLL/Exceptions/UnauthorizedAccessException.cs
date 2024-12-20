using Microsoft.AspNetCore.Http;

namespace Streetcode.BLL.Exceptions
{
    public class UnauthorizedAccessException : HttpException
    {
        public override int StatusCode => StatusCodes.Status401Unauthorized;

        public override string Message => "Authorization is required.";
    }
}