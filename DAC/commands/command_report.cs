using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace DAC
{
    public class command_report : IRocketCommand
    {
        public Darkness_Anti_Cheat instance;

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Name
        {
            get { return "report"; }
        }

        public string Help
        {
            get { return "Report a player via DAC"; }
        }

        public string Syntax
        {
            get { return "<player> <reason>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "rp" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                  "report"
                };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] commands)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            string get_player = commands.GetStringParameter(0);
            string get_reason = commands.GetParameterString(1);

            UnturnedPlayer to_player_report = UnturnedPlayer.FromName(get_player);

            if (commands.Length <= 1)
            {
                ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> Syntax: /report {Syntax}".Replace('(', '<').Replace(')', '>'), UnityEngine.Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);
            }
            else if (player.Equals(to_player_report))
            {
                ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> You can't report your self".Replace('(', '<').Replace(')', '>'), UnityEngine.Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);
            }
            else if (to_player_report == null)
            {
                ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> Player not found".Replace('(', '<').Replace(')', '>'), UnityEngine.Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);
            }
            else
            {
                ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> The report of {to_player_report.CSteamID} has been sent successfully, wait for an administrator to review it".Replace('(', '<').Replace(')', '>'), UnityEngine.Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                Darkness_Anti_Cheat_Functions.send_report(player, to_player_report, Darkness_Anti_Cheat.Instance.Configuration.Instance.report_player_webhook, get_reason);
                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(player, Darkness_Anti_Cheat.Instance.Configuration.Instance.report_player_webhook));
            }
        }
    }
}
