using System;
using NLog;
using Sandbox.Game.GameSystems.BankingAndCurrency;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;

namespace Referrals_project
{
    public class FinancialService
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        public void PlayerId(MyPlayer player)
        {
            long playerId = player.Identity.IdentityId;
            var id = player.Client.SteamUserId;
            if (id <= 0)
                return;
            string name = player.Identity?.DisplayName ?? "player";

        }
        
        /*public static long GetBlance(long accountNumber)
        {
            MyAccountInfo myAccountInfo;
            if (MyBankingSystem.TryGetAccountInfo(walletID, ref myAccountInfo))
                return (long) myAccountInfo.Balance;
            Util.SomeLog("(GetBlance) - wallet not found for walletID: " + walletID.ToString());
            return 0;
        }
        */

        public static bool GivePlayerCredits(long accountNumber, long playerCredit)
        {
            
            try
            {
                // confusionnnnnnnnnnnn
                MyAccountInfo playerAccount;
                if (MyBankingSystem.TryGetAccountInfo(accountNumber, ref playerAccount))
                {
                    if (playerCredit > 0)
                    {
                        if (MyBankingSystem.ChangeBalance(accountNumber, playerCredit))
                            return true;
                    }
                    else
                    {
                        if (playerAccount.Balance < playerCredit)
                        {
                            MyBankingSystem.ChangeBalance(accountNumber,  playerAccount.Balance);
                            playerAccount.Balance = 0;
                            return true;
                        }
                        if (MyBankingSystem.ChangeBalance(accountNumber, playerCredit))
                            return true;
                    }
                }
                
            }
            catch (Exception error)
            {
                Log.Error("failed to pay");
            }
            
            return false;
        }
    }
}