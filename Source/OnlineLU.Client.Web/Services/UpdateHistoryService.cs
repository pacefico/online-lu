using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.Models;
using OnlineLU.Server.BTC;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace OnlineLU.Client.Web.Services
{
    public class UpdateHistoryService : IDisposable
    {
        private Thread m_Thread;
        private bool m_Started;
        private QueueHelper m_QueueHelper;

        public UpdateHistoryService()
        {
            this.m_QueueHelper = new QueueHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);
        }

        private void ProcessUpdate()
        {
            string _message;
            string _messageID;
            string _popReceipt;

            while (true)
            {
                if (m_QueueHelper.GetMessage(OnlineLUConstants.QueueNameResult, out _message, out _messageID, out _popReceipt))
                {
                    var _queueMessage = OnlineLU.Client.Library.JsonHelper.JsonSerialize.DeserializeHistory(_message);

                    if (_queueMessage.ProjectID > 0)
                    {
                        var _paramTO = this.TranslatorQueueMessage(_queueMessage);

                        var _respBTC = new HomeBTC().SetHistory(_paramTO);

                        if (!_respBTC.HasErrors())
                        {
                            m_QueueHelper.DeleteMessage(OnlineLUConstants.QueueNameResult, _messageID, _popReceipt);
                        }
                    }
                    else
                    {
                        var _newParamTO = new ProjectTO()
                        {
                            Status = false,
                            ContainerName = OnlineLUConstants.ContainerName,
                            ContainerNameResult = OnlineLUConstants.ContainerNameResult,
                            QueueName = OnlineLUConstants.QueueName,
                            QueueNameResult = OnlineLUConstants.QueueNameResult,
                            Range = _queueMessage.queueMessage.range,
                        };

                        var _btcResp = new HomeBTC().SaveExecution(_newParamTO);

                        if (!_btcResp.HasErrors())
                        {
                            _queueMessage.ProjectID = _btcResp.Result;

                            var _paramTO = this.TranslatorQueueMessage(_queueMessage);

                            var _respBTC = new HomeBTC().SetHistory(_paramTO);

                            if (!_respBTC.HasErrors())
                            {
                                m_QueueHelper.DeleteMessage(OnlineLUConstants.QueueNameResult, _messageID, _popReceipt);
                            }
                        }
                    }
                }
                else
                {
                }

                Thread.Sleep(1000);
            }
        }

        private SetHistoryParamTO TranslatorQueueMessage(HistoryModel historyModel)
        {
            SetHistoryParamTO _response = new SetHistoryParamTO()
            {
                History = new HistoryTO()
            };

            var _historyTO = new HistoryTO();
            _historyTO.ExecutionDate = historyModel.ExecutionDate;
            _historyTO.TimeCudaMs = historyModel.TimeCudaMs;
            _historyTO.Success = historyModel.Success;
            _historyTO.Range = historyModel.queueMessage.range;
            _historyTO.TotalTime = historyModel.TotalTime;
            _historyTO.ProjectID = historyModel.ProjectID;

            if (historyModel.Download != null)
            {
                _historyTO.TimeDownload = historyModel.Download.TimeMs;
                
                if (_historyTO.Detail == null)
                {
                    _historyTO.Detail = new HistoryDetailTO();
                }
                _historyTO.TimeDownload = historyModel.Download.TimeMs;
                _historyTO.Detail.ByteDownload = historyModel.Download.Bytes;
                _historyTO.Detail.RateDownload = historyModel.Download.RateKbs;
            }
            
            if (historyModel.Upload != null)
            {
                if (_historyTO.Detail == null)
                {
                    _historyTO.Detail = new HistoryDetailTO();
                }
                _historyTO.TimeUpload = historyModel.Upload.TimeMs;
                _historyTO.Detail.ByteUpload = historyModel.Upload.Bytes;
                _historyTO.Detail.RateUpload = historyModel.Upload.RateKbs;
            }

            if (historyModel.HardwareInfo != null)
            {
                _historyTO.Hardware = new HardwareTO()
                {
                    CoreNumber = historyModel.HardwareInfo.System.Cores,
                    CudaCapable = historyModel.HardwareInfo.System.CudaCapable,
                    HardwareKey = historyModel.HardwareInfo.HardwareKey,
                    MemoryAmount = historyModel.HardwareInfo.System.Memory,
                    ProcessorName = historyModel.HardwareInfo.System.Name,
                    SystemName = historyModel.HardwareInfo.System.SystemName,
                };
            }
            _response.History = _historyTO;

            return _response;
        }

        public void Start()
        {
            if (!m_Started)
            {
                this.m_Thread = new Thread(ProcessUpdate);
                this.m_Thread.IsBackground = true;
                this.m_Thread.Start();
                this.m_Started = true;
            }
        }

        public void Dispose()
        {
            m_Thread.Join();
            m_QueueHelper = null;
        }
    }
}