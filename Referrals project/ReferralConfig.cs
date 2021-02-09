using System.Windows.Documents;
using Torch;

namespace Referrals_project
{
    public class ReferralConfig : ViewModel
    {
        private bool _referralrewards = true;
        private bool _promotionrewardenabled = true;
        private string _serverreferralcode = "testcode";
        private string _promotionrewardscode = "Promotiontestcode";
        private string _serverreferralgrid = "Grid Name";
        private string _promotionrewardsgrid = "Grid Name";
        private string _playerreferralgrid = "Grid Name";
        
        
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
    }
}