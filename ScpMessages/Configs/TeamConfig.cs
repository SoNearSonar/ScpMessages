using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class TeamConfig
    {
        [Description("The message that is displayed when a Nine Tailed Fox mobile task force team spawns in")]
        public string NineTailedFoxSpawnMessage { get; set; } = "You spawned as a Mobile Task Force operative under team %team";

        [Description("The message that is displayed when a Chaos Insurgency team spawns in")]
        public string ChaosInsurgencySpawnMessage { get; set; } = "You spawned as a Chaos Insurgency operative";
    }
}
