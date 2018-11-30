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
        public static void createDipFile(CcsServerEntities ctx, JArray mappingIndex)
        {
            //Loop through Doc Type Mapping and add to a list of the mappings object
            List < Mappings > dtmList = new List<Mappings>();
            foreach(doc_type_mapping dMap in ctx.doc_type_mapping)
            {
                dtmList.Add(new Mappings { onbaseDocType = dMap.onbase_doc_type.name, lookUp0 = dMap.lookup_000, lookUp1 = dMap.lookup_001, lookUp2 = dMap.lookup_002, lookUp3 = dMap.lookup_003, lookUp4 = dMap.lookup_004 });
            }
            
        }
        private static doc_type_mapping findMapping(document doc, JArray mappingIndex)
        {
            var mappingCol = "col_" + mappingIndex[0].ToString().PadLeft(3, '0');
            var mappingValue = doc.metadata.GetType().GetProperty(mappingCol).GetValue(doc.metadata, null);

            CcsServerEntities ctx = new CcsServerEntities();
            var dtm = ctx.doc_type_mapping
                .Where(d => d.lookup_000 == mappingValue.ToString())
                .FirstOrDefault();

            //loop through keytype mapset and pull out the non null values
            foreach (var prop in dtm.key_type_mapset.GetType().GetProperties())
            {
                var property = prop.Name;
                var value =  prop.GetValue(dtm.key_type_mapset, null);

                if(value != null && property.ToString().Length > 3 && property.ToString().Substring(0,4) == "col_")
                {
                    
                    var keytypeId = value.ToString();
                    //Look for the OnBase keyword name
                    
                    var onbaseKeyType = ctx.onbase_key_type
                        .Where(o => o.id.ToString() == keytypeId)
                        .FirstOrDefault();
                    
                    //Debug.WriteLine("{0} = {1}", prop.Name, onbaseKeyType.name);
                }
                    
            }
            

            return dtm;
        }
    }
}
