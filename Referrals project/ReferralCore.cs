using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using NLog;
using System.Xml.Serialization;
using Sandbox.Game.Multiplayer;
using SteamKit2.GC.Dota.Internal;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;

namespace Referrals_project
{
    public class ReferralCore : TorchPluginBase, IWpfPlugin
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static TorchSessionManager SessionManager;
        public static ReferralCore Instance { get; private set; }
        public static string UserDataPath = "";
        private ReferralControls _control;
        private Persistent<ReferralConfig> _config;
        public ReferralConfig Config => _config?.Data;

        public UserControl GetControl()
        {
            return _control ?? (_control = new ReferralControls(this));
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            Instance = this;
            SessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            SessionManager.SessionStateChanged += SessionManagerOnSessionStateChanged;
            UserDataPath = Path.Combine(StoragePath, "Users.xml");
            /*if (!File.Exists(UserDataPath))
            {
                File.Create(UserDataPath);
                var serializer = new XmlSerializer(typeof(UserData));
                var userData = new UserData { Users = new List<CMsgDOTAFrostivusTimeElapsed.User>()};
                using (var writer =
                    new StreamWriter(UserDataPath))
                {
                    serializer.Serialize(writer, userData);
                }
            }*/
        }

        private static void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    ;
                    break;
                case TorchSessionState.Unloading:
                    break;
                case TorchSessionState.Unloaded:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
            }
        }

        private void SetupConfig()
        {
            var configFile = Path.Combine(StoragePath, "ReferralConfig.cfg");
            try
            {
                _config = Persistent<ReferralConfig>.Load(configFile);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data != null) return;
            Log.Info("Creating Default Config");
            _config = new Persistent<ReferralConfig>(configFile, new ReferralConfig());
            _config.Save();
        }

        /*public static UserData UserDataFromStorage()
        {
            var serializer = new XmlSerializer(typeof(UserData));
            using (var reader = new StreamReader(UserDataPath))
            {
                return (UserData) serializer.Deserialize(reader);
            }
        }*/

        /*public static void WriteReferralDataToStorage()
        {
            var serializer = new XmlSerializer(typeof(UserData));
            using (var writer = new StreamWriter(Instance.StoragePath))
            {
                serializer.Serialize(writer, Instance.StoragePath + "/Users.data");
            }
        }*/

        /*public static Users GetUser(ulong steamId)
        {
            var userData = ReadUser().User;
            if (userData.Any(user => user.SteamId == steamId))
            {
                return userData.Find(user => user.SteamId == steamId);
            }

            return new Users()
            {
                Name = Sync.Players.TryGetIdentityNameFromSteamId(steamId) ?? "Unknown User",
                ReferralByUser = null,
                ReferralByCode = null,
                SteamId = steamId,
                ReferredBy = null,
                ReferralCode = null
            };
        }*/


        public static void SaveUser(Users user)
        {
            List<Users> User = new List<Users>();
            XmlSerializer searial = new XmlSerializer(typeof(List<Users>));
            var check = User.Any(x => x.SteamId == user.SteamId);
            
            User.Add(new Users(){Name = Sync.Players.TryGetIdentityNameFromSteamId(10001100)});
            User.Add(new Users(){ReferralByUser = true});
            User.Add(new Users(){ReferralCode = null});
            User.Add(new Users(){ReferralByCode = null});
            User.Add(new Users(){SteamId = 10001100});
            
            //ToDo add list to User for ReferredDescriptions
            
            
            using (FileStream fs = new FileStream(UserDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                searial.Serialize(fs, User);
            }    
            //var userData = UserDataFromStorage();
            //var check = userData.Users.Any(x => x.SteamId == user.SteamId);
            /*if (check)
            {
                var index = User.FindIndex(u => u.SteamId == user.SteamId);
                User[index] = user;
            }
            else
            {
                User.Add(user);
            }


            using (var writer =
                new StreamWriter(UserDataPath))
            {
                searial.Serialize(writer, userData);
            }*/
        }

        public void ReadUser(Users user)
        {
            List<Users> User = new List<Users>();
            XmlSerializer searial = new XmlSerializer(typeof(List<Users>));

            using (FileStream fs = new FileStream(UserDataPath, FileMode.Open, FileAccess.Read))
            { 
                User = searial.Deserialize(fs) as List<Users>;
            }
            
            //User is data output 
        }


        public void Save()
        {
            _config.Save();
        }
        
        public override void Dispose()
        {
            if (SessionManager != null)
                 SessionManager.SessionStateChanged -= SessionManagerOnSessionStateChanged;
            SessionManager = null;
        }
    }
}