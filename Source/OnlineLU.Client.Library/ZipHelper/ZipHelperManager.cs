using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OnlineLU.Client.Library.ZipHelper
{
    public static class ZipHelperManager
    {
        public static byte[] UnzipStreamToByte(Stream streamToUnZip, int ordem, int precisionChar)
        {
            byte[] _decompressed = new byte[ordem*precisionChar];
            streamToUnZip.Position = 0;
            using (ZipInputStream decompressStream = new ZipInputStream(streamToUnZip, true))
            {
                streamToUnZip.Position = 0;
                var entry = decompressStream.GetNextEntry();
                decompressStream.Read(_decompressed, 0, _decompressed.Length);
            }

            return _decompressed;
        }

        public static byte[] ZipByteToByte(ref byte[] BytesToZip, int Id)
        {
            byte[] BytesOutput = null;

            using (MemoryStream zipStream = new MemoryStream())
            {
                //Zip data
                using (ZipOutputStream compressStream = new ZipOutputStream(zipStream, true))
                {
                    compressStream.PutNextEntry(Id.ToString());
                    compressStream.Write(BytesToZip, 0, BytesToZip.Length);
                }
                zipStream.Position = 0;
                BytesOutput = new byte[zipStream.Length];
                zipStream.Read(BytesOutput, 0, BytesOutput.Length);
            }

            return BytesOutput;
        }
    }
}
