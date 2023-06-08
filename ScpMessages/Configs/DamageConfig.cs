using System.Collections.Generic;
using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class DamageConfig
    {

        [Description("The message that is displayed when a player attacks another player with a firearm")]
        public string FirearmDamageDealtHuman { get; set; } = "You shot %player on the %hitbox";

        [Description("The message that is displayed when a player is attacked by a human with a firearm")]
        public string FirearmDamageReceivedHuman { get; set; } = "You were shot by %player on the %hitbox";

        [Description("The message that is displayed when a player attacks another player with an explosive")]
        public string ExplosiveDamageDealtHuman { get; set; } = "You hit %player with an explosive on their %hitbox";

        [Description("The message that is displayed when a player is attacked by a human with an explosive")]
        public string ExplosiveDamageReceivedHuman { get; set; } = "You were hit by an explosive from %player on the %hitbox";

        [Description("The message that is displayed when a player attacks another player with the jailbird")]
        public string JailbirdDamageDealtHuman { get; set; } = "You hit %player on the %hitbox";

        [Description("The message that is displayed when a player is attacked by a human with the jailbird")]
        public string JailbirdDamageReceivedHuman { get; set; } = "You were hit by %player on the %hitbox";

        [Description("The message that is displayed when a player attacks an SCP entity")]
        public string AttackDamageDealtScp { get; set; } = "You attacked %player";

        [Description("The message that is displayed when a SCP entity is attacked by a human")]
        public string AttackDamageReceivedScp { get; set; } = "You were attacked by %player";

        [Description("The name for the hitboxes on a playermodel (Left = Hitbox name, Right = Translated name (Change this part))")]
        public Dictionary<string, string> HitboxTranslations { get; set; } = new Dictionary<string, string>()
        {
            { "HEADSHOTS", "head" },
            { "BODY", "body" },
            { "LIMB", "legs" }
        };
    }
}
