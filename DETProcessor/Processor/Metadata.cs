using System;
using System.Collections.Generic;
using System.Text;

namespace DETProcessor.Processor
{
    public class Metadata
    {
        public string LocID { get; set; }
        public string CitationName { get; set; }
        public Purpose Purp { get; set; }

        public Keywords Keys { get; set; }
        public SitePI Pi { get; set; }

        public List<Person> Persons {get; set;} 

        public DateTime PeriodBegin { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int NumberOfYears { get; set; }

        public BoundingBox Box { get; set; }

        public List<MetaDataPair> MDP { get; set; }
    }

    public class MetaDataPair
    {
        public string Column { get; set; }
        public string Description { get; set; }
        public string Units { get; set; }
        public string DataType { get; set; }

    }

    public class Purpose
    {
        public string Site { get; set; }
        public string Description { get; set; }
    }

    public class Keywords
    {
        public List<string> words { get; set; } = new List<string>();
    }

    public class BoundingBox
    {
        public double West { get; set; }
        public double North { get; set; }
        public double East { get; set; }
        public double South { get; set; }
    }

}
