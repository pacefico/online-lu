using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.ConsoleService.AzureHelper
{
    public class PutBlobAsync : IDisposable
    {
        readonly object _locker = new object();
        readonly object _lockerError = new object();
        readonly object _lockerTimes = new object();

        private Queue<BlobInfo> m_queue = new Queue<BlobInfo>();

        private List<Task> m_tasks = new List<Task>();
        private SemaphoreSlim m_semaphore;
        private Thread _worker;
        private EventWaitHandle _wh = new AutoResetEvent(false);

        private static int m_MaxSimultaneous = 200;
        private int m_ErrorTimeOut = 0;

        private Stopwatch m_Watcher;

        private int m_Range;
        private MeasuringTime[] m_Measuring;
        private bool m_ShowSummary;

        public PutBlobAsync(ref MeasuringTime[] measuring, int simultaneous, int range, bool showSummary)
        {
            m_Range = range;
            m_Measuring = measuring;
            m_Watcher = Stopwatch.StartNew();
            m_Measuring[m_Range].InitialTime = m_Watcher.ElapsedMilliseconds;
            
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
                        Console.WriteLine("         Performing task: " + task.Id);
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
                Console.WriteLine("             Uploading: " + _blobinfo.BlobName);
            }
            _blobinfo.Times.InitialTransfer = m_Watcher.ElapsedMilliseconds;
            bool _success = RestBlob.PutBlob(_blobinfo);
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
                    Console.WriteLine("######## Erro em: " + _blobinfo.BlobName);
                }
            }
            else
            {
                _blobinfo.Times.FinalTime = m_Watcher.ElapsedMilliseconds;
                lock (_lockerTimes)
                {
                    m_Measuring[_blobinfo.Id] = _blobinfo.Times;
                }
                if (m_ShowSummary)
                {
                    Console.WriteLine("                 Finished: " + _blobinfo.BlobName + " in " + _blobinfo.Times.GetTotalTime() + "ms / Transfer rate: " + Convert.ToString(_blobinfo.Times.GetRateTransfer()) + " bytes/ms");
                }
            }
            m_semaphore.Release();
        }
     
        public void Dispose()
        {
            Task.WaitAll(m_tasks.ToArray());
            EnqueueTask(null);     // Signal the consumer to exit.
            _worker.Join();         // Wait for the consumer's thread to finish.
            _wh.Close();

            m_Watcher.Stop();
            m_Measuring[m_Range].FinalTime = m_Watcher.ElapsedMilliseconds;
            m_Watcher = null;

            Console.WriteLine("PutBlobAsync Timeout Errors: " + m_ErrorTimeOut.ToString() + " Total Time: " + Convert.ToString(m_Measuring[m_Range].GetTotalTime()) + "ms");
        }


    }
}
