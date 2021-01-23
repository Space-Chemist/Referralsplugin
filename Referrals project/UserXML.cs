using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Referrals_project
{
    public class UserXML
    {
        [Serializable()]
        [XmlRoot(ElementName = "User")]
        public class User
        {
            [XmlElement("ReferralData")] public List<ReferralData> ReferralData { get; set; }
        }

        [Serializable()]
        [XmlRoot(ElementName = "ReferralData")]
        public class ReferralData
        {
            [XmlElement] public string name { get; set; }
            [XmlElement] public List<ReferralDescriptions> ReferralDescriptions { get; set; }
        }

        [Serializable()]
        [XmlRoot(ElementName = "ReferralDescription")]
        public class ReferralDescriptions
        {
            [XmlElement] public bool ReferralByUser { get; set; }
            [XmlElement] public bool ReferralByCode { get; set; }
            [XmlElement] public long SteamId { get; set; }
            [XmlElement] public long ReferredBy{ get; set; }
            [XmlElement] public string ReferralCode { get; set; }
            [XmlElement] public List<ReferredDescriptions> ReferredDescriptions { get; set; }
        }

        [Serializable()]
        [XmlRoot(ElementName = "ReferredDescriptions")]
        public class ReferredDescriptions
        {
            [XmlElement] public string Referredname { get; set; }
            [XmlElement] public bool ReferredUser { get; set; }
        }

    }
}


 