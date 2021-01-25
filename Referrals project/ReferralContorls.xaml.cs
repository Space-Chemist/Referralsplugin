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
            var userData = ReferralCore.UserDataFromStorage().Users;
            DataContext = userData;
        }
        
        private Referrals_project.ReferralCore Plugin { get; }
        
        private void ServerReferralRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerReferralRewardsConfig.IsOpen = true;
        }

        private void ServerReferralRewardConfigCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerReferralRewardsConfig.IsOpen = false;
            Plugin.Save();
        }
        
        private void UserReferralRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserReferralRewardsConfig.IsOpen = true;
        }
        
        private void UserReferralRewardConfigCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserReferralRewardsConfig.IsOpen = false;
            Plugin.Save();
        }
        
        private void PromotionCodeRewardConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            PromotionCodeRewardsConfig.IsOpen = true;
        }
        
        private void PromotionCodeRewardConfigCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            PromotionCodeRewardsConfig.IsOpen = false;
            Plugin.Save();
        }

    }
}
