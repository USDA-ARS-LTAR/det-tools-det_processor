using DETProcessor.Processor;
using DETProcessor.Utils;
using Newtonsoft.Json.Linq;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DETProcessor.Citation
{
    class CitationWriter
    {
        public RunProcessor TheProcessor { get; set; }

        private string citationTemplatePath;
        private string citationSaveDir;

        public CitationWriter(string tempPath, RunProcessor theProc)
        {
            citationTemplatePath = tempPath;
            citationSaveDir = theProc.ConfigFile.GetValue("citationSaveDir").Value<string>();
            TheProcessor = theProc;
        }

        public bool CreateCitation(Metadata md)
        {

            IWorkbook wb = TheProcessor.OpenXLSXWorkBook(citationTemplatePath);
            if (wb == null) return false; // failed to open book, dont do anything.
            string citation = CreateCitationString(md, TheProcessor.IsMaster);

            wb.Worksheets["Version6"].Range[2, 1].Value = citation;
            string newPath = Path.Combine(citationSaveDir, md.CitationName);
            
            using (FileStream fs = FileUtils.CreateNewFile(newPath))
            {
                wb.SaveAs(fs);
            }
            wb.Close();
            return true;
        }

        private string CreateCitationString(Metadata md, bool mastersheet)
        {
            StringBuilder citation = new StringBuilder();
            foreach (Person p in md.Persons)
            {
                citation.Append(String.Format("{0}, {1,1}.; ", p.LastName, p.FirstName.Substring(0,1)));
            }
            citation.Length = citation.Length - 2; // get rid of last ';'
            citation.Append(". ");
            // dl date
            citation.Append(DateTime.Now.Year + ". ");
            // querytitle
            citation.Append("All; ");
            // data date range
            AddDateRange(md, mastersheet, citation);

            // version
            citation.Append("ver. GPSR_NATRES. ");
            // location
            AddLocation(md, mastersheet, citation);

            // download date
            citation.Append("File Downloaded: " + DateTime.Now.ToString("yyyy-MMM-dd;[hh:mm:ss]") + ". ");
            // PID = (this method returns a rfc 4122 UUID)
            citation.Append("PID: " + Guid.NewGuid().ToString() + ".");
            return citation.ToString();
        }

        private static void AddLocation(Metadata md, bool mastersheet, StringBuilder citation)
        {
            citation.Append("Fort Collins, CO: USDA-ARS Natural Resources Database. ");
            if (mastersheet)
            {
                citation.Append("Project: Natural Resources and Genomics. URL: https://gpsr.ars.usda.gov/natres. ");
            }
            else
            {
                citation.Append("Project: " + md.LocID + ". URL: https://gpsr.ars.usda.gov/" + md.LocID + ". ");
            }
        }

        private static void AddDateRange(Metadata md, bool mastersheet, StringBuilder citation)
        {
            int endYear = DateTime.Now.Year;
            if (mastersheet)
            {
                citation.Append("1980-" + DateTime.Now.Year + ". ");
            }
            else
            {
                citation.Append(md.PeriodBegin.Year + "-" + md.PeriodEnd.Year + ". ");
            }
        }
    }
}
