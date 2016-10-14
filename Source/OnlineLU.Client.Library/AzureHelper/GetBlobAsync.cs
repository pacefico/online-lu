using OnlineLU.Client.Library.ConverterHelper;
using OnlineLU.Client.Library.Events;
using OnlineLU.Client.Library.ZipHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.Library.AzureHelper
{
    public class GetBlobAsync : IDisposable
    {
        public event EventHandler<SummaryEventArgs> SummaryChange;

        readonly object _locker = new object();
        readonly object _lockerError = new object();
        readonly object _lockerMatrix = new object();

        private List<Task> m_tasks = new List<Task>();
        private SemaphoreSlim m_semaphore;

        private static int m_MaxSimultaneous;
        private int m_ErrorTimeOut = 0;
        EventWaitHandle _wh = new AutoResetEvent(false);
        Thread _worker;
        private Queue<BlobInfo> m_queue = new Queue<BlobInfo>();

        private Stopwatch m_Watcher;

        private int m_Range;
        public long BytesReaded;

        private float[] matrix;
        
        private bool m_ShowSummary;

        private BlobHelper m_BlobHelper;

        public GetBlobAsync(int simultaneous, int range, ref float[] matrix, bool showSummary)
        {
            m_BlobHelper = new BlobHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);
            this.m_ShowSummary = showSummary;
            m_Range = range;
            m_Watcher = Stopwatch.StartNew();

            m_MaxSimultaneous = simultaneous;
            this.matrix = matrix;

            System.Net.ServicePointManager.DefaultConnectionLimit = simultaneous;
            m_semaphore = new SemaphoreSlim(simultaneous);
            
            _worker = new Thread(Work);
            _worker.Start();
        }

        public void EnqueueTask(BlobInfo task)
        {
            lock (_locker)
            {
                if (task != null)
                {
                    task.Times.QueueInitial = m_Watcher.ElapsedMilliseconds;
                }
                m_queue.Enqueue(task);
            }
            _wh.Set();
        }

        private void Work()
        {
            while (true)
            {
                BlobInfo task = null;
                lock (_locker)
                    if (m_queue.Count > 0)
                    {
                        task = m_queue.Dequeue();
                        if (task == null) return;
                    }
                if (task != null)
                {
                    task.Times.QueueFinal = m_Watcher.ElapsedMilliseconds;
                    if (m_ShowSummary)
                    {
                        SendSummaryEvent("         Performing task: " + task.Id);
                    }
                    m_tasks.Add(Task.Factory.StartNew(ProcessTaskRestBlob, task));
                }
                else
                    _wh.WaitOne();         // No more tasks - wait for a signal
            }
        }

        private void ProcessTaskRestBlob(Object obj)
        {
            var _blobinfo = (BlobInfo)obj;
            m_semaphore.Wait();
            if (m_ShowSummary)
            {
                SendSummaryEvent("             Downloading: " + _blobinfo.BlobName);
            }
            _blobinfo.Times.InitialTransfer = m_Watcher.ElapsedMilliseconds;
            bool _success = m_BlobHelper.GetBlob(_blobinfo);
            _blobinfo.Times.FinalTransfer = m_Watcher.ElapsedMilliseconds;

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
                    //Console.WriteLine("######## Erro em: " + _blobinfo.BlobName);
                }
            }
            else
            {
                _blobinfo.Times.TotalByteLength = _blobinfo.BlobStreamSource.Length;

                lock (matrix)
                {
                    BytesReaded += _blobinfo.BlobStreamSource.Length;

                   TypeConverter.ByteToFloat(
                        ZipHelperManager.UnzipStreamToByte(_blobinfo.BlobStreamSource, _blobinfo.Range, _blobinfo.PrecisionChar),
                        ref matrix,
                        _blobinfo.Id,
                        _blobinfo.Range,
                        _blobinfo.PrecisionChar);

                    _blobinfo.Times.FinalTime = m_Watcher.ElapsedMilliseconds;
   
                }
                if (m_ShowSummary)
                {
                    SendSummaryEvent("                 Finished: " + _blobinfo.BlobName + " in " + Convert.ToString(_blobinfo.Times.GetQueueTime()) + "ms / Transfer Rate : " + Convert.ToString(_blobinfo.Times.GetRateTransfer()) + " bytes/ms");
                }
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
            //Console.WriteLine("Timeout Errors: " + m_ErrorTimeOut.ToString());
            EnqueueTask(null);     // Signal the consumer to exit.
            _worker.Join();         // Wait for the consumer's thread to finish.
            _wh.Close();

            m_Watcher.Stop();
            m_Watcher = null;

            if (m_ShowSummary)
            {
                SendSummaryEvent("GetBlobAsync Timeout Errors: " + m_ErrorTimeOut.ToString());
            }
        }


    }
}
