using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLU.Client.ConsoleService.AzureHelper;
using OnlineLU.Client.ConsoleService.Resolve;

namespace OnlineLU.Client.ConsoleService
{
    class Program
    {

        #region data

        static float[] A5 = { 25,  5,  1, 6,  3,   7,
					          64,  8,  1, 8,  2,   3,
					          144, 12,  1, 2, 57,  35,
					          52, 35,  5, 1,  5,   8,
					          74, 14, 34, 1,  4,  10,
					          12, 43,  1, 4,  8, 110 };

        static float[] A4 = { 9, 3, 2, 0, 7,
                              63, 27, 23, 6, 53,
                              18,48, 74, 50, 44,
                              0, 54, 130, 112, 52,
                              63, 39, 83, 74, 84};

        static float[][] A3 = { new float[] { 25,  5,  1, 6,  3,   7},
					            new float[] { 64,  8,  1, 8,  2,   3},
					            new float[] { 144, 12,  1, 2, 57,  35},
					            new float[] { 52, 35,  5, 1,  5,   8},
					            new float[] { 74, 14, 34, 1,  4,  10},
					            new float[] { 12, 43,  1, 4,  8, 110 }};

        static float[][] A2 = { new float[] {9, 3, 2, 0, 7},
                                new float[] {63, 27, 23, 6, 53},
                                new float[] {18,48, 74, 50, 44},
                                new float[] {0, 54, 130, 112, 52},
                                new float[] {63, 39, 83, 74, 84}};

        static float[] b = { 35, 303, 522, 858, 763 };

        static float[][] A1 = { new float[] {-2, -4, -1},
                                 new float[] {0, 1, 1},
                                 new float[] {1, 2, 0}};
        #endregion data

        static int[] ranges = {10, 100, 1000, 5000, 10000};

        static void Main(string[] args)
        {
            CreateFile();
            
            //int ordem = 10;

            //float[] matrix = new float[ordem*ordem];

            ////_file.OpenFile(ordem, ref matrix);
            
            //_file.OpenFileAndSendToBlob(ordem, _precision, "001");


            //_file.GetMatrixFromBlob(ordem, _precision, "001", ref matrix);

            //LuCuda(ordem, ref matrix);
            ////LuSync(ordem);
            ////LuAsync(ordem);



            Console.ReadLine();
        }

        static void CreateFile()
        {
            int _precision = 8;

            var _file = new TestFileCreator();

            for (int i = 0; i < 5; i++)
            {
                var watch = Stopwatch.StartNew();    
                int range = ranges[i];
                Console.WriteLine("Criando arquivo de orderm: " + range.ToString());
                _file.CreateFilePrecision(range, _precision);

                var elapsedMsProcess = watch.ElapsedMilliseconds;
                Console.WriteLine("Total: {0}ms", elapsedMsProcess);
                watch.Stop();
            }

            Console.ReadLine();
        }

        static void Upload()
        {
            var _precision = 8;
            var _file = new TestFileCreator();

            for (int i = 0; i < 5; i++)
            {
                int range = ranges[i];
                Console.WriteLine("######### Processando Upload ordem: " + range.ToString());

                _file.OpenFileAndSendToBlob(range, _precision, "000-" + range.ToString(), false, 1);

            }
        }

        static void Download()
        {
            var _precision = 8;
            var _file = new TestFileCreator();

            for (int i = 0; i < 5; i++)
            {
                int range = ranges[i];
                Console.WriteLine("######### Processando Download ordem: " + range.ToString());
                float[] matrix = new float[range * range];
                _file.GetMatrixFromBlob(range, _precision, "000-" + range.ToString(), ref matrix, false, 100);
            }
        }

        static void TestLocalSync()
        {
            int precision = 8;
            MeasuringTime[] measure = new MeasuringTime[5];
            TestFileCreator _testfile = new TestFileCreator();
            LuSync _luS = new LuSync();

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 5; i++)
            {
                int range = ranges[i];
                Console.WriteLine("######### Processando Sync ordem: " + range.ToString());

                measure[i] = new MeasuringTime(range);
                measure[i].InitialTime = watch.ElapsedMilliseconds;

                float[][] matrix = new float[range][];
                _testfile.OpenFilePrecision(range, ref matrix, precision);
                measure[i].PreparingBytes = watch.ElapsedMilliseconds - measure[i].InitialTime;

                Console.WriteLine("Leitura Arquivo para memória: " + measure[i].PreparingBytes.ToString());

                measure[i].InitialProcessing = watch.ElapsedMilliseconds;
                _luS.ResolveLU(matrix);
                measure[i].FinalProcessing = watch.ElapsedMilliseconds;

                Console.WriteLine("Resolução Decomposição Lu: " + Convert.ToString(measure[i].GetProcessingTime()));
                measure[i].FinalTime = watch.ElapsedMilliseconds;

                Console.WriteLine("Tempo Total: " + Convert.ToString(measure[i].GetTotalTime()) );
            }
            watch.Stop();
            
