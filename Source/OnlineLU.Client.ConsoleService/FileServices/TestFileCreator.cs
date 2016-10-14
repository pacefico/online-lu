using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLU.Client.ConsoleService.FileServices
{
    public class TestFileCreator
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

        public TestFileCreator()
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
        public void CreateFile2(int ordem)
        {
            Random rand = new Random();

            using (StreamWriter file = new StreamWriter(CombinePathFile(ordem), false))
            {
                for (int i = 0; i < ordem; i++)
                {
                    Console.WriteLine(i);
                    for (int j = 0; j < ordem; j++)
                    {
                        file.Write(string.Format("{0:0.00000000};", rand.NextDouble()));
                    }
                }
            }
        }

        public void SplitFile(int ordem)
        {
        
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

        public void OpenFile(int ordem, ref float[][] matrix)
        {
            if (File.Exists(CombinePathFile(ordem)))
            {
                int _floatSizeOrdem = 11;
                StreamReader strRead = new StreamReader(CombinePathFile(ordem));
                char[] charReaded = new char[_floatSizeOrdem];
                string floatString;
                for (int i = 0; i < ordem; i++)
                {
                    matrix[i] = new float[ordem];

                    for (int j = 0; j < ordem; j++)
                    {
                        strRead.ReadBlock(charReaded, 0, _floatSizeOrdem);
                        floatString = new string(charReaded).Substring(0, _floatSizeOrdem - 1);

                        matrix[i][j] = float.Parse(floatString);
                    }
                }
            }
        }

        public void OpenFileToLinearMatrix(int ordem, float[] matrix)
        {
            int dimension = ordem * ordem;
            string _path = Path.Combine(m_pathBase, ordem.ToString());
            //double[][] matrix = new double[ordem][];

            if (File.Exists(_path))
            {
                StreamReader strRead = new StreamReader(_path);

                char[] charReaded = new char[dimension];
                strRead.Read(charReaded, 0, dimension);

                for (int i = 0; i < dimension; i++)
                {
                    matrix[i] = float.Parse(charReaded[i].ToString());
                    
                }
            }
        }
     }
}
