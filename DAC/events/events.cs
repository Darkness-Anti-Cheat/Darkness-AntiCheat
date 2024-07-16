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

            // We use this here, because we don't need to add another LOOP for this literally, we can detect someone using pitch 89 or up, disabled at this moment
            /*if (Darkness_Anti_Cheat.Instance.Configuration.Instance.anti_aim_detection)
            {
                if (UnturnedPlayer.FromCSteamID(killer).Player.look.pitch >= 89)
                {
                    // Increase rate...
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate++;
                }
                else if (UnturnedPlayer.FromCSteamID(killer).Player.look.pitch >= -89)
                {
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate++;
                }

                if (UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate == 3)
                {
                    // Reset
                    UnturnedPlayer.FromCSteamID(killer).Player.GetComponent<PlayerComponent>().Rate = 0;

                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                        ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {UnturnedPlayer.FromCSteamID(killer).DisplayName} has been kicked for (<color=red>Anti-Aim</color>)").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);
                    Darkness_Anti_Cheat_Functions.send_report(UnturnedPlayer.FromPlayer(player), UnturnedPlayer.FromCSteamID(killer), Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Anti-Aim)"); // lets take a screenshot and generate auto report
                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(UnturnedPlayer.FromCSteamID(killer), Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report

                    // Kick user if shot through the walls
                    UnturnedPlayer.FromCSteamID(killer).Kick("[DAC] You has been kicked for (Anti-Aim)");

                }
            }*/

            if (!Darkness_Anti_Cheat.Instance.Configuration.Instance.aimbot_detection) return;

            UnturnedPlayer BeingKilled = UnturnedPlayer.FromPlayer(player);
            UnturnedPlayer Killer = UnturnedPlayer.FromCSteamID(killer);

            // Detect if player has too much ping, just return
            //if (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping_high && Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping >= Darkness_Anti_Cheat_Functions.GetPlayerPing(Killer)) return;
            UnturnedChat.Say($"{Killer.Ping}");
            RaycastHit hit;

            if (cause == EDeathCause.GUN)
            {
                // Getting range of weapon
                var asset = UnturnedPlayer.FromCSteamID(killer).Player.equipment.asset;
                if (asset is ItemGunAsset gunAsset)
                {

                    // Silent aimbot detection through the walls, if player is looking at wall, structure or anything and not a player, and still hitting the player, is using cheats
                    // Only with weapons, not with grenades or rocket launchers
                    if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, gunAsset.range, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE | RayMasks.RESOURCE))
                    {
                        if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, gunAsset.range, RayMasks.PLAYER))
                        {
                            Killer.GetComponent<PlayerComponent>().RateAim++;

                            if (Killer.GetComponent<PlayerComponent>().RateAim == Darkness_Anti_Cheat.Instance.Configuration.Instance.aimbot_detection_rate) // Ratelimit, cause can do false positive
                            {
                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for '<color=red>Silent-aimbot</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                Killer.GetComponent<PlayerComponent>().RateAim = 0;

                                ThreadPool.QueueUserWorkItem((yes) =>
                                {
                                    Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Silent-aimbot)", false, null); // lets take a screenshot and generate auto report
                                    Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Silent Aim)");
                                });

                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook));
                            }
                        }
                    }
                    
                }
            }
            else
            {
                // Cannot detect Melee, i'm do lazy do it xd
                // Range changer hack detection, max of punch distance is 2, but i put more cause the lag compensation can fail
                if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, 15, RayMasks.PLAYER))
                {
                    switch(cause)
                    {
                        case EDeathCause.PUNCH:
                            if (Darkness_Anti_Cheat_Functions.CalculateDistance(Killer, BeingKilled) >= 2.5f) // If distance equals or high than, he is using punch hack...
                            {
                                Killer.GetComponent<PlayerComponent>().RatePunch++;

                                if (Killer.GetComponent<PlayerComponent>().RatePunch == Darkness_Anti_Cheat.Instance.Configuration.Instance.punch_distance_rate) // Ratelimit, cause can do false positive
                                {
                                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                        ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for '<color=red>Punch distance hack</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                    Killer.GetComponent<PlayerComponent>().RatePunch = 0;

                                    canDamage = false;

                                    ThreadPool.QueueUserWorkItem((yes) =>
                                    {
                                        Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Punch Distance Hack)", false, null); // lets take a screenshot and generate auto report
                                        Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Punch Distance Hack)");
                                    });

                                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                }
                            }
                            break;
                        case EDeathCause.MELEE:
                            var asset = UnturnedPlayer.FromCSteamID(killer).Player.equipment.asset;
                            if (asset is ItemMeleeAsset gunAsset)
                            {
                                if (Darkness_Anti_Cheat_Functions.CalculateDistance(Killer, BeingKilled) >= gunAsset.range) // If distance equals or high than, he is using melee hack...
                                {
                                    Killer.GetComponent<PlayerComponent>().RatePunch++;

                                    if (Killer.GetComponent<PlayerComponent>().RatePunch == Darkness_Anti_Cheat.Instance.Configuration.Instance.punch_distance_rate) // Ratelimit, cause can do false positive
                                    {
                                        if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                            ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been kicked for '<color=red>Melee distance hack</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                        Killer.GetComponent<PlayerComponent>().RatePunch = 0;

                                        canDamage = false;

                                        ThreadPool.QueueUserWorkItem((yes) =>
                                        {
                                            Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, "(Melee Distance Hack)", false, null); // lets take a screenshot and generate auto report
                                            Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Melee Distance Hack)");
                                        });

                                        Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report


                                    }
                                }
                            }
                            break;
                    }
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
                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(No-clip)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for '<color=red>No-clip</color>'", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
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

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Fake-Lag)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for '<color=red>Fake-Lag/color>'", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
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

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, "(Free-Cam)");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been kicked for '<color=red>Free-Cam/color>'", Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");


                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
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

                ThreadPool.QueueUserWorkItem((yes) =>
                {
                    Darkness_Anti_Cheat_Functions.send_report(null, player, Darkness_Anti_Cheat.Instance.Configuration.Instance.abuse_player_webhook, "Admin abuse logger", true, message, "FF0000"); // lets take a screenshot and generate auto report
                });

                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(player, Darkness_Anti_Cheat.Instance.Configuration.Instance.abuse_player_webhook));
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
                if (killer.Player.GetComponent<PlayerComponent>().Kills >= Darkness_Anti_Cheat.Instance.Configuration.Instance.player_kills_alert && 
                    (killer.Player.GetComponent<PlayerComponent>().Headshots / (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_kills_alert / 2)) * 100 == 100)
                {
                    ThreadPool.QueueUserWorkItem((yes) =>
                    {
                        Darkness_Anti_Cheat_Functions.send_report(null, killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook); // lets take a screenshot and generate auto report
                    });
                    

                    // reset the killer kills
                    killer.Player.GetComponent<PlayerComponent>().Kills = 0;
                    killer.Player.GetComponent<PlayerComponent>().Headshots = 0;

                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook));
                }
            }
        }
    }
}
