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
    }
}
