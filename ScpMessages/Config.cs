using System.ComponentModel;

namespace ScpMessages
{
    public class Config
    {
        [Description("If the plugin should or should not be loaded on a server")]
        public bool EnableScpMessages { get; set; } = true;

        [Description("The message that is displayed when a player bypasses the lock on an item (door, locker, and generator)")]
        public string BypassLockMesage { get; set; } = "The keycard reader was bypassed";

        [Description("The message that is displayed when a player cannot open a locked door")]
        public string DoorLockedMessage { get; set; } = "A keycard is required to operate this door";

        [Description("The message that is displayed when a player cannot open a locked door with the current keycard they have")]
        public string DoorLockedKeycardMessage { get; set; } = "A keycard with a higher clearance is required to operate this door";

        [Description("The message that is displayed when a player opens a locked door")]
        public string DoorUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player cannot open a locker")]
        public string LockerLockedMessage { get; set; } = "A keycard is required to operate this locker";

        [Description("The message that is displayed when a player cannot open a locker with the current keycard they have")]
        public string LockerLockedKeycardMessage { get; set; } = "A keycard with a higher clearance is required to operate this locker";

        [Description("The message that is displayed when a player opens a locker")]
        public string LockerUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player opens a generator")]
        public string GeneratorUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player uses painkillers")]
        public string PainkillerUsedMessage { get; set; } = "You swallowed the painkillers";

        [Description("The message that is displayed when a player uses a first aid (med) kit")]
        public string MedkitUsedMessage { get; set; } = "You used a medkit";

        [Description("The message that is displayed when a player uses an adrenaline shot")]
        public string AdrenalineUsedMessage { get; set; } = "You used an adrenaline shot";

        [Description("The message that is displayed when a player uses SCP-207 (a consumable item used for boosting move speed)")]
        public string Scp207UsedMessage { get; set; } = "You used SCP-207";

        [Description("The message that is displayed when a player uses SCP-268 (a throwable item used for door interference)")]
        public string Scp268UsedMessage { get; set; } = "You used SCP-268";

        [Description("The message that is displayed when a player uses SCP-500 (a medical item that can cure anything)")]
        public string Scp500UsedMessage { get; set; } = "You used SCP-500";

        [Description("The message that is displayed when a player uses SCP-1853 (a consumable item used for boosting interaction speed)")]
        public string Scp1853UsedMessage { get; set; } = "You used SCP-1853";
    }
}
