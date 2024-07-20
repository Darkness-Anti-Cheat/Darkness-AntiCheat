[![Join in our discord](https://discordapp.com/api/guilds/869187450159923221/widget.png)](https://discord.gg/wWtjUcvXQp)
[![Download latest release here](https://img.shields.io/badge/download-latest_release-brightgreen.svg?maxAge=2592000)](https://github.com/Darkness-Anti-Cheat/Darkness-AntiCheat/releases/)
<div align="center">
    <h1>Server Side Anti Cheat</h1>
    <img height="150px;" src="https://darknesscommunity.club/assets/plugins/images/server/anticheat.png"></img>
</div>

<h1>Features</h1>

You can have a reporting system that generate <b>spy images</b>, where it will send you detailed information via <b>discord webhook</b>, detect any type of <b>silent aim, detect</b> <b>no-clip</b> or <b>glitching</b>, send statistics of <b>suspicious players</b>, detect <b>admin abuse</b> automatically.
<div align="center">
    <img height="500px;" src=".github\image-examples\example.png"></img>
</div>
<div align="center">
    <p>Some proofs of testings with real Silent-Aimbot</p>
    https://www.youtube.com/embed/hzJNutQ1Lms?si=FkRRyAlj_WwEONwR
    <p>Detecting Melee, Punch override distance</p>
    https://www.youtube.com/watch?v=wo3vEtpbGfU
</div>

<h1>What can detect?</h1>

- Silent-Aim <b>(Aimbot, Calculate KDA Players with auto report system, check for punch, melee distance override)</b>
- Anti-Aim
- No-Clip
- Fake-Lag <b>(Clumsy or other programs) (beta feature)</b>
- Admin Abuse <b>(Send automatically commands that type administrators)</b>
- Anti-Free Cam

<h1>Installation and requirements</h1>

- rocketmod
- netstandard

<h1>Permissions & Commands</h1>

- /report <b>(player)</b> <b>(reason)</b> - Permission: report
- Permission: dac_bypass <b>(Bypass)</b>

<h3>Important things</h3>

```
<?xml version="1.0" encoding="utf-8"?>
<Configuration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <abuse_detection>true</abuse_detection>
  <auto_reports_webhook></auto_reports_webhook> <--- WEBHOOK DISCORD
  <report_player_webhook></report_player_webhook> <--- WEBHOOK DISCORD
  <abuse_player_webhook></abuse_player_webhook> <--- WEBHOOK DISCORD
  <logs></logs> <--- WEBHOOK DISCORD
  <aimbot_detection>true</aimbot_detection>
  <aimbot_detection_rate>3</aimbot_detection_rate> <--- HOW MANY TIMES NEED FOR BAN, KICK
  <player_kills_alert>20</player_kills_alert> <--- REPORT A PLAYER WITH X KILLS, JUST A REPORT TO DISCORD, NOT BAN OR KICK
  <punch_override_distance_detection>true</punch_override_distance_detection>
  <punch_distance_rate>3</punch_distance_rate> <--- HOW MANY TIMES NEED FOR BAN, KICK
  <noclip_detection>true</noclip_detection>
  <noclip_detection_rate>3</noclip_detection_rate> <--- HOW MANY TIMES NEED FOR BAN, KICK
  <anti_free_cam>true</anti_free_cam>
  <anti_aim_detection>true</anti_aim_detection>
  <player_ping_high>true</player_ping_high>
  <clumsy_detect_fake_lag>true</clumsy_detect_fake_lag>
  <player_ping>180</player_ping> <-- If player has that ping, aimbot and other detections will stop, due can do false positives
  <take_items_through_walls_detection>true</take_items_through_walls_detection> <--- NOT WORKING
  <kick>true</kick> <--- KICK THE USER
  <ban>false</ban> <--- BAN THE USER
  <ban_seconds>65000</ban_seconds>
  <global_chat>true</global_chat> <--- GLOBAL CHAT WHEN A PLAYER GET PUNISHED
  <CommandEntries>
    <CommandEntry name="god" aliases="" />  <--- COMMANDS AND ALIASES FOR GETTING ADMIN ABUSE REPORTS
    <CommandEntry name="vanish" aliases="" />
    <CommandEntry name="kill" aliases="" />
    <CommandEntry name="wreck" aliases="" />
  </CommandEntries>
</Configuration>
```

## License

Open Source Non-Commercial License (Version 1.0)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

This license is not intended for commercial use. Any commercial use, including but not limited to selling or distributing the Software for commercial purposes, is prohibited. Users of the Software are required to attribute the author(s) of the original Software.




