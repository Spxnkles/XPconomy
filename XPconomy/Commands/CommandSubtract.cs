using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Chat;
using UnityEngine;
using SDG.Unturned;

namespace XPconomy.Commands
{
    public class CommandSubtract : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Subtract";

        public string Help => "Subtract money away from a player. Can't go negative.";

        public string Syntax => "/subtract (Player) (Amount)";

        public List<string> Aliases => new List<string> { "sbt" };

        public List<string> Permissions => new List<string> { "XPconomy.Subtract" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Player = command.GetUnturnedPlayerParameter(0);
            uint Amount;

            if (!UInt32.TryParse(command[1], out Amount))
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("SpecifyAmount"), Color.green);
                return;
            }

            if (Amount <= 0)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("SpecifyAmount"), Color.green);
                return;
            }

            if (Player.Id == null)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PayPlayerNotFound"), Color.green);
                return;
            }

            if (Player.Id == caller.Id)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("CantSubSelf"), Color.green);
                return;
            }

            if (Player.Experience >= Amount)
            {
                Player.Experience = Player.Experience - Amount;

                UnturnedChat.Say(Player, XPconomy.Instance.Translate("Subtract", Amount));
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("SubtractAdmin", Amount, Player.CharacterName));
            }
            else if (Player.Experience < Amount)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("SubtractInsufficient", Amount));
                return;
            }
        }
    }
}
