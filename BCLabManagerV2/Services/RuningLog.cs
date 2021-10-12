﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    static public class RuningLog
    {
        //static private StreamWriter sw;
        //static void Init()
        //{
        //}
        static public void Write(string log)
        {
            FileStream fs = new FileStream(GlobalSettings.RunningLogFilePath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(log);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        static public void NewLog(string logName = "")
        {
            if (logName != "")
                logName = '-' + logName;
            GlobalSettings.RunningLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running Log\\", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}{logName}.log");
        }
    }
}
