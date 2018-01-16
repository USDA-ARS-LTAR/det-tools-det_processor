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
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "gco", "http://www.isotc211.org/2005/gco"),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink")
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
          new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=beb1fcde-3ac6-4f19-921a-67923e25a018"),
          new XAttribute(xlink + "title", "GRACEnet REAP POC")

          ), // end contact,
      new XElement(gmd + "dateStamp",
          new XElement(gco + "DateTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ff"))
          ),
      new XElement(gmd + "metadataStandardName",
          new XElement(gco + "CharacterString", "ISO 19115:2003/19139")
          ),
      new XElement(gmd + "metadataStandardVersion",
          new XElement(gco + "CharacterString", "1.0")
          ),
      new XElement(gmd + "referenceSystemInfo",
          new XElement(gmd + "MD_ReferenceSystem",
              new XElement(gmd + "referenceSystemIdentifier",
                  new XElement(gmd + "RS_Identifier",
                      new XElement(gmd + "authority"),
                      new XElement(gmd + "code",
                          new XElement(gco + "CharacterString", "WSG 1984")
                          )
                      )
                  )
              )
          ), // end refsysteminfo
      //new XElement(gmd + "dataSetURI",
      //    new XElement(gco + "CharacterString", "https://gpsr.ars.usda.gov/" + metDat.LocID)
      //    ),
      new XElement(gmd + "identificationInfo",
          new XElement(gmd + "MD_DataIdentification",
              new XElement(gmd + "citation",
                  new XElement(gmd + "CI_Citation",
                      new XAttribute("id", "citation"),
                      new XElement(gmd + "title",
                          new XElement(gco + "CharacterString", metDat.Purp.Site)
                          ),
                      new XElement(gmd + "alternateTitle",
                          new XElement(gco + "CharacterString", metDat.Purp.Alternate) // todo: add alternate title to json and use here
                          ),
                      new XElement(gmd + "date",
                          new XElement(gmd + "CI_Date",
                              new XElement(gmd + "date",
                                  new XElement(gco + "Date", DateTime.Now.ToString("yyyy-MM-dd"))
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
                  new XElement(gmd + "status",
                      new XElement(gmd + "MD_ProgressCode",
                          new XAttribute("codeListValue", "onGoing"),
                          new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_ProgressCode")
                          )
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
                                              new XAttribute(gco + "nilReason", "missing"),
                                              new XElement(gco + "CharacterString")

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
                                              new XElement(gco + "CharacterString", "United States of America")
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
                  new XElement(gmd + "pointOfContact",
                      new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=beb1fcde-3ac6-4f19-921a-67923e25a018"),
                      new XAttribute(xlink + "title", "GRACEnet REAP POC")
                      ), // END POC
                  new XElement(gmd + "resourceMaintenance",
                      new XElement(gmd + "MD_MaintenanceInformation",
                          new XElement(gmd + "maintenanceAndUpdateFrequency",
                              new XElement(gmd + "MD_MaintenanceFrequencyCode",
                                  new XAttribute("codeListValue", "irregular"),
                                  new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_MaintenanceFrequencyCode")
                                  )
                              )
                          )
                      ),
                  new XElement(gmd + "graphicOverview",
                      new XElement(gmd + "MD_BrowseGraphic",
                          new XElement(gmd + "fileName",
                              new XElement(gco + "CharacterString", String.Format("https://gpsr.ars.usda.gov/jpg/{0}.png", metDat.LocID))
                              ),
                          new XElement(gmd + "fileDescription",
                              new XElement(gco + "CharacterString", "Experiment Overview")
                              )
                          )
                      ),
                  new XElement(gmd + "graphicOverview",
                      new XElement(gmd + "MD_BrowseGraphic",
                          new XElement(gmd + "fileName",
                              new XElement(gco + "CharacterString", String.Format("https://gpsr.ars.usda.gov/jpg/{0}.jpg", metDat.LocID))
                              ),
                          new XElement(gmd + "fileDescription",
                              new XElement(gco + "CharacterString", "Plot Layout")
                              )
                          )
                      ),
                  // start physical site loc
                  new XElement(gmd + "descriptiveKeywords",  
                      new XElement(gmd + "MD_Keywords",
                          new XElement(gmd + "keyword",
                              new XElement(gco + "CharacterString", XMLUtils.RemoveValidNodeText(metDat.NALData.FIPSCode))
                              ),
                          new XElement(gmd + "type",
                              new XElement( gmd + "MD_KeywordTypeCode",
                                  new XAttribute("codeList", "http://www.isotc211.org/2005/resources/codeList.xml#MD_KeywordTypeCode"),
                                  new XAttribute("codeListValue", "theme")
                                  )
                              ),
                          new XElement(gmd + "thesaurusName",
                              new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=5a903ecd-1d1c-4618-8194-8c9c32377783"),
                              new XAttribute(xlink + "title", "2010 FIPS Codes for Counties and County Equivalent Entities")
                              )
                          )
                      ), // end physical site loc
                  // Write nasa location
                  new XElement(gmd + "descriptiveKeywords",
                      new XElement(gmd + "MD_Keywords",
                          new XElement(gmd + "keyword",
                              new XElement(gco + "CharacterString", XMLUtils.RemoveValidNodeText(metDat.NALData.NASALocationCode))
                              ),
                          new XElement(gmd + "type",
                              new XElement(gmd + "MD_KeywordTypeCode",
                                  new XAttribute("codeList", "http://www.isotc211.org/2005/resources/codeList.xml#MD_KeywordTypeCode"),
                                  new XAttribute("codeListValue", "place")
                                  )
                              ),
                          new XElement(gmd + "thesaurusName",
                              new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=2b203a27-3b14-4a26-b31a-5151b8bf6fa6"),
                              new XAttribute(xlink + "title", "NASA Global Change Master Directory Locations")
                              )
                          )
                      ), // end nasa loc
                  WriteKeywords(metDat, gmd, gco), // end descriptiveKeywords
                                                                     // department sub loc info
                  WriteNALDataSourceAffiliation(gmd, gco, xlink, metDat.NALData.NALDataSourceCode),
                  // write Federal Prog Code
                  new XElement(gmd + "descriptiveKeywords",
                      new XElement(gmd + "MD_Keywords",
                          new XElement(gmd + "keyword",
                              new XElement(gco + "CharacterString", XMLUtils.RemoveValidNodeText(metDat.NALData.FederalProgramCode))
                              ),
                          new XElement(gmd + "type",
                              new XElement(gmd + "MD_KeywordTypeCode",
                                  new XAttribute("codeList", "http://www.isotc211.org/2005/resources/codeList.xml#MD_KeywordTypeCode"),
                                  new XAttribute("codeListValue", "theme")
                                  )
                              ),
                          new XElement(gmd + "thesaurusName",
                              new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=36abb798-de40-4d75-ba45-864b43b2a49e"),
                              new XAttribute(xlink + "title", "US Federal Program Codes")
                              )
                          )
                      ), // end fed prog codes
                  // write omb bureau codes
                  new XElement(gmd + "descriptiveKeywords",
                      new XElement(gmd + "MD_Keywords",
                          new XElement(gmd + "keyword",
                              new XElement(gco + "CharacterString", XMLUtils.RemoveValidNodeText(metDat.NALData.OMBCode))
                              ),
                          new XElement(gmd + "type",
                              new XElement(gmd + "MD_KeywordTypeCode",
                                  new XAttribute("codeList", "http://www.isotc211.org/2005/resources/codeList.xml#MD_KeywordTypeCode"),
                                  new XAttribute("codeListValue", "theme")
                                  )
                              ),
                          new XElement(gmd + "thesaurusName",
                              new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=6ba5a0f5-d960-4e34-9cc9-80e5217961a7"),
                              new XAttribute(xlink + "title", "Office of Management and Budget Bureau Codes")
                              )
                          )
                      ), // end omb bureau codes
                  new XElement(gmd+ "resourceConstraints",
                      new XElement(gmd + "MD_LegalConstraints",
                          new XElement(gmd + "useLimitation",
                              new XElement(gco + "CharacterString", "Citation requested if data is used.")
                              ),
                          new XElement(gmd + "accessConstraints",
                              new XElement(gmd + "MD_RestrictionCode",
                                  new XAttribute("codeListValue", "otherRestictions"),
                                  new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_RestrictionCode" )
                                  )
                              ),
                          new XElement(gmd + "otherConstraints",
                              new XElement(gco + "CharacterString", "PUBLIC DOMAIN NOTICE:     This database is a \"United States Government Work\" under the terms of the United States Copyright Act.     It was written as part of the author's official duties as a United States Government employee and thus cannot be copyrighted.     This software/database is freely available to the public for use. The Agricultural Research Service (ARS) and the U.S. Government have not placed any restriction on its use or reproduction.     Although all reasonable efforts have been taken to ensure the accuracy and reliability of the software and data, the ARS and the U.S. Government do not and cannot warrant the performance or results that may be obtained by using this software or data.     The ARS and the U.S. Government disclaim all warranties, express or implied, including warranties of performance, merchantability or fitness for any particular purpose.")
                              )
                          )
                      ),
                  new XElement(gmd + "spatialRepresentationType",
                      new XElement(gmd+ "MD_SpatialRepresentationTypeCode",
                          new XAttribute("codeListValue", "vector"),
                          new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_SpatialRepresentationTypeCode")
                          )
                      ),
                  // write all the keywords present in the keywords file.
                  new XElement(gmd + "language",
                      new XElement(gmd + "LanguageCode",
                          new XAttribute("codeList", "http://www.loc.gov/standards/iso639-2/"),
                          new XAttribute("codeListValue", "eng")
                          )
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
          new XElement(gmd + "contentInfo", 
              new XElement(gmd + "MD_FeatureCatalogueDescription",
                  new XElement(gmd+ "includedWithDataset"),
                  new XElement(gmd + "featureCatalogueCitation")
                  )
              ),
              WriteDistributionInfo(metDat, gmd, gco, csvSaveLoc),
              new XElement( gmd + "dataQualityInfo",
                  new XElement(gmd + "DQ_DataQuality",
                      new XElement( gmd + "scope"),
                      new XElement( gmd + "lineage",
                          new XElement(gmd + "LI_Lineage",
                              new XElement(gmd + "statement",
                                  new XElement(gco + "CharacterString", "Discuss amount of knowledge of data quality")
                                  )
                              )
                          )
                      )
                  ),
              new XElement(gmd + "dataQualityInfo",
                  new XElement(gmd + "DQ_DataQuality",
                      new XElement(gmd + "scope",
                          new XElement(gmd + "DQ_Scope",
                              new XElement(gmd + "level",
                                  new XElement(gmd + "MD_ScopeCode",
                                      new XAttribute("codeListValue", "dataset"),
                                      new XAttribute("codeList", "http://standards.iso.org/ittf/PubliclyAvailableStandards/ISO_19139_Schemas/resources/codelist/ML_gmxCodelists.xml#MD_ScopeCode")
                                      )
                                  )
                              )
                          ),
                      new XElement(gmd + "lineage",
                          new XElement(gmd + "LI_Lineage",
                              new XElement(gmd + "statement",
                                  new XElement(gco + "CharacterString", "Discuss knowledge of data acquisation, proceesing and analysis. Add processing steps for details.")
                                  )
                              )
                          )
                      )
                  )

   ); // end mi_metadata
            doc.Add(mi_md);
            return doc;
        }


        private static XElement WriteNALDataSourceAffiliation(XNamespace gmd, XNamespace gco, XNamespace xlink, List<string> nalSources)
        {
            XElement mdkw = new XElement(gmd + "MD_Keywords");
            foreach(string s in nalSources)
            {
                mdkw.Add(
                         new XElement(gmd + "keyword",
                             new XElement(gco + "CharacterString", XMLUtils.RemoveValidNodeText(s))
                             ));
            }
            mdkw.Add(
                 new XElement(gmd + "type",
                      new XElement(gmd + "MD_KeywordTypeCode",
                          new XAttribute("codeList", "http://www.isotc211.org/2005/resources/codeList.xml#MD_KeywordTypeCode"),
                          new XAttribute("codeListValue", "theme")
                          )
                      ),
                 new XElement(gmd + "thesaurusName",
                     new XAttribute(xlink + "href", "local://eng/srv/subtemplate?uuid=dea454ff-f5ca-441e-9f4e-3647c1a1725f"),
                     new XAttribute(xlink + "title", "USDA NAL Data Source Affiliation")
                     )
            ); // end md_kw
            return new XElement(gmd + "descriptiveKeywords", mdkw);
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
                                    new XElement(gco + "CharacterString", metDat.Purp.Site)
                                    ),
                                new XElement(gmd + "description",
                                    new XElement(gco + "CharacterString", "Web link to dataset")
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
            //mdKws.Add(new XElement(gmd + "type",
            //              new XAttribute(gco+ "nilReason", "unknown")
            //              ),
            //          new XElement(gmd + "thesaurusName",
            //              new XAttribute(gco + "nilReason", "inapplicable")
            //              )
            //         );
            return new XElement(gmd + "descriptiveKeywords", mdKws);
        }

    }
}
