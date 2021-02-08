using System.Linq;
using NLog;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.Screens.ViewModels;
using Sandbox.Game.World;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace Referrals_project
{
    [Category("referral")]
    public class ReferralCommands : CommandModule
    {
        private static readonly Logger Log = LogManager.GetLogger("koth");

        public Referrals_project.ReferralCore Plugin
        {
            get { return (Referrals_project.ReferralCore) this.Context.Plugin; }
        }

        [Command("test")]
        [Permission(MyPromoteLevel.None)]
        public void thng()
        {
           Context.Respond(ReferralCore.Instance.StoragePath); 
        }
            


        [Command("new", "get your referral bonus", "requries steamId/Name")]
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
                Context.Respond("X: player not found, are you sure you have the right steam id or name?");
                return;
            }

            var user1 = ReferralCore.GetUser(Context.Player.SteamUserId);

            if (user1.ReferralByUser != null)
            {
                if ((bool) user1.ReferralByUser)
                {
                    Context.Respond("X: You already did this?");
                    return;
                }
            }

            if (user1.ReferralByCode != null)
            {
                if ((bool) user1.ReferralByCode)
                {
                    Context.Respond("X: You already did this?");
                    return;
                }
            }

            var user2 = ReferralCore.GetUser(MySession.Static.Players.TryGetSteamId(identity.IdentityId));

            var check = ReferralCore.Dostuff(user1);
            if (!check)
            {
                Log.Error("Failed to do stuff");
                Context.Respond("X: Failed to do stuff");
                return;
            }

            var referredDescription = new ReferredDescription
                {ReferredUserName = user1.Name, ReferredUserId = user1.SteamId, Claimed = false};
            user1.ReferralByUser = true;
            user1.ReferredBy = user2.SteamId;
            user2.ReferredDescriptions.Add(referredDescription);

            ReferralCore.SaveUser(user1);
            ReferralCore.SaveUser(user1);
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
                    var c = ReferralCore.Dostuff(user);
                    if (c)
                    {
                        rd.Claimed = true;
                        ReferralCore.SaveUser(user);
                        Context.Respond($"Claimed for {rd.ReferredUserName}");
                    }
                    else
                    {
                        //lmao
                        Context.Respond($"Failed to Claim for {rd.ReferredUserName}, This should never happen, see a admin and report code 42");
                    }
                }
            );
            Context.Respond("done.");

        }
    }
}