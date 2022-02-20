using System.Diagnostics;
using System.IO;
using RoNPakEditor.Controller;
using RoNPakEditor.Interfaces;

namespace RoNPakEditor.Logic
{
    public class LPAKManager : IPAKManager
    {
        private FileInfo fileInfo;
        public void ExtractPAK()
        {
            fileInfo = new FileInfo(GlobalCache.PathToPAK);
            Directory.CreateDirectory(GlobalCache.PathToOutput);

            // Calculate to Gigabytes
            // Execute when file is bigger than 4 gig
            if (fileInfo.Length / 1000 / 1000 / 1000 > 4)
            {
                Process extractProcess = Process.Start("QuickBMS/quickbms_4gb_files.exe", "-a QuickBMS/ron.bms" + " " + "QuickBMS/ron.bms" + " " + GlobalCache.PathToPAK + " " + GlobalCache.PathToOutput);
                while (!extractProcess.HasExited)
                {
                    // Do nothing
                }
            }
            else
            {
                Process extractProcess = Process.Start("QuickBMS/quickbms.exe", "-a QuickBMS/ron.bms" + " " + "QuickBMS/ron.bms" + " " + GlobalCache.PathToPAK + " " + GlobalCache.PathToOutput);
                while (!extractProcess.HasExited)
                {
                    // Do nothing
                }
            }
        }

        public void RepackPAK()
        {
            // Calculate to Gigabytes
            // Execute when file is bigger than 4 gig
            if (fileInfo.Length / 1000 / 1000 / 1000 > 4)
            {
                Process.Start("QuickBMS/quickbms_4gb_files.exe", "-r -w -a QuickBMS/ron.bms" + " " + "QuickBMS/ron.bms" + " " + GlobalCache.PathToPAK + " " + GlobalCache.PathToOutput);
            }
            else
            {
                Process.Start("QuickBMS/quickbms.exe", "-r -w -a QuickBMS/ron.bms" + " " + "QuickBMS/ron.bms" + " " + GlobalCache.PathToPAK + " " + GlobalCache.PathToOutput);
            }
        }
    }
}
