using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace XPconomy.Commands
{
    public class CommandBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Balance";

        public string Help => "View your balance.";

        public string Syntax => "/balance";

        public List<string> Aliases => new List<string> { "Bal" };

        public List<string> Permissions => new List<string> { "XPconomy.Balance" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            uint Balance = XPconomy.Instance.GetBalance(player);

            UnturnedChat.Say(caller, XPconomy.Instance.Translate("Balance", Balance), Color.green);
        }
    }
}
