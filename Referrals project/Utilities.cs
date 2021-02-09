using Newtonsoft.Json;
using NLog;
using Sandbox;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.GameSystems;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Screens.Helpers;
using SteamKit2.GC.Dota.Internal;
using Torch.Commands;
using Torch.Mod;
using Torch.Mod.Messages;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Groups;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace Referrals_project
{
    /*public class Utilities
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public CommandContext Context;
        public ReferralCore Plugin;
        private Chat chat;
        public bool _Admin;


        private MyCharacter myCharacter;
        private MyIdentity myIdentity;
        private long TargetIdentity { get; set; }
        private ulong PlayerSteamID { get; set; }
        private int MaxHangarSlots;
        public string PlayerHangarPath { get; set; }
        private GridMethods Methods;

        private double LoadRadius;
        
        public void SaveGrid()
        {


            Result result = GetGrids(myCharacter);

            if (!result.GetGrids)
            {
                return;
            }


            if (!BeginSave(result, Data))
            {
                return;
            }
        }

        public void LoadGrid(string GridNameOrNumber, bool ForceLoadAtSavePosition = false)
        {

            if (Data.Grids.Count == 0)
            {
                chat.Respond("You have no grids in your hangar!");
                return;
            }

            int result = 0;
            try
            {
                result = Int32.Parse(GridNameOrNumber);
                //Got result. Check to see if its not an absured number
                if (result < 0)
                {
                    chat.Respond("Jeez! Why so negative! Maybe you should try positive numbers for a change!");
                    return;
                }

                if (result == 0)
                {
                    chat.Respond("OHH COME ON! There is no ZEROTH hangar slot! Start with 1!!");
                    return;
                }

                if (result > Data.Grids.Count)
                {
                    chat.Respond("This hangar slot is empty! Select a grid that is in your hangar!");
                    return;
                }
            }
            catch
            {
                //If failed cont to normal string name
            }



            if (result != 0)
            {

                GridStamp Grid = Data.Grids[result - 1];

                if (CheckEnemyDistance(ForceLoadAtSavePosition, Grid.GridSavePosition))
                    return;


                //Check PCU!
                if (!CheckGridLimits(myIdentity, Grid))
                    return;

                if (!LoadFromOriginalPositionCheck(Grid))
                    return;

                //Check to see if the grid is on the market!
                if (!CheckIfOnMarket(Grid, myIdentity))
                    return;

                if (!RequireLoadCurrency(Grid))
                    return;


                //string path = Path.Combine(PlayerHangarPath, Grid.GridName + ".sbc");

                if (!LoadGridFile(Grid.GridName, Data, Grid))
                {
                    return;
                }
            }


            if (GridNameOrNumber != "")
            {
                //Scan containg Grids if the user typed one
                foreach (var grid in Data.Grids)
                {

                    if (grid.GridName == GridNameOrNumber)
                    {
                        //Check BlockLimits
                        if (!CheckGridLimits(myIdentity, grid))
                        {
                            return;
                        }

                        //Check to see if the grid is on the market!
                        if (!CheckIfOnMarket(grid, myIdentity))
                        {
                            return;
                        }

                        string path = Path.Combine(PlayerHangarPath, grid.GridName);
                        if (!LoadGridFile(path, Data, grid))
                        {
                            return;
                        }

                    }
                }
            }
        }
        
        private Result GetGrids(MyCharacter character, string GridName = null)
        {
            List<MyCubeGrid> grids = GridMethods.FindGridList(GridName, character, Plugin.Config);
            Chat chat = new Chat(Context);


            Result Return = new Result();
            Return.grids = grids;
            MyCubeGrid biggestGrid = new MyCubeGrid();

            if (grids == null)
            {
                chat.Respond("Multiple grids found. Try to rename them first or try a different subgrid for identification!");
                Return.GetGrids = false;
                return Return;
            }

            if (grids.Count == 0)
            {
                chat.Respond("No grids found. Check your viewing angle or try the correct name!");
                Return.GetGrids = false;
                return Return;
            }


            foreach (var grid in grids)
            {
                if (biggestGrid.BlocksCount < grid.BlocksCount)
                {
                    biggestGrid = grid;
                }
            }


            if (biggestGrid == null)
            {
                chat.Respond("Grid incompatible!");
                Return.GetGrids = false;
                return Return;
            }

            Return.biggestGrid = biggestGrid;

            long playerId;

            if (biggestGrid.BigOwners.Count == 0)
                playerId = 0;
            else
                playerId = biggestGrid.BigOwners[0];


            if (playerId != Context.Player.IdentityId)
            {
                chat.Respond("You are not the owner of this grid!");
                Return.GetGrids = false;
                return Return;
            }

            Return.GetGrids = true;
            return Return;
        }

        private bool LoadGridFile(string GridName, CMsgGCPlayerInfo.PlayerInfo Data, GridStamp Grid, bool admin = false)
        {
            if (Methods.LoadGrid(GridName, myCharacter, TargetIdentity, LoadFromSavePosition, chat, Plugin, Grid.GridSavePosition, true, admin))
            {
                chat.Respond("Load Complete!");
                Data.Grids.Remove(Grid);
                if (!admin)
                {
                    TimeStamp stamp = new TimeStamp();
                    stamp.OldTime = DateTime.Now;
                    stamp.PlayerID = myIdentity.IdentityId;
                    Data.Timer = stamp;
                }


                FileSaver.Save(Path.Combine(PlayerHangarPath, "PlayerInfo.json"), Data);
                //File.WriteAllText(Path.Combine(IDPath, "PlayerInfo.json"), JsonConvert.SerializeObject(Data));
                return true;
            }
            else
            {
                //chat.Respond("Load Failed!");
                return false;
            }
        }
        
        private bool BeginSave(Result result, CMsgGCPlayerInfo.PlayerInfo Data)
        {

            if (Methods.SaveGrids(result.grids, result.GridName, Plugin))
            {

                TimeStamp stamp = new TimeStamp();
                stamp.OldTime = DateTime.Now;
                stamp.PlayerID = myIdentity.IdentityId;


                //Load player file and update!





                //Fill out grid info and store in file
                //GridStamp Grid = new GridStamp();

                GetBPDetails(result, Plugin.Config, out GridStamp Grid);




                Grid.ServerPort = MySandboxGame.ConfigDedicated.ServerPort;
                Data.Grids.Add(Grid);
                Data.Timer = stamp;

                //Overwrite file
                FileSaver.Save(Path.Combine(PlayerHangarPath, "PlayerInfo.json"), Data);
                chat.Respond("Save Complete!");
                return true;
            }
            else
            {
                Chat.Respond("Export Failed!", Context);
                return false;
            }

        }
        
        public bool InitilizeCharacter()
        {

            if (!Plugin.Config.PluginEnabled)
                return false;

            if (Context.Player == null)
            {
                chat.Respond("You cant do this via console stupid!");
                return false;
            }
            myIdentity = ((MyPlayer)Context.Player).Identity;
            TargetIdentity = myIdentity.IdentityId;

            if (myIdentity.Character == null)
            {
                chat.Respond("Player has no Character");
                return false;
            }

            myCharacter = myIdentity.Character;

            Methods = new GridMethods(PlayerSteamID, Plugin.Config.FolderDirectory, this);
            PlayerHangarPath = Methods.FolderPath;


            MaxHangarSlots = Plugin.Config.NormalHangarAmount;


            if (Context.Player.PromoteLevel == MyPromoteLevel.Scripter)
            {
                MaxHangarSlots = Plugin.Config.ScripterHangarAmount;
            }
            else if (Context.Player.PromoteLevel == MyPromoteLevel.Moderator)
            {
                MaxHangarSlots = Plugin.Config.ScripterHangarAmount * 2;
            }
            else if (Context.Player.PromoteLevel >= MyPromoteLevel.Admin)
            {
                MaxHangarSlots = Plugin.Config.ScripterHangarAmount * 10;
            }



            return true;

        }
        
        public class GridMethods
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        private static bool KeepProjectionsOnSave = false;
        private static bool KeepOriginalOwner = true;

        public string FolderPath;
        private ulong SteamID;
        private HangarChecks HangarChecker;
        private CommandContext Context;
        private Settings Config;
        private Chat chat;


        public GridMethods(ulong UserSteamID, string FolderDirectory, HangarChecks checks = null)
        {
            FolderPath = Path.Combine(FolderDirectory, UserSteamID.ToString());
            Directory.CreateDirectory(FolderPath);
            SteamID = UserSteamID;


            if (checks != null)
            {
                HangarChecker = checks;
                Context = checks.Context;
                Config = checks.Plugin.Config;
                chat = new Chat(Context, checks._Admin);
            }

        }

        private bool SaveGridToFile(string path, string filename, List<MyObjectBuilder_CubeGrid> objectBuilders)
        {

            MyObjectBuilder_ShipBlueprintDefinition definition = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_ShipBlueprintDefinition>();

            definition.Id = new MyDefinitionId(new MyObjectBuilderType(typeof(MyObjectBuilder_ShipBlueprintDefinition)), filename);
            definition.CubeGrids = objectBuilders.Select(x => (MyObjectBuilder_CubeGrid)x.Clone()).ToArray();

            /* Reset ownership as it will be different on the new server anyway #1#
            foreach (MyObjectBuilder_CubeGrid cubeGrid in definition.CubeGrids)
            {
                foreach (MyObjectBuilder_CubeBlock cubeBlock in cubeGrid.CubeBlocks)
                {
                    if (!KeepOriginalOwner)
                    {
                        cubeBlock.Owner = 0L;
                        cubeBlock.BuiltBy = 0L;
                    }

                    /* Remove Projections if not needed #1#
                    if (!KeepProjectionsOnSave)
                        if (cubeBlock is MyObjectBuilder_ProjectorBase projector)
                            projector.ProjectedGrids = null;

                    /* Remove Pilot and Components (like Characters) from cockpits #1#
                    if (cubeBlock is MyObjectBuilder_Cockpit cockpit)
                    {
                        cockpit.Pilot = null;

                        if (cockpit.ComponentContainer != null)
                        {
                            var components = cockpit.ComponentContainer.Components;

                            if (components != null)
                            {

                                for (int i = components.Count - 1; i >= 0; i--)
                                {

                                    var component = components[i];

                                    if (component.TypeId == "MyHierarchyComponentBase")
                                    {
                                        components.RemoveAt(i);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MyObjectBuilder_Definitions builderDefinition = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Definitions>();
            builderDefinition.ShipBlueprints = new MyObjectBuilder_ShipBlueprintDefinition[] { definition };


            return MyObjectBuilderSerializer.SerializeXML(path, false, builderDefinition);
        }


        public bool LoadGrid(string GridName, MyCharacter Player, long TargetPlayerID, bool keepOriginalLocation, Chat chat, Hangar Plugin, Vector3D GridSaveLocation, bool force = false, bool Admin = false)
        {
            string path = Path.Combine(FolderPath, GridName + ".sbc");

            if (!File.Exists(path))
            {
                chat.Respond("Grid doesnt exist! Admin should check logs for more information.");
                Log.Fatal("Grid doesnt exsist @" + path);
                return false;
            }

            try
            {
                if (MyObjectBuilderSerializer.DeserializeXML(path, out MyObjectBuilder_Definitions myObjectBuilder_Definitions))
                {
                    var shipBlueprints = myObjectBuilder_Definitions.ShipBlueprints;


                    if (shipBlueprints == null)
                    {

                        Hangar.Debug("No ShipBlueprints in File '" + path + "'");
                        chat.Respond("There arent any Grids in your file to import!");
                        return false;
                    }

                    if (!HangarChecker.BlockLimitChecker(shipBlueprints))
                    {
                        Hangar.Debug("Block Limiter Checker Failed");
                        return false;
                    }

                    if (Config.OnLoadTransfer)
                    {

                        Log.Warn("Target player: " + TargetPlayerID);

                        //Will transfer pcu to new player
                        foreach (MyObjectBuilder_ShipBlueprintDefinition definition in shipBlueprints)
                        {

                            foreach (MyObjectBuilder_CubeGrid CubeGridDef in definition.CubeGrids)
                            {
                                foreach (MyObjectBuilder_CubeBlock block in CubeGridDef.CubeBlocks)
                                {

                                    block.Owner = TargetPlayerID;
                                    block.BuiltBy = TargetPlayerID;

                                }
                            }
                        }
                    }

                    //If the configs have keep originial position on, we dont want to align this to gravity.

                    foreach (var shipBlueprint in shipBlueprints)
                    {
                        Vector3D TargetSpawnPos = Vector3D.Zero;
                        if (Player != null)
                        {
                            TargetSpawnPos = Player.PositionComp.GetPosition();
                        }


                        if (!LoadShipBlueprint(shipBlueprint, GridSaveLocation, TargetSpawnPos, keepOriginalLocation, chat, Plugin))
                        {
                            Hangar.Debug("Error Loading ShipBlueprints from File '" + path + "'");
                            return false;
                        }
                    }

                    File.Delete(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                chat.Respond("This ship failed to load. Contact staff & Check logs!");
                Log.Error(ex, "Failed to deserialize grid: " + path + " from file! Is this a shipblueprint?");
            }

            return false;
        }

        private bool LoadShipBlueprint(MyObjectBuilder_ShipBlueprintDefinition shipBlueprint, Vector3D GridSaveLocation, Vector3D PlayerLocation, bool keepOriginalLocation, Chat chat, Hangar Plugin, bool force = false)
        {
            var grids = shipBlueprint.CubeGrids;

            if (grids == null || grids.Length == 0)
            {
                chat.Respond("No grids in blueprint!");
                return false;
            }

            try
            {
                MyIdentity IDentity = MySession.Static.Players.TryGetPlayerIdentity(new MyPlayer.PlayerId(SteamID));

                if (Plugin.GridBackup != null)
                {
                    Plugin.GridBackup.GetType().GetMethod("BackupGridsManuallyWithBuilders", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new Type[2] { typeof(List<MyObjectBuilder_CubeGrid>), typeof(long) }, null).Invoke(Plugin.GridBackup, new object[] { grids.ToList(), IDentity.IdentityId });
                    Log.Warn("Successfully BackedUp grid!");
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }

            Vector3D TargetLocation;
            bool AlignToGravity = false;
            if (keepOriginalLocation)
            {
                TargetLocation = GridSaveLocation;
            }
            else
            {
                AlignToGravity = true;
                TargetLocation = PlayerLocation;
            }

            ParallelSpawner Spawner = new ParallelSpawner(grids, chat, AlignToGravity);
            Log.Info("Attempting Grid Spawning @" + TargetLocation.ToString());
            return Spawner.Start(keepOriginalLocation, TargetLocation);
        }




        public static List<MyCubeGrid> FindGridList(string gridNameOrEntityId, MyCharacter character, Settings Config)
        {

            List<MyCubeGrid> grids = new List<MyCubeGrid>();

            if (gridNameOrEntityId == null && character == null)
                return new List<MyCubeGrid>();



            if (Config.EnableSubGrids)
            {
                //If we include subgrids in the grid grab

                long EntitiyID = character.Entity.EntityId;


                ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> groups;

                if (gridNameOrEntityId == null)
                    groups = GridFinder.FindLookAtGridGroup(character);
                else
                    groups = GridFinder.FindGridGroup(gridNameOrEntityId);

                if (groups.Count() > 1)
                    return null;


                foreach (var group in groups)
                {
                    foreach (var node in group.Nodes)
                    {
                        MyCubeGrid Grid = node.NodeData;

                        if (Grid.Physics == null)
                            continue;

                        grids.Add(Grid);
                    }
                }



                for (int i = 0; i < grids.Count(); i++)
                {
                    MyCubeGrid grid = grids[i];

                    if (Config.AutoDisconnectGearConnectors && !grid.BigOwners.Contains(character.GetPlayerIdentityId()))
                    {
                        //This disabels enemy grids that have clamped on
                        Action ResetBlocks = new Action(delegate
                        {
                            foreach (MyLandingGear gear in grid.GetFatBlocks<MyLandingGear>())
                            {
                                gear.AutoLock = false;
                                gear.RequestLock(false);
                            }
                        });

                        Task Bool = Events.GameEvents.InvokeActionAsync(ResetBlocks);
                        if (!Bool.Wait(1000))
                            return null;

                    }
                    else if (Config.AutoDisconnectGearConnectors && grid.BigOwners.Contains(character.GetPlayerIdentityId()))
                    {
                        //This will check to see 
                        Action ResetBlocks = new Action(delegate
                        {
                            foreach (MyLandingGear gear in grid.GetFatBlocks<MyLandingGear>())
                            {
                                IMyEntity Entity = gear.GetAttachedEntity();
                                if (Entity == null || Entity.EntityId == 0)
                                {
                                    continue;
                                }


                                //Should prevent entity attachments with voxels
                                if (!(Entity is MyCubeGrid))
                                {
                                    //If grid is attacted to voxel or something
                                    gear.AutoLock = false;
                                    gear.RequestLock(false);

                                    continue;
                                }

                                MyCubeGrid attactedGrid = (MyCubeGrid)Entity;

                                //If the attaced grid is enemy
                                if (!attactedGrid.BigOwners.Contains(character.GetPlayerIdentityId()))
                                {
                                    gear.AutoLock = false;
                                    gear.RequestLock(false);

                                }
                            }
                        });

                        Task Bool = Events.GameEvents.InvokeActionAsync(ResetBlocks);
                        if (!Bool.Wait(1000))
                            return null;
                    }
                }


                for (int i = 0; i < grids.Count(); i++)
                {
                    if (!grids[i].BigOwners.Contains(character.GetPlayerIdentityId()))
                    {
                        grids.RemoveAt(i);
                    }
                }
            }
            else
            {

                ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group> groups;

                if (gridNameOrEntityId == null)
                    groups = GridFinder.FindLookAtGridGroupMechanical(character);
                else
                    groups = GridFinder.FindGridGroupMechanical(gridNameOrEntityId);


                if (groups.Count > 1)
                    return null;



                Action ResetBlocks = new Action(delegate
                {
                    foreach (var group in groups)
                    {
                        foreach (var node in group.Nodes)
                        {

                            MyCubeGrid grid = node.NodeData;


                            foreach (MyLandingGear gear in grid.GetFatBlocks<MyLandingGear>())
                            {
                                gear.AutoLock = false;
                                gear.RequestLock(false);
                            }


                            if (grid.Physics == null)
                                continue;

                            grids.Add(grid);
                        }
                    }
                });

                Task Bool = Events.GameEvents.InvokeActionAsync(ResetBlocks);
                if (!Bool.Wait(1000))
                    return null;


            }

            return grids;
        }

        public bool SaveGrids(List<MyCubeGrid> grids, string GridName, Hangar Plugin)
        {
            List<MyObjectBuilder_CubeGrid> objectBuilders = new List<MyObjectBuilder_CubeGrid>();

            foreach (MyCubeGrid grid in grids)
            {
                /* Remove characters from cockpits #1#

                Action P = delegate
                {

                    foreach (var blck in grid.GetFatBlocks().OfType<MyCockpit>())
                    {
                        if (blck.Pilot != null)
                        {
                            blck.RemovePilot();
                        }
                    }

                };

                Task KickPlayers = Events.GameEvents.InvokeActionAsync(P);
                KickPlayers.Wait(5000);



                /* What else should it be? LOL? #1#
                if (!(grid.GetObjectBuilder() is MyObjectBuilder_CubeGrid objectBuilder))
                    throw new ArgumentException(grid + " has a ObjectBuilder thats not for a CubeGrid");

                objectBuilders.Add(objectBuilder);
            }





            try
            {
                //Need To check grid name

                string GridSavePath = Path.Combine(FolderPath, GridName + ".sbc");

                //Log.Info("SavedDir: " + pathForPlayer);
                bool saved = SaveGridToFile(GridSavePath, GridName, objectBuilders);

                try
                {
                    
                    MyIdentity IDentity = MySession.Static.Players.TryGetPlayerIdentity(new MyPlayer.PlayerId(SteamID));

                    if (Plugin.GridBackup != null)
                    {
                        Plugin.GridBackup.GetType().GetMethod("BackupGridsManuallyWithBuilders", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new Type[2] { typeof(List<MyObjectBuilder_CubeGrid>), typeof(long) }, null).Invoke(Plugin.GridBackup, new object[] { objectBuilders, IDentity.IdentityId });
                        Log.Warn("Successfully BackedUp grid!");
                    }
                }
                catch (Exception e)
                {
                    Log.Fatal(e);
                }

                if (saved)
                {
                    DisposeGrids(grids);
                }


                return saved;
            }
            catch (Exception e)
            {
                Hangar.Debug("Saving Grid Failed!", e, Hangar.ErrorType.Fatal);
                return false;
            }
        }

        public void DisposeGrids(List<MyCubeGrid> Grids)
        {

            foreach (MyCubeGrid Grid in Grids)
            {
                foreach (MyCockpit Block in Grid.GetBlocks().OfType<MyCockpit>())
                {
                    if (Block.Pilot != null)
                    {
                        Block.RemovePilot();
                    }
                }

                Grid.Close();
            }
        }
        public void SaveInfoFile(CMsgGCPlayerInfo.PlayerInfo Data)
        {
            FileSaver.Save(Path.Combine(FolderPath, "PlayerInfo.json"), Data);
        }

        public bool LoadInfoFile(out CMsgGCPlayerInfo.PlayerInfo Data)
        {
            CMsgGCPlayerInfo.PlayerInfo Info = new CMsgGCPlayerInfo.PlayerInfo();

            string FilePath = Path.Combine(FolderPath, "PlayerInfo.json");


            if (!File.Exists(FilePath))
            {
                Data = Info;
                return true;
            }


            try
            {
                Info = JsonConvert.DeserializeObject<CMsgGCPlayerInfo.PlayerInfo>(File.ReadAllText(FilePath));
            }
            catch (Exception e)
            {
                Log.Warn(e, "For some reason the file is broken");
                Data = Info;
                return false;
            }


            Data = Info;
            return true;
        }

        public class GridFinder
        {
            //Thanks LordTylus, I was too lazy to create my own little utils

            public static ConcurrentBag<List<MyCubeGrid>> FindGridList(long playerId, bool includeConnectedGrids)
            {

                ConcurrentBag<List<MyCubeGrid>> grids = new ConcurrentBag<List<MyCubeGrid>>();

                if (includeConnectedGrids)
                {

                    Parallel.ForEach(MyCubeGridGroups.Static.Physical.Groups, group =>
                    {

                        List<MyCubeGrid> gridList = new List<MyCubeGrid>();

                        foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node groupNodes in group.Nodes)
                        {

                            MyCubeGrid grid = groupNodes.NodeData;

                            if (grid.Physics == null)
                                continue;

                            gridList.Add(grid);
                        }

                        if (IsPlayerIdCorrect(playerId, gridList))
                            grids.Add(gridList);
                    });

                }
                else
                {

                    Parallel.ForEach(MyCubeGridGroups.Static.Mechanical.Groups, group =>
                    {

                        List<MyCubeGrid> gridList = new List<MyCubeGrid>();

                        foreach (MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Node groupNodes in group.Nodes)
                        {

                            MyCubeGrid grid = groupNodes.NodeData;

                            if (grid.Physics == null)
                                continue;

                            gridList.Add(grid);
                        }

                        if (IsPlayerIdCorrect(playerId, gridList))
                            grids.Add(gridList);
                    });
                }

                return grids;
            }
        }
        
        public static ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> FindGridGroup(string gridName)
        {

            ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> groups = new ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group>();
            Parallel.ForEach(MyCubeGridGroups.Static.Physical.Groups, group =>
            {

                foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node groupNodes in group.Nodes)
                {

                    MyCubeGrid grid = groupNodes.NodeData;

                    if (grid.Physics == null)
                        continue;

                    /* Gridname is wrong ignore #1#
                    if (!grid.DisplayName.Equals(gridName) && grid.EntityId + "" != gridName)
                        continue;

                    groups.Add(group);
                }
            });

            return groups;
        }

        public static ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> FindLookAtGridGroup(IMyCharacter controlledEntity)
        {

            const float range = 5000;
            Matrix worldMatrix;
            Vector3D startPosition;
            Vector3D endPosition;

            worldMatrix = controlledEntity.GetHeadMatrix(true, true, false); // dead center of player cross hairs, or the direction the player is looking with ALT.
            startPosition = worldMatrix.Translation + worldMatrix.Forward * 0.5f;
            endPosition = worldMatrix.Translation + worldMatrix.Forward * (range + 0.5f);

            var list = new Dictionary<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group, double>();
            var ray = new RayD(startPosition, worldMatrix.Forward);

            foreach (var group in MyCubeGridGroups.Static.Physical.Groups)
            {

                foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node groupNodes in group.Nodes)
                {

                    IMyCubeGrid cubeGrid = groupNodes.NodeData;

                    if (cubeGrid != null)
                    {

                        if (cubeGrid.Physics == null)
                            continue;

                        // check if the ray comes anywhere near the Grid before continuing.    
                        if (ray.Intersects(cubeGrid.WorldAABB).HasValue)
                        {

                            Vector3I? hit = cubeGrid.RayCastBlocks(startPosition, endPosition);

                            if (hit.HasValue)
                            {

                                double distance = (startPosition - cubeGrid.GridIntegerToWorld(hit.Value)).Length();


                                if (list.TryGetValue(group, out double oldDistance))
                                {

                                    if (distance < oldDistance)
                                    {
                                        list.Remove(group);
                                        list.Add(group, distance);
                                    }

                                }
                                else
                                {

                                    list.Add(group, distance);
                                }
                            }
                        }
                    }
                }
            }

            ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> bag = new ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group>();

            if (list.Count == 0)
                return bag;

            // find the closest Entity.
            var item = list.OrderBy(f => f.Value).First();
            bag.Add(item.Key);

            return bag;
        }

        public static ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group> FindGridGroupMechanical(string gridName)
        {

            ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group> groups = new ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group>();
            Parallel.ForEach(MyCubeGridGroups.Static.Mechanical.Groups, group =>
            {

                foreach (MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Node groupNodes in group.Nodes)
                {

                    MyCubeGrid grid = groupNodes.NodeData;

                    if (grid.Physics == null)
                        continue;

                    /* Gridname is wrong ignore #1#
                    if (!grid.DisplayName.Equals(gridName) && grid.EntityId + "" != gridName)
                        continue;

                    groups.Add(group);
                }
            });

            return groups;
        }

        public static ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group> FindLookAtGridGroupMechanical(IMyCharacter controlledEntity)
        {
            try
            {
                const float range = 5000;
                Matrix worldMatrix;
                Vector3D startPosition;
                Vector3D endPosition;

                worldMatrix = controlledEntity.GetHeadMatrix(true, true, false); // dead center of player cross hairs, or the direction the player is looking with ALT.
                startPosition = worldMatrix.Translation + worldMatrix.Forward * 0.5f;
                endPosition = worldMatrix.Translation + worldMatrix.Forward * (range + 0.5f);

                var list = new Dictionary<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group, double>();
                var ray = new RayD(startPosition, worldMatrix.Forward);

                foreach (var group in MyCubeGridGroups.Static.Mechanical.Groups)
                {

                    foreach (MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Node groupNodes in group.Nodes)
                    {

                        IMyCubeGrid cubeGrid = groupNodes.NodeData;

                        if (cubeGrid != null)
                        {

                            if (cubeGrid.Physics == null)
                                continue;

                            // check if the ray comes anywhere near the Grid before continuing.    
                            if (ray.Intersects(cubeGrid.WorldAABB).HasValue)
                            {

                                Vector3I? hit = cubeGrid.RayCastBlocks(startPosition, endPosition);

                                if (hit.HasValue)
                                {

                                    double distance = (startPosition - cubeGrid.GridIntegerToWorld(hit.Value)).Length();


                                    if (list.TryGetValue(group, out double oldDistance))
                                    {

                                        if (distance < oldDistance)
                                        {
                                            list.Remove(group);
                                            list.Add(group, distance);
                                        }

                                    }
                                    else
                                    {

                                        list.Add(group, distance);
                                    }
                                }
                            }
                        }
                    }
                }

                ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group> bag = new ConcurrentBag<MyGroups<MyCubeGrid, MyGridMechanicalGroupData>.Group>();

                if (list.Count == 0)
                    return bag;

                // find the closest Entity.
                var item = list.OrderBy(f => f.Value).First();
                bag.Add(item.Key);

                return bag;
            }
            catch (Exception e)
            {
                //Hangar.Debug("Matrix Error!", e, Hangar.ErrorType.Trace);
                return null;
            }

        }

    }
        public static void SendGps(Vector3D Position, string name, long EntityID, double Miniutes = 5)
        {


            MyGps myGps = new MyGps();
            myGps.ShowOnHud = true;
            myGps.Coords = Position;
            myGps.Name = name;
            myGps.Description = "Hangar location for loading grid at or around this position";
            myGps.AlwaysVisible = true;

            MyGps gps = myGps;
            gps.DiscardAt = TimeSpan.FromMinutes(MySession.Static.ElapsedPlayTime.TotalMinutes + Miniutes);
            gps.GPSColor = Color.Yellow;
            MySession.Static.Gpss.SendAddGps(EntityID, ref gps, 0L, true);
        }

    }*/
}