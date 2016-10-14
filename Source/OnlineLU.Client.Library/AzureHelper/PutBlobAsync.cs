using OnlineLU.Client.Library.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.AzureHelper
{
    public class PutBlobAsync : IDisposable
    {
        public event EventHandler<SummaryEventArgs> SummaryChange;

        readonly object _locker = new object();
        readonly object _lockerError = new object();

        private Queue<BlobInfo> m_queue = new Queue<BlobInfo>();

        private List<Task> m_tasks = new List<Task>();
        private SemaphoreSlim m_semaphore;
        private Thread _worker;
        private EventWaitHandle _wh = new AutoResetEvent(false);

        private static int m_MaxSimultaneous = OnlineLUConstants.ThreadUpload;
        private int m_ErrorTimeOut = 0;

        private int m_Range;
        private bool m_ShowSummary;
        private BlobHelper m_BlobHelper;

        public PutBlobAsync(int simultaneous, int range, bool showSummary)
        {
            m_ShowSummary = showSummary;
            m_BlobHelper = new BlobHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);
            m_Range = range;
           
            m_MaxSimultaneous = simultaneous;
            System.Net.ServicePointManager.DefaultConnectionLimit = simultaneous;
            m_semaphore = new SemaphoreSlim(m_MaxSimultaneous);

            _worker = new Thread(Work);
            _worker.Start();
        }

        public void EnqueueTask(BlobInfo task)
        {
            lock (_locker)
            {
                m_queue.Enqueue(task);
            }
            _wh.Set();
        }

        private void Work()
        {
            while (true)
            {
                BlobInfo taskObject = null;
                lock (_locker)
                    if (m_queue.Count > 0)
                    {
                        taskObject = m_queue.Dequeue();
                        if (taskObject == null) return;
                    }
                if (taskObject != null)
                {
                    if (m_ShowSummary)
                    {
                        SendSummaryEvent("         Performing task: " + taskObject.Id);
                        //Console.WriteLine("         Performing task: " + task.Id);
                    }
                    m_tasks.Add(Task.Factory.StartNew(ProcessTaskRestBlob, taskObject));
                }
                else
                    _wh.WaitOne();         // No more tasks - wait for a signal
            }
        }

        private void ProcessTaskRestBlob(Object obj)
        {
            var _blobinfo = (BlobInfo)obj;
            m_semaphore.Wait();
            Thread.CurrentThread.Name = "BlobInfo - line: " + _blobinfo.Id.ToString();
            if (m_ShowSummary)
            {
                SendSummaryEvent("             Uploading: " + _blobinfo.BlobName);
            }
            bool _success = m_BlobHelper.PutBlob(_blobinfo);
            if (!_success)
            {
                lock (_lockerError)
                {
                    m_ErrorTimeOut += 1;
                }
                _blobinfo.Retries += 1;
                EnqueueTask(_blobinfo);
                if (m_ShowSummary)
                {
                    SendSummaryEvent("######## Erro em: " + _blobinfo.BlobName);
                }
            }
            else
            {
                if (m_ShowSummary)
                {
                    SendSummaryEvent("                 Finished: " + _blobinfo.BlobName + " in " + _blobinfo.Times.GetTotalTime() + "ms / Transfer rate: " + Convert.ToString(_blobinfo.Times.GetRateTransfer()) + " bytes/ms");
                }
                _blobinfo.BlobByteSource = null;
                _blobinfo = null;
                obj = null;
            }
            m_semaphore.Release();
        }

        private void SendSummaryEvent(string Message, bool clear = false)
        {
            RaiseSummaryEvent(null, new SummaryEventArgs(Message, clear));
        }

        private void RaiseSummaryEvent(object sender, SummaryEventArgs e)
        {
            if (SummaryChange != null)
            {
                SummaryChange(sender, e);
            }
        }

        public void Dispose()
        {
            Task.WaitAll(m_tasks.ToArray());
            EnqueueTask(null);     // Signal the consumer to exit.
            _worker.Join();         // Wait for the consumer's thread to finish.
            _wh.Close();

            if (m_ShowSummary)
            {
                SendSummaryEvent("PutBlobAsync Timeout Errors: " + m_ErrorTimeOut.ToString());
            }
        }


    }
}
