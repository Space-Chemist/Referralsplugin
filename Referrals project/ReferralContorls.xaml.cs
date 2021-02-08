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
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Caliburn.Micro;


namespace Referrals_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ReferralControls : UserControl
    {
        public BindableCollection<User> Data { get; set; }
        //public ObservableCollection<User> UserData { get; } = new ObservableCollection<User>();
        //public ObservableCollection<User> UserData { get; set; }
        public ReferralControls()
        {
            InitializeComponent();
            //UserData = new ObservableCollection<User>();
        }


        public ReferralControls(ReferralCore plugin) : this() {
            Plugin = plugin;
            DataContext = plugin.Config;
            //var userData = ReferralCore.UserDataFromStorage().Users;
            //ObservableCollection<User> userInfo = new ObservableCollection<User>(userData);
           //DataGrid.DataContext = userInfo;
           //DataSet dataSet = new DataSet();
           //string sampleXmlFile = ReferralCore.UserDataPath;
           //dataSet.ReadXml(sampleXmlFile);
           //DataView dataView = new DataView(dataSet.Tables[0]);
           //DataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
           //string sampleXmlFile = ReferralCore.UserDataPath;
           //(this.Resources["XmlData"] as XmlDataProvider).Source = new Uri(sampleXmlFile);
           //Info.DataContext = Data;
           
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
