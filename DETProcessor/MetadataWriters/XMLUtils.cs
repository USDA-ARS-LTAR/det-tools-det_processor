using System;
using System.Collections.Generic;
using System.Text;

namespace DETProcessor.MetadataWriters
{
    public static class XMLUtils
    {
        public static string RemoveValidNodeText(string text)
        {
            return text.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
        }
    }
}
