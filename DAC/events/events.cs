using Darkness_Anti_Cheat.components;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
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

        // check aimbots for players
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

            UnturnedPlayer BeingKilled = UnturnedPlayer.FromPlayer(player);
            UnturnedPlayer Killer = UnturnedPlayer.FromCSteamID(killer);
            RocketPlayer rocketPlayer = new RocketPlayer(Killer.Id);
            var reason = "";

            // Detect if player has too much ping, just return
            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping_high)
            {
                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_ping <= Darkness_Anti_Cheat_Functions.GetPlayerPing(Killer))
                    return;
            }

            RaycastHit hit, other_hit;
            if (!Killer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
            {
                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.aimbot_detection && cause == EDeathCause.GUN)
                {
                    // Getting range of weapon
                    var asset = UnturnedPlayer.FromCSteamID(killer).Player.equipment.asset;
                    if (asset is ItemGunAsset gunAsset)
                    {

                        // Silent aimbot detection through the walls, if player is looking at wall, structure or anything and not a player, and still hitting the player, is using cheats
                        // Only with weapons, not with grenades or rocket launchers
                        if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, gunAsset.range, RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.VEHICLE | RayMasks.RESOURCE | RayMasks.PLAYER))
                        {
                            // Test it with real anti cheat, if player is not visible or not targeting him and shooting through walls, detect it as silent-aim
                            if (hit.transform == null || !hit.transform.CompareTag("Enemy"))
                            {
                                Killer.GetComponent<PlayerComponent>().RateAim++;

                                if (Killer.GetComponent<PlayerComponent>().RateAim == Darkness_Anti_Cheat.Instance.Configuration.Instance.aimbot_detection_rate) // Ratelimit, cause can do false positive
                                {
                                    reason = "Silent-Aimbot";

                                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                        ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been punished for '<color=red>{reason}</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                    Killer.GetComponent<PlayerComponent>().RateAim = 0;

                                    ThreadPool.QueueUserWorkItem((yes) =>
                                    {
                                        Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"({reason})", false, null); // lets take a screenshot and generate auto report
                                        Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");
                                    });

                                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook));
                                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(Killer, reason));
                                }
                            }
                        }
                    }
                }

                // Cannot detect Melee, i'm do lazy do it xd
                // Range changer hack detection, max of punch distance is 2, but i put more cause the lag compensation can fail
                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.punch_override_distance_detection)
                {
                    if (Physics.Raycast(Killer.Player.look.aim.position, Killer.Player.look.aim.forward, out hit, 15f, RayMasks.PLAYER))
                    {
                        switch (cause)
                        {
                            case EDeathCause.PUNCH:
                                reason = "Punch Override Hack";
                                if (Darkness_Anti_Cheat_Functions.CalculateDistance(Killer, BeingKilled) >= 2.5f) // If distance equals or high than, he is using punch hack...
                                {
                                    Killer.GetComponent<PlayerComponent>().RatePunch++;

                                    if (Killer.GetComponent<PlayerComponent>().RatePunch == Darkness_Anti_Cheat.Instance.Configuration.Instance.punch_distance_rate) // Ratelimit, cause can do false positive
                                    {
                                        if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                            ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been punished for '<color=red>{reason}</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                        Killer.GetComponent<PlayerComponent>().RatePunch = 0;

                                        canDamage = false;

                                        ThreadPool.QueueUserWorkItem((yes) =>
                                        {
                                            Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"({reason})", false, null); // lets take a screenshot and generate auto report
                                            Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");
                                        });

                                        Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                        Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(Killer, reason));
                                    }
                                }
                                break;
                            case EDeathCause.MELEE:
                                reason = "Melee Override Hack";
                                var asset = UnturnedPlayer.FromCSteamID(killer).Player.equipment.asset;
                                if (asset is ItemMeleeAsset gunAsset)
                                {
                                    if (Darkness_Anti_Cheat_Functions.CalculateDistance(Killer, BeingKilled) >= gunAsset.range) // If distance equals or high than, he is using melee hack...
                                    {
                                        Killer.GetComponent<PlayerComponent>().RatePunch++;

                                        if (Killer.GetComponent<PlayerComponent>().RatePunch == Darkness_Anti_Cheat.Instance.Configuration.Instance.punch_distance_rate) // Ratelimit, cause can do false positive
                                        {
                                            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                                ChatManager.serverSendMessage(($"<color=#2391DE>[DAC]</color> {Killer.DisplayName} has been punished for '<color=red>{reason}</color>'").Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png", true);

                                            Killer.GetComponent<PlayerComponent>().RatePunch = 0;

                                            canDamage = false;

                                            ThreadPool.QueueUserWorkItem((yes) =>
                                            {
                                                Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"({reason})", false, null); // lets take a screenshot and generate auto report
                                                Darkness_Anti_Cheat_Functions.webhook_logs(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");
                                            });

                                            Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                            Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(Killer, reason));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public static IEnumerator get_players_detection()
        {
            RaycastHit raycastHit = new RaycastHit();
            while (true)
            {
                // Don't rape the CPU server
                yield return new WaitForSeconds(1f);

                foreach (SteamPlayer list in Provider.clients)
                {
                    UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(list);
                    RocketPlayer rocketPlayer = new RocketPlayer(unturnedPlayer.Id);
                    var reason = "";


                    // Prevent cheaters to use his custom salvage time
                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.anti_salvage_time_override)
                    {
                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
                        {
                            unturnedPlayer.Player.interact.tellSalvageTimeOverride(Provider.server, 10f);
                        }
                    }

                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.noclip_detection)
                    {
                        reason = "No-Clip";

                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass") && !unturnedPlayer.IsInVehicle)
                        {
                            // Ray trace from ground?
                            if (!Physics.Raycast(unturnedPlayer.Position, Vector3.down, out raycastHit, 2048f, RayMasks.RESOURCE | RayMasks.LARGE | RayMasks.MEDIUM | RayMasks.SMALL | RayMasks.GROUND | RayMasks.GROUND2))
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"{reason})"); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been punished for '<color=red>{reason}</color>'".Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(unturnedPlayer, reason));
                            }
                        }
                    }

                    if (Darkness_Anti_Cheat.Instance.Configuration.Instance.clumsy_detect_fake_lag)
                    {
                        reason = "Fake-Lag";
                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
                        {
                            // it's using fake lag?
                            if (250 <= Darkness_Anti_Cheat_Functions.GetPlayerPing(unturnedPlayer) && unturnedPlayer.Player.input.IsUnderFakeLagPenalty)
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"({reason})"); // lets take a screenshot and generate auto report

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been punished for '<color=red>{reason}/color>'".Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");

                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(unturnedPlayer, reason));
                            }
                        }
                    }
                   
                    if(Darkness_Anti_Cheat.Instance.Configuration.Instance.anti_free_cam)
                    {
                        reason = "Free-Cam";

                        if (!unturnedPlayer.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
                        {
                            // Prevent cheaters to use freemcam, anyways is this obsolete, but we use it
                            unturnedPlayer.Player.look.tellFreecamAllowed(Provider.server, false);

                            // Player without permissions it's using freecam?
                            if (unturnedPlayer.Player.look.isOrbiting || unturnedPlayer.Player.look.isTracking)
                            {
                                Darkness_Anti_Cheat_Functions.send_report(null, unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook, $"({reason})"); // lets take a screenshot and generate auto report

                                Darkness_Anti_Cheat_Functions.webhook_logs(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.logs, $"({reason})");

                                if (Darkness_Anti_Cheat.Instance.Configuration.Instance.global_chat)
                                    ChatManager.serverSendMessage($"<color=#2391DE>[DAC]</color> {unturnedPlayer.DisplayName} has been punished for '<color=red>{reason}color>'".Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.GLOBAL, "https://darknesscommunity.club/assets/plugins/images/server/anticheat.png");


                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(unturnedPlayer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.execute(unturnedPlayer, reason));
                            }
                        }
                    }

                    // Just doing some test with vehicle no-clip, getting distance to detect if is insane of a wall
                    /*if (Physics.Raycast(unturnedPlayer.Position, unturnedPlayer.Player.look.aim.forward, out hit, 100, RayMasks.BARRICADE | RayMasks.LARGE | RayMasks.MEDIUM | RayMasks.SMALL | RayMasks.STRUCTURE | RayMasks.RESOURCE))
                    {
                        if (hit.distance <= 0.0)
                        {
                            UnturnedChat.Say($"Detected");
                        }
                        else
                        {
                            UnturnedChat.Say($"Vehicle-noclip: {hit.distance}");
                        }
                    }*/

                    unturnedPlayer = null;
                    raycastHit = new RaycastHit();
                }
            }
        }

        [Obsolete]
        public static void take_items_through_walls(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P)
        {
            RocketPlayer rocketPlayer = new RocketPlayer(player.Id);

            if (!player.IsAdmin && !rocketPlayer.HasPermission("*") && !rocketPlayer.HasPermission("dac_bypass"))
            {
                RaycastHit hit;

                // I'm working on it
                /*if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out hit, 2.5f, RayMasks.ITEM | RayMasks.PLAYER_INTERACT))
                {

                }*/
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

                Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(player, Darkness_Anti_Cheat.Instance.Configuration.Instance.abuse_player_webhook)); // lets take a screenshot and generate auto report
            }
        }

        // auto report system
        public static void auto_report(CSteamID killer, UnturnedPlayer player, ELimb limb)
        {
            UnturnedPlayer Killer = UnturnedPlayer.FromCSteamID(killer) ?? null;
            if (Killer != null && player != null && limb.ToString() != "")
            {
                // check headshots
                if (limb == ELimb.SKULL)
                    Killer.Player.GetComponent<PlayerComponent>().Headshots++;

                // add a kill
                Killer.Player.GetComponent<PlayerComponent>().Kills++;

                // reset the murder kills
                player.Player.GetComponent<PlayerComponent>().Kills = 0;
                player.Player.GetComponent<PlayerComponent>().Headshots = 0;

                // if the killer has 100% of headshot, send the report
                if (Killer.Player.GetComponent<PlayerComponent>().Kills >= Darkness_Anti_Cheat.Instance.Configuration.Instance.player_kills_alert && 
                    (Killer.Player.GetComponent<PlayerComponent>().Headshots / (Darkness_Anti_Cheat.Instance.Configuration.Instance.player_kills_alert / 2)) * 100 == 100)
                {
                    ThreadPool.QueueUserWorkItem((yes) =>
                    {
                        Darkness_Anti_Cheat_Functions.send_report(null, Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook); // lets take a screenshot and generate auto report
                    });

                    // reset the killer kills
                    Killer.Player.GetComponent<PlayerComponent>().Kills = 0;
                    Killer.Player.GetComponent<PlayerComponent>().Headshots = 0;

                    Darkness_Anti_Cheat.Instance.StartCoroutine(Darkness_Anti_Cheat_Functions.screenshot(Killer, Darkness_Anti_Cheat.Instance.Configuration.Instance.auto_reports_webhook)); // lets take a screenshot and generate auto report
                }
            }
        }
    }
}
