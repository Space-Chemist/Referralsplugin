using System.Collections.Generic;
using System.Linq;
using NLog;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.Screens.ViewModels;
using Sandbox.Game.World;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;
using System.Threading.Tasks;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using VRage.Game.Definitions.SessionComponents;
using VRage.Groups;
using VRageMath;

namespace Referrals_project
{
    [Category("r")]
    public class ReferralCommands : CommandModule
    {
        private static readonly Logger Log = LogManager.GetLogger("Referrals");

        public Referrals_project.ReferralCore Plugin
        {
            get { return (Referrals_project.ReferralCore) this.Context.Plugin; }
        }

        [Command("save", "Saves reward code")]
        [Permission(MyPromoteLevel.Admin)]
        public void Save()
        {
            var controlledEntity = Context.Player.Character;
            var result = Utilities.GetShip(controlledEntity);
        }

        [Command("testload", "Loads reward grid to ensure proper save")]
        [Permission(MyPromoteLevel.Admin)]
        public void Load(string GridName)
        {
            var FolderDirectory = ReferralCore.Instance.StoragePath;
            var myIdentity = ((MyPlayer) Context.Player).Identity;
            var TargetIdentity = myIdentity.IdentityId;
            var myCharacter = myIdentity.Character;

            GridMethods methods = new GridMethods(FolderDirectory, GridName);
            Task T = new Task(() => methods.LoadGrid(GridName, myCharacter, TargetIdentity));
            T.Start();
        }


        [Command("player", "get your referral bonus", "requires steamId/Name/")]
        [Permission(MyPromoteLevel.None)]
        public void Knew(string player)
        {
            if (Context.Player == null)
            {
                Log.Error("Why are you running this in the console?");
                return;
            }

            var identity = ReferralCore.GetIdentityByNameOrIds(player);
            if (identity == null)
            {
                Context.Respond("player not found, are you sure you have the right steam id or name?");
                return;
            }

            if (identity == Context.Player.Identity)
            {
                Context.Respond("Dont try to claim yourself");
                return;
            }    

            var user1 = ReferralCore.GetUser(Context.Player.SteamUserId);
            if (user1.ReferralByUser != null)
            {
                if ((bool) user1.ReferralByUser)
                {
                    Context.Respond("Referral already claimed");
                    return;
                }
            }

            if (user1.ReferralByCode != null)
            {
                if ((bool) user1.ReferralByCode)
                {
                    Context.Respond("Referral already claimed");
                    return;
                }
            }

            var user2 = ReferralCore.GetUser(MySession.Static.Players.TryGetSteamId(identity.IdentityId));
            var check = ReferralCore.Dostuff(user1, Context.Player, false);
            if (!check)
            {
                Log.Error("Failed to do stuff");
                Context.Respond("Failed to do stuff");
                Context.Respond($"This should never happen, see a admin and report code 42");
                return;
            }

            var referredDescription = new ReferredDescription
                {ReferredUserName = user1.Name, ReferredUserId = user1.SteamId, Claimed = false};
            user1.ReferralByUser = true;
            user1.ReferredBy = user2.SteamId;
            user2.ReferredDescriptions.Add(referredDescription);

            ReferralCore.SaveUser(user1);
            ReferralCore.SaveUser(user2);
            Context.Respond("Claimed");
        }


        [Command("code", "get your referral bonus", "requires code")]
        [Permission(MyPromoteLevel.None)]
        public void Knew2(string code)
        {
            if (Context.Player == null)
            {
                Log.Error("Why are you running this in the console?");
                return;
            }

            if (code != ReferralCore.Instance.Config.ServerReferralCode)
            {
                Context.Respond("code not found, are you sure you have the right one?");
                return;
            }

            var user = ReferralCore.GetUser(Context.Player.SteamUserId);

            if (user.ReferralByUser != null)
            {
                if ((bool) user.ReferralByUser)
                {
                    Context.Respond("Referral already claimed");
                    return;
                }
            }

            if (user.ReferralByCode != null)
            {
                if ((bool) user.ReferralByCode)
                {
                    Context.Respond("Referral already claimed");
                    return;
                }
            }

            user.ReferralByCode = true;

            var check = ReferralCore.Dostuff(user, Context.Player, false);
            if (!check)
            {
                Log.Error("Failed to do stuff");
                Context.Respond("Failed to do stuff");
                Context.Respond($"This should never happen, see a admin and report code 42");
                return;
            }


            ReferralCore.SaveUser(user);
            Context.Respond("Claimed");
        }


        [Command("claim", "Get your referral bonus", "Get your referral bonus")]
        [Permission(MyPromoteLevel.None)]
        public void Claim()
        {
            if (Context.Player == null)
            {
                Log.Error("Why are you running this in the console?");
                return;
            }

            var user = ReferralCore.GetUser(Context.Player.SteamUserId);

            user.ReferredDescriptions.ForEach(
                rd
                    =>
                {
                    if (rd.Claimed) return;
                    var c = ReferralCore.Dostuff(user, Context.Player, false);
                    if (c)
                    {
                        rd.Claimed = true;
                        ReferralCore.SaveUser(user);
                        Context.Respond($"Claimed for {rd.ReferredUserName}");
                    }
                    else
                    {
                        //lmao
                        Context.Respond(
                            $"Failed to Claim for {rd.ReferredUserName}, This should never happen, see a admin and report code 42");
                    }
                }
            );
            Context.Respond("done.");
        }


        [Command("promo", "Get your promo bonus", "Get your promo bonus")]
        [Permission(MyPromoteLevel.None)]
        public void promo(string code)
        {
            if (Context.Player == null)
            {
                Log.Error("Why are you running this in the console?");
                return;
            }

            if (code != ReferralCore.Instance.Config.PromotionRewardsCode)
            {
                Context.Respond("Incorrect Promotion Code.");
                return;
            }

            var user = ReferralCore.GetUser(Context.Player.SteamUserId);

            if (user.PromoCodes.Any(userPromoCode => code == userPromoCode))
            {
                Context.Respond("Promotion Code already used");
                return;
            }

            var check = ReferralCore.Dostuff(user, Context.Player, true);
            if (!check)
            {
                Log.Error("Failed to do stuff");
                Context.Respond("Failed to do stuff");
                Context.Respond($"This should never happen, see a admin and report code 42");
                return;
            }

            user.PromoCodes.Add(code);
            user.PromoCodes = user.PromoCodes;
            ReferralCore.SaveUser(user);
            Context.Respond("done.");
        }
    }
}