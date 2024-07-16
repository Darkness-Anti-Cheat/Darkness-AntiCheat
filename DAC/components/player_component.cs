using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkness_Anti_Cheat.components
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        public int Kills { get; set; }
        public int Deaths { get; set; } // not used
        public int Headshots { get; set; }
        public int Rate { get; set; } // Using if detect several times, but we only using this in anti aim section
        public int RateAim { get; set; }
    }
}
