using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Referrals_project
{
    public class UserData
    {
        public List<User> Users { get; set; }
    }


    public class User
    {
        public string Name { get; set; }
        public bool? ReferralByUser { get; set; }
        public bool? ReferralByCode { get; set; }
        public ulong SteamId { get; set; }
        public ulong? ReferredBy { get; set; }
        public string ReferralCode { get; set; }
        public List<ReferredDescription> ReferredDescriptions { get; set; }
    }

    public class ReferredDescription
    {
        public string ReferredUserName { get; set; }
        public ulong ReferredUserId { get; set; }
    }
}