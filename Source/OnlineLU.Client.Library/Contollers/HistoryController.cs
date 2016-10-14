using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.JsonHelper;
using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.Contollers
{
    public class HistoryController
    {
        private static string m_QueueResult = "luprocessed";
        private QueueHelper m_QueueHelper;

        public HistoryController()
        {
            m_QueueHelper = new QueueHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);
        }

        public bool SendHistoryResult(HistoryModel history)
        {
            if (history.Success)
            {
                m_QueueHelper.DeleteMessage(history.queueMessage.queueName, history.queueMessage.messageID, history.queueMessage.popReceipt);
                
            }
            string _message = JsonSerialize.SerializeHistory(history);
            return m_QueueHelper.PutMessage(m_QueueResult, _message);
            
        }
    }
}
