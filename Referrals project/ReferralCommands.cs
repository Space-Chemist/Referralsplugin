using KothPlugin;
using Nest;
using NLog;
using Sandbox.Game.Multiplayer;
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
            get
            {
                return (Referrals_project.ReferralCore)this.Context.Plugin;
            }
        }

        [Command("Examlpe command", "Example Description", "help text")]
        [Permission(MyPromoteLevel.Admin)]
        public void Example()
        {
            this.Context.Respond("response string");
                
        }


        [Command("Test", "Adds money to player. Needed SteamID.", null)]
        [Permission(MyPromoteLevel.None)]
        public void GiveCredits(ulong steamId, long amount)
        {
            long identityId = Sync.Players.TryGetIdentityId(steamId, 0);
            if (identityId == 0)
                this.Context.Respond("Fuck"+ Sync.Players.TryGetIdentityNameFromSteamId(steamId).ToString());
            else if (FinancialService.GivePlayerCredits(identityId, amount))
                this.Context.Respond("test worked money added fuck yeah" + Sync.Players.TryGetIdentityNameFromSteamId(steamId));
           
        }
        
        [Command("yes", "yes", "yes")]
        [Permission(MyPromoteLevel.None)]
        public void yes()
        {
            /*var u = ReferralCore.GetUser(76561198992724985L);
            u.ReferralCode = "fucking yes";
            u.ReferralByUser = true;
            ReferralCore.SaveUser(u);
            Context.Respond("Check your data bro");*/
        }
    }
}