using ScpMessages.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class MainConfig
    {
        [Description("If the plugin should or should not be loaded on a server")]
        public bool EnableScpMessages { get; set; } = true;

        [Description("If the plugin should or should not play messages when users take/deal damage")]
        public bool EnableDamageMessages { get; set; } = true;

        [Description("If the plugin should or should not play messages when users use/throw/toss items")]
        public bool EnableItemMessages { get; set; } = true;

        [Description("If the plugin should or should not play messages when users try to use doors/lockers/generators")]
        public bool EnableMapMessages { get; set; } = true;

        [Description("If the plugin should or should not play messages when users try to use doors/lockers/generators")]
        public bool EnableTeamRespawnMessages { get; set; } = true;

        [Description("If the plugin should or should not play broadcast messages when users join the server")]
        public bool EnableBroadcastMessages { get; set; } = true;

        [Description("The broadcast messages that are played when the user has ScpMessages on/off or has Do Not Track on")]
        public Dictionary<string, ScpMessageBroadcast> BroadcastMessages { get; set; } = new Dictionary<string, ScpMessageBroadcast>()
        {
            { 
                "enabled_for_player", 
                new ScpMessageBroadcast
                (
                    "ScpMessages is on for you, you will see messages at the bottom of your screen when you do certain actions\nFor usage, do <color=orange>.scpmsg</color> in your console (tilde (~) key)",
                    15
                )
            },
            {
                "disabled_for_player",
                new ScpMessageBroadcast
                (
                    "ScpMessages is off for you, you will not see messages at the bottom of your screen when you do certain actions\nFor usage, do <color=orange>.scpmsg</color> in your console (tilde (~) key)",
                    15
                )
            }
        };
    }
}
