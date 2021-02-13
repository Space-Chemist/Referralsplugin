using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using NLog;
using System.Xml.Serialization;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;
using VRage.Game.ModAPI;
using VRage.RemoteClient.Core;

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
        public Persistent<UserData> _userdata;

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
            SetupConfig();
            UserDataPath = Path.Combine(StoragePath, "Users.xml");
            if (!File.Exists(UserDataPath))
            {
                var serializer = new XmlSerializer(typeof(UserData));
                var userData = new UserData {Users = new List<User>()};
                using (var fileWriter = new FileStream(UserDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite))
                {
                    serializer.Serialize(fileWriter, userData);
                }
            }
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

        public static UserData UserDataFromStorage()
        {
            var serializer = new XmlSerializer(typeof(UserData));
            using (var reader = new StreamReader(UserDataPath))
            {
                return (UserData) serializer.Deserialize(reader);
            }
        }


        public static User GetUser(ulong steamId)
        {
            var userData = UserDataFromStorage().Users;
            if (userData.Any(user => user.SteamId == steamId))
            {
                return userData.Find(user => user.SteamId == steamId);
            }

            return new User()
            {
                Name = Sync.Players.TryGetIdentityNameFromSteamId(steamId) ?? "Unknown User",
                ReferralByUser = null,
                ReferralByCode = null,
                SteamId = steamId,
                ReferredBy = null,
                ReferralCode = null,
                ReferredDescriptions = new List<ReferredDescription>()
            };
        }


        public static void SaveUser(User user)
        {
            var serializer = new XmlSerializer(typeof(UserData));
            var userData = UserDataFromStorage();
            var check = userData.Users.Any(x => x.SteamId == user.SteamId);
            if (check)
            {
                var index = userData.Users.FindIndex(u => u.SteamId == user.SteamId);
                userData.Users[index] = user;
            }
            else
            {
                userData.Users.Add(user);
            }


            using (var fileWriter = new FileStream(UserDataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                serializer.Serialize(fileWriter, userData);
            }
        }


        public static bool Dostuff(User user, IMyPlayer player)
        {
            var folderDirectory = ReferralCore.Instance.StoragePath;
            var myIdentity = ((MyPlayer) player).Identity;
            var targetIdentity = myIdentity.IdentityId;
            var myCharacter = myIdentity.Character;
            if (user.ReferralByUser != null)
            {
                if ((bool) user.ReferralByUser)
                {
                    var methods = new GridMethods(folderDirectory, "GridName");
                    return methods.LoadGrid("GridNme", myCharacter, targetIdentity);
                }
            }

            if (user.ReferralByCode == null) return false;
            {
                if (!(bool) user.ReferralByCode) return false;
                var methods = new GridMethods(folderDirectory, "GridName");
                return methods.LoadGrid("GridNme", myCharacter, targetIdentity);
            }
        }

        //full credit to https://github.com/TorchAPI/Essentials/blob/415a7e12809c75fc1efcfbc878cfee1730efa6ff/Essentials/Utilities.cs#L56
        public static IMyIdentity GetIdentityByNameOrIds(string playerNameOrIds)
        {
            foreach (var identity in MySession.Static.Players.GetAllIdentities())
            {
                if (identity.DisplayName == playerNameOrIds)
                    return identity;

                if (long.TryParse(playerNameOrIds, out var identityId))
                    if (identity.IdentityId == identityId)
                        return identity;

                if (!ulong.TryParse(playerNameOrIds, out var steamId)) continue;
                var id = MySession.Static.Players.TryGetSteamId(identity.IdentityId);
                if (id == steamId)
                    return identity;
            }

            return null;
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