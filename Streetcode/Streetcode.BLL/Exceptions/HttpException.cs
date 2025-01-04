namespace Streetcode.BLL.Exceptions
{
    public abstract class HttpException : Exception
    {
        public abstract int StatusCode { get; }
    }
}
