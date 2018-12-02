using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SbdNetLib;

namespace CcsEnityFramework
{
    class Operations
    {
        public static void updateName(document doc)
        {
            doc.metadata.col_000 = "Churchill";
        }
        public static void updateFilePath(document doc)
        {
            foreach(var file in doc.files)
            {
                if(file.volume_id == 1)
                {
                    file.relative_path = "C:\\Users\\kchurchill\\Documents\\test.PNG";
                }
                else
                {
                    file.relative_path = "C:\\Users\\kchurchill\\Documents\\test2.PNG";
                }
                
            }
        }
        public static void getSbFileType(document doc, List<FileBytes> fbList)
        {
            var snow = new Snowbnd();
            foreach (var file in doc.files)
            {
                var bytes = CcsFunctions.readFile(file, fbList, doc);
                
                if(doc.exception_code == null)
                {
                    
                    var sbFileType = snow.CIMGLOW_get_filetype_mem(bytes);

                    CcsServerEntities ctx = new CcsServerEntities();
                    var fileType = ctx.file_type
                        .Where(f => f.sb_file_type == sbFileType)
                        .FirstOrDefault();

                    file.file_type_id = fileType.id;
                    file.sb_file_type = fileType.sb_file_type;
                    

                }
                
            }
        }
    }
}
