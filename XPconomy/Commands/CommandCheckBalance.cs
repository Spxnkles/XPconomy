using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using UnityEngine;

namespace XPconomy.Commands
{
    public class CommandCheckBalance : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Checkbalance";

        public string Help => "Check other users balance!";

        public string Syntax => "/checkbalance (username)";

        public List<string> Aliases => new List<string> { "cbal", "cb" };

        public List<string> Permissions => new List<string> { "XPconomy.Checkbalance" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = command.GetUnturnedPlayerParameter(0);

            if (player.Id == null)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PlayerNotFound"), Color.green);
                return;
            }

            uint Balance = XPconomy.Instance.GetBalance(player);

            UnturnedChat.Say(caller, XPconomy.Instance.Translate("CheckBalance", player.CharacterName, Balance), Color.green);
        }
    }
}
