using System;
using System.Collections.Generic;
using System.IO;

namespace RoNPakEditor.Controller
{
    public static class GlobalCache
    {
        #region Dictionaries

        public static Dictionary<string, string> FileToPathDictionary = new Dictionary<string, string>();

        public static Dictionary<string, string> FolderPathsDictionary = new Dictionary<string, string>();

        #endregion

        #region Bools

        public static bool WriteAll = false;

        #endregion

        #region ReadOnly Strings

        public static readonly string Author = "Artur Bobb";

        public static readonly string PathToDevFolder =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AZDev/RoNPakEditor/";

        public static readonly string PathToOutput = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AZDev/RoNPakEditor/ExctractedOutput";

        #endregion

        #region Strings

        public static string PathToPAK
        {
            get => _pathToPAK;

            set
            {
                if (value != _pathToPAK)
                {
                    _pathToPAK = value;
                }
            }
        }

        private static string _pathToPAK = "";

        public static string Version = "";

        #endregion

    }
}
