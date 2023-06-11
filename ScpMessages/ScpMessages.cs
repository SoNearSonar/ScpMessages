using CustomPlayerEffects;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration.Distributors;
using Newtonsoft.Json;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Helpers;
using ScpMessages.Configs;
using ScpMessages.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static RoundSummary;

namespace ScpMessages
{
    public class ScpMessages
    {
        public static ScpMessages Instance { get; private set; }
        public Dictionary<string, IndividualUserToggleChoice> ToggleScpMessages;

        [PluginConfig("main_config.yml")] 
        public MainConfig MainConfig;

        [PluginConfig("item_config.yml")]
        public ItemConfig ItemConfig;

        [PluginConfig("map_config.yml")]
        public MapConfig MapConfig;

        [PluginConfig("damage_config.yml")]
        public DamageConfig DamageConfig;

        private const string Version = "1.0.0";
        private readonly string toggleDir = Path.Combine(Paths.Configs, "ScpMessages", "Stored");


        [PluginEntryPoint("ScpMessages", Version, "Displays messages based on player interactions", "SoNearSonar")]
        void LoadPlugin()
        {
            if (!MainConfig.EnableScpMessages)
            {
                return;
            }

            Instance = this;
            EventManager.RegisterEvents(this);
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        bool OnWaitingForPlayers()
        {
            try
            {
                string filePath = Path.Combine(toggleDir, "toggles.json");
                if (!Directory.Exists(toggleDir))
                {
                    Directory.CreateDirectory(toggleDir);
                }

                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }

                ToggleScpMessages = JsonConvert.DeserializeObject<Dictionary<string, IndividualUserToggleChoice>>(File.ReadAllText(filePath));
                if (ToggleScpMessages == null)
                {
                    ToggleScpMessages = new Dictionary<string, IndividualUserToggleChoice>();
                }
            }
            catch (Exception)
            {
                Log.Error("There was an error trying to read the playerbase toggle preferences, using default of enabled for everyone. The plugin could be loaded for the first time!", "ScpMessages");
                ToggleScpMessages = new Dictionary<string, IndividualUserToggleChoice>();
            }

            return true;
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        bool OnRoundEnd(LeadingTeam leadingTeam)
        {
            SaveTogglesToFile();
            return true;
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        bool OnRoundRestart()
        {
            // Just in case a restart was forced
            SaveTogglesToFile();
            return true;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        bool OnPlayerJoined(Player ply)
        {
            if (ply.DoNotTrack)
            {
                if (ToggleScpMessages.ContainsKey(ply.UserId))
                {
                    ToggleScpMessages.Remove(ply.UserId);
                }

                ply.SendBroadcast("ScpMessages is on this server which displays messages at the bottom of your screen. You have set your account to not be tracked so messages are disabled", 15);
                return true;
            }

            if (!ToggleScpMessages.ContainsKey(ply.UserId))
            {
                ToggleScpMessages[ply.UserId] = new IndividualUserToggleChoice();
            }

            if (ToggleScpMessages[ply.UserId].EnableScpMessages)
            {
                ply.SendBroadcast("ScpMessages is on for you, you will see messages at the bottom of your screen when you do certain actions\nFor usage, do <color=orange>.scpmsg</color> in your console (tilde (~) key)", 15);
            }
            else
            {
                ply.SendBroadcast("ScpMessages is off for you, you will not see messages at the bottom of your screen when you do certain actions\nFor usage, do <color=orange>.scpmsg</color> in your console (tilde (~) key)", 15);
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerGameConsoleCommand)]
        bool OnConsoleCommandUsed(Player ply, string command, string[] arguments)
        {
            if (command.ToLowerInvariant().Equals("scpmsg"))
            {
                if (ply.DoNotTrack)
                {
                    ply.SendConsoleMessage("[ScpMessages] You cannot enable/disable messages because your account has do not track enabled. Messages are disabled by default", "red");
                    return false;
                }


                string additionalText = string.Empty;
                if (!ToggleScpMessages[ply.UserId].EnableScpMessages)
                {
                    additionalText = ", however you have all interactions disabled so toggling this interaction type will have no effect";
                }

                switch (arguments.Length)
                {
                    case 1:
                        switch (arguments[0].ToLowerInvariant())
                        {
                            case "all":
                                if (!MainConfig.EnableScpMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] All messages cannot be toggled as they are disabled for all users", "red");
                                    break;
                                }

                                ToggleScpMessages[ply.UserId].EnableScpMessages = !ToggleScpMessages[ply.UserId].EnableScpMessages;
                                if (ToggleScpMessages[ply.UserId].EnableScpMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] All messages will display at the bottom for interactions");
                                }
                                else
                                {
                                    ply.SendConsoleMessage("[ScpMessages] All messages will not display at the bottom for interactions", "red");
                                }
                                break;
                            case "damage":
                                if (!MainConfig.EnableDamageMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Damage messages cannot be toggled as they are disabled for all users", "red");
                                    break;
                                }

                                ToggleScpMessages[ply.UserId].EnableDamageMessages = !ToggleScpMessages[ply.UserId].EnableDamageMessages;
                                if (ToggleScpMessages[ply.UserId].EnableDamageMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Damage messages will display at the bottom for interactions" + additionalText);
                                }
                                else
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Damage messages will not display at the bottom for interactions" + additionalText, "red");
                                }
                                break;
                            case "item":
                                if (!MainConfig.EnableItemMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Item messages cannot be toggled as they are disabled for all users", "red");
                                    break;
                                }

                                ToggleScpMessages[ply.UserId].EnableItemMessages = !ToggleScpMessages[ply.UserId].EnableItemMessages;
                                if (ToggleScpMessages[ply.UserId].EnableItemMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Item messages will display at the bottom for interactions" + additionalText);
                                }
                                else
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Item messages will not display at the bottom for interactions" + additionalText, "red");
                                }
                                break;
                            case "map":
                                if (!MainConfig.EnableMapMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Map messages cannot be toggled as they are disabled for all users", "red");
                                    break;
                                }

                                ToggleScpMessages[ply.UserId].EnableMapMessages = !ToggleScpMessages[ply.UserId].EnableMapMessages;
                                if (ToggleScpMessages[ply.UserId].EnableMapMessages)
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Map messages will display at the bottom for interactions" + additionalText);
                                }
                                else
                                {
                                    ply.SendConsoleMessage("[ScpMessages] Map messages will not display at the bottom for interactions" + additionalText, "red");
                                }
                                break;
                            case "list":
                                StringBuilder builder1 = StringBuilderPool.Shared.Rent();
                                builder1.Append(Environment.NewLine + "[ScpMessages] These are the messages that are toggled for you:" + Environment.NewLine);
                                builder1.Append("- Plugin messages (all): " + ToggleScpMessages[ply.UserId].EnableScpMessages + Environment.NewLine);
                                builder1.Append("- Damage messages (damage): " + ToggleScpMessages[ply.UserId].EnableDamageMessages + Environment.NewLine);
                                builder1.Append("- Item messages (item): " + ToggleScpMessages[ply.UserId].EnableItemMessages + Environment.NewLine);
                                builder1.Append("- Map messages (map): " + ToggleScpMessages[ply.UserId].EnableMapMessages + Environment.NewLine);
                                ply.SendConsoleMessage(builder1.ToString(), "yellow");
                                break;
                            case "help":
                                StringBuilder builder2 = StringBuilderPool.Shared.Rent();
                                builder2.Append(Environment.NewLine + "[ScpMessages] These are the commands you can do" + Environment.NewLine);
                                builder2.Append("- .scpmsg all (Toggle all plugin messages)" + Environment.NewLine);
                                builder2.Append("- .scpmsg damage (Toggle damage messages)" + Environment.NewLine);
                                builder2.Append("- .scpmsg item (Toggle item messages)" + Environment.NewLine);
                                builder2.Append("- .scpmsg map (Toggle map messages)" + Environment.NewLine);
                                builder2.Append("- .scpmsg list (List all message toggles on your account)" + Environment.NewLine);
                                builder2.Append("- .scpmsg help (Displays this message again)" + Environment.NewLine);
                                ply.SendConsoleMessage(builder2.ToString(), "yellow");
                                break;
                            default:
                                ply.SendConsoleMessage(Environment.NewLine + "[ScpMessages] Usage: .scpmsg (all, damage, item, map, list, help)", "yellow");
                                break;
                        }
                        break;
                    default:
                        ply.SendConsoleMessage(Environment.NewLine + "[ScpMessages] Usage: .scpmsg (all, damage, item, map, list, help)", "yellow");
                        break;

                }

