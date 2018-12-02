using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace CcsEnityFramework
{
    class CcsFunctions
    {
        [DebuggerHidden]
        public static byte[] readFile (file file,List<FileBytes> fbList, document doc)
        {
            //Set rp equal to the files relative path
            var rp = file.relative_path;

            var fb = fbList
                .Where(f => f.filePath == rp)
                .FirstOrDefault();

            //if the file path is already in the list, return the byte array
            if(fb != null)
            {
                //Debug.WriteLine("Bytes already exist for rp " + rp + file.doc_id);
                return fb.byteArray;
            }
            //If the file path isn't in the file bytes list, then add it and return
            else
            {
                byte[] bytes;
                try
                {
                    //Debug.WriteLine("Bytes don't exist...adding them for rp " + rp + file.doc_id);
                    bytes = File.ReadAllBytes(rp);
                    fbList.Add(new FileBytes { filePath = rp, byteArray = bytes });
                }
                catch (Exception ex)
                {
                    file.document.exception_code = 16;
                    bytes = new byte[0];
                }


                return bytes;
            }
        }
        public static void createDipFile(CcsServerEntities ctx, JObject mappingJsonConfig, int snapShotId)
        {
            //Get docs from the database
            var docs = ctx.documents
                .Where(d => d.snapshot_id == snapShotId);

            //Initialize list for mappings
            List<Mappings> mappingList = getMappingList(ctx);
            //Get mappings from database
            var mappings = ctx.doc_type_mapping;


            //Parse the mappings.json file
            JArray mappingIndex = (JArray)mappingJsonConfig["LookupAssignments"];
            string conversionIdProperty = (string)mappingJsonConfig["CcsOnBaseDocumentIdKeyTypeName"];
            string legacyIdProperty = (string)mappingJsonConfig["LegacyDocumentIdKeyTypeName"];
            JObject staticKeywords = (JObject)mappingJsonConfig["StaticKeywords"];


            //Initialize a new list for the dipfile entries
            List<string> dipFileList = new List<string>();

            //Loop through the docs and add to dipFileList as it goes
            foreach (document doc in docs)
            {

                if (doc.exception_code == null && doc.exclusion_code == null)
                {
                    //Get the legacy doc type value from metadata
                    var mappingCol = "col_" + mappingIndex[0].ToString().PadLeft(3, '0');
                    var legacyDocType = doc.metadata.GetType().GetProperty(mappingCol).GetValue(doc.metadata, null);

                    //lookup the mapping for the individual doc
                    var mapping = mappingList
                        .Where(m => m.lookUp0 == legacyDocType.ToString())
                        .FirstOrDefault();


                    //Add the appropriate values to the DIP file
                    var onBaseDocType = mapping.onbaseDocType;

                    dipFileList.Add(">>BEGIN:");
                    dipFileList.Add(">>DocTypeName: " + onBaseDocType);
                    dipFileList.Add(">>DocDate: " + "Needs to be added");
                    dipFileList.Add(String.Format("{0}: {1}", conversionIdProperty, doc.id));
                    dipFileList.Add(String.Format("{0}: {1}", legacyIdProperty, doc.legacy_id));
                    dipFileList.Add("Legacy Document Type: " + legacyDocType);

                    foreach (var item in staticKeywords)
                    {
                        dipFileList.Add(String.Format("{0}: {1}", item.Key, item.Value));
                    }

                    foreach (var map in mapping.keyTypeMapset)
                    {
                        var value = doc.metadata.GetType().GetProperty(map.metadataColIndex).GetValue(doc.metadata, null);
                        dipFileList.Add(String.Format("{0}: {1}", map.onBaseKeywordName, value));
                    }

                    foreach (var file in doc.files.OrderBy(f => f.seq))
                    {
                        var fileType = file.file_type;
                        if(fileType !=null && fileType.hsi_filetypenum.ToString() != null)
                        {
                            dipFileList.Add(">>FileTypeNum: " + file.file_type.hsi_filetypenum);
                            dipFileList.Add(">>FullPath: " + file.relative_path);
                        }
                        else
                        {
                            doc.exception_code = 34;
                        }

                    }
                }
            }
            
            TextWriter tw = new StreamWriter(@".\dip_01.txt");
            foreach(string s in dipFileList)
            {
                tw.WriteLine(s);
            }
            tw.Close();
        }
        private static List<Mappings> getMappingList(CcsServerEntities ctx)
        {
            //Loop through OnBase Key Types and add to a list of the OnBaseKeyType object
            List<OnBaseKeyType> oktList = new List<OnBaseKeyType>();
            foreach (onbase_key_type okt in ctx.onbase_key_type)
            {
                oktList.Add(new OnBaseKeyType { onbaseKeyTypeId = okt.id, onbaseKeyTypeName = okt.name });
            }

            List<Mappings> maps = new List<Mappings>();
            //Loop through Doc Type Mapping and add to the object
            foreach (doc_type_mapping dMap in ctx.doc_type_mapping)
            {
                var ktmList = getKeyTypeMapset(dMap.key_type_mapset, oktList);
                maps.Add(new Mappings { onbaseDocType = dMap.onbase_doc_type.name.ToString(), keyTypeMapset = ktmList, lookUp0 = dMap.lookup_000.ToString() });
            }
            return maps;

        }
        private static List<KeyTypeMapsets> getKeyTypeMapset(key_type_mapset ktm, List<OnBaseKeyType> onbaseKeyTypes)
        {
            List < KeyTypeMapsets > ktmList = new List<KeyTypeMapsets>();
            //loop through keytype mapset and pull out the non null values
            foreach (var prop in ktm.GetType().GetProperties())
            {
                var value = prop.GetValue(ktm, null);
                var property = prop.Name;

                if(value != null && property.ToString().Length > 3 && property.ToString().Substring(0, 4) == "col_")
                {
                    var keytypeId = value.ToString();
                    //Look for the OnBase keyword name
                    var onbaseKeyType = onbaseKeyTypes
                        .Where(o => o.onbaseKeyTypeId.ToString() == keytypeId)
                        .FirstOrDefault();

                    //Add non-nulls to the KeyTypeMapset list and return
                    ktmList.Add(new KeyTypeMapsets { keyTypeMapsetId = ktm.id, metadataColIndex = property, onBaseKeyTypeId = onbaseKeyType.onbaseKeyTypeId, onBaseKeywordName = onbaseKeyType.onbaseKeyTypeName });

                    //Debug.WriteLine("{0} = {1}", prop.Name, onbaseKeyType.onbaseKeyTypeName);
                }
            }
            return ktmList;
        }
    }
}
