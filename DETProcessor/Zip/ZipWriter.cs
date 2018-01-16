using DETProcessor.Processor;
using System;
using System.IO;
using System.IO.Compression;

namespace DETProcessor.Zip
{
    class ZipWriter
    {
        public static void CreateCompressedDirectory(CompressedConfiguration config, Metadata md, string citationSaveDir)
        {
            Directory.CreateDirectory(config.CompressedDir);
            string zipName = Path.Combine(config.CompressedDir, config.CompressedName);
            Console.WriteLine("Zip: Archive name->\t" +config.CompressedName);
            Console.WriteLine("Zip: Writing to->\t"+config.CompressedDir);
            
            using (FileStream ms = new FileStream(zipName, FileMode.Create))
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    ZipIncludeFiles(config, archive);
                    ZipIncludeDirs(config, archive, md.LocID);
                    ZipCitation(config, md, citationSaveDir, archive);
                }
            }
            
        }

        private static void ZipCitation(CompressedConfiguration config, Metadata md, string citationSaveDir, ZipArchive archive)
        {
            if (config.IncludeCitation)
            {
                string citationDir = Path.Combine(citationSaveDir, md.CitationName);
                Console.WriteLine("Adding file to zip: " + citationDir);
                archive.CreateEntryFromFile(citationDir, md.CitationName);
            }
        }

        private static void ZipIncludeDirs(CompressedConfiguration config, ZipArchive archive, string siteName)
        {
            if (config.SearchDir == null) return;
            foreach (string dir in config.SearchDir)
            {
                string dirPath = dir;
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                string dirName = dirInfo.Name;
                FileInfo[] files = dirInfo.GetFiles("*"+siteName+"*");
                foreach (var file in files)
                {
                    archive.CreateEntryFromFile(file.FullName, dirName + "\\" + file.Name);
                }

            }
        }

        private static void ZipIncludeFiles(CompressedConfiguration config, ZipArchive archive)
        {
            if (config.IncludeFiles == null) return;
            foreach (string file in config.IncludeFiles)
            {
                FileInfo f = new FileInfo(file);
                Console.WriteLine("Adding file to zip: " + file);
                archive.CreateEntryFromFile(f.FullName, f.Name);
            }
        }
    }
}
