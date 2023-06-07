using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration.Distributors;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using UnityEngine;

namespace ScpMessages
{
    public class ScpMessages
    {
        public static ScpMessages Instance { get; private set; }

        [PluginConfig] 
        public Config Config;

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
                if (ply.CurrentItem is KeycardItem keycard && (lockerChamber.RequiredPermissions > keycard.Permissions))
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
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        bool OnPlayerThrowProjectile(Player ply, ThrowableItem item, ThrowableItem.ProjectileSettings projectileSettings, bool fullForce)
        {
            if (!Config.EnableScpMessages || ply.IsSCP)
            {
                return true;
            }

            switch (item.ItemTypeId)
            {
                case ItemType.GrenadeHE:
                    ply.SendHintToPlayer(Config.GrenadeUsedMessage);
                    break;
                case ItemType.GrenadeFlash:
                    ply.SendHintToPlayer(Config.FlashGrenadeUsedMessage);
                    break;
                case ItemType.SCP018:
                    ply.SendHintToPlayer(Config.Scp018UsedMessage);
                    break;
                case ItemType.SCP2176:
                    ply.SendHintToPlayer(Config.Scp2176UsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowItem)]
        bool OnPlayerThrowItem(Player ply, ItemBase item, Rigidbody rigidBody)
        {
            if (!Config.EnableScpMessages || ply.IsSCP)
            {
                return true;
            }

            Tuple<string, object>[] pairs = new Tuple<string, object>[]
            {
                new Tuple<string, object>("item", item.ItemTypeId.ToString())
            };
            ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(Config.ItemTossed, '%', pairs));

            return true;
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        bool OnPlayerDamage(Player ply, Player attacker, DamageHandlerBase damageHandler)
        {
            if (!Config.EnableScpMessages || ply == null || attacker == null)
            {
                return true;
            }

            // Create a list to hold all the tokens to replace (Then replace items in their respective index slot)
            // Order: 0 (Hitbox), 1 (Player), 2 (Damage)
            Tuple<string, object>[] humanPair = new Tuple<string, object>[3];
            humanPair[1] = new Tuple<string, object>("player", ply.Nickname);

            if (ply.IsHuman)
            {
                if (damageHandler is FirearmDamageHandler fiHandler)
                {
                    humanPair[0] = new Tuple<string, object>("hitbox", fiHandler.Hitbox.ToString().ToUpperInvariant());
                    humanPair[2] = new Tuple<string, object>("damage", fiHandler.DealtHealthDamage);
                }
                else if (damageHandler is ExplosionDamageHandler exHandler)
                {
                    humanPair[0] = new Tuple<string, object>("hitbox", exHandler.Hitbox.ToString().ToUpperInvariant());
                    humanPair[2] = new Tuple<string, object>("damage", exHandler.DealtHealthDamage);
                }
                else if (damageHandler is MicroHidDamageHandler micHandler)
                {
                    humanPair[0] = new Tuple<string, object>("hitbox", micHandler.Hitbox.ToString().ToUpperInvariant());
                    humanPair[2] = new Tuple<string, object>("damage", micHandler.DealtHealthDamage);
                }
                else if (damageHandler is JailbirdDamageHandler jaHandler)
                {
                    humanPair[0] = new Tuple<string, object>("hitbox", jaHandler.Hitbox.ToString().ToUpperInvariant());
                    humanPair[2] = new Tuple<string, object>("damage", jaHandler.DealtHealthDamage);
                }

                attacker.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(Config.BulletDamageDealtHuman, '%', humanPair));

                humanPair[1] = new Tuple<string, object>("player", attacker.Nickname);

                ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(Config.BulletDamageReceivedHuman, '%', humanPair));
            }
            else
            {
                if (damageHandler is FirearmDamageHandler handler)
                {
                    attacker.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(Config.BulletDamageDealtSCP, '%', humanPair));

                    humanPair[1] = new Tuple<string, object>("player", attacker.Nickname);

                    ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(Config.BulletDamageReceivedSCP, '%', humanPair));
                }
            }

            return true;
        }
    }
}
