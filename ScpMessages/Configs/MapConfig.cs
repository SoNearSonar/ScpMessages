using System.ComponentModel;

namespace ScpMessages.Configs
{
    public class MapConfig
    {
        [Description("The message that is displayed when a player bypasses the lock on an item (door, locker, and generator)")]
        public string BypassLockMesage { get; set; } = "The keycard reader was bypassed";

        [Description("The message that is displayed when a player cannot open a locked door")]
        public string DoorLockedMessage { get; set; } = "A keycard is required to operate this door";

        [Description("The message that is displayed when a player cannot open a locked door with the current keycard they have")]
        public string DoorLockedKeycardMessage { get; set; } = "A keycard with a higher clearance is required to operate this door";

        [Description("The message that is displayed when a player opens a locked door")]
        public string DoorUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player cannot open a fully locked down door")]
        public string DoorFullLockedMessage { get; set; } = "This door is completely locked down";

        [Description("The message that is displayed when a player cannot open a locker")]
        public string LockerLockedMessage { get; set; } = "A keycard is required to operate this locker";

        [Description("The message that is displayed when a player cannot open a locker with the current keycard they have")]
        public string LockerLockedKeycardMessage { get; set; } = "A keycard with a higher clearance is required to operate this locker";

        [Description("The message that is displayed when a player opens a locker")]
        public string LockerUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player opens a generator")]
        public string GeneratorUnlockedMessage { get; set; } = "The keycard was scanned on the panel";

        [Description("The message that is displayed when a player uses an elevator")]
        public string ElevatorUsedMessage { get; set; } = "You pressed the elevator button";
    }
}
