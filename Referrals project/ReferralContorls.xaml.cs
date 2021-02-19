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
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.GameSystems;


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
        public ReferralControls(ReferralCore plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
            var userData = ReferralCore.UserDataFromStorage().Users;
            //MyTreeView.DataContext = userInfo;
            Box.ItemsSource = userData;
        }
        private Referrals_project.ReferralCore Plugin { get; }

        private void Box_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            User SelectedUser = (User)Box.SelectedItem;
            
            if (SelectedUser == null)
                return;

            Info.DataContext = SelectedUser;




        }
    }
}
