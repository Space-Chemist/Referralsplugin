using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using NLog;
using System.Xml.Serialization;
using KothPlugin;
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
        private ReferralControls _control;
        private Persistent<ReferralConfig> _config;
        private UserData _userData;
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
        
        public static UserData ReferralDataFromStorage()
        {
            var serializer = new XmlSerializer(typeof(UserData));
            using (var reader = new StreamReader(Instance.StoragePath))
            {
                return (UserData) serializer.Deserialize(reader);
            }
        }
        
        public static void WriteReferralDataToStorage()
        {
            var serializer = new XmlSerializer(typeof(UserData));
            using (var writer = new StreamWriter(Instance.StoragePath))
            {
                serializer.Serialize(writer, Instance.StoragePath+"/Users.data");
            }
        }

        public void PlayerToUser()
        {
            var sd = _userData.Users;
            var h = sd.Any(user => user.SteamId== PlayerSteamId);
            //MyVisualScriptLogicProvider.SendChatMessage(g.ToString());
            if (!h)
            {
                sd.Add(new User()
                {
                    Name = //playername,
                    ReferralByUser = //bool,
                    ReferralByCode = //bool
                    SteamId = //steamdid,
                    ReferredBy = //people who they referred,
                    ReferralCode = "filler", //Referral cod if not null,
                    User.Add( new ReferredDescriptions()
                    {
                        ReferredUserName = //referred user name
                        ReferredUserId = 1111, // referred user steamID
                    })
                });
            }
            else
            {
                sd.Find(user => user.SteamId == PlayerSteamId); //.Points += points; need event 
            }
        }

        public void Save()
        {
            _config.Save();
        }
    }
}