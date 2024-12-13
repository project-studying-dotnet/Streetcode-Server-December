using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Exceptions
{
    public class ValidationException(string message, IDictionary<string, string> errors) : Exception(message)
    {
        public IDictionary<string, string> Errors { get; } = errors;
    }
}
