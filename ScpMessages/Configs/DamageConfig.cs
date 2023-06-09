using System.Collections.Generic;
using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class DamageConfig
    {
        [Description("The message that is displayed when a player attacks another player with a firearm")]
        public string FirearmDamageDealtHuman { get; set; } = "You shot %player in the %hitbox";

        [Description("The message that is displayed when a player is attacked by a human with a firearm")]
        public string FirearmDamageReceivedHuman { get; set; } = "You were shot by %player in the %hitbox";

        [Description("The message that is displayed when a player attacks another player with an explosive")]
        public string ExplosiveDamageDealtHuman { get; set; } = "You hit %player with an explosive";

        [Description("The message that is displayed when a player is attacked by a human with an explosive")]
        public string ExplosiveDamageReceivedHuman { get; set; } = "You were hit by an explosive from %player";

        [Description("The message that is displayed when a player attacks another player with the Jailbird")]
        public string JailbirdDamageDealtHuman { get; set; } = "You hit %player in the %hitbox";

        [Description("The message that is displayed when a player is attacked by a human with the Jailbird")]
        public string JailbirdDamageReceivedHuman { get; set; } = "You were hit by %player in the %hitbox";

        [Description("The message that is displayed when a player attacks another player with the Micro HID")]
        public string MicroHidDamageDealtHuman { get; set; } = "You zapped %player";

        [Description("The message that is displayed when a player is attacked by a human with the Micro HID")]
        public string MicroHidDamageReceivedHuman { get; set; } = "You were zapped by %player";

        [Description("The message that is displayed when a player attacks another player with the 3-X Particle Disruptor")]
        public string DisruptorDamageDealtHuman { get; set; } = "You vaporized %player";

        [Description("The message that is displayed when a player is attacked by a human with the 3-X Particle Disruptor")]
        public string DisruptorDamageReceivedHuman { get; set; } = "You were vaporized by %player";

        [Description("The message that is displayed when a player attacks another player with SCP-018")]
        public string Scp018DamageDealtHuman { get; set; } = "You hit %player in the %hitbox with SCP-018";

        [Description("The message that is displayed when a player is attacked by a human with SCP-018")]
        public string Scp018DamageReceivedHuman { get; set; } = "You were hit in the %hitbox by %player throwing SCP-018";

        [Description("The message that is displayed when SCP-049 attacks a player")]
        public string Scp049DamageDealtHuman { get; set; } = "You made contact with %player";

        [Description("The message that is displayed when a player is attacked by SCP-096")]
        public string Scp049DamageReceivedHuman { get; set; } = "You made contact with SCP-049 (%player)";

        [Description("The message that is displayed when SCP-049 attacks a player")]
        public string Scp096DamageDealtHuman { get; set; } = "You attacked %player";

        [Description("The message that is displayed when a player is attacked by SCP-096")]
        public string Scp096DamageReceivedHuman { get; set; } = "You were attacked by SCP-096 (%player)";

        [Description("The message that is displayed when SCP-939 attacks a player")]
        public string Scp939DamageDealtHuman { get; set; } = "You sliced %player";

        [Description("The message that is displayed when a player is attacked by SCP-939")]
        public string Scp939DamageReceivedHuman { get; set; } = "You were sliced by SCP-939 (%player)";

        [Description("The message that is displayed when any other SCP (excluding 049 and 096) attacks a player")]
        public string ScpDamageDealtHuman { get; set; } = "You attacked %player";

        [Description("The message that is displayed when a player is attacked by any other SCP (excluding 049 and 096)6")]
        public string ScpDamageReceivedHuman { get; set; } = "You were attacked by SCP-096 (%player)";

        [Description("The message that is displayed when a player attacks an SCP entity")]
        public string AttackDamageDealtScp { get; set; } = "You attacked %player";

        [Description("The message that is displayed when a SCP entity is attacked by a human")]
        public string AttackDamageReceivedScp { get; set; } = "You were attacked by %role (%player)";

        [Description("The name for the hitboxes on a playermodel (Left = Hitbox name, Right = Translated name (Change this part))")]
        public Dictionary<string, string> HitboxTranslations { get; set; } = new Dictionary<string, string>()
        {
            { "HEADSHOT", "head" },
            { "BODY", "body" },
            { "LIMB", "legs" }
        };
    }
}
