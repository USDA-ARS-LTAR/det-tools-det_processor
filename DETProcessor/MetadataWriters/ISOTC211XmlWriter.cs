using DETProcessor.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DETProcessor.MetadataWriters
{
    class ISOTC211XmlWriter
    {
        internal static void MakeISOTC211XML(string xmlPath, Metadata metDat, string csvSaveLoc)
        {
            XDocument doc = CreateDoc(metDat, csvSaveLoc);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";  // Indent 3 Spaces
            Directory.CreateDirectory(xmlPath); // create if doens't exist
            string name = Path.Combine(xmlPath, metDat.LocID + "_isotc211_meta.xml");
            using (XmlWriter writer = XmlWriter.Create(new FileStream(name, FileMode.Create), settings))
            {
                doc.Save(writer);
            }
        }

        private static XDocument CreateDoc(Metadata metDat, string csvSaveLoc)
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "en", "no"));
            XNamespace gmx = "http://www.isotc211.org/2005/gmx";
            XNamespace xlink = "http://www.w3.org/1999/xlink";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace gco = "http://www.isotc211.org/2005/gco";
            XNamespace gts = "http://www.isotc211.org/2005/gts";
            XNamespace gml = "http://www.opengis.net/gml";
            XNamespace gmd = "http://www.isotc211.org/2005/gmd";

            XElement mi_md = new XElement(gmd + "MD_Metadata",
                new XAttribute(XNamespace.Xmlns + "gts", "http://www.isotc211.org/2005/gts"),
                new XAttribute(XNamespace.Xmlns + "gml", "http://www.opengis.net/gml"),
                new XAttribute(XNamespace.Xmlns + "gmd", "http://www.isotc211.org/2005/gmd"),
                new XAttribute(XNamespace.Xmlns + "gco", "http://www.isotc211.org/2005/gco"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
                new XAttribute(XNamespace.Xmlns + "gmx", "http://www.isotc211.org/2005/gmx"),
                new XAttribute(xsi + "schemaLocation", "http://www.isotc211.org/2005/gmd http://www.isotc211.org/2005/gmd/gmd.xsd http://www.isotc211.org/2005/srv http://schemas.opengis.net/iso/19139/20060504/srv/srv.xsd http://www.isotc211.org/2005/gmx http://www.isotc211.org/2005/gmx/gmx.xsd")
                );
            mi_md.Add(
      new XElement(gmd + "fileIdentifier",
          new XElement(gco + "CharacterString", Guid.NewGuid().ToString())
      ),
      new XElement(gmd + "language",
          new XElement(gmd + "LanguageCode",
              new XAttribute("codeList", "http://www.loc.gov/standards/iso639-2/"),
              new XAttribute("codeListValue", "eng")
              )
          ),
      new XElement(gmd + "characterSet",
          new XElement(gmd + "MD_CharacterSetCode",
              new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_CharacterSetCode"),
              new XAttribute("codeListValue", "utf8")
              )
          ),
      new XElement(gmd + "hierarchyLevel",
          new XElement(gmd + "MD_ScopeCode",
              new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_ScopeCode"),
              new XAttribute("codeListValue", "dataset")
              )
          ),
      new XElement(gmd + "contact",
          new XElement(gmd + "CI_ResponsibleParty",
              new XElement(gmd + "individualName",
                  new XElement(gco + "CharacterString", "Bruce Vandenberg")
                  ),
              new XElement(gmd + "organisationName",
                  new XElement(gco + "CharacterString", "USDA-ARS")
                  ),
              new XElement(gmd + "contactInfo",
                  new XElement(gmd + "CI_Contact",
                      new XElement(gmd + "address",
                          new XElement(gmd + "CI_Address",
                              new XElement(gmd + "electronicMailAddress",
                                  new XElement(gco + "CharacterString", "bruce.vandenberg@ars.usda.gov")
                                  )
                              )
                          )
                      )
                  ),
              new XElement(gmd + "role",
                  new XElement(gmd + "CI_RoleCode",
                      new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#CI_RoleCode"),
                      new XAttribute("codeListValue", "publisher")
                      )
                  )
              )
          ), // end contact
      new XElement(gmd + "dateStamp",
          new XElement(gco + "DateTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ff"))
          ),
      new XElement(gmd + "metadataStandardName",
          new XElement(gco + "CharacterString", "ISO 19115:2003/19139")
          ),
      new XElement(gmd + "metadataStandardVersion",
          new XElement(gco + "CharacterString", "1.0")
          ),
      new XElement(gmd + "dataSetURI",
          new XElement(gco + "CharacterString", "https://gpsr.ars.usda.gov/" + metDat.LocID)
          ),
      new XElement(gmd + "identificationInfo",
          new XElement(gmd + "MD_DataIdentification",
              new XElement(gmd + "citation",
                  new XElement(gmd + "CI_Citation",
                      new XAttribute("id", "citation"),
                      new XElement(gmd + "title",
                          new XElement(gco + "CharacterString", metDat.Purp.Site)
                          ),
                      new XElement(gmd + "date",
                          new XElement(gmd + "CI_Date",
                              new XElement(gmd + "date",
                                  new XElement(gco + "DateTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ff"))
                                  ),
                              new XElement(gmd + "dateType",
                                  new XElement(gmd + "CI_DateTypeCode",
                                      new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#CI_DateTypeCode"),
                                      new XAttribute("codeListValue", "creation")
                                          )
                                      )
                                  ) // end CI_Date
                              ) // end date
                          ) // CI_Citation
                      ), // end citation
                  new XElement(gmd + "abstract",
                      new XElement(gco + "CharacterString", metDat.Purp.Site + "\n" + metDat.Purp.Description)
                      ),
                  new XElement(gmd + "pointOfContact",
                      new XElement(gmd + "CI_ResponsibleParty",
                          new XElement(gmd + "individualName",
                              new XElement(gco + "CharacterString", metDat.Pi.FirstName + " " + metDat.Pi.LastName)
                              ),
                          new XElement(gmd + "organisationName",
                              new XElement(gco + "CharacterString", "USDA-ARS")
                              ),
                          new XElement(gmd + "positionName",
                              new XElement(gco + "CharacterString", "Scientist")
                              ),
                          new XElement(gmd + "contactInfo",
                              new XElement(gmd + "CI_Contact",
                                  new XElement(gmd + "phone",
                                      new XElement(gmd + "CI_Telephone",
                                          new XElement(gmd + "voice",
                                              new XElement(gco + "CharacterString", metDat.Pi.Telephone)
                                              )
                                          )
                                      ), // end phone
                                  new XElement(gmd + "address",
                                      new XElement(gmd + "CI_Address",
                                          new XElement(gmd + "deliveryPoint",
                                              new XElement(gco + "CharacterString", " ")
                                              ),
                                          new XElement(gmd + "city",
                                              new XElement(gco + "CharacterString", metDat.Pi.City)
                                              ),
                                          new XElement(gmd + "administrativeArea",
                                              new XElement(gco + "CharacterString", metDat.Pi.State)
                                              ),
                                          new XElement(gmd + "postalCode",
                                              new XElement(gco + "CharacterString", metDat.Pi.PostalCode)
                                              ),
                                          new XElement(gmd + "country",
                                              new XElement(gco + "CharacterString", "US")
                                              ),
                                          new XElement(gmd + "electronicMailAddress",
                                              new XElement(gco + "CharacterString", metDat.Pi.Email)
                                              )
                                          )
                                      ) // end address
                                  ) // CI_Contact
                              ), // end contactInfo
                          new XElement(gmd + "role",
                              new XElement(gmd + "CI_RoleCode",
                                      new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#CI_RoleCode"),
                                      new XAttribute("codeListValue", "principalInvestigator")
                                  )
                              )
                          ) // end CI_ResponsibleParty
                      ), // end pointOfContact
                         // write all the keywords present in the keywords file.
                  WriteKeywords(metDat, gmd, gco), // end descriptiveKeywords
                  new XElement(gmd + "language",
                          new XElement(gco + "CharacterString", "eng")
                      ),
                  new XElement(gmd + "topicCategory",
                          new XElement(gmd + "MD_TopicCategoryCode", "farming")
                      ),
                  new XElement(gmd + "topicCategory",
                          new XElement(gmd + "MD_TopicCategoryCode", "environment")
                      ),
                  new XElement(gmd + "extent",
                          new XElement(gmd + "EX_Extent",
                              new XAttribute("id", "boundingbox"),
                              new XElement(gmd + "description",
                                  new XElement(gco + "CharacterString", "Time range of experiment")
                                  ),
                              new XElement(gmd + "temporalElement",
                                  new XElement(gmd + "EX_TemporalExtent",
                                   new XElement(gmd + "extent",
                                      new XElement(gml + "TimePeriod",
                                          new XAttribute(gml + "id", "d41586e217a1051934"),
                                          new XElement(gml + "beginPosition", metDat.PeriodBegin.ToString("yyyy-MM-dd")),
                                          new XElement(gml + "endPosition", metDat.PeriodEnd.ToString("yyyy-MM-dd"))
                                          )
                                      )
                                  )
                              )
                          )
                      ), // end extent
                  new XElement(gmd + "extent",
                      new XElement(gmd + "EX_Extent",
                          new XAttribute("id", "tempextent"),
                          new XElement(gmd + "description",
                              new XElement(gco + "CharacterString", "Bounding Box of study")
                              ),
                          new XElement(gmd + "geographicElement",
                              new XElement(gmd + "EX_GeographicBoundingBox",
                                  new XElement(gmd + "westBoundLongitude",
                                      new XElement(gco + "Decimal", metDat.Box.West)
                                      ),
                                  new XElement(gmd + "eastBoundLongitude",
                                      new XElement(gco + "Decimal", metDat.Box.East)
                                      ),
                                  new XElement(gmd + "southBoundLatitude",
                                      new XElement(gco + "Decimal", metDat.Box.South)
                                      ),
                                  new XElement(gmd + "northBoundLatitude",
                                      new XElement(gco + "Decimal", metDat.Box.North)
                                      )
                                  )
                              )
                          )
                      ) // end extent
                  ) // end MD_DataIdentification
              ), // end identificationInfo
              WriteDistributionInfo(metDat, gmd, gco, csvSaveLoc)
   ); // end mi_metadata
            doc.Add(mi_md);
            return doc;
        }

        private static XElement WriteDistributionInfo(Metadata metDat, XNamespace gmd, XNamespace gco, string csvSaveLoc)
        {
            XElement mdDist = new XElement(gmd + "MD_Distribution");
            mdDist.Add(new XElement(gmd + "distributionFormat",
                           new XElement(gmd + "MD_Format",
                               new XElement(gmd + "name",
                                   new XElement(gco + "CharacterString", "Text")
                                       ),
                                   new XElement(gmd + "version",
                                       new XElement(gco + "CharacterString", "1.0")
                                       )
                                   )
                               )
                           );
            mdDist.Add(
                new XElement(gmd + "transferOptions",
                    new XElement(gmd + "MD_DigitalTransferOptions",
                        new XElement(gmd + "onLine",
                            new XElement(gmd + "CI_OnlineResource",
                                new XElement(gmd + "linkage",
                                    new XElement(gmd + "URL", "https://gpsr.ars.usda.gov/" + metDat.LocID)
                                    ),
                                new XElement(gmd + "protocol",
                                    new XElement(gco + "CharacterString", "WWW:LINK-1.0-http--link")
                                    ),
                                new XElement(gmd + "name",
                                    new XAttribute(gco + "nilReason", "missing"),
                                    new XElement(gco + "CharacterString")
                                    ),
                                new XElement(gmd + "description",
                                    new XAttribute(gco + "nilReason", "missing"),
                                    new XElement(gco + "CharacterString")
                                    )
                                ) // CI_OnlineResource
                            ) // onLine
                        ) // MD_DigitalTransferOptions
                    )
                );
            var fNames = Directory.GetFiles(csvSaveLoc, "*.csv")
                        .Select(Path.GetFileName)
                        .ToArray();
            foreach (string file in fNames)
            {
                mdDist.Add(
                           new XElement(gmd + "transferOptions",
                               new XElement(gmd + "MD_DigitalTransferOptions",
                                   new XElement(gmd + "onLine",
                                       new XElement(gmd + "CI_OnlineResource",
                                           new XElement(gmd + "linkage",
                                               new XElement(gmd + "URL", "https://gpsr.ars.usda.gov/csv/" + metDat.LocID + "/" + file)
                                               ),
                                           new XElement(gmd + "protocol",
                                               new XAttribute(gco + "nilReason", "missing"),
                                               new XElement(gco + "CharacterString", "WWW:DOWNLOAD-1.0-http--download")
                                               ),
                                           new XElement(gmd + "name",
                                               new XAttribute(gco + "nilReason", "missing"),
                                               new XElement(gco + "CharacterString")
                                               ),
                                           new XElement(gmd + "description",
                                               new XAttribute(gco + "nilReason", "missing"),
                                               new XElement(gco + "CharacterString")
                                               )
                                           ) // CI_OnlineResource
                                       ) // onLine
                                   ) // MD_DigitalTransferOptions
                               ) // transferOptions
                           );
            }

            return new XElement(gmd + "distributionInfo", mdDist);
        }

        private static XElement WriteKeywords(Metadata metDat, XNamespace gmd, XNamespace gco)
        {

            XElement mdKws = new XElement(gmd + "MD_Keywords");
            foreach (string s in metDat.Keys.words)
            {
                mdKws.Add(new XElement(gmd + "keyword",
                              new XElement(gco + "CharacterString", s)
                              ));
            }
            mdKws.Add(new XElement(gmd + "type",
                          new XElement(gmd + "MD_KeywordTypeCode",
                              new XAttribute("codeList", ""),
                              new XAttribute("codeListValue", "theme")
                              )
                          ));
            return new XElement(gmd + "descriptiveKeywords", mdKws);
        }

    }
}
