using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
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

        [PluginEntryPoint("ScpMessages", Version, "Displays messages based on player interactions", "SoNearSonar")]
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
                if (ply.IsBypassEnabled)
                {
                    ply.SendHintToPlayer(Config.BypassLockMesage);
                }
                else
                {
                    ply.SendHintToPlayer(Config.DoorUnlockedMessage);
                }
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
                if (ply.IsBypassEnabled)
                {
                    ply.SendHintToPlayer(Config.BypassLockMesage);
                }
                else
                {
                    ply.SendHintToPlayer(Config.LockerUnlockedMessage);
                }
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        bool OnPlayerUnlockGenerator(Player ply, Scp079Generator generator)
        {
            if (!Config.EnableScpMessages || ply.IsSCP)
            {
                return true;
            }

            if (ply.IsBypassEnabled)
            {
                ply.SendHintToPlayer(Config.BypassLockMesage);
            }
            else
            {
                ply.SendHintToPlayer(Config.GeneratorUnlockedMessage);
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        bool OnPlayerUsedItem(Player ply, ItemBase item)
        {
            if (!Config.EnableScpMessages || ply.IsSCP)
            {
                return true;
            }
            
            switch (item.Category)
            {
                case ItemCategory.Medical:
                    switch (item.ItemTypeId)
                    {
                        case ItemType.Painkillers:
                            ply.SendHintToPlayer(Config.PainkillerUsedMessage);
                            break;
                        case ItemType.Medkit:
                            ply.SendHintToPlayer(Config.MedkitUsedMessage);
                            break;
                        case ItemType.Adrenaline:
                            ply.SendHintToPlayer(Config.AdrenalineUsedMessage);
                            break;
                    }
                    break;
                case ItemCategory.SCPItem:
                    switch (item.ItemTypeId)
                    {
                        case ItemType.SCP207:
                            ply.SendHintToPlayer(Config.Scp207UsedMessage);
                            break;
                        case ItemType.SCP268:
                            ply.SendHintToPlayer(Config.Scp268UsedMessage);
                            break;
                        case ItemType.SCP500:
                            ply.SendHintToPlayer(Config.Scp500UsedMessage);
                            break;
                        case ItemType.SCP1853:
                            ply.SendHintToPlayer(Config.Scp1853UsedMessage);
                            break;
                    }
                    break;
            }
            return true;
        }
    }
}
