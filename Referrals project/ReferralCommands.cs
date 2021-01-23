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
        
        
        
        [Command("yes", "yes", "yes")]
        [Permission(MyPromoteLevel.Admin)]
        public void yes()
        {
            Context.Respond("response string");
            Context.Respond("test worked money added fuck yeah " + FinancialService.GivePlayerCredits(Context.Player.IdentityId, 500000L));
        }

        
        
        [Command("Test", "Adds money to player. Needed SteamID.", null)]
        [Permission(MyPromoteLevel.None)]
        public void GiveCredits(ulong steamId, long amount)
        {
            long identityId = Sync.Players.TryGetIdentityId(steamId, 0);
            if (identityId == 0L)
                this.Context.Respond("Fuck", (string) null, (string) null);
            else if (FinancialService.GivePlayerCredits(identityId, amount))
                this.Context.Respond("test worked money added fuck yeah");
           
        }
    }
}