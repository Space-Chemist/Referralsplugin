using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;


namespace Referrals_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ReferralControls : UserControl
    {
        public ReferralControls()
        {
            InitializeComponent();
        }
        
        public ReferralControls(ReferralCore plugin) : this() {
            Plugin = plugin;
            DataContext = plugin.Config;
        }
        
        private Referrals_project.ReferralCore Plugin { get; }
        
        private void UserReferralRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Log.Info("Test Webhook Sent");
            //DiscordService.SendDiscordWebHook("Successful WebHook Test");
            
        }
        
        private void ServerReferralRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Log.Info("Scores Cleared");
            //NetworkService.SendPacket("clear");
        }
        
        private void PromotionCodeRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Log.Info("Config updated ");
            Plugin.Save();
        }

    }
}
