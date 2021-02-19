using System.Windows.Documents;
using System.Xml.Serialization;
using Torch;
using VRage.Collections;

namespace Referrals_project
{
    public class ReferralConfig : ViewModel
    {
        private bool _referralrewards = true;
        private bool _promotionrewardenabled = true;
        private bool _givemoney = true;
        private bool _givegrid = true;
        private bool _nexusenabled = false;
        private bool _databaseenabled = false;
        private int _creditamount = 1000;
        private string _serverreferralcode = "testcode";
        private string _promotionrewardscode = "Promotiontestcode";
        private string _serverreferralgrid = "Grid Name";
        private string _promotionrewardsgrid = "Grid Name";
        private string _playerreferralgrid = "Grid Name";
        private string _connectionurl = "url";
        private string _databasetype = "";
        
        public bool ReferralRewardsEnabled
        {
            get => _referralrewards;
            set => SetValue(ref _referralrewards, value);
        }
        public bool PromotionRewardEnabled
        {
            get => _promotionrewardenabled;
            set => SetValue(ref _promotionrewardenabled, value);
        }
        
        public bool GiveMoney
        {
            get => _givemoney;
            set => SetValue(ref _givemoney, value);
        }
        
        public bool NexusEnabled
        {
            get => _nexusenabled;
            set => SetValue(ref _nexusenabled, value);
        }
        
        public bool DataBaseEnabled
        {
            get => _databaseenabled;
            set => SetValue(ref _databaseenabled, value);
        }
        
        public bool GiveGrid
        {
            get => _givegrid;
            set => SetValue(ref _givegrid, value);
        }

        public int CreditAmount
        {
            get => _creditamount;
            set => SetValue(ref _creditamount, value);
        }

        public string ServerReferralCode
        {
            get => _serverreferralcode; 
            set => SetValue(ref _serverreferralcode, value);
        }
        
        public string PromotionRewardsCode
        {
            get => _promotionrewardscode;
            set => SetValue(ref _promotionrewardscode, value);
        }
        
        public string ServerReferralGrid
        {
            get => _serverreferralgrid; 
            set => SetValue(ref _serverreferralgrid, value);
        }
        
        public string PromotionRewardsGrid
        {
            get => _promotionrewardsgrid;
            set => SetValue(ref _promotionrewardsgrid, value);
        }
        
        public string PlayerReferralGrid
        {
            get => _playerreferralgrid;
            set => SetValue(ref _playerreferralgrid, value);
        }
        
        public string ConnectionUrl
        {
            get => _connectionurl;
            set => SetValue(ref _connectionurl, value);
        }
        
        public string DataBaseType
        {
            get => _databasetype;
            set => SetValue(ref _databasetype, value);
        }
    }

}