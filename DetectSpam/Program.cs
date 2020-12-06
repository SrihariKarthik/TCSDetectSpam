using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Microsoft.ML;
using DetectSpam.SupportModel;

namespace DetectSpam
{
    public class Program
    {
        static void Main(string[] args)
        {
            string AppPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            string DataPath = Util.GetDataPath(AppPath, @"Data\SpamData");
            string comment = "";
            while(true)
            {
                Console.WriteLine("Enter your comment - 0 to exit!...... ");
                comment = Console.ReadLine();
                if (comment == "0")
                    break;
                Util.Process(DataPath, comment);
            }
        }
    }
}