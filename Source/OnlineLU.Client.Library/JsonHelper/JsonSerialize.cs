using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace OnlineLU.Client.Library.JsonHelper
{
    public static class JsonSerialize
    {
        public static string SerializeQueueMessage(QueueMessage obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static QueueMessage DeserializeQueueMessage(string jsonString)
        {
            var jsonObj = new JavaScriptSerializer().Deserialize<QueueMessage>(jsonString);//  DeserializeObject(jsonString);
            return (jsonObj as QueueMessage);
        }

        public static HistoryModel DeserializeHistory(string jsonString)
        {
            return new JavaScriptSerializer().Deserialize<HistoryModel>(jsonString);
        }

        public static string SerializeHistory(HistoryModel history)
        {
            return new JavaScriptSerializer().Serialize(history);
        }
    }
}
