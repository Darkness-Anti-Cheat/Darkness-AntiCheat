using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DAC
{
    public struct AbuseCommands
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("aliases")]
        public string Aliases;
        public string[] GetAliases() => (from i in Aliases.Split(',') select i.Trim()).ToArray();
        public AbuseCommands(string Name, params string[] Aliases)
        {
            this.Name = Name;
            this.Aliases = string.Join(",", Aliases);
        }
    }

    public class Configuration : IRocketPluginConfiguration, IDefaultable
    {
        public bool player_ping_high;

        public bool global_chat;

        public bool aimbot_detection;

        public bool noclip_detection;

        public bool anti_free_cam;

        public bool clumsy_detect_fake_lag;

        public bool anti_aim_detection;

        public int player_ping;

        public int player_kills_alert;

        //public bool proxy_detection;

        //public bool trust_detection;

        public bool abuse_detection;

        public string auto_reports_webhook;

        public string report_player_webhook;

        public string abuse_player_webhook;

        public string logs;

        public int aimbot_detection_rate;

        public int punch_distance_rate;

        [XmlArray("CommandEntries"), XmlArrayItem("CommandEntry")]
        public AbuseCommands[] Entries;

        public void LoadDefaults()
        {
            // Logs commands
            Entries = new AbuseCommands[]
            {
                new AbuseCommands("god"),
                new AbuseCommands("vanish"),
                new AbuseCommands("kill"),
                new AbuseCommands("wreck"),
            };

            auto_reports_webhook = "";

            report_player_webhook = "";

            abuse_player_webhook = "";

            logs = "";
            //proxy_detection = false;
            //trust_detection = false;

            player_ping = 150;
            player_ping_high = true;
            clumsy_detect_fake_lag = true;
            anti_aim_detection = true;
            player_kills_alert = 20;
            anti_free_cam = true;
            abuse_detection = true;
            aimbot_detection = true;
            aimbot_detection_rate = 3;
            punch_distance_rate = 3;
            noclip_detection = true;
            global_chat = true;
        }
    }
}