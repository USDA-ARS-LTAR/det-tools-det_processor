using System;
using DETProcessor.Processor;
using Newtonsoft.Json.Linq;

namespace DETProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("DETProcessor: invalid Usage...");
                Console.WriteLine("Usage: <det_config_file.json>");
                return;
            }
            // Assumptions:
            // keywords 
            JObject configFile = JObject.Parse(System.IO.File.ReadAllText(args[0]));
            //Console.WriteLine(configFile);
            try
            {
                RunProcessor processor = new Processor.RunProcessor(configFile);
                PrintOptions(processor.RunConfig);
                processor.ProcessDET();
            } catch (ArgumentException ex)
            {
                Console.WriteLine("DETProcessor: error " + ex.Message);
            }
        }

        private static void PrintOptions(RunOptions runConfig)
        {
            Console.WriteLine("Options:");
            Console.WriteLine("Create CSVs      ->\t" + (runConfig.CreateCSVs ? "yes" : "no"));
            Console.WriteLine("Create Citation  ->\t" + (runConfig.CreateCitation ? "yes" : "no"));
            Console.WriteLine("Create NAL Meta  ->\t" + (runConfig.CreateNALMeta ? "yes" : "no"));
            Console.WriteLine("Create ESRI Meta ->\t" + (runConfig.CreateESRI ? "yes" : "no"));
            Console.WriteLine("Create Zips      ->\t" + (runConfig.CreateZip ? "yes" : "no"));
        }
    }
}
