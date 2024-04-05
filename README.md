<div align="center">
    <h1>Server Side Anti Cheat</h1>
    <img src="https://static.wikia.nocookie.net/unturned-bunker/images/3/3c/Beacon_1194.png/revision/latest/scale-to-width-down/128?cb=20160617225641"></img>
</div>

<h1>Features</h1>

You can have a reporting system that generate <b>spy images</b>, where it will send you detailed information via <b>discord webhook</b>, detect any type of <b>silent aim, detect</b> <b>no-clip</b> or <b>glitching</b>, send statistics of <b>suspicious players</b>, detect <b>admin abuse</b> automatically.
<div align="center">
    <img height="500px;" src=".github\image-examples\example.png"></img>
</div>
<h1>Installation and requirements</h1>

- rocketmod
- netstandard

<h3>Important things</h3>

```
    <global_chat>true</global_chat> # Send message to global chat, when someone gets banned
    <aimbot_detection>true</aimbot_detection>
    <noclip_detection>true</noclip_detection>
    <abuse_detection>true</abuse_detection>
    <auto_reports_webhook>--DISCORD-WEBHOOK--</auto_reports_webhook>
    <report_player_webhook>--DISCORD-WEBHOOK--</report_player_webhook>
    <abuse_player_webhook>--DISCORD-WEBHOOK--</abuse_player_webhook>
    <logs>--DISCORD-WEBHOOK--</logs>
    <CommandEntries> # Commands you want to detect, admin abuse
        <CommandEntry name="god" aliases="gd,g" />
        <CommandEntry name="vanish" aliases="" />
        <CommandEntry name="kill" aliases="" />
        <CommandEntry name="wreck" aliases="" />
    </CommandEntries>
```

## License

Open Source Non-Commercial License (Version 1.0)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

This license is not intended for commercial use. Any commercial use, including but not limited to selling or distributing the Software for commercial purposes, is prohibited. Users of the Software are required to attribute the author(s) of the original Software.




