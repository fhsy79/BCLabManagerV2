﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class GlobalSettings
    {
        public static string DbPath { get; set; }
        public static string ConfigurationFilePath { get; set; } = "BCLabConfiguration.cfg";

        //public static string RootPath { get; set; } = @"Q:\807\Software\WH BC Lab\Data\";
        public static string RootPath { get; set; } = @"D:\Issues\Open\BC_Lab\Data\";
    }
}
