using NLog;
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
    }
}