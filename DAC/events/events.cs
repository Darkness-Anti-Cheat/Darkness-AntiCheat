using Darkness_Anti_Cheat.components;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace DAC
{
    public class Darkness_Anti_Cheat_Events
    {
        // check trust factor
        public static void trust_factor() { }

        // check aimbots
        public static void event_aimbot(Player player, EDeathCause cause, ELimb limb, CSteamID killer, Vector3 direction, float damage, float times, bool canDamage)
        {
            if (cause != EDeathCause.GUN && cause != EDeathCause.PUNCH && cause != EDeathCause.MELEE) return;

            // We use this here, because we don't need to add another LOOP for this literally, we can detect someone using pitch 89 or up
            // 
            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.anti_aim_detection)
            {
                if (UnturnedPlayer.FromCSteamID(killer).Player.look.pitch >= 89)
                {
                    // Increase rate...
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate++;
                }
                else if (UnturnedPlayer.FromCSteamID(killer).Player.look.pitch >= 89)
                {
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate++;
                }

                if(UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate == 3)
                {
                    // Reset
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate = 0;

                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                        ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {UnturnedPlayer.FromCSteamID(killer).DisplayName} has been kicked for (<color=red>Anti-Aim</color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                    Darkness_Anti_Cheat_Functions.send_report(null, UnturnedPlayer.FromCSteamID(killer), Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Anti-Aim)"); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(UnturnedPlayer.FromCSteamID(killer), Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // Kick user if shot through the walls
                    UnturnedPlayer.FromCSteamID(killer).Kick("[DAC] You has been kicked for (Anti-Aim)");

                }
            }

            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.aimbot_detection) return;

            UnturnedPlayer BeingKilled = UnturnedPlayer.FromPlayer(player);
            UnturnedPlayer Killer = UnturnedPlayer.FromCSteamID(killer);

            // Detect if player has too much ping, just return
            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping_high && Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping <= Killer.Ping) return;

            RaycastHit hit;

            if (cause == EDeathCause.GUN)
            {
                // Getting range of weapon
                List<ItemGunAsset> sortedAssets = new List<ItemGunAsset>(Assets.find(EAssetType.ITEM).Cast<ItemGunAsset>());

                ItemGunAsset asset = sortedAssets.Where(i => i.id == Killer.Player.equipment.itemID).FirstOrDefault();

                // Silent aimbot detection through the walls
                if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, asset.range, RayMasks.MEDIUM | RayMasks.LARGE | RayMasks.MEDIUM))
                {
                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                        ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for (<color=red>Silent-aimbot</color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                    Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Silent-aimbot)"); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // Kick user if shot through the walls
                    Killer.Kick("[DAC] You has been kicked for (Silent aimbot)");
                }
                // Here we detect if player was visible, if not, kick
                if(Darkness_Anti_Cheat_Functions.IsPlayerVisible(BeingKilled, Killer))
                {
                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                        ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for (<color=red>Silent-aimbot</color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                    Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Silent-aimbot)"); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // Kick user if shot through the walls
                    Killer.Kick("[DAC] You has been kicked for (Silent aimbot)");
                }
            }
            else
            {
                // Cannot detect Melee, i'm do lazy do it xd
                // Range changer hack detection, max of punch distance is 2, but i put more cause the lag compensation can fail
                if (!Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, 2.5f, RayMasks.PLAYER) && cause == EDeathCause.PUNCH)
                {
                    Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Punch distance hack)");

                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                        ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for (<color=red>Punch distance hack</color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                    Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Punch distance hack)"); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // Kick user
                    Killer.Kick("[DAC] You has been kicked for (Punch distance hack)"); 
                }
            }
        }

        // check no_clip
        public static IEnumerator no_clip()
        {
            RaycastHit raycastHit = new RaycastHit();
            while (true)
            {
                yield return new WaitForSeconds(3f);
                ThreadPool.QueueUserWorkItem((yes) =>
                {
                    foreach (SteamPlayer list in Provider.clients)
                    {
                        UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(list);
                        RocketPlayer rocketPlayer = new RocketPlayer(unturnedPlayer.Id);

                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass") && !unturnedPlayer.IsInVehicle) 
                        {
                            // Ray trace from ground?
                            if (!Physics.Raycast(unturnedPlayer.Position, Vector3.down, out raycastHit, 2048f, RayMasks.RESOURCE | RayMasks.LARGE | RayMasks.MEDIUM | RayMasks.SMALL | RayMasks.GROUND | RayMasks.GROUND2))
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(No-clip)"); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(No-clip)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for (<color=red>No-clip</color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                // No clip detection
                                unturnedPlayer.Kick("[DAC] You has been kicked for (No-clip)");
                            }
                        }
                        unturnedPlayer = null;
                        raycastHit = new RaycastHit();
                    }
                });
            }
        }
       
        // detect external hacks fake-lag
        public static IEnumerator clumsy_detect_fake_lag()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                ThreadPool.QueueUserWorkItem((yes) =>
                {
                    foreach (SteamPlayer list in Provider.clients)
                    {
                        UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(list);
                        RocketPlayer rocketPlayer = new RocketPlayer(unturnedPlayer.Id);

                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
                        {
                            // it's using fake lag?
                            if (unturnedPlayer.Player.input.IsUnderFakeLagPenalty)
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Fake-Lag)"); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Fake-Lag)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for (<color=red>Fake-Lag/color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                // Fake-Lag detection
                                unturnedPlayer.Kick("[DAC] You has been kicked for (Fake-Lag)");
                            }
                        }
                        unturnedPlayer = null;
                    }
                });
            }
        }

        // check anti_free_cam, i don't know if my logic it's working, but let's try
        public static IEnumerator anti_free_cam()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                ThreadPool.QueueUserWorkItem((yes) =>
                {
                    foreach (SteamPlayer list in Provider.clients)
                    {
                        UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(list);
                        RocketPlayer rocketPlayer = new RocketPlayer(unturnedPlayer.Id);

                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
                        {
                            // Player without permissions it's using freecam?
                            if(unturnedPlayer.Player.look.isOrbiting || unturnedPlayer.Player.look.isTracking)
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Free-Cam)"); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Free-Cam)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for (<color=red>Free-Cam/color>)", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                // Free-Cam detection
                                unturnedPlayer.Kick("[DAC] You has been kicked for (Free-Cam)");
                            }
                        }
                        unturnedPlayer = null;
                    }
                });
            }
        }

        public static void chat_log(UnturnedPlayer player, string message)
        {
            if (player.HasPermission("*") || player.IsAdmin)
            {
                if (!message.StartsWith("/"))
                    return;

                // Transform msg
                message = message.Remove(0, 1).Split(' ')[0];

                AbuseCommands Entry = Darkness_Anti_Cheat.Instance.Configuration.Instance.Entries.Where(entry => entry.Name == message ^ entry.GetAliases().Contains(message)).FirstOrDefault();

                if (Entry.Equals(default(AbuseCommands)))
                    return;

                Darkness_Anti_Cheat_Functions.send_report(null, player, Darkness_Anti_Cheat.Instance.Configuration.Instance.abuse_player_webhook, "Admin abuse logger", true, message, "FF0000"); // lets take a screenshot and generate auto report
                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(player, Darkness_Anti_Cheat.Instance.Configuration.Instance.abuse_player_webhook)); // lets take a screenshot and generate auto report
            }
        }

        // auto report system
        public static void auto_report(UnturnedPlayer killer, UnturnedPlayer player, ELimb limb)
        {
            if(killer != null)
            {
                // check headshots
                if (limb == ELimb.SKULL)
                    killer.Player.GetComponent<PlayerComponent>().Headshots++;

                // add a kill
                killer.Player.GetComponent<PlayerComponent>().Kills++;

                // reset the murder kills
                player.Player.GetComponent<PlayerComponent>().Kills = 0;
                player.Player.GetComponent<PlayerComponent>().Headshots = 0;

                // if the killer has 100% of headshot, send the report
                if (killer.Player.GetComponent<PlayerComponent>().Kills >= 20 && (killer.Player.GetComponent<PlayerComponent>().Headshots / 10) * 100 == 100)
                {
                    Darkness_Anti_Cheat_Functions.send_report(null, killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // reset the killer kills
                    killer.Player.GetComponent<PlayerComponent>().Kills = 0;
                    killer.Player.GetComponent<PlayerComponent>().Headshots = 0;
                }
            }
        }
    }
}