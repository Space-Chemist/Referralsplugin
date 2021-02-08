using System.Collections.Generic;
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

        public User()
        {
            ReferredDescriptions = new List<ReferredDescription>();
        }
        
    }
    

    public class ReferredDescription : ViewModel
    {
        public string ReferredUserName { get; set; }
        public ulong ReferredUserId { get; set; }

        public bool Claimed { get; set; }
    }

}