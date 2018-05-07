using Syncfusion.XlsIO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;

namespace DETProcessor.Processor
{
    class ProcessDET
    {
        private Dictionary<string, MetaDataPair> AllMetaData;
        public List<MetaDataPair> UsedMeta { get; } = new List<MetaDataPair>();
        private RunProcessor theProcessor;
        private string xlsxPath;
        private string csvSavePath;
        private bool createCSVs;

        public ProcessDET(Dictionary<string, MetaDataPair> md, RunProcessor proc)
        {
            AllMetaData = md;
            theProcessor = proc;
            xlsxPath = proc.ConfigFile.GetValue("detPath").Value<string>();
            csvSavePath = proc.ConfigFile.GetValue("csvSaveLoc").Value<string>();
            createCSVs = proc.RunConfig.CreateCSVs;
        }

        public bool CreateCSVs(Metadata md)
        {
            // open det
            // for each tab: csv + store metadata
            IWorkbook det = theProcessor.OpenXLSXWorkBook(xlsxPath);
            if (det == null) return false; // det doesn't exist, so stop execution

            SetupDirectory();
            //if (theProcessor.RunConfig.CreateCitation)
            //{
                PopulateMetadata(det, md);
            //}
            PopulateSheetMetadataAndCSVs(det, md);

            return true;
        }

        private void SetupDirectory()
        {
            if (Directory.Exists(csvSavePath))
            {
                DirectoryInfo dir = new DirectoryInfo(csvSavePath);

                foreach (FileInfo csvFile in dir.GetFiles())
                {
                    csvFile.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(csvSavePath); // create if does not exist.
            }
        }

        private void PopulateSheetMetadataAndCSVs(IWorkbook det, Metadata md)
        {
            foreach (IWorksheet sheet in det.Worksheets)
            {
                if (theProcessor.RunConfig.CreateCitation)
                    AddSheetMetadata(sheet, md);
                if (createCSVs)
                {
                    SaveAsCSV(sheet, md.LocID); // send in sheet and the path to the save location.
                }

            }
        }

        private void PopulateMetadata(IWorkbook det, Metadata md)
        {
            IWorksheet ws = det.Worksheets[@"Overview"];
            ws.UsedRangeIncludesFormatting = false;
            md.LocID = ws[theProcessor.DataStartRow, 1].Value; // site name
            md.PeriodBegin = ws[theProcessor.DataStartRow, 7].DateTime;
            if (ws[theProcessor.DataStartRow, 8].Value != "")
                md.PeriodEnd = ws[theProcessor.DataStartRow, 8].DateTime;

            if (ws.UsedRange.LastRow > theProcessor.DataStartRow)
            {
                md.LocID = "NatRes";
                List<DateTime> dateRange = new List<DateTime>();
                int lastRow = ws.UsedRange.LastRow;
                for (int row = theProcessor.DataStartRow; row <= lastRow; row++)
                {
                    // add both start and end dates to range.
                    DateTime theTime = ws[row, 7].DateTime;
                    dateRange.Add( theTime == null ? DateTime.Now : theTime);
                    theTime = ws[row, 8].DateTime;
                    dateRange.Add(theTime == null ? DateTime.Now : theTime);
                }
                dateRange.Sort();
                // earliest date = start
                md.PeriodBegin = dateRange.Min();
                // latest date = end. (Most likely current date)
                md.PeriodEnd = dateRange.Max();
                theProcessor.IsMaster = true;
            }
            md.Persons = GetPeople(det);
        }

        private List<Person> GetPeople(IWorkbook det)
        {
            List<Person> persons = new List<Person>();
            IWorksheet ws = det.Worksheets[@"Persons"];
            int firstNameRow = 3;
            int lastNameRow = 2;
            int roleRow = 6;
            //if (theProcessor.IsMaster)
            //{
            //    firstNameRow++;
            //    lastNameRow++;
            //    roleRow++;
            //}
            int lastRow = ws.UsedRange.LastRow;
            for (int idx = theProcessor.DataStartRow; idx <= lastRow; idx++)
            {
                string ln = ws.Range[idx, lastNameRow].Value;
                string fn = ws.Range[idx, firstNameRow].Value;
                string role = ws.Range[idx, roleRow].Value.ToLower();
                if (ln.Length != 0 && fn.Length != 0)
                {
                    persons.Add(new Person
                    {
                        LastName = ln,
                        FirstName = fn,
                        IsPI = role.Equals("pi")
                    });
                }

            }
            persons.Sort();
            if (theProcessor.IsMaster)
            {
                List<Person> dupfree = persons.Distinct(new PersonComparator()).ToList<Person>();
                persons = dupfree;
            }
            persons.Sort();
            return persons;
        }

        private void AddSheetMetadata(IWorksheet ws, Metadata md)
        {
            List<MetaDataPair> sheetMeta = new List<MetaDataPair>();
            int lastRow = ws.UsedRange.LastRow;
            int lastCol = ws.UsedRange.LastColumn;
            for (int i = theProcessor.DataHeaderRow; i <= lastCol; i++)
            {
                MetaDataPair mdp = null;
                if (AllMetaData.TryGetValue(ws[theProcessor.DataHeaderRow, i].Value.ToLower(), out mdp))
                {
                    if (!ws.Range[theProcessor.DataStartRow, i, lastRow, i].IsBlank)
                    {
                        sheetMeta.Add(mdp);
                        if (!md.MDP.Contains(mdp))
                            md.MDP.Add(mdp);
                    }
                }
            }
        }

        private void SaveAsCSV(IWorksheet ws, string siteName)
        {
            // save loc should be <path>\\sitename\\ if you want it saved for a specific site
            string newfname = Path.Combine(csvSavePath, siteName + "_" + ws.Name + ".csv");
            using (FileStream fs = new FileStream(newfname, FileMode.Create))
            {
                ws.SaveAs(fs, ",");
            }
        }
    }
}
