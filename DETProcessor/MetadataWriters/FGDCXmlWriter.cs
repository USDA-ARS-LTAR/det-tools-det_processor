using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using DETProcessor.Processor;
using System.IO;
using System.Linq;

namespace DETProcessor.MetadataWriters
{
    class FGDCXmlWriter
    {

        internal static void MakeFGDCXML(string xmlPath, Metadata metDat, string csvSaveLoc)
        {
            XDocument md = CreateDoc(metDat, csvSaveLoc);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";  // Indent 3 Spaces


            //if (Directory.Exists(xmlPath))
            //{
            //    DirectoryInfo dir = new DirectoryInfo(xmlPath);

            //    foreach (FileInfo csvFile in dir.GetFiles("*" + metDat.LocID + "*"))
            //    {
            //        csvFile.Delete();
            //    }
            //}
            //else
            //{
            //    Directory.CreateDirectory(xmlPath); // create if does not exist.
            //}
            Directory.CreateDirectory(xmlPath); // create if doens't exist
            string name = Path.Combine(xmlPath, metDat.LocID + "_esri_meta.xml");
            using (XmlWriter writer = XmlWriter.Create(new FileStream(name, FileMode.Create), settings))
            {
                md.Save(writer);
            }
        }

        private static XDocument CreateDoc(Metadata metDat, string csvSaveLoc)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("metadata",
                    new XElement("dataIdInfo",
                        new XElement("idCitation",
                            new XElement("resTitle", metDat.LocID),
                            new XElement("date",
                                new XElement("pubDate", DateTime.Now)
                                ),
                            new XElement("citRespParty",
                                new XElement("rpIndName", "Bruce Vandenberg"),
                                new XElement("rpOrgName", "USDA-ARS-CARR"),
                                new XElement("rpPosName", "Data POC"),
                                new XElement("role",
                                    new XElement("RoleCd", new XAttribute("value", "010"))
                                    ),
                                new XElement("rpCntInfo",
                                    new XElement("cntAddress",
                                        new XAttribute("addressType", "postal"),
                                        new XElement("eMailAdd", "bruce.vandenberg@ars.usda.gov"),
                                        new XElement("delPoint", "2150 Centre Avenue, Building D, Suite 340, Room 3283 "),
                                        new XElement("city", "Fort Collins"),
                                        new XElement("adminArea", "Colorado"),
                                        new XElement("postCode", "80526"),
                                        new XElement("country", "US")
                                        )
                                    )
                                )
                            ),
                        new XElement("idAbs", metDat.Purp.Site),
                        new XElement("idPurp", metDat.Purp.Description.Substring(0, 2048)),
                        new XElement("dataLang",
                            new XElement("languageCode",
                                new XAttribute("value", "eng")
                                )
                            ),
                        new XElement("idPurp",
                            new XElement("CharSetCd",
                                new XAttribute("value", "004")
                                )
                            ),
                        new XElement("idStatus",
                            new XElement("ProgCd",
                                new XAttribute("value", "007")
                                )
                            ),
                        new XElement("tpCat",
                            new XElement("TopicCatCd",
                                new XAttribute("value", "001")
                                )
                            ),
                        new XElement("tpCat",
                            new XElement("TopicCatCd",
                                new XAttribute("value", "007")
                                )
                            ),
                        WriteKeys(metDat.Keys.words),
                        new XElement("dataExt",
                            new XElement("geoEle",
                                new XElement("GeoBndBox",
                                    // values from dataset
                                    new XElement("westBL", metDat.Box.West),
                                    new XElement("eastBL", metDat.Box.East),
                                    new XElement("southBL", metDat.Box.South),
                                    new XElement("northBL", metDat.Box.North)
                                    )
                                ),
                            new XElement("tempEle",
                                new XElement("TempExtent",
                                    new XElement("exTemp",
                                        new XElement("TM_Period",
                                            new XElement("tmBegin", metDat.PeriodBegin),
                                            new XElement("tmEnd", metDat.PeriodEnd)
                                            )
                                        )
                                    )
                                ),
                            new XElement("exDesc", metDat.NumberOfYears)
                            ), // dataExt
                        new XElement("resMaint",
                            new XElement("maintFreq",
                                new XElement("MaintFreqCd", new XAttribute("value", "010"))
                                )
                            )
                    ), // end dataidinfo
                WriteDistInfo(metDat, csvSaveLoc), // end distInfo
                WriteAttributeInfo(metDat), // end eainfo
                new XElement("Esri",
                    new XElement("ArcGISstyle", "FGDC CSDGM Metadata"),
                    new XElement("CreaDate", DateTime.Now.ToString("yyyy-dd-MM")),
                    new XElement("CreaTime", DateTime.Now.ToString("HH:mm:ss.ff")),
                    new XElement("ModDate", DateTime.Now.ToString("yyyy-dd-MM")),
                    new XElement("ModTime", DateTime.Now.ToString("HH:mm:ss.ff")),
                    new XElement("ArcGISFormat", "1.0"),
                    new XElement("ArcGISProfile", "FGDC"),
                    new XElement("PublishStatus", "editor:esri.dijit.metadata.editor")
                    ),// end esri
                new XElement("mdDateSt", DateTime.Now.ToString("yyyy-dd-MM")),
                new XElement("mdFileID", Guid.NewGuid().ToString()),
                new XElement("mdChar",
                    new XElement("CharSetCd", new XAttribute("value", "004"))
                    ),
                new XElement("mdContact",
                    new XElement("rpIndName", metDat.Pi.FirstName + " " + metDat.Pi.LastName),
                    new XElement("rpOrgName", "USDA-ARS"),
                    new XElement("rpPosName", "PI"),
                    new XElement("role",
                    new XElement("roleCd", new XAttribute("value", "008"))
                        ),
                    new XElement("rpCntInfo",
                        new XElement("cntAddress",
                            new XAttribute("addressType", "postal"),
                            new XElement("eMailAdd", metDat.Pi.Email),
                            new XElement("delPoint", " "),
                            new XElement("city", metDat.Pi.City),
                            new XElement("adminArea", metDat.Pi.State),
                            new XElement("postCode", metDat.Pi.PostalCode),
                            new XElement("country", "US")
                            ),
                        new XElement("cntPhone",
                            new XElement("voiceNum", metDat.Pi.Telephone)
                            )
                        )// end rpCntInfo
                    ) // end mdContact
                ) // end metadata
            ); // end document
            return doc;
        }

        private static XElement WriteDistInfo(Metadata metDat, string csvSaveLoc)
        {

            var distTranOps = new XElement("distTranOps",
                                    new XElement("onLineSrc",
                                        new XElement("linkage", "https://gpsr.ars.usda.gov/" + metDat.LocID)
                                        )
                                    );
            var fNames = Directory.GetFiles(csvSaveLoc, "*.csv")
                                    .Select(Path.GetFileName)
                                    .ToArray();
            foreach (string file in fNames)
            {
                distTranOps.Add(new XElement("onLineSrc",
                            new XElement("linkage", "https://gpsr.ars.usda.gov/csv/" + metDat.LocID + "/" + file)
                            )
                        );
            }
            var distinfo = new XElement("distInfo", distTranOps);
            return distinfo;
        }

        private static XElement WriteAttributeInfo(Metadata md)
        {

            var detailed = new XElement("detailed",
                                    new XElement("enttyp",
                                        new XElement("enttypl", "Experiment"),
                                        new XElement("enttypd", md.Purp.Site),
                                        new XElement("enttypds", "Observations")
                                        )
                                    );
            foreach (MetaDataPair datpair in md.MDP)
            {
                detailed.Add(
                    new XElement("attr",
                        new XElement("attrlabl", datpair.Column),
                        new XElement("attrdef", datpair.Description),
                        new XElement("attrdefs", "Observation"),
                        new XElement("attrtype", datpair.DataType),
                        new XElement("attwidth", "10"),
                        new XElement("attrdomv",
                            new XElement("udom", datpair.Units)
                            )
                        )
                    );
            }

            var eainfo = new XElement("eainfo", detailed);
            return eainfo;
        }

        private static XElement WriteKeys(List<string> words)
        {
            var xe = new XElement("themeKeys");
            foreach (string w in words)
            {
                if (string.IsNullOrEmpty(w)) break;
                xe.Add(new XElement("keyword", w));
            }
            return xe;
        }

    }
}
