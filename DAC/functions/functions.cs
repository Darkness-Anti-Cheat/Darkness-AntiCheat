using Newtonsoft.Json.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DAC
{
    public class Darkness_Anti_Cheat_Functions
    {
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
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(to_player.SteamPlayer()).SteamProfile.AvatarFull.AbsoluteUri);
            objEmbed.Add("title", "Logs Darkness Anti Cheat");
            objEmbed.Add("color", int.Parse(color, NumberStyles.HexNumber));
            objEmbed.Add("author", objAuthor);

            JArray arrFields = new JArray();
            arrFields.Add(new JObject { { "name", "From Player" }, { "value", player == null ? "System" : $"[{player.DisplayName}](http://steamcommunity.com/profiles/" + player.CSteamID + ")" }, { "inline", true } });

            if(!admin)
            {
                arrFields.Add(new JObject { { "name", "Kills" }, { "value", player.Player.GetComponent<PlayerComponent>().Kills }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "Deaths" }, { "value", player.Player.GetComponent<PlayerComponent>().Deaths }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "Headshots" }, { "value", player.Player.GetComponent<PlayerComponent>().Headshots }, { "inline", true } });
                arrFields.Add(new JObject { { "name", "KDA" }, { "value", (player.Player.GetComponent<PlayerComponent>().Kills / (double)player.Player.GetComponent<PlayerComponent>().Deaths) }, { "inline", true } });
            }
            else
            {
                arrFields.Add(new JObject { { "name", "Command Executed" }, { "value", command }, { "inline", true } });
            }

            arrFields.Add(new JObject { { "name", "Location" }, { "value", player.Position.ToString() }, { "inline", true } });
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
            objAuthor.Add("icon_url", UnturnedPlayer.FromSteamPlayer(player.SteamPlayer()).SteamProfile.AvatarFull.AbsoluteUri);
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
