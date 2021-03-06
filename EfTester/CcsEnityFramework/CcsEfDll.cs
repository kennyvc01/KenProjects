﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CcsEnityFramework
{
    public class CcsEfDll
    {
        public static JObject getMappingIndex()
        {

            //grab mapping column from dip.json
            JObject o1 = JObject.Parse(File.ReadAllText(@"C:\Users\kenny\Documents\Developer\ccs-2.3.305\CcsServer\cfg\projects\1\mappings.json"));

            return o1;
        }
        public static void processSnapshot(int snapshotId)
        {
            CcsServerEntities ctx = new CcsServerEntities();
            
            //Grab docs from context
            var docs = ctx.documents
                .Where(d => d.snapshot_id == snapshotId);
            
            //Pipeline
            Debug.WriteLine("Beginning Pipeline for snapshot id " + snapshotId);
            Console.WriteLine("Beginning Pipeline for snapshot id " + snapshotId);
            
            var count = 1;
            foreach (var doc in docs)
            {
                //Initialize byte array list
                List < FileBytes > fbList = new List<FileBytes>();

                if(doc.exception_code == null && doc.exclusion_code == null)
                {
                    Operations.updateName(doc);
                }
                if (doc.exception_code == null && doc.exclusion_code == null)
                {
                    //Operations.updateFilePath(doc);
                }
                if (doc.exception_code == null && doc.exclusion_code == null)
                {
                    Operations.getSbFileType(doc,fbList);
                }

                count++;
                if(count % 1000 == 0)
                {
                    Console.WriteLine(count + " Docs processed for snapshot id " + snapshotId);
                    Debug.WriteLine(count + " Docs processed for snapshot id " + snapshotId);
                }

            }
            
            Console.WriteLine("Pipleine has completed for snapshot id " + snapshotId);
            Debug.WriteLine("Pipleine has completed for snapshot id " + snapshotId);

            Debug.WriteLine("Creating DIP for for snapshot id: " + snapshotId);

            var mappingIndex = getMappingIndex();
            CcsFunctions.createDipFile(ctx, mappingIndex, snapshotId);

            Debug.WriteLine("Done creating DIP file");

            ctx.SaveChanges();

        }
    }
}
