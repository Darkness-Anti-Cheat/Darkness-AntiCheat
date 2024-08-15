using Darkness_Anti_Cheat.components;
using Newtonsoft.Json.Linq;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DAC
{
    public class Darkness_Anti_Cheat_Functions
    {
        // Future Usage
        public enum keys : int
        {
            Unknown = -1,
            Jump = 0,
            LPunch = 1,
            RPunch = 2,
            Crouch = 3,
            Prone = 4,
            Sprint = 5,
            Leanleft = 6,
            LeanRight = 7,
            HoldBreath = 9,
            // Plugin keys
            CodeHotkey1 = 10,
            CodeHotkey2 = 11,
            CodeHotkey3 = 12,
            CodeHotkey4 = 13
        }

        public static UnityEngine.Vector3 PiVector = new UnityEngine.Vector3(0, Mathf.PI, 0);
        public static UnityEngine.Vector3 GetHitbox(Transform target, ELimb hitbox)
        {
            if (target == null)
                return PiVector;

            Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();

            if (componentsInChildren == null)
                return PiVector;

            Transform[] array = componentsInChildren;

            for (int i = 0; i < array.Length; i++)
            {
                Transform tr = array[i];
                if (tr.name == hitbox.ToString())
                    return tr.position + new UnityEngine.Vector3(0f, 0.4f, 0f);
            }

            return PiVector;
        }

        public static void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[Darkness_Anti_Cheat] {message}");
        }

        public static float CalculateDistance(UnturnedPlayer killer, UnturnedPlayer victim)
        {
            UnityEngine.Vector3 killerPosition = killer.Player.transform.position;
            UnityEngine.Vector3 victimPosition = victim.Player.transform.position;

            float distance = UnityEngine.Vector3.Distance(killerPosition, victimPosition);

            return distance;
        }

        public static float GetPlayerPing(UnturnedPlayer killer)
        {
            return (float)Math.Round((float)killer.Ping * 1000);
        }

        [Obsolete]
        public static bool IsPlayerVisible(UnturnedPlayer player1, UnturnedPlayer player2)
        {
            RaycastHit hit;
            if (Physics.Raycast(player1.Player.transform.position, player2.Player.transform.position - player1.Player.transform.position, out hit))
            {
                return hit.collider.GetComponent<UnturnedPlayer>() == player2;
            }
            return false;
        }

        [Obsolete]
        public UnturnedPlayer FindClosestPlayer(UnturnedPlayer targetPlayer)
        {
            UnturnedPlayer closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var otherPlayer in Provider.clients.Select(c => UnturnedPlayer.FromSteamPlayer(c)))
            {
                if (otherPlayer != targetPlayer)
                {
                    float distance = UnityEngine.Vector3.Distance(targetPlayer.Position, otherPlayer.Position);

                    if (distance < closestDistance)
                    {
                        closestPlayer = otherPlayer;
                        closestDistance = distance;
                    }
                }
            }

            return closestPlayer;
        }

        // Reverse from random cheat
        public static void GetItemsInRadius(UnturnedPlayer player, out List<InteractableItem> items, int RegionReach, float MaxMagnitude)
        {
            items = new List<InteractableItem>();

            List<ItemRegion> regionsInRadius = new List<ItemRegion>();

            for (int x = 0; x < 64; x++)
                for (int y = 0; y < 64; y++)
                    if (ItemManager.regions[x, y] != null)
                        if (ItemManager.regions[x, y].drops.Count > 0)
                            if ((x <= player.Player.movement.region_x && y <= player.Player.movement.region_y && (player.Player.movement.region_x - x) <= RegionReach && (player.Player.movement.region_y - y) <= RegionReach) ||
                            (x >= player.Player.movement.region_x && y >= player.Player.movement.region_y && (x - player.Player.movement.region_x) <= RegionReach && (y - player.Player.movement.region_y) <= RegionReach))
                                regionsInRadius.Add(ItemManager.regions[x, y]);

            foreach (ItemRegion x in regionsInRadius)
                foreach (ItemDrop y in x.drops)
                    if (!y.interactableItem.checkInteractable() || !y.interactableItem.checkUseable())
                        continue;
                    else if ((y.interactableItem.transform.position - player.Player.transform.position).sqrMagnitude < MaxMagnitude)
                        items.Add(y.interactableItem);
        }

        public static byte[] ByteArray(string imagePath)
        {
            byte[] imageByteArray = null;
            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                imageByteArray = new byte[reader.BaseStream.Length];
                for (int i = 0; i < reader.BaseStream.Length; i++)
                    imageByteArray[i] = reader.ReadByte();
            }
            return imageByteArray;
        }

        public string get_ip_address_server()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString("http://icanhazip.com/");
            }
        }

        public static string os()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "//" : "/";
        }

        public static JObject send_report(UnturnedPlayer player, UnturnedPlayer to_player, string uri, string reason = "The player may be using third-party software", bool admin = false, string command = "", string color = "0085EA")
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            objAuthor.Add("name", to_player.SteamPlayer().player.name);
            objAuthor.Add("url", "http://steamcommunity.com/profiles/" + to_player.CSteamID);
            objEmbed.Add("title", "Logs Darkness Anti Cheat");
            objEmbed.Add("color", int.Parse(color, NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);

            JArray arrFields = new JArray();
            arrFields.Add(new JObject { { "name", "From Player" }, { "value", player == null ? "System" : $"[{player.DisplayName}](http://steamcommunity.com/profiles/" + player.CSteamID + ")" }, { "inline", true } });
            if(!admin)
            {
                arrFields.Add(new JObject { { "name", "Kills" }, { "value", to_player.Player.GetComponent<PlayerComponent>().Kills }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "Deaths" }, { "value", to_player.Player.GetComponent<PlayerComponent>().Deaths }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "Headshots" }, { "value", to_player.Player.GetComponent<PlayerComponent>().Headshots }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "KDA" }, { "value", (to_player.Player.GetComponent<PlayerComponent>().Kills / (double)to_player.Player.GetComponent<PlayerComponent>().Deaths) }, { "inline", true } });
            }
            else
            {
                arrFields.Add(new JObject { { "name", "Command Executed" }, { "value", command }, { "inline", true } });
            }

            arrFields.Add(new JObject { { "name", "Location" }, { "value", to_player.Position.ToString() }, { "inline", true } });
            arrFields.Add(new JObject { { "name", "Map" }, { "value", Provider.map }, { "inline", true } });
            arrFields.Add(new JObject { { "name", "Date" }, { "value", DateTime.UtcNow.ToString() }, { "inline", true } });
            arrFields.Add(new JObject { { "name", "Reason" }, { "value", reason }, { "inline", false } });
            objEmbed.Add("fields", arrFields);

            arrEmbeds.Add(objEmbed);

            obj.Add("username", "Darkness Anti Cheat");
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            using (WebClient web = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
                web.Headers[HttpRequestHeader.ContentType] = "application/json";
                web.UploadString(new Uri(uri), obj.ToString(Newtonsoft.Json.Formatting.None));
            }

            return obj;
        }

        public static JObject webhook_logs(UnturnedPlayer player, string uri, string reason = "Cheating")
        {
            JObject obj = new JObject();
            JArray arrEmbeds = new JArray();
            JObject objEmbed = new JObject();
            JObject objAuthor = new JObject();

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            objAuthor.Add("name", player.SteamPlayer().player.name);
            objAuthor.Add("url", "http://steamcommunity.com/profiles/" + player.CSteamID);
            objEmbed.Add("title", $"Kicked for {reason}");
            objEmbed.Add("color", int.Parse("0085EA", NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);

            arrEmbeds.Add(objEmbed);

            obj.Add("username", "Darkness Anti Cheat");
            obj.Add("tts", false);
            obj.Add("embeds", arrEmbeds);

            using (WebClient web = new WebClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
                web.Headers[HttpRequestHeader.ContentType] = "application/json";
                web.UploadString(new Uri(uri), obj.ToString(Newtonsoft.Json.Formatting.None));
            }

            return obj;
        }
        // we use this function, cause the screenshots can do some delay to get it
        public static IEnumerator execute(UnturnedPlayer player, string reason)
        {
            yield return new WaitForSeconds(5.0f);
            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.ban)
            {
                player.Ban($"[DAC] You have been banned for using ({reason})", Darkness_Anti_Cheat.Instance.Configuration.Instance.ban_seconds);
            }

            if (Darkness_Anti_Cheat.Instance.Configuration.Instance.kick)
            {
                player.Kick($"[DAC] You have been kicked for using ({reason})");
            }
        }

        public static IEnumerator screenshot(UnturnedPlayer player, string uri)
        {
            player.Player.sendScreenshot((CSteamID)0);
            player.Player.sendScreenshot((CSteamID)0);

            string moveToDir = System.Environment.CurrentDirectory;
            string spyFolderDir = "";

            yield return new WaitForSeconds(1.0f);

            Rocket.Core.Logging.Logger.Log(moveToDir);

            moveToDir = moveToDir.Remove(moveToDir.LastIndexOf($"|Rocket".Replace("|", os())), 7);
            spyFolderDir = moveToDir + "|Spy|".Replace("|", os());

            Rocket.Core.Logging.Logger.Log(spyFolderDir);

            using (HttpClient httpClient = new HttpClient())
            {
                var imagePath = "";

                imagePath = spyFolderDir + player.CSteamID.ToString() + ".jpg";

                var file_bytes = ByteArray(imagePath);
                var file = player.CSteamID.ToString() + ".jpg";

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "file", file);
                var responde = httpClient.PostAsync(new Uri(uri), form).Result;

                if (responde.IsSuccessStatusCode)
                {
                    Rocket.Core.Logging.Logger.Log("[Darkness_Anti_Cheat] A report has been sent automatically");
                }
            }
        }
    }
}
