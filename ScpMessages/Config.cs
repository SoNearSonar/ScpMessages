using System.ComponentModel;

namespace ScpMessages
{
    public class Config
    {
        [Description("If the plugin should or should not be loaded on a server")]
        public bool EnableScpMessages { get; set; } = true;

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
    }
}
