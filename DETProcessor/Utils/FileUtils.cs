using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DETProcessor.Utils
{
    class FileUtils
    {
        public static FileStream OpenReadOnlyFile(string file)
        {
            FileStream ret = null;
            try
            {
                ret = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch
            {
                Console.WriteLine("Cannot open" + file + ".");
            }
            return ret;
        }

        public static FileStream CreateNewFile(string file)
        {
            FileStream ret = null;
            try
            {
                ret = File.Open(file, FileMode.Create);
            }
            catch
            {
                Console.WriteLine("Cannot open" + file + ".");
            }
            return ret;
        }
    }
}
