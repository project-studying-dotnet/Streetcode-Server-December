using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validators.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(List<ValidationError> errors)
        {
            Errors = errors;
        }

        public List<ValidationError> Errors { get; }
        public override string ToString()
        {
            var errorMessages = string.Join(Environment.NewLine, Errors.Select(e => e.ToString()));
            return $"{Message}: {Environment.NewLine}{errorMessages}";
        }
    }
}
