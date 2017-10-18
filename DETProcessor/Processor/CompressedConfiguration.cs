using System;
using System.Collections.Generic;
using System.Text;

namespace DETProcessor.Processor
{
    class CompressedConfiguration
    {
        public string CompressedDir { get; set; }
        public string CompressedName { get; set; }
        public string IncludeBase { get; set; }
        public List<string> SearchDir { get; set; }
        public List<string> IncludeFiles { get; set; }
        public bool IncludeCitation { get; set; }
    }
}
