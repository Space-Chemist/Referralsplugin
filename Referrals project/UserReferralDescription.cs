using System.Collections.Generic;

namespace Referrals_project
{
    public class UserReferralDescription
    {
        public bool ReferralByUser { get; set; }
        
        public bool ReferralByCode { get; set; }
        
        public long ReferredBy { get; set; }
        
        public string ReferralCode { get; set; }
        
        public List<ReferredListData> UsersReferred { get; set; } = new List<ReferredListData>();
        
    }
}