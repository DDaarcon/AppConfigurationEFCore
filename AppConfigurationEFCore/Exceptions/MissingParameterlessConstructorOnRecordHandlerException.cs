using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConfigurationEFCore.Exceptions
{
    public class MissingParameterlessConstructorOnRecordHandlerException : Exception
    {
        public MissingParameterlessConstructorOnRecordHandlerException(string message) : base(message)
        {
        }

        public MissingParameterlessConstructorOnRecordHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MissingParameterlessConstructorOnRecordHandlerException() : base("RecordHandler is missing parameterless constructor.")
        {
        }
    }
}
