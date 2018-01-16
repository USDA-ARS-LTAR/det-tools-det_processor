using DETProcessor.Citation;
using DETProcessor.MetadataWriters;
using DETProcessor.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace DETProcessor.Processor
{
    class RunProcessor
    {
        private Dictionary<string, MetaDataPair> allMetaData;

        private ExcelEngine ee = new ExcelEngine();
        private ProcessDET csvWriter;
        private Metadata metadata = new Metadata();
        private CitationWriter citeWriter;

        public int DataStartRow { get; set; }
        public int DataHeaderRow { get; set; }
        public bool IsMaster { get; set; }

        public JObject ConfigFile { get; }
        public RunOptions RunConfig { get; }

        public RunProcessor(JObject configFile)
        {
            ConfigFile = configFile;
            metadata = createMetadata(configFile);
            DataStartRow = configFile.GetValue("dataStartRow").Value<int>(); // required
            DataHeaderRow = configFile.GetValue("dataHeaderRow").Value<int>(); // required
            RunConfig = configFile.GetValue("runflags").ToObject<RunOptions>(); ; // required
            if (RunConfig.CreateNALMeta)
            {
                if (configFile.TryGetValue("nalMetadata", out JToken token))
                {
                    try
                    {
                        metadata.NALData = token.ToObject<NALMetadata>(); // try list otherwise:
                    } catch (JsonSerializationException e)
                    {
                        metadata.NALData = new NALMetadata();
                        metadata.NALData.FIPSCode = token.SelectToken("fipsCode").Value<string>().Replace("&gt;", ">");
                        metadata.NALData.NASALocationCode = token.SelectToken("nasaLocationCode").Value<string>();
                        metadata.NALData.NALDataSourceCode = new List<string>() { token.SelectToken("nalDataSourceCode").Value<string>() };
                        metadata.NALData.FederalProgramCode = token.SelectToken("federalProgramCode").Value<string>();
                        metadata.NALData.OMBCode = token.SelectToken("ombCode").Value<string>();
                    }
                }
                else
                {
                    throw new ArgumentException("NAL-XML -> Tag \"nalMetadata\" is missing, tag is required to complete XML format.");
                }
            }
        }

        ~RunProcessor()
        {
            ee.Dispose();
        }

        public void ProcessDET()
        {
            dynamic templates = ConfigFile.GetValue("templates"); // required
            ReadDataDef((string)templates.dataDef);
            ProcessXLSXDET();
            // locid and mdp now populated.
            CreateCitation(templates);
            // metadata created
            // write xmls
            CreateXMLMetadataFiles(ConfigFile, metadata);
            CreateZipForWeb(ConfigFile, metadata);
        }

        private void CreateCitation(dynamic templates)
        {
            metadata.CitationName = metadata.LocID + "_citation.xlsx";
            if (RunConfig.CreateCitation)
            {
                Console.WriteLine("Citation: Started...");
                citeWriter = new CitationWriter((string)templates.citationTemplatePath, this);
                citeWriter.CreateCitation(metadata);
                Console.WriteLine("Citation: Done.");
            }
        }

        private void ProcessXLSXDET()
        {
            csvWriter = new ProcessDET(allMetaData, this);
            Console.WriteLine("CSVs: Started...");
            csvWriter.CreateCSVs(metadata);
            Console.WriteLine("CSVs: Done.");
        }

        private Metadata createMetadata(JObject configFile)
        {
            Metadata md = new Metadata();
            md.MDP = new List<MetaDataPair>();
 
            if (configFile.TryGetValue("keywordsDir", out JToken token))
            {
                md.Keys = GetKeys(token.Value<string>()); // optional
            }
            if (configFile.TryGetValue("abstract", out token))
            {
                md.Purp = token.ToObject<Purpose>(); // optional
            }
            if (configFile.TryGetValue("sitePI", out token))
            {
                md.Pi = token.ToObject<SitePI>(); // optional
            }
            if (configFile.TryGetValue("boundingBox", out token))
            {
                md.Box = token.ToObject<BoundingBox>(); // optional
            }
            return md;
        }

        private void CreateZipForWeb(JObject configFile, Metadata md)
        {
            if (RunConfig.CreateZip)
            {
                Console.WriteLine("Zip: Started...");
                CompressedConfiguration config = configFile.GetValue("compressedConfig").ToObject<CompressedConfiguration>();
                Zip.ZipWriter.CreateCompressedDirectory(config, md, configFile.GetValue("citationSaveDir").Value<string>());
                Console.WriteLine("Zip: Done.");
                return;
            }
        }

        private void CreateXMLMetadataFiles(JObject configFile, Metadata md)
        {
            if (RunConfig.CreateESRI || RunConfig.CreateNALMeta)
            {
                string csvSaveLoc = configFile.GetValue("csvSaveLoc").Value<string>();
                string xmlSaveLoc = configFile.GetValue("xmlSaveLoc").Value<string>();
                if (RunConfig.CreateESRI)
                {
                    Console.WriteLine("ESRI Meta: Started...");
                    FGDCXmlWriter.MakeFGDCXML(xmlSaveLoc, md, csvSaveLoc);
                    Console.WriteLine("ESRI Meta: Done.");
                }
                if (RunConfig.CreateNALMeta)
                {
                    Console.WriteLine("NAL Meta: Started...");
                    ISOTC211XmlWriter.MakeISOTC211XML(xmlSaveLoc, md, csvSaveLoc);
                    Console.WriteLine("NAL Meta: Done.");
                }
            }
        }

        private Keywords GetKeys(string keywordPath)
        {
            Keywords p = new Keywords();
            using (var stream = new FileStream(keywordPath, FileMode.Open, FileAccess.Read))
            {
                using (var rdr = new StreamReader(stream))
                {
                    string word;
                    while ((word = rdr.ReadLine()) != null)
                    {
                        if (String.IsNullOrEmpty(word)) break;
                        p.words.Add(word);
                    }
                }
            }
            return p;
        }

        private bool ReadDataDef(string dataDefPath)
        {
            IWorkbook definitions = OpenXLSXWorkBook(dataDefPath);
            if (definitions == null) return false;
            allMetaData = CreateMetaData(definitions);
            return true;
        }

        private Dictionary<string, MetaDataPair> CreateMetaData(IWorkbook wb)
        {
            Dictionary<string, MetaDataPair> allMetaData = new Dictionary<string, MetaDataPair>();
            IWorksheet ws = wb.Worksheets[0];
            ws.UsedRangeIncludesFormatting = false;
            int lastrow = ws.UsedRange.LastRow;
            for (int idx = DataStartRow; idx <= lastrow; idx++)
            {
                string column_name = ws[idx, 1].Value.ToLower();
                if (!allMetaData.ContainsKey(column_name))
                {
                    string desc = ws[idx, 2].Value;
                    string units = ws[idx, 3].Value;
                    string dataType = ws[idx, 4].Value;

                    allMetaData.Add(column_name,
                        new MetaDataPair
                        {
                            Column = column_name,
                            Description = desc,
                            Units = units,
                            DataType = dataType
                        });
                }
            }
            return allMetaData;
        }


        public IWorkbook OpenXLSXWorkBook(string pathToFile)
        {
            FileStream fs = FileUtils.OpenReadOnlyFile(pathToFile);
            if (fs == null) return null;
            IWorkbook wb = ee.Excel.Workbooks.Open(fs);
            wb.Version = ExcelVersion.Excel2016;
            return wb;
        }
    }
}
