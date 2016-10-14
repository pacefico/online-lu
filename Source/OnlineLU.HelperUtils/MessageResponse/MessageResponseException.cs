using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLU.HelperUtils;

namespace OnlineLU.HelperUtils.MessageResponse
{
    public class MessageResponseException
    {
        public Exception InnerException { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
        public ResponseKind Kind { get; set; }

        public MessageResponseException() : this(null)
        {
        
        }

        public MessageResponseException(string description) :
            this(description, ResponseKind.Info)
        {

        }

        public MessageResponseException(string description, ResponseKind kindException)
            : this(null, description, kindException)
        {
        }

        public MessageResponseException(Exception exception, string description, ResponseKind kindException)
        {
            this.InnerException = exception;
            this.Kind = kindException;
            this.Description = description;
        }
    }
}
