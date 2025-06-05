using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Core.Utils;
using Steamworks;
using Rocket.API.Collections;
using Rocket.Core.Permissions;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using System.Collections;

namespace XPconomy
{
    public class XPconomy : RocketPlugin<XPconomyConfig>
    {
        public static XPconomy Instance;
        public static XPconomyDB Database;
        public List<Salary> SalaryList;

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"Balance", "Your balance is ${0}."},
            {"PlayerNotFound", "Player not found."},
            {"CantPaySelf", "You can't pay yourself."},
            {"CantSubSelf", "You can't subtract money from yourself."},
            {"PaySpecifyAmount", "Please specify the amount you'd like to pay."},
            {"InsufficientBalance", "You have insufficient balance."},
            {"PaySend", "You payed {0} ${1}."},
            {"PayReceive", "You received ${0} from {1}."},
            {"SpecifyAmount", "Please specify an amount."},
            {"SubtractInsufficient", "This player has insufficient balance."},
            {"Subtract", "You have been subtracted ${0}."},
            {"SubtractAdmin", "You have subtracted ${0} from {1}."},
            {"SalaryPay", "You have been payed your salary of ${0}."},
            {"SalaryPayUnemployed", "You have been payed ${0} for playing."},
            {"CheckBalance", "{0}'s balance is ${1}."}
        };



        protected override void Load()
        {
            Instance = this;

            U.Events.OnPlayerConnected += OnPlayerConnected;

            UnturnedPlayerEvents.OnPlayerUpdateExperience += OnPlayerUpdateExperience;

            Database = new XPconomyDB();
            SalaryList = Configuration.Instance.Salaries;

            if (Configuration.Instance.SalaryEnabled)
            {
                TaskDispatcher.QueueOnMainThread(() => SalaryTask());

                //await Task.Run(StartSalary);
            }

            Logger.Log($"Loaded {name} version {Assembly.GetName().Version} by Spinkles");
        }


        protected override void Unload()
        {
            Logger.Log($"Unloaded {name} version {Assembly.GetName().Version} by Spinkles");
        }


        private void OnPlayerConnected(UnturnedPlayer player)
        {
            uint PlayerExp = player.Experience;
            ulong SteamID = Convert.ToUInt64(player.Id);


            if (!CheckForAccount(player))
            {
                uint Balance = Configuration.Instance.StartingBalance;
                player.Experience = 0 + Configuration.Instance.StartingBalance;
                Database.CreateNewBalance(SteamID: SteamID, Balance: player.Experience);
                return;
            }
            else if (CheckForAccount(player))
            {
                uint Balance = GetBalance(player);

                player.Experience = Balance;
            }
        }

        private bool CheckForAccount(UnturnedPlayer player)
        {
            ulong SteamID = Convert.ToUInt64(player.Id);

            bool Exists = Database.CheckIfExists(SteamID);

            return Exists;
        }

        private void OnPlayerUpdateExperience(UnturnedPlayer player, uint amount)
        {
            ulong SteamID = Convert.ToUInt64(player.Id);
            ulong Balance = player.Experience;

            Database.UpdateBalance(SteamID: SteamID, Balance: Balance);

        }

        public uint GetBalance(UnturnedPlayer player)
        {
            ulong SteamID = Convert.ToUInt64(player.Id);
            uint Balance = Database.GetBalance(SteamID);

            return Balance;
        }

        /*private async Task StartSalary()
        {
            if (Configuration.Instance.SalaryEnabled)
            {
                await Task.Run(PaySalary);

                if (Configuration.Instance.Debug) Logger.Log("Starting salary task!");
            }
        }*/

        private async Task SalaryTask()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(Configuration.Instance.SalaryInterval));
                if (Configuration.Instance.Debug) Logger.Log("Loop happened.");
                SalaryPayment();
            }
        }

        public void SalaryPayment()
        {
            if (Configuration.Instance.Debug) Logger.Log("Running salary payments.");

            foreach (var client in Provider.clients)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(client);

                if (Configuration.Instance.Debug) Logger.Log("Foreach var client running.");

                foreach (Salary salary in XPconomy.Instance.SalaryList)
                {
                    if (Configuration.Instance.Debug) Logger.Log("Foreach salary permissions.");

                    if (player.HasPermission(salary.SalaryPermission))
                    {
                        if (Configuration.Instance.Debug) Logger.Log("Permission matches, paying salary now.");

                        player.Experience = player.Experience + salary.SalaryAmount;

                        SalaryAnnounce(salary.SalaryAmount, player);

                        break;
                    }
                }
            }
        }

        public void SalaryAnnounce(ulong salaryAmount, UnturnedPlayer player)
        {
            UnturnedChat.Say(player, Translate("SalaryPay", salaryAmount), Color.green);
        }
    }


}
