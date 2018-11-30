using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

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
        public static void createDipFile(CcsServerEntities ctx)
        {
            foreach(document doc in ctx.documents)
            {
                var dtm = findMapping(doc);
            }
        }
        private static doc_type_mapping findMapping(document doc)
        {
            CcsServerEntities ctx = new CcsServerEntities();
            var dtm = ctx.doc_type_mapping
                .Where(d => d.lookup_000 == doc.metadata.col_005)
                .FirstOrDefault();

            //loop through keytype mapset and pull out the non null values
            foreach (var prop in dtm.key_type_mapset.GetType().GetProperties())
            {
                var property = prop.Name;
                var value =  prop.GetValue(dtm.key_type_mapset, null).ToString() ;

                var onbaseKeyType = ctx.onbase_key_type
                    .Where(o => o.id == int.Parse(value))
                    .FirstOrDefault();

                if(value != null)
                {
                   // Debug.WriteLine("{0} = {1}", prop.Name, onbaseKeyType.name);
                }
                    
            }
            

            return dtm;
        }
    }
}