                return false;
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        bool OnPlayerInteractDoor(Player ply, DoorVariant door, bool canOpen)
        {
            if (!MainConfig.EnableMapMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId)
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableMapMessages
                || door.RequiredPermissions.RequiredPermissions == 0 
                || ply.IsSCP)
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
            if (!MainConfig.EnableMapMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId)
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableMapMessages 
                || lockerChamber.RequiredPermissions == 0 
                || ply.IsSCP)
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
            if (!MainConfig.EnableMapMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId)
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableMapMessages 
                || ply.IsSCP)
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

        [PluginEvent(ServerEventType.PlayerInteractElevator)]
        bool OnPlayerInteractElevator(Player ply, ElevatorChamber elevator)
        {
            if (!MainConfig.EnableMapMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableMapMessages 
                || ply == null)
            {
                return true;
            }

            Tuple<string, object>[] pairs = new Tuple<string, object>[]
            {
                new Tuple<string, object>("level", elevator.CurrentLevel)
            };
            ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(MapConfig.ElevatorUsedMessage, '%', pairs));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        bool OnPlayerUsedItem(Player ply, ItemBase item)
        {
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableItemMessages 
                || ply.IsSCP)
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

        [PluginEvent(ServerEventType.PlayerPickupArmor)]
        bool OnPlayerPickupArmor(Player ply, ItemPickupBase item)
        {
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableItemMessages 
                || ply.IsSCP)
            {
                return true;
            }

            switch (item.Info.ItemId)
            {
                case ItemType.ArmorLight:
                    ply.SendHintToPlayer(ItemConfig.LightArmorUsedMessage);
                    break;
                case ItemType.ArmorCombat:
                    ply.SendHintToPlayer(ItemConfig.CombatArmorUsedMessage);
                    break;
                case ItemType.ArmorHeavy:
                    ply.SendHintToPlayer(ItemConfig.HeavyArmorUsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        bool OnPlayerThrowProjectile(Player ply, ThrowableItem item, ThrowableItem.ProjectileSettings projectileSettings, bool fullForce)
        {
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableItemMessages 
                || ply.IsSCP)
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
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableItemMessages 
                || ply.IsSCP)
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
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId)
                || !ToggleScpMessages[ply.UserId].EnableScpMessages
                || !ToggleScpMessages[ply.UserId].EnableItemMessages)
            {
                return true;
            }
            
            ply.SendHintToPlayer(ItemConfig.Scp330CandyPickedUpMessage);
            return true;
        }

        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        bool OnPlayerReceiveEffect(Player ply, StatusEffectBase statusEffect, byte intensity, float duration)
        {
            if (!MainConfig.EnableItemMessages 
                || !ToggleScpMessages.ContainsKey(ply.UserId) 
                || !ToggleScpMessages[ply.UserId].EnableScpMessages 
                || !ToggleScpMessages[ply.UserId].EnableItemMessages)
            {
                return true;
            }

            switch (statusEffect)
            {
                case SeveredHands _:
                    ply.SendHintToPlayer(ItemConfig.Scp330CandyPickedUpTooManyMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        bool OnPlayerDamage(Player ply, Player attacker, DamageHandlerBase damageHandler)
        {
            if (!MainConfig.EnableDamageMessages || ply == null || attacker == null)
            {
                return true;
            }

            // Create a list to hold all the tokens to replace (Then replace items in their respective index slot)
            // Order: 0 (Player), 1 (Role), 2 (Hitbox), 3 (Damage)
            Tuple<string, object>[] humanPair = new Tuple<string, object>[4];
            humanPair[0] = new Tuple<string, object>("player", ply.Nickname);
            humanPair[1] = new Tuple<string, object>("role", attacker.Role.ToString());

            switch (damageHandler)
            {
                case FirearmDamageHandler fiHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[fiHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", fiHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.FirearmDamageDealt, DamageConfig.FirearmDamageReceived);
                    break;
                case ExplosionDamageHandler exHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[exHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", exHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.ExplosiveDamageDealt, DamageConfig.ExplosiveDamageReceived);
                    break;
                case MicroHidDamageHandler micHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[micHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", micHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.MicroHidDamageDealt, DamageConfig.MicroHidDamageReceived);
                    break;
                case JailbirdDamageHandler jaHandler:
                    if (ply != attacker) // Swinging/using the jailbird on nothing damages the player for 0 HP (Likely a bug)
                    {
                        humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[jaHandler.Hitbox.ToString().ToUpperInvariant()]);
                        humanPair[3] = new Tuple<string, object>("damage", jaHandler.DealtHealthDamage);
                        SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.JailbirdDamageDealt, DamageConfig.JailbirdDamageReceived);
                    }
                    break;
                case DisruptorDamageHandler diHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[diHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", diHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.DisruptorDamageDealt, DamageConfig.DisruptorDamageReceived);
                    break;
                case Scp018DamageHandler scp018Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp018Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp018Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp018DamageDealt, DamageConfig.Scp018DamageReceived);
                    break;
                case Scp049DamageHandler scp049Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp049Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp049Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp049DamageDealt, DamageConfig.Scp049DamageReceived);
                    break;
                case Scp096DamageHandler scp096Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp096Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp096Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp096DamageDealt, DamageConfig.Scp096DamageReceived);
                    break;
                case Scp939DamageHandler scp939Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp939Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp939Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.Scp939DamageDealt, DamageConfig.Scp939DamageReceived);
                    break;
                case ScpDamageHandler scpHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scpHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scpHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(ply, attacker, humanPair, DamageConfig.ScpDamageDealt, DamageConfig.ScpDamageReceived);
                    break;
            }

            return true;
        }


        void SendDamageMessagesToPlayers(Player ply, Player attacker, Tuple<string, object>[] pair, string dealtMessage, string receivedMessage)
        {
            if (ToggleScpMessages.ContainsKey(attacker.UserId) 
                && (ToggleScpMessages[attacker.UserId].EnableScpMessages 
                && ToggleScpMessages[attacker.UserId].EnableDamageMessages))
            {
                attacker.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(dealtMessage, '%', pair));
            }

            pair[0] = new Tuple<string, object>("player", attacker.Nickname);

            if (ToggleScpMessages.ContainsKey(ply.UserId) 
                && (ToggleScpMessages[attacker.UserId].EnableScpMessages 
                && ToggleScpMessages[attacker.UserId].EnableDamageMessages))
            {
                ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(receivedMessage, '%', pair));
            }
        }

        void SaveTogglesToFile()
        {
            try
            {
                string contents = JsonConvert.SerializeObject(ToggleScpMessages, Formatting.Indented);
                string filePath = Path.Combine(toggleDir, "toggles.json");
                File.WriteAllText(filePath, contents);
            }
            catch (Exception)
            {
                Log.Error("There was an error trying to save the playerbase toggle preferences", "ERROR");
            }
        }
    }
}
