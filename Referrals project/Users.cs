using System.Collections.Generic;

namespace Referrals_project
{
    public class Users
    {
        public string Name { get; set; }
        public bool? ReferralByUser { get; set; }
        public bool? ReferralByCode { get; set; }
        public ulong SteamId { get; set; }
        
        public ulong ReferredBy{ get; set; }
        public string ReferralCode { get; set; }

    }
    
    public class ReferredDescriptions
    {
         public string ReferredUserName { get; set; }
         
         public ulong ReferredUserId { get; set; }
    }
}