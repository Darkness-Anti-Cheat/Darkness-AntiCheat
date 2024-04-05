using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        public int Kills { get; set; }
        public int Deaths { get; set; } // not used
        public int Headshots { get; set; } 
    }
}
