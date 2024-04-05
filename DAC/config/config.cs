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

        public bool global_chat;

        public bool aimbot_detection;

        public bool noclip_detection;

        //public bool proxy_detection;

        //public bool trust_detection;

        public bool abuse_detection;

        public string auto_reports_webhook;

        public string report_player_webhook;

        public string abuse_player_webhook;

        public string logs;

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
            abuse_detection = true;
            aimbot_detection = true;
            noclip_detection = true;
            global_chat = true;
        }
    }
}