using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.ConverterHelper
{
    public static class TypeConverter
    {
        public static void ByteToFloat(byte[] byteSource, ref float[] floatDestination, int initialPosition, int ordem, int precisionChar)
        {
            //float[] _array = new float[ordem];
            //float[] floatDestination = new float[ordem];
            try
            {
                int j = initialPosition * ordem;
                
                char[] _charReaded = new char[precisionChar];
                for (int k = 0; k < byteSource.Length; k += precisionChar)
                {
                    _charReaded = new char[precisionChar];
                    int _idx = 0;
                    for (int l = k; l < k + precisionChar; l++)
                    {
                        _charReaded[_idx] = Convert.ToChar(byteSource[l]);
                        _idx++;
                    }
                    string floatString = new string(_charReaded).Substring(0, precisionChar - 1);

                    floatDestination[j] = float.Parse(floatString);
                    j++;
                }

            }
            catch (Exception ex)
            {
            }
            if (initialPosition == 4000)
            {
            }
            //return floatDestination;
            //return _array;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] ConvertFloatToByteArray(float[] floats)
        {
            byte[] ret = new byte[floats.Length * 4];// a single float is 4 bytes/32 bits

            for (int i = 0; i < floats.Length; i++)
            {
                // todo: stuck...I need to append the results to an offset of ret
                ret = BitConverter.GetBytes(floats[i]);

            }
            return ret;
        }
    }
}
