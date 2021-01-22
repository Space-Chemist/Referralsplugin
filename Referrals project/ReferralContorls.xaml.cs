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

    }
}
