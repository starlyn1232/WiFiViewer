using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFiViewer.Utils
{
    internal static class CMD
    {
        //Variables
        internal static Process pro = new();

        //CMD Processor
        internal static void CMDRun(string exe, string args, bool wait)
        {
            pro.StartInfo.FileName = exe;
            pro.StartInfo.Arguments = $" /K {args} && exit";

            pro.Start();

            if (wait)
                pro.WaitForExit();
        }

        //Read Buffer Edition
        internal static string CMDStr(string args, bool wait)
        {
            return CMDStr("cmd.exe", args, wait);
        }
         
        internal static string CMDStr(string exe, string args, bool wait)
        {
            CMDRun(exe, args, wait);

            int c;

            char ch;
            string buff = "";

            while ((c = pro.StandardOutput.Read()) != '\0' && c != null)
            {
                try
                {
                    ch = Convert.ToChar(c);
                    buff += ch;
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message, "Error!");

                    break;
                }
            }

            return buff;

            //return (buff[0].Length > buff[1].Length) ? buff[0] : buff[1];
        }
    }
}
