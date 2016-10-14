using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.ConsoleService.AzureHelper
{
    public class QueueInfoBase
    {
        protected string m_QueueName;

        public string QueueName
        {
            get
            {
                return this.m_QueueName;
            }
        }

        public QueueInfoBase()
        {

        }

        public QueueInfoBase(string QueueName)
        {
            this.m_QueueName = QueueName;
        }


    }

    public class QueueInfoMessage : QueueInfoBase
    {
        private string m_SourceMessage;

        public string SourceMessage
        {
            get
            {
                return this.m_SourceMessage;
            }
            set
            {
                this.m_SourceMessage = value;
            }
        }

        public QueueInfoMessage(string QueueName)
        {
            this.m_QueueName = QueueName;
        }
    }
}
