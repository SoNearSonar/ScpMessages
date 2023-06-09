﻿using CustomPlayerEffects;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration.Distributors;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using ScpMessages.Configs;
using System;
using UnityEngine;

namespace ScpMessages
{
    public class ScpMessages
    {
        public static ScpMessages Instance { get; private set; }

        [PluginConfig("main_config.yml")] 
        public MainConfig MainConfig;

        [PluginConfig("item_config.yml")]
        public ItemConfig ItemConfig;

        [PluginConfig("map_config.yml")]
        public MapConfig MapConfig;

        [PluginConfig("damage_config.yml")]
        public DamageConfig DamageConfig;

        private const string Version = "1.0.0";

        [PluginEntryPoint("ScpMessages", Version, "Displays messages based on player interactions", "SoNearSonar")]
        void LoadPlugin()
        {
            if (!MainConfig.EnableScpMessages)
            {
                return;
            }

            Instance = this;
            EventManager.RegisterEvents(this);
            Harmony Harmory = new Harmony("ScpMessages");
            Harmory.PatchAll();
        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        bool OnPlayerInteractDoor(Player ply, DoorVariant door, bool canOpen)
        {
            if (!MainConfig.EnableMapMessages || door.RequiredPermissions.RequiredPermissions == 0 || ply.IsSCP)
            {
                return true;
            }

            if (!canOpen)
            {
                if (ply.CurrentItem is KeycardItem && !door.RequiredPermissions.CheckPermissions(ply.CurrentItem, ply.ReferenceHub))
                {
                    ply.SendHintToPlayer(MapConfig.DoorLockedKeycardMessage);
                }
                else
                {
                    if (door.ActiveLocks > 0)
                    {
                        ply.SendHintToPlayer(MapConfig.DoorFullLockedMessage);
                    }
                    else
                    {
                        ply.SendHintToPlayer(MapConfig.DoorLockedMessage);
                    }
                }
            }
            else
            {
                if (ply.IsBypassEnabled)
                {
                    ply.SendHintToPlayer(MapConfig.BypassLockMesage);
                }
                else
                {
                    ply.SendHintToPlayer(MapConfig.DoorUnlockedMessage);
                }
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        bool OnPlayerInteractLocker(Player ply, Locker locker, LockerChamber lockerChamber, bool canOpen)
        {
            if (!MainConfig.EnableMapMessages || lockerChamber.RequiredPermissions == 0 || ply.IsSCP)
            {
                return true;
            }

            if (!canOpen)
            {
                if (ply.CurrentItem is KeycardItem keycard && (lockerChamber.RequiredPermissions > keycard.Permissions))
                {
                    ply.SendHintToPlayer(MapConfig.LockerLockedKeycardMessage);
                }
                else
                {
                    ply.SendHintToPlayer(MapConfig.LockerLockedMessage);
                }
            }
            else
            {
                if (ply.IsBypassEnabled)
                {
                    ply.SendHintToPlayer(MapConfig.BypassLockMesage);
                }
                else
                {
                    ply.SendHintToPlayer(MapConfig.LockerUnlockedMessage);
                }
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        bool OnPlayerUnlockGenerator(Player ply, Scp079Generator generator)
        {
            if (!MainConfig.EnableMapMessages || ply.IsSCP)
            {
                return true;
            }

            if (ply.IsBypassEnabled)
            {
                ply.SendHintToPlayer(MapConfig.BypassLockMesage);
            }
            else
            {
                ply.SendHintToPlayer(MapConfig.GeneratorUnlockedMessage);
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        bool OnPlayerUsedItem(Player ply, ItemBase item)
        {
            if (!MainConfig.EnableItemMessages || ply.IsSCP)
            {
                return true;
            }

            switch (item.ItemTypeId)
            {
                case ItemType.Painkillers:
                    ply.SendHintToPlayer(ItemConfig.PainkillerUsedMessage);
                    break;
                case ItemType.Medkit:
                    ply.SendHintToPlayer(ItemConfig.MedkitUsedMessage);
                    break;
                case ItemType.Adrenaline:
                    ply.SendHintToPlayer(ItemConfig.AdrenalineUsedMessage);
                    break;
                case ItemType.SCP330:
                    ply.SendHintToPlayer(ItemConfig.Scp330CandyUsedMessage);
                    break;
                case ItemType.SCP207:
                    ply.SendHintToPlayer(ItemConfig.Scp207UsedMessage);
                    break;
                case ItemType.SCP268:
                    ply.SendHintToPlayer(ItemConfig.Scp268UsedMessage);
                    break;
                case ItemType.SCP500:
                    ply.SendHintToPlayer(ItemConfig.Scp500UsedMessage);
                    break;
                case ItemType.SCP1853:
                    ply.SendHintToPlayer(ItemConfig.Scp1853UsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        bool OnPlayerThrowProjectile(Player ply, ThrowableItem item, ThrowableItem.ProjectileSettings projectileSettings, bool fullForce)
        {
            if (!MainConfig.EnableItemMessages || ply.IsSCP)
            {
                return true;
            }

            switch (item.ItemTypeId)
            {
                case ItemType.GrenadeHE:
                    ply.SendHintToPlayer(ItemConfig.GrenadeUsedMessage);
                    break;
                case ItemType.GrenadeFlash:
                    ply.SendHintToPlayer(ItemConfig.FlashGrenadeUsedMessage);
                    break;
                case ItemType.SCP018:
                    ply.SendHintToPlayer(ItemConfig.Scp018UsedMessage);
                    break;
                case ItemType.SCP2176:
                    ply.SendHintToPlayer(ItemConfig.Scp2176UsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowItem)]
        bool OnPlayerThrowItem(Player ply, ItemBase item, Rigidbody rigidBody)
        {
            if (!MainConfig.EnableItemMessages || ply.IsSCP)
            {
                return true;
            }

            Tuple<string, object>[] pairs = new Tuple<string, object>[]
            {
                new Tuple<string, object>("item", item.ItemTypeId.ToString())
            };
            ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.ItemTossed, '%', pairs));

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractScp330)]
        bool OnPlayerInteractScp330(Player ply)
        {
            if (!MainConfig.EnableItemMessages)
            {
                return true;
            }

            ply.SendHintToPlayer(ItemConfig.Scp330CandyPickedUpMessage);
            return true;
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        bool OnPlayerDamage(Player ply, Player attacker, DamageHandlerBase damageHandler)
        {
            if (!MainConfig.EnableDamageMessages  || ply == null || attacker == null)
            {
                return true;
            }

            // Create a list to hold all the tokens to replace (Then replace items in their respective index slot)
            // Order: 0 (Player), 1 (Hitbox), 2 (Damage), 3 (Role)
            Tuple<string, object>[] humanPair = new Tuple<string, object>[4];
            humanPair[0] = new Tuple<string, object>("player", ply.Nickname);
            humanPair[3] = new Tuple<string, object>("role", attacker.Role.ToString());

            switch (damageHandler)
            {
                case FirearmDamageHandler fiHandler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[fiHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", fiHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.FirearmDamageDealt, DamageConfig.FirearmDamageReceived);
                    break;
                case ExplosionDamageHandler exHandler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[exHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", exHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.ExplosiveDamageDealt, DamageConfig.ExplosiveDamageReceived);
                    break;
                case MicroHidDamageHandler micHandler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[micHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", micHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.MicroHidDamageDealt, DamageConfig.MicroHidDamageReceived);
                    break;
                case JailbirdDamageHandler jaHandler:
                    if (ply != attacker) // Swinging/using the jailbird on nothing damages the player for 0 HP (Likely a bug)
                    {
                        humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[jaHandler.Hitbox.ToString().ToUpperInvariant()]);
                        humanPair[2] = new Tuple<string, object>("damage", jaHandler.DealtHealthDamage);
                        SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.JailbirdDamageDealt, DamageConfig.JailbirdDamageReceived);
                    }
                    break;
                case DisruptorDamageHandler diHandler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[diHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", diHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.DisruptorDamageDealt, DamageConfig.DisruptorDamageReceived);
                    break;
                case Scp018DamageHandler scp018Handler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp018Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", scp018Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp018DamageDealt, DamageConfig.Scp018DamageReceived);
                    break;
                case Scp049DamageHandler scp049Handler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp049Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", scp049Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp049DamageDealt, DamageConfig.Scp049DamageReceived);
                    break;
                case Scp096DamageHandler scp096Handler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp096Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", scp096Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp096DamageDealt, DamageConfig.Scp096DamageReceived);
                    break;
                case Scp939DamageHandler scp939Handler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp939Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", scp939Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp939DamageDealt, DamageConfig.Scp939DamageReceived);
                    break;
                case ScpDamageHandler scpHandler:
                    humanPair[1] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scpHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[2] = new Tuple<string, object>("damage", scpHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.ScpDamageDealt, DamageConfig.ScpDamageReceived);
                    break;
            }

            return true;
        }

        void SendDamageMessagesToPlayers(Player ply, Player attacker, Tuple<string, object>[] pair, string dealtMessage, string receivedMessage)
        {
            attacker.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(dealtMessage, '%', pair));
            pair[0] = new Tuple<string, object>("player", attacker.Nickname);
            ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(receivedMessage, '%', pair));
        }
    }
}
