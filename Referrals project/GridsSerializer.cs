﻿using ProtoBuf;
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
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Screens.DebugScreens.Game;
using Sandbox.Game.World;
using Torch.Commands;
using VRage.Game;
using VRage.ObjectBuilders;

namespace Referrals_project
{
    public class GridMethods
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string FolderPath;
        private ulong SteamID;
        private CommandContext Context;

        public GridMethods(string FolderDirectory, string GridName) 
        {
            FolderPath = Path.Combine(FolderDirectory, GridName.ToString());
            Directory.CreateDirectory(FolderPath);
        }

        private bool SaveGridToFile(string path, string filename, List<MyObjectBuilder_CubeGrid> objectBuilders)
        {

            MyObjectBuilder_ShipBlueprintDefinition definition =
                MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_ShipBlueprintDefinition>();

            definition.Id = new MyDefinitionId(new MyObjectBuilderType(typeof(MyObjectBuilder_ShipBlueprintDefinition)),
                filename);
            definition.CubeGrids = objectBuilders.Select(x => (MyObjectBuilder_CubeGrid) x.Clone()).ToArray();

            /* Reset ownership as it will be different on the new server anyway */
            foreach (MyObjectBuilder_CubeGrid cubeGrid in definition.CubeGrids)
            {
                foreach (MyObjectBuilder_CubeBlock cubeBlock in cubeGrid.CubeBlocks)
                {
                    cubeBlock.Owner = 0L;
                    cubeBlock.BuiltBy = 0L;
                }
            }

            MyObjectBuilder_Definitions builderDefinition =
                MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Definitions>();
            builderDefinition.ShipBlueprints = new MyObjectBuilder_ShipBlueprintDefinition[] {definition};


            return MyObjectBuilderSerializer.SerializeXML(path, false, builderDefinition);
        }


        public async Task<bool>  LoadGrid(string GridName, MyCharacter Player, long TargetPlayerID)
        {
            Log.Warn("LoadGrid ran");

            string path = Path.Combine(FolderPath, GridName + ".sbc");

            if (!File.Exists(path))
            {
                Log.Fatal("Grid doesnt exsist @" + path);
                return false;
            }

            try
            {
                Log.Warn("83");
                if (MyObjectBuilderSerializer.DeserializeXML(path,
                    out MyObjectBuilder_Definitions myObjectBuilder_Definitions))
                {
                    Log.Warn("87");
                    var shipBlueprints = myObjectBuilder_Definitions.ShipBlueprints;
                    Log.Warn("89");

                    if (shipBlueprints == null)
                    {
                        Log.Error("not grid found for blueprint");
                        return false;
                    }
                    
                    Log.Warn("97");

                    //If the configs have keep originial position on, we dont want to align this to gravity.
                    foreach (MyObjectBuilder_ShipBlueprintDefinition definition in shipBlueprints)
                    {
                        Log.Warn("102");
                        foreach (MyObjectBuilder_CubeGrid CubeGridDef in definition.CubeGrids)
                        {
                            Log.Warn("105");
                            foreach (MyObjectBuilder_CubeBlock block in CubeGridDef.CubeBlocks)
                            {
                                Log.Warn("108");
                                block.Owner = TargetPlayerID;
                                block.BuiltBy = TargetPlayerID;
                            }
                        }
                    }
                    Log.Warn("114");
                    foreach (var shipBlueprint in shipBlueprints)
                    {
                        Log.Warn("117");
                        Vector3D TargetSpawnPos = Vector3D.Zero;
                        if (Player != null)
                        {
                            Log.Warn("121");
                            TargetSpawnPos = Player.PositionComp.GetPosition();
                            Log.Warn(TargetSpawnPos.ToString);
                        }
                        
                        Log.Warn("126");
                        Log.Warn(shipBlueprint.ToString);
                        Log.Warn(TargetSpawnPos.ToString);

                        if (!LoadShipBlueprint(shipBlueprint, TargetSpawnPos))
                        {
                            Log.Debug("Error Loading ShipBlueprints from File '" + path + "'");
                            return false;
                        }
                    }
                    //File.Delete(path);
                    return true;
                    
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to deserialize grid: " + path + " from file! Is this a shipblueprint?");
            }

            return false;
        }

        private bool LoadShipBlueprint(MyObjectBuilder_ShipBlueprintDefinition shipBlueprint,
            Vector3D PlayerLocation)
        {
            var grids = shipBlueprint.CubeGrids;

            if (grids == null || grids.Length == 0)
            { 
                Log.Info("No grids in blueprint!");
                return false;
            }

            Vector3D TargetLocation = PlayerLocation;
            bool AlignToGravity = true;
            

            ParallelSpawner Spawner = new ParallelSpawner(grids, AlignToGravity);
            Log.Info("Attempting Grid Spawning @" + TargetLocation.ToString());
            return Spawner.Start(TargetLocation);
        }
        
        public bool SaveGrids(MyCubeGrid cubeGrid, string GridName)
        {
            List<MyObjectBuilder_CubeGrid> objectBuilders = new List<MyObjectBuilder_CubeGrid>();

            if (!(cubeGrid.GetObjectBuilder() is MyObjectBuilder_CubeGrid objectBuilder))
                throw new ArgumentException(cubeGrid + " has a ObjectBuilder thats not for a CubeGrid");

            objectBuilders.Add(objectBuilder);

            try
            {
                //Need To check grid name

                string GridSavePath = Path.Combine(FolderPath, GridName + ".sbc");

                //Log.Info("SavedDir: " + pathForPlayer);
                bool saved = SaveGridToFile(GridSavePath, GridName, objectBuilders);

                try
                {
                    
                    MyIdentity IDentity = MySession.Static.Players.TryGetPlayerIdentity(new MyPlayer.PlayerId(SteamID));

                }
                catch (Exception e)
                {
                    Log.Fatal(e);
                }

                if (saved)
                {
                    Log.Error("grid saved");
                }


                return saved;
            }
            catch (Exception e)
            {
                Log.Error("Saving Grid Failed!", e);
                return false;
            }

        }
    }
}