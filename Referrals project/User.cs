using System;
using System.Collections.Generic;
using System.IO;
using KothPlugin;
using ProtoBuf;
using Sandbox.ModAPI;
using VRage.Utils;

namespace Referrals_project
{
    [ProtoContract]
    public class User
    {
        [ProtoMember(1)]
        public List<UserReferralDescription> UserReferral { get; set; } = new List<UserReferralDescription>();
    }
}


 