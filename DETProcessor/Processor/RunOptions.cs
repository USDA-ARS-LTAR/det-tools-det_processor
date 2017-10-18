using System;
using System.Collections.Generic;
using System.Text;

namespace DETProcessor.Processor
{
    class RunOptions
    {
        public bool CreateCSVs { get; set; }
        public bool CreateCitation { get; set; }
        public bool CreateNALMeta { get; set; }
        public bool CreateESRI { get; set; }
        public bool CreateZip { get; set; }

    }
}
