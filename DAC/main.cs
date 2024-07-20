using Darkness_Anti_Cheat.components;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Threading;
using UnityEngine;
using static Rocket.Unturned.Events.UnturnedPlayerEvents;

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

            Darkness_Anti_Cheat_Functions.Log("(WARNING) This is not an anti cheat at all, but a tool that improves the server to prevent people with cheats.", ConsoleColor.Green);
            Darkness_Anti_Cheat_Functions.Log("(WARNING) ¡This plugin require a discord Server!", ConsoleColor.DarkYellow);


            if (Configuration.Instance.noclip_detection) Darkness_Anti_Cheat_Functions.Log("no_clip event loaded", ConsoleColor.DarkCyan);
            if (Configuration.Instance.anti_free_cam) Darkness_Anti_Cheat_Functions.Log("anti_free_cam event loaded", ConsoleColor.DarkCyan);
            if (Configuration.Instance.clumsy_detect_fake_lag) Darkness_Anti_Cheat_Functions.Log("clumsy_detect_fake_lag event loaded", ConsoleColor.DarkCyan);
            ThreadPool.QueueUserWorkItem((yes) => StartCoroutine(Darkness_Anti_Cheat_Events.get_players_detection()));

            if (Configuration.Instance.abuse_detection)
            {
                Darkness_Anti_Cheat_Functions.Log("onPlayerDeath event loaded", ConsoleColor.DarkCyan);
                UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            }

            UnturnedPlayerEvents.OnPlayerChatted += OnPlayerChatted; Darkness_Anti_Cheat_Functions.Log("OnPlayerChatted event loaded", ConsoleColor.DarkCyan);
            DamageTool.playerDamaged += OnPlayerDamage; Darkness_Anti_Cheat_Functions.Log("OnPlayerDamage event loaded", ConsoleColor.DarkCyan);
            U.Events.OnPlayerConnected += OnPlayerConnect; Darkness_Anti_Cheat_Functions.Log("OnPlayerConnect event loaded", ConsoleColor.DarkCyan);
            UnturnedPlayerEvents.OnPlayerInventoryAdded += OnPlayerInventoryAdded; Darkness_Anti_Cheat_Functions.Log("OnPlayerInventoryAdded event loaded", ConsoleColor.DarkCyan);

            Level.onPostLevelLoaded += OnPostLevelLoaded;
            if (Level.isLoaded)
            {
                OnPostLevelLoaded(420);
            }

            InvokeRepeating("AutoAnnouncement", 350, 350);
        }

        protected override void Unload()
        {
            // Unload all events
            U.Events.OnPlayerConnected -= OnPlayerConnect; Darkness_Anti_Cheat_Functions.Log("OnPlayerConnect event unloaded", ConsoleColor.DarkCyan);
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath; Darkness_Anti_Cheat_Functions.Log("onPlayerDeath event unloaded", ConsoleColor.DarkCyan);

            if (Configuration.Instance.abuse_detection)
            {
                UnturnedPlayerEvents.OnPlayerChatted -= OnPlayerChatted; Darkness_Anti_Cheat_Functions.Log("OnPlayerChatted event unloaded", ConsoleColor.DarkCyan);
            }

            DamageTool.playerDamaged -= OnPlayerDamage; Darkness_Anti_Cheat_Functions.Log("OnPlayerDamage event unloaded", ConsoleColor.DarkCyan);
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= OnPlayerInventoryAdded; Darkness_Anti_Cheat_Functions.Log("OnPlayerInventoryAdded event unloaded", ConsoleColor.DarkCyan);
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
            CancelInvoke("AutoAnnouncement");

            Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] unloaded", ConsoleColor.DarkYellow);
        }

        public void OnPostLevelLoaded(int level)
        {
            Darkness_Anti_Cheat_Functions.Log("Let's destroy the game breakers...", ConsoleColor.Green);
        }

        private void OnPlayerInventoryAdded(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P) => Darkness_Anti_Cheat_Events.take_items_through_walls(player, inventoryGroup, inventoryIndex, P);
        private void OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, SDG.Unturned.EChatMode chatMode, ref bool cancel) => Darkness_Anti_Cheat_Events.chat_log(player, message);
        private void OnPlayerConnect(UnturnedPlayer player) { player.Player.GetComponent<PlayerComponent>().Kills = 0; player.Player.GetComponent<PlayerComponent>().Deaths = 0; player.Player.GetComponent<PlayerComponent>().Headshots = 0; }
        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID killer) => Darkness_Anti_Cheat_Events.auto_report(killer, player, limb);
        private void OnPlayerDamage(Player player, ref EDeathCause cause, ref ELimb limb, ref CSteamID killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage) => Darkness_Anti_Cheat_Events.event_aimbot(player, cause, limb, killer, direction, damage, times, canDamage);
    
    }
}