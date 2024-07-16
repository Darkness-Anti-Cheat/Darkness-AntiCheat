using Darkness_Anti_Cheat.components;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Threading;
using UnityEngine;

namespace DAC
{
    public class Darkness_Anti_Cheat : RocketPlugin<Configuration>
    {
        public static Darkness_Anti_Cheat Instance;

        public void AutoAnnouncement()
        {
            ChatManager.serverSendMessage(Translate("AutoAnnouncement").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);
        }

        // Translations
        public override TranslationList DefaultTranslations => new TranslationList()
        {
           { "AutoAnnouncement", "<color=#2391DE>[DAC]</color> This server is protected by DAC" },
        };

        protected override void Load()
        {
            Instance = this;

            Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] This is not an anti cheat at all, but a tool that improves the server to prevent people with cheats.", ConsoleColor.Green);
            Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] (WARNING) ¡This plugin require a discord Server!", ConsoleColor.DarkYellow);

            if (Configuration.Instance.noclip_detection)
            {
                Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] no_clip event loaded", ConsoleColor.DarkCyan);
                ThreadPool.QueueUserWorkItem((yes) => Darkness_Anti_Cheat_Events.no_clip());
            }

            if (Configuration.Instance.anti_free_cam)
            {
                Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] anti_free_cam event loaded", ConsoleColor.DarkCyan);
                ThreadPool.QueueUserWorkItem((yes) => Darkness_Anti_Cheat_Events.anti_free_cam());
            }

            if(Configuration.Instance.clumsy_detect_fake_lag)
            {
                Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] clumsy_detect_fake_lag event loaded", ConsoleColor.DarkCyan);
                ThreadPool.QueueUserWorkItem((yes) => Darkness_Anti_Cheat_Events.clumsy_detect_fake_lag());
            }

            if (Configuration.Instance.abuse_detection)
            {
                UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] onPlayerDeath event loaded", ConsoleColor.DarkCyan);
            }

            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerChatted event loaded", ConsoleColor.DarkCyan);
            DamageTool.playerDamaged += OnPlayerDamage; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerDamage event loaded", ConsoleColor.DarkCyan);
            U.Events.OnPlayerConnected += OnPlayerConnect; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerConnect event loaded", ConsoleColor.DarkCyan);

            InvokeRepeating("AutoAnnouncement", 350, 350);
        }

        protected override void Unload()
        {
            // Unload all events
            U.Events.OnPlayerConnected -= OnPlayerConnect; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerConnect event unloaded", ConsoleColor.DarkCyan);
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] onPlayerDeath event unloaded", ConsoleColor.DarkCyan);
            
            if (Configuration.Instance.abuse_detection)
            {
                UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerChatted event unloaded", ConsoleColor.DarkCyan);
            }

            DamageTool.playerDamaged -= OnPlayerDamage; Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] OnPlayerDamage event unloaded", ConsoleColor.DarkCyan);

            CancelInvoke("AutoAnnouncement");

            Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] unloaded", ConsoleColor.DarkYellow);
        }

        private void OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, SDG.Unturned.EChatMode chatMode, ref bool cancel) => Darkness_Anti_Cheat_Events.chat_log(player, message);
        private void OnPlayerConnect(UnturnedPlayer player) { player.Player.GetComponent<PlayerComponent>().Kills = 0; player.Player.GetComponent<PlayerComponent>().Deaths = 0; player.Player.GetComponent<PlayerComponent>().Headshots = 0; }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID killer) => Darkness_Anti_Cheat_Events.auto_report(UnturnedPlayer.FromCSteamID(killer), player, limb);

        private void OnPlayerDamage(Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage) => Darkness_Anti_Cheat_Events.event_aimbot(player, cause, limb, killer, direction, damage, times, canDamage);
    }
}