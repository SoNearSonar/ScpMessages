using System.Collections.Generic;
using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class ItemConfig
    {
        [Description("The message that is displayed when a player equips light armor")]
        public string LightArmorUsedMessage { get; set; } = "You put on some light armor";

        [Description("The message that is displayed when a player equips combat armor")]
        public string CombatArmorUsedMessage { get; set; } = "You put on some combat armor";

        [Description("The message that is displayed when a player equips heavy armor")]
        public string HeavyArmorUsedMessage { get; set; } = "You put on some heavy armor";

        [Description("The message that is displayed when a player throws a grenade")]
        public string GrenadeUsedMessage { get; set; } = "You threw a grenade";

        [Description("The message that is displayed when a player throws a flash grenade")]
        public string FlashGrenadeUsedMessage { get; set; } = "You threw a flash grenade";

        [Description("The message that is displayed when a player uses painkillers")]
        public string PainkillerUsedMessage { get; set; } = "You swallowed the painkillers";

        [Description("The message that is displayed when a player uses a first aid (med) kit")]
        public string MedkitUsedMessage { get; set; } = "You used a medkit";

        [Description("The message that is displayed when a player uses an adrenaline shot")]
        public string AdrenalineUsedMessage { get; set; } = "You used an adrenaline shot";

        [Description("The message that is displayed when a player throws SCP-018 (a throwable item used for destruction of environments & players)")]
        public string Scp018UsedMessage { get; set; } = "You threw SCP-018";

        [Description("The message that is displayed when a player consumes a SCP-330 candy")]
        public string Scp330CandyUsedMessage { get; set; } = "You consumed a SCP-330 candy";

        [Description("The message that is displayed when a player uses SCP-207 (a consumable item used for boosting move speed)")]
        public string Scp207UsedMessage { get; set; } = "You used SCP-207";

        [Description("The message that is displayed when a player uses SCP-268 (a wearable item used for stealth)")]
        public string Scp268UsedMessage { get; set; } = "You used SCP-268";

        [Description("The message that is displayed when a player uses SCP-500 (a medical item that can cure anything)")]
        public string Scp500UsedMessage { get; set; } = "You used SCP-500";

        [Description("The message that is displayed when a player uses SCP-1576 (an item which lets you talk to spectators while you are alive)")]
        public string Scp1576UsedMessage { get; set; } = "You used SCP-1576";

        [Description("The message that is displayed when a player uses SCP-1853 (a consumable item used for boosting interaction speed)")]
        public string Scp1853UsedMessage { get; set; } = "You used SCP-1853";

        [Description("The message that is displayed when a player throws SCP-2176 (a throwable item used for room interference)")]
        public string Scp2176UsedMessage { get; set; } = "You threw SCP-2176";

        [Description("The message that is displayed when a player picks up candy from SCP-330 (a bowl with candies that grant various effects)")]
        public string Scp330CandyPickedUpMessage { get; set; } = "You picked up a %color candy from SCP-330";

        [Description("The message that is displayed when a player picks up a pink candy from SCP-330 (Not part of natural gameplay)")]
        public string Scp330PinkCandyPickedUpMessage { get; set; } = "You picked up a pink candy from SCP-330";

        [Description("The message that is displayed when a player picks up more than two candies from SCP-330")]
        public string Scp330CandyPickedUpTooManyMessage { get; set; } = "Your hands got severed at the wrist";

        [Description("The message that is displayed when a player tosses an item from their inventory (Using the T key (default))")]
        public string ItemTossed { get; set; } = "You tossed a %item";

        [Description("The message that is displayed when a player drops an item from their inventory")]
        public string ItemDropped { get; set; } = "You dropped a %item";

        [Description("The message that is displayed when a player picks up an item")]
        public string ItemPickedUp { get; set; } = "You picked up a %item";

        [Description("The message that is displayed when a player drops ammo")]
        public string AmmoDropped { get; set; } = "You dropped %amount ammo of %item";

        [Description("The message that is displayed when a player picks up ammo")]
        public string AmmoPickedUp { get; set; } = "You picked up a box of %item ammo";

        [Description("The name of each candy color for a candy from SCP-330 (Left = Candy color name, Right = Translated name (Change this part))")]
        public Dictionary<string, string> Scp330CandyTranslations { get; set; } = new Dictionary<string, string>()
        {
            { "RAINBOW", "rainbow" },
            { "YELLOW", "yellow" },
            { "PURPLE", "purple" },
            { "RED", "red" },
            { "GREEN", "green" },
            { "BLUE", "blue" },
            { "NONE", "none" },
        };
    }
}
