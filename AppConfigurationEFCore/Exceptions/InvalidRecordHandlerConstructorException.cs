using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfigurationEFCore.Exceptions
{
    public class InvalidRecordHandlerConstructorException : Exception
    {
        public InvalidRecordHandlerConstructorException(string message) : base(message)
        {
        }

        public InvalidRecordHandlerConstructorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidRecordHandlerConstructorException() : base("Invoking RecordHandler constructor did not resulted object creation")
        {
        }
    }
}
