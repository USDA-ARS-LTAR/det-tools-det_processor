using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using OfficeOpenXml;
using System.Text;

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
            using (ExcelPackage file = new ExcelPackage(new FileInfo(xlsxPath)))
            {
                var det = file.Workbook;
                if (det == null) return false; // det doesn't exist, so stop execution

                SetupDirectory();
                //if (theProcessor.RunConfig.CreateCitation)
                //{
                PopulateMetadata(det, md);
                //}
                PopulateSheetMetadataAndCSVs(det, md);

                return true;
            }
        }

        private void SetupDirectory()
        {
            if (Directory.Exists(csvSavePath))
            {
                DirectoryInfo dir = new DirectoryInfo(csvSavePath);

                foreach (FileInfo csvFile in dir.GetFiles("*.csv"))

                {
                    csvFile.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(csvSavePath); // create if does not exist.
            }
        }

        private void PopulateSheetMetadataAndCSVs(ExcelWorkbook det, Metadata md)
        {
            foreach (var sheet in det.Worksheets)
            {
                if (theProcessor.RunConfig.CreateCitation)
                    AddSheetMetadata(sheet, md); 
                if (createCSVs)
                {
                    SaveAsCSV(sheet, md.LocID); // send in sheet and the path to the save location.
                }

            }
        }

        private void PopulateMetadata(ExcelWorkbook det, Metadata md)
        {
            var ws = det.Worksheets[@"Overview"];
            //ws.UsedRangeIncludesFormatting = false;
            md.LocID = ws.Cells[theProcessor.DataStartRow, 1].Value.ToString(); // site name
            md.PeriodBegin = DateTime.Parse(ws.Cells[theProcessor.DataStartRow, 7].Value.ToString());
            if (ws.Cells[theProcessor.DataStartRow, 8].Value.ToString() != "")
                md.PeriodEnd = DateTime.Parse(ws.Cells[theProcessor.DataStartRow, 8].Value.ToString());
            var endRow = ws.Dimension.End.Row;
            if (endRow > theProcessor.DataStartRow)
            {
                md.LocID = "NatRes";
                List<DateTime> dateRange = new List<DateTime>();
                for (int row = theProcessor.DataStartRow; row <= endRow; row++)
                {
                    // add both start and end dates to range.
                    var theTime = DateTime.Parse(ws.Cells[row, 7].Value?.ToString() ?? DateTime.Now.ToString());
                    dateRange.Add( theTime == null ? DateTime.Now : theTime);
                    theTime = DateTime.Parse(ws.Cells[row, 8].Value?.ToString() ?? DateTime.Now.ToString());
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

        private List<Person> GetPeople(ExcelWorkbook det)
        {
            List<Person> persons = new List<Person>();
            ExcelWorksheet ws = det.Worksheets[@"Persons"];
            int firstNameCol = 3;
            int lastNameCol = 2;
            int roleRow = 6;
           
            int lastRow = ws.Dimension.End.Row;
            for (int idx = theProcessor.DataStartRow; idx < lastRow; idx++)
            {
                if (RowEmpty(idx, ws)) { break; }
                string ln = ws.Cells[idx, lastNameCol].Value.ToString();
                string fn = ws.Cells[idx, firstNameCol].Value.ToString();
                string role = ws.Cells[idx, roleRow].Value?.ToString().ToLower() ?? "";
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

        private bool RowEmpty(int row, ExcelWorksheet ws)
        {
            bool isEmpty = true;
            for (int col = 1; col <= ws.Dimension.End.Column; col++)
            {
                if (ws.Cells[row, col].Value != null)
                    isEmpty = false;
            }
            return isEmpty;
        }

        private void AddSheetMetadata(ExcelWorksheet ws, Metadata md)
        {
            List<MetaDataPair> sheetMeta = new List<MetaDataPair>();
            int lastRow = ws.Dimension.End.Row;
            int lastCol = ws.Dimension.End.Column;
            for (int i = theProcessor.DataHeaderRow; i <= lastCol; i++)
            {
                MetaDataPair mdp = null;
                string value = ws.Cells[theProcessor.DataHeaderRow, i].Value?.ToString() ?? "";
                if (AllMetaData.TryGetValue(value, out mdp))
                {
                    if (ws.Cells[theProcessor.DataStartRow, i, lastRow, i].Value != null)
                    {
                        sheetMeta.Add(mdp);
                        if (!md.MDP.Contains(mdp))
                            md.MDP.Add(mdp);
                    }
                }
            }
        }

        private void SaveAsCSV(ExcelWorksheet ws, string siteName)
        {
            // save loc should be <path>\\sitename\\ if you want it saved for a specific site
            string newfname = Path.Combine(csvSavePath, siteName + "_" + ws.Name + ".csv");
            using (FileStream fs = new FileStream(newfname, FileMode.Create))
            {
                int colCount = ws.Dimension.End.Column;  //get Column Count
                int rowCount = ws.Dimension.End.Row;     //get row count
                for (int row = 1; row <= rowCount; row++)
                {
                    var tokens = new List<string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        tokens.Add(ws.Cells[row, col].Value?.ToString().Trim());
                    }
                    fs.Write(Encoding.UTF8.GetBytes(string.Join(',', tokens)));
                    fs.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
                }
               //ws.SaveAs(fs, ",");
            }
        }

    }
}
