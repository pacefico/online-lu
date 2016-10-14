using Ionic.Zip;
using OnlineLU.Client.Library.ConverterHelper;
using OnlineLU.Client.Library.ZipHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace OnlineLU.Client.Library.AzureHelper
{
    public class TestFileManager
    {
        private string m_pathBase = @"c:\temp\";

        private string PathBase
        {
            get {
             if (!Directory.Exists(m_pathBase))
             {
                 Directory.CreateDirectory(m_pathBase);
             }
             return m_pathBase;
            }
            set {
                this.m_pathBase = value;
            }
        }

        public TestFileManager()
        {
        
        }

        private string CombinePathFile(int ordem)
        {
            return Path.Combine(PathBase, ordem.ToString());
        }

        public void CreateFile(int ordem)
        {
            using (StreamWriter file = new StreamWriter(CombinePathFile(ordem), true))
            {
                Random rand = new Random();
                for (int i = 0; i < ordem; i++)
                {
                    
                    Console.WriteLine(i);
                    for (int j = 0; j < ordem; j++)
                    {
                        file.Write(rand.NextDouble().ToString());
                        
                    }
                }
            }
        }

        /// <summary>
        /// Cria um arquivo de matriz contendo ordem X ordem items do tipo double randomicos com separador ";"
        /// com no caminho\nome padrão c:\temp\"ordem"
        /// </summary>
        /// <param name="ordem"></param>
        public void CreateFilePrecision(int ordem, int precision, string fileName)
        {
            Random rand = new Random();

            StringBuilder _strPrecision = new StringBuilder();
            _strPrecision.Append("{0:0.");
            for (int i = 0; i < precision; i++)
            {
                _strPrecision.Append("0");
            }
            _strPrecision.Append("};");

            string _precision = _strPrecision.ToString();
            using (StreamWriter file = new StreamWriter(fileName, false))
            {
                for (int i = 0; i < ordem; i++)
                {
                    //Console.WriteLine(i);
                    for (int j = 0; j < ordem; j++)
                    {
                        file.Write(string.Format(_precision, rand.NextDouble()));
                    }
                }
            }
        }

        public void CreateFile3(int ordem)
        {
            Random rand = new Random();

            using (StreamWriter file = new StreamWriter(CombinePathFile(ordem), false))
            {
                for (int i = 0; i < ordem; i++)
                {
                    Console.WriteLine(i);
                    for (int j = 0; j < ordem; j++)
                    {
                        file.Write(rand.Next(0,9));
                    }
                }
            }
        }

        public void CreateFileSplited(int ordem)
        {
            Random rand = new Random();
            string _path = Path.Combine(m_pathBase, ordem.ToString());

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            else
            {
                Directory.Delete(_path, true);
                Directory.CreateDirectory(_path);
            }

            for (int i = 0; i < ordem; i++)
            {
                using (StreamWriter fileLinha = new StreamWriter(_path + "\\" +  i))
                {
                    for (int j = 0; j < ordem; j++)
                    {
                        fileLinha.WriteLine(rand.NextDouble().ToString());
                    }
                }
            }
        }

        public void OpenFileToMatrix(int ordem, ref float[][] matrix)
        {
            if (File.Exists(CombinePathFile(ordem)))
            {
                StreamReader strRead = new StreamReader(CombinePathFile(ordem));
                for (int i = 0; i < ordem; i++)
                {
                    matrix[i] = new float[ordem];
                    char[] charReaded = new char[ordem];
                    strRead.ReadBlock(charReaded, 0, ordem);

                    for (int j = 0; j < ordem; j++)
                    {
                        matrix[i][j] = float.Parse(charReaded[j].ToString());
                    }
                }
            }
        }

        public void OpenFilePrecision(int ordem, ref float[][] matrix, int precision)
        {
            precision += 3; 
            if (File.Exists(CombinePathFile(ordem)))
            {
                using (StreamReader strRead = new StreamReader(CombinePathFile(ordem)))
                {
                    char[] charReaded = new char[precision];
                    string floatString;
                    for (int i = 0; i < ordem; i++)
                    {
                        matrix[i] = new float[ordem];

                        for (int j = 0; j < ordem; j++)
                        {
                            strRead.ReadBlock(charReaded, 0, precision);
                            floatString = new string(charReaded).Substring(0, precision - 1);
                            matrix[i][j] = float.Parse(floatString);
                        }
                    }
                }
            }
        }

        public long VerifyFileSize(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream _file = File.OpenRead(fileName);
                return _file.Length;
            }
            return 0;
        }

        public void OpenFileAndSendToBlob(int range, int precisionChar, string container, bool showSummary, int nthreads, long projectid)
        {
            precisionChar += 3; //para o parser da string de precisão é necessário considerar os caracteres "0" "." e ";"
            MeasuringTime[] measuring = new MeasuringTime[range + 2];
            measuring[range] = new MeasuringTime(-1); //tempos de processamento da fila
            measuring[range + 1] = new MeasuringTime(-2); //tempos de processamento deste método

            var watch = Stopwatch.StartNew();
            //measuring[range + 1].InitialTime = watch.ElapsedMilliseconds;
            if (File.Exists(CombinePathFile(range)))
            {
                int _bytesToRead = range * precisionChar;

                using (Stream _file = File.OpenRead(CombinePathFile(range)))
                {
                    using (PutBlobAsync _queue = new PutBlobAsync(nthreads, range, showSummary))
                    {
                        for (int i = 0; i < range; i++)
                        {
                            var _blobFile = new BlobInfo(i, container, range, precisionChar);
                            _blobFile.SetBlobName(projectid);
                            //_blobFile.Times.InitialTime = watch.ElapsedMilliseconds;

                            if (showSummary)
                            {
                                Console.WriteLine("Initializing byte[] read: " + i);
                            }
                            byte[] _bytesReaded = new byte[_bytesToRead];
                            _file.Read(_bytesReaded, 0, _bytesToRead);
  
                            _blobFile.BlobByteSource = ZipHelperManager.ZipByteToByte(ref _bytesReaded, i);
                            //_blobFile.Times.PreparingBytes = _blobFile.Times.InitialTime - watch.ElapsedMilliseconds;
                            //_blobFile.Times.TotalByteLength = _blobFile.BlobByteSource.LongLength;

                            _queue.EnqueueTask(_blobFile);

                            if (i == range-1)
                            {
                                Console.WriteLine("Enqueue complete");
                            }
                        }
                    }
                }
                
            }
            measuring[range + 1].FinalTime = watch.ElapsedMilliseconds; //tempo final de processamento deste método
            watch.Stop();

            long _bytesUploaded = 0;
            long _timeUploaded = 0;
            int _errosMeasuring = 0;
            for (int i = 0; i < range; i++)
            {
                if ((measuring[i] != null) && (measuring[i].TotalByteLength != null))
                {
                    _bytesUploaded += measuring[i].TotalByteLength;
                    _timeUploaded += measuring[i].GetTransferTime();
                }
                else
                {
                    _errosMeasuring += 1;
                    if ((i+1 >= 0) || (i+1 < range))
                    {
                        _bytesUploaded += measuring[i+1].TotalByteLength;
                        _timeUploaded += measuring[i+1].GetTransferTime();
                    }
                    else if ((i - 1 >= 0) || (i - 1 < range))
                    {
                        _bytesUploaded += measuring[i - 1].TotalByteLength;
                        _timeUploaded += measuring[i - 1].GetTransferTime();
                    }
                }
            }
            Console.WriteLine("Erros measuring: " + _errosMeasuring.ToString());
            Console.WriteLine("Bytes upload total: " + Convert.ToString(_bytesUploaded));
            Console.WriteLine("Time upload total: " + Convert.ToString(_timeUploaded));
            double _bytes = _bytesUploaded / 1024; //kb
            double _time = _timeUploaded / 1000 / range; // s
            double _rate = _bytes / _time;
            Console.WriteLine("Rate Upload: " + Convert.ToString(_rate));

        }

        public bool GetMatrixFromBlob(int range, int precisionChar, string container, ref float[] matrix, bool showSummary, int nthreads, long projectid)
        {
            precisionChar += 3; //para o parser da string de precisão é necessário considerar os caracteres "0" "." e ";"
            MeasuringTime[] measuring = new MeasuringTime[range + 2];
            measuring[range] = new MeasuringTime(-1); //tempos de processamento da fila
            measuring[range + 1] = new MeasuringTime(-2); //tempos de processamento deste método

            var watcher = Stopwatch.StartNew();
            measuring[range + 1].InitialTime = watcher.ElapsedMilliseconds;
            long _bytesReaded = 0;

            int _bytesToRead = range * precisionChar;

            using (GetBlobAsync _queue = new GetBlobAsync(nthreads, range, ref matrix, showSummary))
            {
                for (int i = 0; i < range; i++)
                {
                    string _blobName = i.ToString() + ".zip";
                    var _blobFileDes = new BlobInfo(i, container, range, precisionChar);
                    _blobFileDes.SetBlobName(projectid);
                    _blobFileDes.Times.InitialTime = watcher.ElapsedMilliseconds;
                    
                    _queue.EnqueueTask(_blobFileDes);
                    if (i == range - 1)
                    {
                        Console.WriteLine("Enqueue Complete");
                    }
                    Thread.Sleep(1);
                }
                
            }

            for (int i = 0; i < range*range; i++)
            {
                if (matrix[i] == null)
                {
                    Console.WriteLine("Erro em: " + i.ToString());
                    return false;
                }
            }

            measuring[range + 1].FinalTime = watcher.ElapsedMilliseconds; //tempo final de processamento deste método
            watcher.Stop();
            
            long _bytesDownloaded = 0;
            long _timeDownloaded = 0;
            int _errosMeasuring = 0;
            for (int i = 0; i < range; i++)
            {
                if (measuring[i] != null)
                {
                    _bytesDownloaded += measuring[i].TotalByteLength;
                    _timeDownloaded += measuring[i].GetTransferTime();
                }
                else
                {
                    _errosMeasuring += 1;
                    if ((i + 1 >= 0) || (i + 1 < range))
                    {
                        _bytesDownloaded += measuring[i + 1].TotalByteLength;
                        _timeDownloaded += measuring[i + 1].GetTransferTime();
                    }
                    else if ((i - 1 >= 0) || (i - 1 < range))
                    {
                        _bytesDownloaded += measuring[i - 1].TotalByteLength;
                        _timeDownloaded += measuring[i - 1].GetTransferTime();
                    }
                }
            }

            Console.WriteLine("Erros measuring: " + _errosMeasuring.ToString());
            Console.WriteLine("Bytes download total: " + Convert.ToString(_bytesDownloaded));
            Console.WriteLine("Time download total: " + Convert.ToString(_timeDownloaded));
            double _bytes = _bytesDownloaded / 1024 /range; //kb
            double _time = _timeDownloaded / 1000 / range; // s
            double _rate = _bytes / _time;
            Console.WriteLine("Rate Download: " + Convert.ToString(_rate));

            return true;
        }
        
        //public void OpenFileAndSendToQueue(int ordem)
        //{
        //    if (File.Exists(CombinePathFile(ordem)))
        //    {
        //        int _floatSizeOrdem = 6;
        //        int _blockToRead = ordem * _floatSizeOrdem;
        //        string _queueName = "processing";

        //        using (StreamReader strRead = new StreamReader(CombinePathFile(ordem)))
        //        {
        //            char[] charReaded = new char[_blockToRead];

        //            using (PutBlobAsync _queue = new PutBlobAsync(_queueName))
        //            {
        //                for (int i = 0; i < 1; i++)
        //                {
        //                    strRead.ReadBlock(charReaded, 0, _blockToRead);

        //                    var _queueMessage = new QueueInfoMessage(_queueName, StorageType.String);
        //                    //_queueMessage.SourceMessage = ZipManager.CompressStringToString(new string(charReaded));
        //                    _queueMessage.SourceMessage = new string(charReaded);

        //                    _queue.EnqueueTask(_queueMessage);
        //                }
        //            }
                
        //        }

        //    }
        //}

        //public void OpenFullFileAndSendToBlob(int ordem)
        //{
        //    if (File.Exists(CombinePathFile(ordem)))
        //    {
        //        int _floatSizeOrdem = 11;

        //        //FileStream _file = File.OpenRead(CombinePathFile(ordem));
        //        string _container = "lu";

        //        int _bytesToRead = ordem * ordem * _floatSizeOrdem;

        //        using (FileStream _file = File.OpenRead(CombinePathFile(ordem)))
        //        {
        //            Console.WriteLine("Initializing byte[]...");
        //            byte[] _bytesReaded = new byte[_bytesToRead];

        //            _file.Read(_bytesReaded, 0, _bytesToRead);
        //            string _blobName = ordem + ".zip";
        //            var _blobFile = new BlobInfoByte(_container, _blobName, StorageType.Byte);
        //            Console.WriteLine("Compressing...");
        //            _blobFile.BlobByteSource = ZipManager.CompressBytes(_bytesReaded);

        //            using (BlobTransfer _blobTransfer = new BlobTransfer(_blobFile.ContainerName))
        //            {
        //                Console.WriteLine("Uploading...");
        //                _blobTransfer.UploadBlobAsync(_blobFile);
        //            }
        //        }
        //    }
        //}

        public void OpenFileToLinearMatrix(int range, ref float[] matrix, int precision)
        {
            precision += 3; 
            int dimension = range * range;
            string _path = Path.Combine(m_pathBase, range.ToString());

            if (File.Exists(_path))
            {
                using (StreamReader strRead = new StreamReader(CombinePathFile(range)))
                {
                    char[] charReaded = new char[precision];
                    string floatString;

                    for (int i = 0; i < dimension; i++)
                    {
                        strRead.ReadBlock(charReaded, 0, precision);
                        floatString = new string(charReaded).Substring(0, precision - 1);

                        matrix[i] = float.Parse(floatString);
                    }
                }
            }
        }
     }
}
