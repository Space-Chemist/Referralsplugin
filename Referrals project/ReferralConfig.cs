using Torch;

namespace Referrals_project
{
    public class ReferralConfig : ViewModel
    {
        private bool _referralrewards = true;
        private bool _promotionrewardenabled = true;
        private string _serverreferralcode = "serverntest code";
        private string _promotionrewardscode = "Promotiontestcode";
        
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
    }
}