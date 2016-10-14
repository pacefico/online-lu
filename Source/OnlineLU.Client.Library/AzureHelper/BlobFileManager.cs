using OnlineLU.Client.Library.ConverterHelper;
using OnlineLU.Client.Library.Events;
using OnlineLU.Client.Library.Models;
using OnlineLU.Client.Library.Resolve;
using OnlineLU.Client.Library.ZipHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OnlineLU.Client.Library.AzureHelper
{
    public class BlobFileManager
    {
        public event EventHandler<SummaryEventArgs> SummaryChange;

        public BlobFileManager()
        {
        
        }

        public HistoryTransfer GetMatrixFromBlob(int range, string container, ref float[] matrix, bool showSummary, int nthreads, long projectid)
        {
            var _response = new HistoryTransfer();
            try
            {
                 //para o parser da string de precisão é necessário considerar os caracteres "0" "." e ";"

                var watcher = Stopwatch.StartNew();

                int _bytesToRead = range * OnlineLUConstants.PrecisionChar;
                
                GetBlobAsync _queue = new GetBlobAsync(nthreads, range, ref matrix, showSummary);
                
                    _queue.SummaryChange += RaiseSummaryEvent;

                    for (int i = 0; i < range; i++)
                    {
                        var _blobFileDes = new BlobInfo(i, container, range, OnlineLUConstants.PrecisionChar);
                        _blobFileDes.SetBlobName(projectid);
                        _blobFileDes.Times.InitialTime = watcher.ElapsedMilliseconds;

                        _queue.EnqueueTask(_blobFileDes);
                        if (i == range - 1)
                        {
                            if (showSummary)
                            {
                                SendSummaryEvent("Enqueue Complete");
                            }
                        }
                        Thread.Sleep(5);
                    }
                    _queue.Dispose();

                long _bytesReaded = _queue.BytesReaded;
                
                watcher.Stop();

                //for (int i = 0; i < range * range; i++)
                //{
                //    if (matrix[i] == 0.0000000)
                //    {
                //        SendSummaryEvent("0 em: " + i.ToString());
                //        //_response.Success = false;
                //        //return _response;
                //    }
                //}

                long _timeQueue = watcher.ElapsedMilliseconds;  // measuring[range].GetTotalTime();

                if (showSummary)
                {
                    //SendSummaryEvent("Erros Medição tempo: " + _errosMeasuring.ToString());
                    SendSummaryEvent("Bytes download total: " + Convert.ToString(_bytesReaded));
                    SendSummaryEvent("Time download total: " + Convert.ToString(_timeQueue));
                }

                double _bytes = _bytesReaded; //kb
                
                double _rate = _bytes / _timeQueue;
                //Console.WriteLine("Rate Download: " + Convert.ToString(_rate));

                SendSummaryEvent(string.Format("Download: {0} bytes | Rate: {1} kb/s | Tempo: {2} ms", _bytes, string.Format("{0:0.00}", _rate), _timeQueue));
                _response.Bytes = _bytes.ToString();
                _response.RateKbs = string.Format("{0:0.00}", _rate);
                _response.TimeMs = _timeQueue;
                _response.Success = true;
            }
            catch (Exception ex)
            {
                SendSummaryEvent("[BlobFileManager] Erro GetMatrixFromBlob, Exception: " + ex.Message);
                _response.ErrorMessage = ex.Message;
                _response.Success = false;
                return _response;
            }
            _response.Success = true;
            return _response;
        }

        public HistoryTransfer PutMatrixOnBlob(int range, string container, ref LuMatrix luMatrix, bool showSummary, int nthreads, long projectid)
        {
            var _response = new HistoryTransfer();
            try
            {
                var watch = Stopwatch.StartNew();

                long _bytesUploaded = 0;
                int _bytesToRead = range * OnlineLUConstants.PrecisionChar;

                using (PutBlobAsync _queue = new PutBlobAsync(nthreads, range, showSummary))
                {
                    _queue.SummaryChange += RaiseSummaryEvent;

                    int k = 0;
                    StringBuilder _strBuilder = new StringBuilder();
                    byte[] _bytesReaded = new byte[_bytesToRead];
                    for (int i = 0; i < range; i++)
                    {
                        var _blobFile = new BlobInfo(i, container, range, OnlineLUConstants.PrecisionChar);
                        _blobFile.SetBlobName(projectid);
                        _blobFile.Times.InitialTime = watch.ElapsedMilliseconds;

                        if (showSummary)
                        {
                            SendSummaryEvent("Initializing byte[] read: " + i);
                        }

                        for (int j = 0; j < range; j++)
                        {
                            _strBuilder.Append(string.Format("{0:0.0000000};", luMatrix.n[k]));
                            k++;
                        }
                        _bytesReaded = Encoding.UTF8.GetBytes(_strBuilder.ToString());
                        _strBuilder.Clear();

                        _blobFile.BlobByteSource = ZipHelperManager.ZipByteToByte(ref _bytesReaded, i);
                        _blobFile.Times.PreparingBytes = watch.ElapsedMilliseconds - _blobFile.Times.InitialTime;
                        _blobFile.Times.TotalByteLength = _blobFile.BlobByteSource.LongLength;

                        _bytesUploaded += _blobFile.BlobByteSource.LongLength;
                        _queue.EnqueueTask(_blobFile);

                        if (i == range - 1)
                        {
                            if (showSummary)
                            {
                                SendSummaryEvent("Enqueue complete");
                            }
                        }
                    }
                }
                watch.Stop();

                long _timeQueue = watch.ElapsedMilliseconds;

                if (showSummary)
                {
                    SendSummaryEvent("Bytes upload total: " + Convert.ToString(_bytesUploaded));
                    SendSummaryEvent("Time upload total: " + Convert.ToString(_timeQueue));
                }
                double _rate = _bytesUploaded / _timeQueue;

                SendSummaryEvent(string.Format("Upload: {0} bytes | Rate: {1} kb/s | Tempo: {2} ms", _bytesUploaded, string.Format("{0:0.00}", _rate), _timeQueue));
                _response.Bytes = _bytesUploaded.ToString();
                _response.RateKbs = string.Format("{0:0.00}", _rate);
                _response.TimeMs = _timeQueue;
                _response.Success = true;
            }
            catch (Exception ex)
            {
                SendSummaryEvent("[BlobFileManager] Erro PutMatrixOnBlob, Exception: " + ex.Message);
                _response.ErrorMessage = ex.Message;
                _response.Success = false;
                return _response;
            }
            
            return _response;
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
    }
}
