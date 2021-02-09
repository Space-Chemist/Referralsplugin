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
using SpaceEngineers.Game.Entities.Blocks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Utilities
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

    }
}