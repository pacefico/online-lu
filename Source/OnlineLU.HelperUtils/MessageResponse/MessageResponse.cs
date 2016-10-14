using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.HelperUtils.MessageResponse
{
    public class MessageResponse <TResult>
    {
        private List<MessageResponseException> m_Exceptions = new List<MessageResponseException>();

        public TResult Result { get; set; }

        public List<MessageResponseException> Exceptions
        {
            get { return m_Exceptions; }
            set { m_Exceptions = value; }
        }

        public MessageResponse()
            : this(null, ResponseKind.Info)
        {
        }

        public MessageResponse(string responseMessage, ResponseKind responseKind)
        {
            if (!string.IsNullOrEmpty(responseMessage))
            {
                this.Exceptions.Add(new MessageResponseException(responseMessage, responseKind));
            }
        }

        public MessageResponse(string errorMessage)
            : this()
        {
            this.Exceptions.Add(new MessageResponseException(errorMessage, ResponseKind.Error));
        }

        public MessageResponse(TResult result)
            : this()
        {
            this.Result = result;
        }

        public bool HasErrors()
        {
            return this.HasErrorKind(ResponseKind.Error);
        }

        private bool HasErrorKind(ResponseKind responseKind)
        {
            return this.Exceptions.Where(a => a.Kind == responseKind).Any();
        }
    }
}
