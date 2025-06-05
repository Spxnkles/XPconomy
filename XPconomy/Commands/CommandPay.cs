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
    public class CommandPay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "Pay";

        public string Help => "Pay other plays from your balance!";

        public string Syntax => "/pay [Player] [Amount]";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "XPconomy.Pay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer Sender = (UnturnedPlayer)caller;
            UnturnedPlayer Receiver = command.GetUnturnedPlayerParameter(0);
            uint Amount;

            if (!UInt32.TryParse(command[1], out Amount))
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PaySpecifyAmount"), Color.green);
                return;
            }

            if (Amount <= 0)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PaySpecifyAmount"), Color.green);
                return;
            }

            if (Receiver.Id == null)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PlayerNotFound"), Color.green);
                return;
            }

            if (Receiver.Id == Sender.Id)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("CantPaySelf"), Color.green);
                return;
            }


            if (Sender.Experience >= Amount)
            {
                Sender.Experience = Sender.Experience - Amount;
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("PaySend", Receiver.CharacterName, Amount), Color.green);

                Receiver.Experience = Receiver.Experience + Amount;
                UnturnedChat.Say(Receiver, XPconomy.Instance.Translate("PayReceive", Amount, Sender.CharacterName), Color.green);
            }
            else if (Sender.Experience < Amount)
            {
                UnturnedChat.Say(caller, XPconomy.Instance.Translate("InsufficientBalance"), Color.green);
                return;
            }







        }
    }
}
