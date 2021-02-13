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
    public class Utilities
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public CommandContext Context;
        public ReferralCore Plugin;
        public static ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> FindLookAtGridGroup(IMyCharacter controlledEntity) {

            const float range = 5000;
            Matrix worldMatrix;
            Vector3D startPosition;
            Vector3D endPosition;

            worldMatrix = controlledEntity.GetHeadMatrix(true, true, false); // dead center of player cross hairs, or the direction the player is looking with ALT.
            startPosition = worldMatrix.Translation + worldMatrix.Forward * 0.5f;
            endPosition = worldMatrix.Translation + worldMatrix.Forward * (range + 0.5f);

            var list = new Dictionary<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group, double>();
            var ray = new RayD(startPosition, worldMatrix.Forward);

            foreach(var group in MyCubeGridGroups.Static.Physical.Groups) {

                foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node groupNodes in group.Nodes) {

                    MyCubeGrid cubeGrid = groupNodes.NodeData;

                    if (cubeGrid != null) {

                        if (cubeGrid.Physics == null)
                            continue;

                        // check if the ray comes anywhere near the Grid before continuing.    
                        if (ray.Intersects(cubeGrid.PositionComp.WorldAABB).HasValue) {

                            Vector3I? hit = cubeGrid.RayCastBlocks(startPosition, endPosition);

                            if (hit.HasValue) {   
                                
                                

                                double distance = (startPosition - cubeGrid.GridIntegerToWorld(hit.Value)).Length();

                                if (list.TryGetValue(group, out double oldDistance)) {

                                    if (distance < oldDistance) {
                                        list.Remove(group);
                                        list.Add(group, distance);
                                    }

                                } else {

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
        
        public static CheckResult GetShip(IMyCharacter character) {

            ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> groups = FindLookAtGridGroup(character);

            return GetGroups(groups);
        }

        private static CheckResult GetGroups(ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> groups) {

            var result = CheckGroups(groups, out MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group group);

            if (result != CheckResult.OK)
                return result;

            return GetGroup(group);
        }

        private static CheckResult GetGroup(MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group group) 
        {
            List<MyObjectBuilder_EntityBase> objectBuilderList = new List<MyObjectBuilder_EntityBase>();
            List<MyCubeGrid> gridsList = new List<MyCubeGrid>();

            foreach (MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Node groupNodes in group.Nodes) {

                MyCubeGrid grid = groupNodes.NodeData;
                gridsList.Add(grid);
            }

            foreach (MyCubeGrid grid in gridsList) 
            {
                Log.Error(grid.DisplayName.ToString);
                var GridName = grid.DisplayName;
                var FolderDirectory = ReferralCore.Instance.StoragePath;
                GridMethods methods = new GridMethods(FolderDirectory, GridName);
                Task T = new Task(() => methods.SaveGrids(grid, GridName));
                T.Start();
            }

            return CheckResult.SHIP_FIXED;
        }
        
        public static CheckResult CheckGroups(ConcurrentBag<MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group> groups, 
            out MyGroups<MyCubeGrid, MyGridPhysicalGroupData>.Group group) {

            /* No group or too many groups found */
            if (groups.Count < 1) {
                group = null;
                return CheckResult.TOO_FEW_GRIDS;
            }

            /* too many groups found */
            if (groups.Count > 1) {
                group = null;
                return CheckResult.TOO_MANY_GRIDS;
            } 
            
            if (!groups.TryPeek(out group)) 
                return CheckResult.UNKNOWN_PROBLEM;

            return CheckResult.OK;
        }


    }
}