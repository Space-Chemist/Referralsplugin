using System.Collections.Generic;
using SharpDX;
using Torch;

namespace Referrals_project
{
    public class UserData : ViewModel
    {
        public List<User> Users { get; set; }
    }


    public class User : ViewModel
    {
        public string Name { get; set; }
        public bool? ReferralByUser { get; set; }
        public bool? ReferralByCode { get; set; }
        public ulong SteamId { get; set; }
        public ulong? ReferredBy { get; set; }
        public string ReferralCode { get; set; }
        public List<ReferredDescription> ReferredDescriptions { get; set; }
        public List<PromoCode> PromoCodes { get; set; }

        public User()
        {
            ReferredDescriptions = new List<ReferredDescription>();
            PromoCodes = new List<PromoCode>();
        }
        
    }
    

    public class ReferredDescription : ViewModel
    {
        public string ReferredUserName { get; set; }
        public ulong ReferredUserId { get; set; }

        public bool Claimed { get; set; }
    }

    public class PromoCode : ViewModel
    {
        public string UsedPromoCode { get; set; } 
        
    }    

}