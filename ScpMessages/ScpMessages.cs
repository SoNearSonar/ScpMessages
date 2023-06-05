using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace ScpMessages
{

    public class ScpMessages
    {
        public static ScpMessages Instance { get; private set; }

        [PluginConfig] public Config Config;

        private const string Version = "1.0.0";

        [PluginEntryPoint("ScpMessages", Version, "Displays SCP:CB style messages when certain actions are done", "SoNearSonar")]
        void LoadPlugin()
        {
            Instance = this;
            EventManager.RegisterEvents(this);
        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        bool OnPlayerInteractDoor(Player ply, DoorVariant door, bool canOpen)
        {
            if (!Config.EnableScpMessages || door.RequiredPermissions.RequiredPermissions == 0 || ply.IsSCP)
            {
                return true;
            }

            if (!canOpen)
            {
                if (ply.CurrentItem is KeycardItem && !door.RequiredPermissions.CheckPermissions(ply.CurrentItem, ply.ReferenceHub))
                {
                    ply.SendHintToPlayer(Config.DoorLockedKeycardMessage);
                }
                else
                {
                    ply.SendHintToPlayer(Config.DoorLockedMessage);
                }
            }
            else
            {
                ply.SendHintToPlayer(Config.DoorUnlockedMessage);
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        bool OnPlayerInteractLocker(Player ply, Locker locker, LockerChamber lockerChamber, bool canOpen)
        {
            if (!Config.EnableScpMessages || lockerChamber.RequiredPermissions == 0 || ply.IsSCP)
            {
                return true;
            }

            if (!canOpen)
            {
                if (ply.CurrentItem is KeycardItem && lockerChamber.RequiredPermissions > (ply.CurrentItem as KeycardItem).Permissions)
                {
                    ply.SendHintToPlayer(Config.LockerLockedKeycardMessage);
                }
                else
                {
                    ply.SendHintToPlayer(Config.LockerLockedMessage);
                }
            }
            else
            {
                ply.SendHintToPlayer(Config.LockerUnlockedMessage);
            }
            return true;
        }
    }
}
