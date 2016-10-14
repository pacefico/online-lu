using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.ConsoleService.ConverterHelper
{
    public static class TypeConverter
    {
        public static void ByteToFloat(byte[] byteSource, ref float[] floatDestination, int initialPosition, int ordem, int precisionChar)
        {
            //float[] _array = new float[ordem];
            int j = initialPosition;
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
            //return _array;
        }

        
    }
}
