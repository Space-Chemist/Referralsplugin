using ProtoBuf;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.Entity;
using VRageMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using NLog;

namespace Referrals_project
{
    public class GridsSerializer
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
         
        public static void Save(string dir, object data)
         {
        
             var p = Task.Run(() => FileSaveTask(dir,data));
         }
        
         private static void FileSaveTask(string dir, object data)
         {
             try
             {
                 File.WriteAllText(dir, JsonConvert.SerializeObject(data));
        
        
             }catch(Exception e)
             {
                 Log.Info("Unable to save file @" + e);
             }
        
        
         }
        
         protected static bool IsFileLocked(FileInfo file)
         {
             try
             {
                 using (FileStream stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.None))
                 {
                     stream.Close();
                 }
             }
             catch (IOException)
             {
                 //the file is unavailable because it is:
                 //still being written to
                 //or being processed by another thread
                 //or does not exist (has already been processed)
                 return true;
             }
        
             //file is not locked
             return false;
         }

    }
}