            var elapsedMsProcess = watch.ElapsedMilliseconds;

        
        }

        static void TestLocalASync()
        {
            int precision = 8;
            MeasuringTime[] measure = new MeasuringTime[5];
            TestFileCreator _testfile = new TestFileCreator();
            LuAsync _luS = new LuAsync();

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 5; i++)
            {
                int range = ranges[i];
                Console.WriteLine("######### Processando Async de ordem: " + range.ToString());

                measure[i] = new MeasuringTime(range);
                measure[i].InitialTime = watch.ElapsedMilliseconds;

                float[][] matrix = new float[range][];
                _testfile.OpenFilePrecision(range, ref matrix, precision);
                measure[i].PreparingBytes = watch.ElapsedMilliseconds - measure[i].InitialTime;

                Console.WriteLine("Leitura Arquivo para memória: " + measure[i].PreparingBytes.ToString());

                measure[i].InitialProcessing = watch.ElapsedMilliseconds;
                _luS.ResolveLU(matrix);
                measure[i].FinalProcessing = watch.ElapsedMilliseconds;

                Console.WriteLine("Resolução Decomposição Lu: " + Convert.ToString(measure[i].GetProcessingTime()));
                measure[i].FinalTime = watch.ElapsedMilliseconds;

                Console.WriteLine("Tempo Total: " + Convert.ToString(measure[i].GetTotalTime()));
            }
            watch.Stop();

            var elapsedMsProcess = watch.ElapsedMilliseconds;


        }

        static void TestLocalCuda()
        {
            int precision = 8;
            MeasuringTime[] measure = new MeasuringTime[5];
            TestFileCreator _testfile = new TestFileCreator();
            LuCuda _luC = new LuCuda();

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                int range = ranges[4];
                LuMatrix luMatrix = new LuMatrix(range, range, true);
                Console.WriteLine("######### Processando Cuda de ordem: " + range.ToString());

                measure[i] = new MeasuringTime(range);
                measure[i].InitialTime = watch.ElapsedMilliseconds;

                float[] matrix = new float[range*range];
                _testfile.OpenFileToLinearMatrix(range, ref matrix, precision);
                measure[i].PreparingBytes = watch.ElapsedMilliseconds - measure[i].InitialTime;
                luMatrix.n = matrix;
                Console.WriteLine("Leitura Arquivo para memória: " + measure[i].PreparingBytes.ToString());

                measure[i].InitialProcessing = watch.ElapsedMilliseconds;
                _luC.ResolveLuBlock(ref luMatrix);
                measure[i].FinalProcessing = watch.ElapsedMilliseconds;

                Console.WriteLine("Resolução Decomposição Lu: " + Convert.ToString(measure[i].GetProcessingTime()));
                measure[i].FinalTime = watch.ElapsedMilliseconds;

                Console.WriteLine("Tempo Total: " + Convert.ToString(measure[i].GetTotalTime()));
            }
            watch.Stop();

            var elapsedMsProcess = watch.ElapsedMilliseconds;


        }

        static void LuCuda(int ordem, ref float[] matrix)
        {
            LuCuda lucuda = new LuCuda();
            LuMatrix luMatrix = new LuMatrix(ordem, ordem, true);
            luMatrix.n = matrix;

            TestFileCreator _testfile = new TestFileCreator();
            //_testfile.CreateFile(ordem, TypeDataFile.INT);
            //_testfile.OpenFileToLinearMatrix(ordem, luMatrix.n);


            LuMatrix luMatrixA5 = new LuMatrix(6, 6, true);
            luMatrixA5.n = A5;
            LuMatrix luMatrixA4 = new LuMatrix(5, 5, true);
            luMatrixA4.n = A4;

            var watch = Stopwatch.StartNew();
            lucuda.ResolveLuBlock(ref luMatrix);
            watch.Stop();
            var elapsedMsProcess = watch.ElapsedMilliseconds;
        }

        static void LuSync(int ordem)
        {
            TestFileCreator _testfile = new TestFileCreator();

            float[][] matrix = new float[ordem][];
            _testfile.OpenFileToMatrix(ordem, ref matrix);

            LuSync _luS = new LuSync();
            _luS.ResolveLU(matrix);
            
            
        }

        static void LuAsync(int ordem)
        {
            TestFileCreator _testfile = new TestFileCreator();

            float[][] matrix = new float[ordem][];
            _testfile.OpenFileToMatrix(ordem, ref matrix);

            LuAsync _lu = new LuAsync();

            var watch = Stopwatch.StartNew();
            _lu.ResolveLU(matrix);
            watch.Stop();
            var elapsedMsProcess = watch.ElapsedMilliseconds;
        }

        static void LuResolve(ref float[][] A)
        {
            LuCuda lucuda = new LuCuda();
            //lucuda.ResolveLU(ref A);
            if (true)
            {
                int n = A3.Length;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write("{0:N6}", A3[i][j]);
                        Console.Write("  ");
                    }
                    Console.Write("\n");
                }
            }
        }

    }
}
