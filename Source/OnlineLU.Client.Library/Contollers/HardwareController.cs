using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace OnlineLU.Client.Library.Contollers
{
    public class HardwareController
    {
        public HardwareController()
        {

        }

        public HardwareInfoModel GetHardwareInfo()
        {
            return new HardwareInfoModel()
            {
                HardwareKey = GenerateMachineIdentification(),
                System = GetCpuId(),
                Video = GetVideoControllerDescription()
            };
        }

        private string GenerateMachineIdentification()
        {
            //  NOTA: FOI ESCOLHIDO NÃO UTILIZAR INFORMAÇÕES DE PLACA DE REDE E DISCO RÍGIDO, JÁ QUE SÃO ITENS MAIS SUBSTITUÍVEIS

            //constants
            //string[,] check = new string[,] {
            //    {"Win32_NetworkAdapterConfiguration","MACAddress"},
            //    {"Win32_Processor", "UniqueId"},
            //    {"Win32_Processor", "ProcessorId"},
            //    {"Win32_Processor", "Name"},
            //    {"Win32_Processor", "Manufacturer"},
            //    {"Win32_BIOS", "Manufacturer"},
            //    {"Win32_BIOS", "SMBIOSBIOSVersion"},
            //    {"Win32_BIOS", "IdentificationCode"},
            //    {"Win32_BIOS", "SerialNumber"},
            //    {"Win32_BIOS", "ReleaseDate"},
            //    {"Win32_BIOS", "Version"},
            //    {"Win32_DiskDrive", "Model"},
            //    {"Win32_DiskDrive", "Manufacturer"},
            //    {"Win32_DiskDrive", "Signature"},
            //    {"Win32_DiskDrive", "TotalHeads"},
            //    {"Win32_BaseBoard", "Model"},
            //    {"Win32_BaseBoard", "Manufacturer"},
            //    {"Win32_BaseBoard", "Name"},
            //    {"Win32_BaseBoard", "SerialNumber"},
            //    {"Win32_VideoController", "DriverVersion"},
            //    {"Win32_VideoController", "Name"}
            //};

            string[,] check = new string[,] {
                {"Win32_Processor", "UniqueId"},
                {"Win32_Processor", "ProcessorId"},
                {"Win32_Processor", "Name"},
                {"Win32_Processor", "Manufacturer"},
                {"Win32_BIOS", "Manufacturer"},
                {"Win32_BIOS", "SMBIOSBIOSVersion"},
                {"Win32_BIOS", "IdentificationCode"},
                {"Win32_BIOS", "SerialNumber"},
                {"Win32_BIOS", "ReleaseDate"},
                {"Win32_BIOS", "Version"},
                {"Win32_BaseBoard", "Model"},
                {"Win32_BaseBoard", "Manufacturer"},
                {"Win32_BaseBoard", "Name"},
                {"Win32_BaseBoard", "SerialNumber"},
                {"Win32_VideoController", "DriverVersion"},
                {"Win32_VideoController", "Name"}
            };

            //WMI query
            //string query = "SELECT {1} FROM {0}", queryex = " WHERE IPEnabled = 'True'";
            string query = "SELECT {1} FROM {0}", queryex = String.Empty;

            string result = null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < check.GetLength(0); i++)
            {
                System.Management.ManagementObjectSearcher oWMI = new System.Management.ManagementObjectSearcher(
                    string.Format(query, check[i, 0], check[i, 1]) + (i == 0 ? queryex : string.Empty));
                foreach (System.Management.ManagementObject mo in oWMI.Get())
                {
                    result = mo[check[i, 1]] as string;
                    //Console.WriteLine(result);
                    if (result != null) sb.AppendLine(result);
                    break;
                }
            }

            //Hashing & format
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(sb.ToString());
            bt = sec.ComputeHash(bt);
            sb.Clear();
            for (int i = 0; i < bt.Length; i++)
            {
                if (i > 0 && i % 2 == 0) sb.Append('-');
                sb.AppendFormat("{0:X2}", bt[i]);
            }

            return sb.ToString();
        }

        private List<VideoInfo> GetVideoControllerDescription()
        {
            var _response = new List<VideoInfo>();

            var s1 = new ManagementObjectSearcher("select * from Win32_VideoController");
            
            foreach (ManagementObject oReturn in s1.Get())
            {
                var desc = oReturn["AdapterRam"];
                long _ram = long.Parse(oReturn["AdapterRam"].ToString());
                double _ramMB = _ram / Math.Pow(1024, 2);
                //if (desc == null) continue;
                var _name = oReturn["Name"].ToString();
                var _driverVersion = oReturn["DriverVersion"].ToString();

                var _video = new VideoInfo
                {
                    Name = _name,
                    Memory = _ramMB
                };
                _response.Add(_video);
            }

            return _response;
        }

        private static string GetComputerModel()
        {
            var s1 = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (ManagementObject oReturn in s1.Get())
            {
                return oReturn["Model"].ToString().Trim();
            }
            return string.Empty;
        }

        private static int GetMemoryAmountGB()
        {
            var s1 = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            int memoryTotal = 0;
            foreach (ManagementObject oReturn in s1.Get())
            {
                //return oReturn["Capacity"].ToString().Trim();
                long _memory = long.Parse(oReturn["Capacity"].ToString().Trim());
                double _memoryc = _memory / Math.Pow(1024, 3);
                memoryTotal += int.Parse(_memoryc.ToString());
            }
            return memoryTotal;
        }

        private static string GetVolumeSerial()
        {
            Console.WriteLine("GetVolumeSerial");
            var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            disk.Get();

            string volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        private static SystemInfo GetCpuId()
        {
            var _response = new SystemInfo();
            _response.Memory = GetMemoryAmountGB();

            var managClass = new ManagementClass("win32_processor");
            var managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                //Get only the first CPU's ID
                _response.SystemName = managObj.Properties["SystemName"].Value.ToString();
                _response.Name = managObj.Properties["Name"].Value.ToString();
                _response.Cores = int.Parse(managObj.Properties["NumberOfLogicalProcessors"].Value.ToString());
                return _response;
            }
            return _response;
        }




    }
}
