using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static DAC.Darkness_Anti_Cheat_Functions;

namespace Darkness_Anti_Cheat.components
{
    public class player_inputs : UnturnedPlayerComponent
    {
        // I will do this in other moment, but the logic i want to do here, if player press INSERT or F1, why he using those keys?, so annoying right?¿
        // So lets target those players, because probably they are opening a menu of a cheat or we don't know

        // Also wanna use this thing for block his menu and prevent changing settings: 
        // player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
        // player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

        /*public void FixedUpdate()
        {
            UnturnedKey Key;

            bool[] Inputs = Player.Player.input.keys;

            if (Player.Player.input.keys)
        }*/
    }
}
