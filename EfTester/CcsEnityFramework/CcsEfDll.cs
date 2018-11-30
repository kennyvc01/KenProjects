using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CcsEnityFramework
{
    public class CcsEfDll
    {
        public static int getSnapshotId(int snapshotId)
        {
            CcsServerEntities ctx = new CcsServerEntities();

            var sId = ctx.snapshots
                .Where(s => s.id == snapshotId)
                .FirstOrDefault();

            return sId.id;
        }

        public static void processSnapshot(int snapshotId)
        {
            CcsServerEntities ctx = new CcsServerEntities();

            //Grab docs from context
            var docs = ctx.documents
                .Where(d => d.snapshot_id == snapshotId);

            //Pipeline
            Debug.WriteLine("Beginning Pipeline");
            foreach(var doc in docs)
            {
                //Initialize byte array list
                List < FileBytes > fbList = new List<FileBytes>();

                if(doc.exception_code == null && doc.exclusion_code == null)
                {
                    Operations.updateName(doc);
                }
                if (doc.exception_code == null && doc.exclusion_code == null)
                {
                    Operations.updateFilePath(doc);
                }
                if (doc.exception_code == null && doc.exclusion_code == null)
                {
                    //Operations.getSbFileType(doc,fbList);
                }

            }
            Debug.WriteLine("Pipleine has completed");

            Debug.WriteLine("Creating DIP for for snapshot id: " + snapshotId);
            CcsFunctions.createDipFile(ctx);
            //Check the results
            foreach (var doc in docs)
            {
                var f = doc.files
                    .FirstOrDefault();

                //Debug.WriteLine(f.sb_file_type);
                //Debug.WriteLine(f.file_type_id);
            }
        }
    }
}
