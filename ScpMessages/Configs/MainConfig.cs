using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class MainConfig
    {
        [Description("If the plugin should or should not be loaded on a server")]
        public bool EnableScpMessages { get; set; } = true;
    }
}
