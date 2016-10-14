using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.Events;
using OnlineLU.Client.Library.Models;
using OnlineLU.Client.Library.Resolve;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.Contollers
{
    public class ProcessController : IDisposable
    {
        public event EventHandler<SummaryEventArgs> SummaryChange;
        public event EventHandler FinishChange;
        public event EventHandler<HistoryEventArgs> HistoryEventChange;

        private Thread m_ThreadQueue;
        private bool m_ListenQueue = false;
        private bool m_Started = false;
        private int m_TimeIntervalQueue = 10000;

        private BlobFileManager m_BlobFileManager;
        private QueueHelper m_QueueHelper;
        private bool m_ShowSummary;
        private Stopwatch m_Stopwatch;
        private long m_InitialTime;
        

        public ProcessController()
        {
            m_QueueHelper = new QueueHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);
            m_BlobFileManager = new BlobFileManager();
            m_BlobFileManager.SummaryChange += RaiseSummaryEvent;
            m_Stopwatch = Stopwatch.StartNew();
        }

        private void ListeningQueue(Object stateInfo)
        {
            if (m_ShowSummary)
            {
                SendSummaryEvent("[ProcessController] - Iniciando verificação da fila");
            }

            string _message;
            string _messageID;
            string _popReceipt;
            
            while (m_ListenQueue)
            {
                if (m_QueueHelper.GetMessage(OnlineLUConstants.QueueName, out _message, out _messageID, out _popReceipt))
                {
                    this.m_ListenQueue = false;
                    this.m_Started = false;
                    this.m_InitialTime = m_Stopwatch.ElapsedMilliseconds;

                    var _queueMessage = JsonHelper.JsonSerialize.DeserializeQueueMessage(_message);
                    SendSummaryEvent("[ProcessController] - Nova Tarefa Encontrada: Ordem " + _queueMessage.range);
                    _queueMessage.messageID = _messageID;
                    _queueMessage.popReceipt = _popReceipt;
                    _queueMessage.queueName = OnlineLUConstants.QueueName;

                    InitializeProcessing(_queueMessage);
                }
                else
                {
                    SendSummaryEvent(
                        string.Format("[ProcessController] - Nenhuma tarefa até o momento: {0} - {1} ",
                        DateTime.Now.ToShortDateString(),
                        DateTime.Now.ToShortTimeString()), true);
                }

                Thread.Sleep( m_TimeIntervalQueue);
            }
        }

        private void InitializeProcessing(QueueMessage queueMessage)
        {
            if (queueMessage != null)
            {
                this.m_ListenQueue = false;
                this.Processing(queueMessage);
            }
            else
            {
                SendSummaryEvent("Erro em QueueMessageJson");
            }
        }

        private void Processing(QueueMessage queueMessage)
        {
            var _history = new HistoryModel();
            _history.ExecutionDate = DateTime.Now;
            _history.queueMessage = queueMessage;


            int range = queueMessage.range;
            float[] matrix = new float[range * range];

            try
            {
                if (m_BlobFileManager == null)
                {
                    this.m_BlobFileManager = new BlobFileManager();
                }

                m_QueueHelper.DeleteMessage(queueMessage.queueName, queueMessage.messageID, queueMessage.popReceipt);
                
                SendSummaryEvent("Downloading...");
                var _historyDownload = m_BlobFileManager.GetMatrixFromBlob(range, queueMessage.containerSource, ref matrix, m_ShowSummary, OnlineLUConstants.ThreadDownload, queueMessage.projectid);

                _history.Download = _historyDownload;
                if (_historyDownload.Success && matrix != null)
                {
                    if (m_ShowSummary)
                    {
                        SendSummaryEvent("Download dos arquivos da matriz concluido com sucesso");
                    }
                    LuMatrix _luParam = new LuMatrix(range, range, true);
                    _luParam.n = matrix;
                    if (m_ShowSummary)
                    {
                        SendSummaryEvent("Resolvendo decomposição LU utilizando CUDA");
                    }
                    long _initialtime = m_Stopwatch.ElapsedMilliseconds;

                    using (LuCuda _luCuda = new LuCuda())
                    {
                        _luCuda.ResolveLuBlock(ref _luParam);
                    }

                    long _finaltime = m_Stopwatch.ElapsedMilliseconds;
                    SendSummaryEvent(string.Format("Resolução CUDA | Tempo: {0} ms", _finaltime - _initialtime));
                    _history.TimeCudaMs = _finaltime - _initialtime;

                    SendSummaryEvent("Uploading...");
                    var _historyUpload = m_BlobFileManager.PutMatrixOnBlob(range, queueMessage.containerResult, ref _luParam, m_ShowSummary, 100, queueMessage.projectid);

                    _history.Upload = _historyUpload;
                    long _finalProcessTime = m_Stopwatch.ElapsedMilliseconds;
                    SendSummaryEvent(string.Format("Tempo Total do Processo: {0} ms", _finalProcessTime - m_InitialTime));
                    _history.TotalTime = _finalProcessTime - m_InitialTime;
                    _history.ProjectID = queueMessage.projectid;
                    _history.Success = true;
                    RaiseHistoryEvent(new HistoryEventArgs() { History = _history });
                    RaiseFinishEvent(null, new EventArgs());
                }
                else
                {
                    string _newMessage = JsonHelper.JsonSerialize.SerializeQueueMessage(queueMessage);
                    var _newQueueMessage = JsonHelper.JsonSerialize.DeserializeQueueMessage(_newMessage);

                    //                    m_QueueHelper.PutMessage(queueMessage.queueName, queueMessage.mes
                    SendSummaryEvent("Erro ao obter arquivos");
                    _history.Success = false;
                    RaiseHistoryEvent(new HistoryEventArgs() { History = _history });
                    RaiseFinishEvent(null, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                _history.Success = false;
                _history.ErrorMessage = ex.Message;
                RaiseHistoryEvent(new HistoryEventArgs() { History = _history });
                RaiseFinishEvent(null, new EventArgs());
            }
            finally
            {
                matrix = null;
                this.m_BlobFileManager = null;
            }
        }

        private void SendSummaryEvent(string Message, bool clear = false)
        {
            var _message = string.Format("[{0}] - {1} ", DateTime.Now.ToShortTimeString(), Message);
            RaiseSummaryEvent(null, new SummaryEventArgs(_message, clear));
        }
        
        private void RaiseSummaryEvent(object sender, SummaryEventArgs e)
        {
            if (SummaryChange != null)
            {
                SummaryChange(sender, e);
            }
        }

        private void RaiseFinishEvent(object sender, EventArgs e)
        {
            if (FinishChange != null)
            {
                FinishChange(sender, e);
            }
            if (this.m_ThreadQueue != null)
            {
                this.m_ThreadQueue.Join();
            }
        }

        private void RaiseHistoryEvent(HistoryEventArgs e)
        {
            if (this.HistoryEventChange != null)
            {
                HistoryEventChange(null, e);
            }
        }

        public void SetShowSummary(bool showSummary)
        {
            this.m_ShowSummary = showSummary;
        }

        public void Start(bool createQueue, string range)
        {
            if (createQueue)
            {
                if (range == "10" ||
                    range == "100" ||
                    range == "1000" ||
                    range == "5000" ||
                    range == "10000")
                {
                    string message = JsonHelper.JsonSerialize.SerializeQueueMessage(
                        new QueueMessage { containerSource = OnlineLUConstants.ContainerName, range = int.Parse(range), containerResult = OnlineLUConstants.ContainerNameResult });
                    m_QueueHelper.PutMessage(OnlineLUConstants.QueueName, message);
                    SendSummaryEvent("Mensagem criada na fila para processar ordem " + range);
                }
            }
            
            if (!m_Started)
            {
                this.m_ListenQueue = true;

                this.m_ThreadQueue = new Thread(ListeningQueue);
                this.m_ThreadQueue.IsBackground = true;
                this.m_ThreadQueue.Start();
                this.m_Started = true;
            }
        }

        public void Stop()
        {
            this.m_ListenQueue = false;
        }

        public void Dispose()
        {
        
        }
    }
}