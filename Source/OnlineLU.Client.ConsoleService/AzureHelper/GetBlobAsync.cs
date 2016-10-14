using OnlineLU.Client.ConsoleService.ConverterHelper;
using OnlineLU.Client.ConsoleService.ZipHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLU.Client.ConsoleService.AzureHelper
{
    public class GetBlobAsync : IDisposable
    {
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

        private float[] matrix;
        private MeasuringTime[] m_Measuring;
        private bool m_ShowSummary;

        public GetBlobAsync(ref MeasuringTime[] measuring, int simultaneous, int range, ref float[] matrix, bool showSummary)
        {
            this.m_ShowSummary = showSummary;
            m_Range = range;
            m_Measuring = measuring;
            m_Watcher = Stopwatch.StartNew();
            m_Measuring[m_Range].InitialTime = m_Watcher.ElapsedMilliseconds;

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
                Console.WriteLine("             Downloading: " + _blobinfo.BlobName);
            }
            _blobinfo.Times.InitialTransfer = m_Watcher.ElapsedMilliseconds;
            bool _success = RestBlob.GetBlob(_blobinfo);
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
                _blobinfo.Times.TotalByteLength = _blobinfo.BlobStreamSource.Length;

                    TypeConverter.ByteToFloat(
                        ZipHelperManager.UnzipStreamToByte(_blobinfo.BlobStreamSource, _blobinfo.Range, _blobinfo.PrecisionChar), 
                        ref matrix, 
                        _blobinfo.Id,
                        _blobinfo.Range,
                        _blobinfo.PrecisionChar);
                    
                _blobinfo.Times.FinalTime = m_Watcher.ElapsedMilliseconds;
                lock (_lockerMatrix)
                {
                    m_Measuring[_blobinfo.Id] = _blobinfo.Times;
                }
                if (m_ShowSummary)
                {
                    Console.WriteLine("                 Finished: " + _blobinfo.BlobName + " in " + Convert.ToString(_blobinfo.Times.GetQueueTime()) + "ms / Transfer Rate : " + Convert.ToString(_blobinfo.Times.GetRateTransfer()) + " bytes/ms");
                }
            }
            m_semaphore.Release();
        }
     
        public void Dispose()
        {
            Task.WaitAll(m_tasks.ToArray());
            Console.WriteLine("Timeout Errors: " + m_ErrorTimeOut.ToString());
            EnqueueTask(null);     // Signal the consumer to exit.
            _worker.Join();         // Wait for the consumer's thread to finish.
            _wh.Close();

            m_Watcher.Stop();
            m_Measuring[m_Range].FinalTime = m_Watcher.ElapsedMilliseconds;
            m_Watcher = null;

            Console.WriteLine("GetBlobAsync Timeout Errors: " + m_ErrorTimeOut.ToString() + " Total Time: " + m_Measuring[m_Range].GetTotalTime().ToString() + "ms");

        }


    }
}
