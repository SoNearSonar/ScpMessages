using CustomPlayerEffects;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Usables.Scp330;
using Newtonsoft.Json;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Helpers;
using Respawning;
using ScpMessages.Configs;
using ScpMessages.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScpMessages
{
    public class ScpMessages
    {
        public static ScpMessages Instance { get; private set; }
        public Dictionary<string, IndividualUserPreferences> IndividualUserPreferences;
        public int TeamNamesAssigned = 0;
        public List<Player> PeopleRespawnedOnMTF = new List<Player>();
        public SpawnableTeamType NextKnownTeam;

        [PluginConfig("main_config.yml")] 
        public MainConfig MainConfig;

        [PluginConfig("item_config.yml")]
        public ItemConfig ItemConfig;

        [PluginConfig("map_config.yml")]
        public MapConfig MapConfig;

        [PluginConfig("damage_config.yml")]
        public DamageConfig DamageConfig;

        [PluginConfig("team_config.yml")]
        public TeamConfig TeamConfig;

        private const string Version = "2.1.0";
        private readonly string _toggleDir = Path.Combine(Paths.Configs, "ScpMessages", "Stored");
        private Harmony _harmony;


        [PluginEntryPoint("ScpMessages", Version, "Displays messages based on player interactions", "SoNearSonar")]
        void LoadPlugin()
        {
            if (!MainConfig.EnableScpMessages)
            {
                return;
            }

            Instance = this;
            EventManager.RegisterEvents(this);

            try
            {
                _harmony = new Harmony("com.sonearsonar.scpmessages");
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to patch all Harmony patches\n{e}", "ERROR");
                _harmony.UnpatchAll(_harmony.Id);
            }
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        bool OnWaitingForPlayers(WaitingForPlayersEvent args)
        {
            try
            {
                string filePath = Path.Combine(_toggleDir, "toggles.json");
                if (!Directory.Exists(_toggleDir))
                {
                    Directory.CreateDirectory(_toggleDir);
                }

                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }

                IndividualUserPreferences = JsonConvert.DeserializeObject<Dictionary<string, IndividualUserPreferences>>(File.ReadAllText(filePath));
                if (IndividualUserPreferences == null)
                {
                    IndividualUserPreferences = new Dictionary<string, IndividualUserPreferences>();
                }
            }
            catch (Exception)
            {
                Log.Error("There was an error trying to read the playerbase toggle preferences, using default of enabled for everyone. The plugin could be loaded for the first time!", "ScpMessages");
                IndividualUserPreferences = new Dictionary<string, IndividualUserPreferences>();
            }

            return true;
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        bool OnRoundEnd(RoundEndEvent args)
        {
            SaveTogglesToFile();
            return true;
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        bool OnRoundRestart(RoundRestartEvent args)
        {
            // Just in case a restart was forced
            SaveTogglesToFile();
            return true;
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        bool OnPlayerJoined(PlayerJoinedEvent args)
        {
            if (!IndividualUserPreferences.ContainsKey(args.Player.UserId))
            {
                IndividualUserPreferences[args.Player.UserId] = new IndividualUserPreferences();
            }

            if (MainConfig.EnableBroadcastMessages)
            {
                if (IndividualUserPreferences[args.Player.UserId].EnableScpMessages)
                {
                    args.Player.SendBroadcast(MainConfig.BroadcastMessages["enabled_for_player"].Message, MainConfig.BroadcastMessages["enabled_for_player"].Time);
                }
                else
                {
                    args.Player.SendBroadcast(MainConfig.BroadcastMessages["disabled_for_player"].Message, MainConfig.BroadcastMessages["disabled_for_player"].Time);
                }
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        bool OnPlayerInteractDoor(PlayerInteractDoorEvent args)
        {
            if (!MainConfig.EnableMapMessages 
                || !IndividualUserPreferences.ContainsKey(args.Player.UserId)
                || !IndividualUserPreferences[args.Player.UserId].EnableScpMessages 
                || !IndividualUserPreferences[args.Player.UserId].EnableMapMessages
                || args.Door.RequiredPermissions.RequiredPermissions == 0 
                || args.Player.IsSCP)
            {
                return true;
            }

            if (!args.CanOpen)
            {
                if (args.Door.ActiveLocks > 0)
                {
                    args.Player.SendHintToPlayer(MapConfig.DoorFullLockedMessage);
                    return true;
                }

                if (args.Player.CurrentItem is KeycardItem)
                {
                    args.Player.SendHintToPlayer(MapConfig.DoorLockedKeycardMessage);
                }
                else
                {
                    args.Player.SendHintToPlayer(MapConfig.DoorLockedMessage);
                }
            }
            else
            {
                if (args.Player.IsBypassEnabled)
                {
                    args.Player.SendHintToPlayer(MapConfig.BypassLockMesage);
                }
                else
                {
                    args.Player.SendHintToPlayer(MapConfig.DoorUnlockedMessage);
                }
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        bool OnPlayerInteractLocker(PlayerInteractLockerEvent args)
        {
            if (!MainConfig.EnableMapMessages 
                || !IndividualUserPreferences.ContainsKey(args.Player.UserId)
                || !IndividualUserPreferences[args.Player.UserId].EnableScpMessages 
                || !IndividualUserPreferences[args.Player.UserId].EnableMapMessages 
                || args.Chamber.RequiredPermissions == 0 
                || args.Player.IsSCP)
            {
                return true;
            }

            if (!args.CanOpen)
            {
                if (args.Player.CurrentItem is KeycardItem)
                {
                    args.Player.SendHintToPlayer(MapConfig.LockerLockedKeycardMessage);
                }
                else
                {
                    args.Player.SendHintToPlayer(MapConfig.LockerLockedMessage);
                }
            }
            else
            {
                if (args.Player.IsBypassEnabled)
                {
                    args.Player.SendHintToPlayer(MapConfig.BypassLockMesage);
                }
                else
                {
                    args.Player.SendHintToPlayer(MapConfig.LockerUnlockedMessage);
                }
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        bool OnPlayerUnlockGenerator(PlayerUnlockGeneratorEvent args)
        {
            if (!MainConfig.EnableMapMessages 
                || !IndividualUserPreferences.ContainsKey(args.Player.UserId)
                || !IndividualUserPreferences[args.Player.UserId].EnableScpMessages 
                || !IndividualUserPreferences[args.Player.UserId].EnableMapMessages 
                || args.Player.IsSCP)
            {
                return true;
            }

            if (args.Player.IsBypassEnabled)
            {
                args.Player.SendHintToPlayer(MapConfig.BypassLockMesage);
            }
            else
            {
                args.Player.SendHintToPlayer(MapConfig.GeneratorUnlockedMessage);
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractElevator)]
        bool OnPlayerInteractElevator(PlayerInteractElevatorEvent args)
        {
            if (!MainConfig.EnableMapMessages 
                || !IndividualUserPreferences.ContainsKey(args.Player.UserId) 
                || !IndividualUserPreferences[args.Player.UserId].EnableScpMessages 
                || !IndividualUserPreferences[args.Player.UserId].EnableMapMessages 
                || args.Player == null)
            {
                return true;
            }

            Tuple<string, object>[] pairs = new Tuple<string, object>[]
            {
                new Tuple<string, object>("level", args.Elevator.CurrentLevel)
            };
            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(MapConfig.ElevatorUsedMessage, '%', pairs));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        bool OnPlayerUsedItem(PlayerUsedItemEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            switch (args.Item.ItemTypeId)
            {
                case ItemType.Painkillers:
                    args.Player.SendHintToPlayer(ItemConfig.PainkillerUsedMessage);
                    break;
                case ItemType.Medkit:
                    args.Player.SendHintToPlayer(ItemConfig.MedkitUsedMessage);
                    break;
                case ItemType.Adrenaline:
                    args.Player.SendHintToPlayer(ItemConfig.AdrenalineUsedMessage);
                    break;
                case ItemType.SCP330:
                    args.Player.SendHintToPlayer(ItemConfig.Scp330CandyUsedMessage);
                    break;
                case ItemType.SCP207:
                    args.Player.SendHintToPlayer(ItemConfig.Scp207UsedMessage);
                    break;
                case ItemType.SCP268:
                    args.Player.SendHintToPlayer(ItemConfig.Scp268UsedMessage);
                    break;
                case ItemType.SCP500:
                    args.Player.SendHintToPlayer(ItemConfig.Scp500UsedMessage);
                    break;
                case ItemType.SCP1576:
                    args.Player.SendHintToPlayer(ItemConfig.Scp1576UsedMessage);
                    break;
                case ItemType.SCP1853:
                    args.Player.SendHintToPlayer(ItemConfig.Scp1853UsedMessage);
                    break;

            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerSearchedPickup)]
        bool OnPlayerPickedUpItem(PlayerSearchedPickupEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            Tuple<string, object>[] itemName = 
            {
                new Tuple<string, object>("item", args.Item.Info.ItemId) 
            };

            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.ItemPickedUp, '%', itemName));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerDropItem)]
        bool OnPlayerDropItem(PlayerDropItemEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            Tuple<string, object>[] itemName =
            {
                new Tuple<string, object>("item", args.Item.ItemTypeId)
            };

            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.ItemDropped, '%', itemName));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerPickupAmmo)]
        bool OnPlayerPickedUpAmmo(PlayerPickupAmmoEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            Tuple<string, object>[] itemName =
            {
                new Tuple<string, object>("item", args.Item.Info.ItemId)
            };

            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.AmmoPickedUp, '%', itemName));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerDroppedAmmo)]
        bool OnPlayerDroppedAmmo(PlayerDroppedAmmoEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player) || args.Player.Health <= 0)
                return true;

            Tuple<string, object>[] itemName =
            {
                new Tuple<string, object>("item", args.Item.Info.ItemId),
                new Tuple<string, object>("amount", args.Amount),
            };

            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.AmmoDropped, '%', itemName));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerPickupArmor)]
        bool OnPlayerPickupArmor(PlayerPickupArmorEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            switch (args.Item.Info.ItemId)
            {
                case ItemType.ArmorLight:
                    args.Player.SendHintToPlayer(ItemConfig.LightArmorUsedMessage);
                    break;
                case ItemType.ArmorCombat:
                    args.Player.SendHintToPlayer(ItemConfig.CombatArmorUsedMessage);
                    break;
                case ItemType.ArmorHeavy:
                    args.Player.SendHintToPlayer(ItemConfig.HeavyArmorUsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowProjectile)]
        bool OnPlayerThrowProjectile(PlayerThrowProjectileEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Thrower))
                return true;

            switch (args.Item.ItemTypeId)
            {
                case ItemType.GrenadeHE:
                    args.Thrower.SendHintToPlayer(ItemConfig.GrenadeUsedMessage);
                    break;
                case ItemType.GrenadeFlash:
                    args.Thrower.SendHintToPlayer(ItemConfig.FlashGrenadeUsedMessage);
                    break;
                case ItemType.SCP018:
                    args.Thrower.SendHintToPlayer(ItemConfig.Scp018UsedMessage);
                    break;
                case ItemType.SCP2176:
                    args.Thrower.SendHintToPlayer(ItemConfig.Scp2176UsedMessage);
                    break;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerThrowItem)]
        bool OnPlayerThrowItem(PlayerThrowItemEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            Tuple<string, object>[] pairs = new Tuple<string, object>[] 
            {
                new Tuple<string, object>("item", args.Item.ItemTypeId.ToString())
            };

            args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.ItemTossed, '%', pairs));
            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractScp330)]
        bool OnPlayerInteractScp330(PlayerInteractScp330Event args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;
            
            foreach (ItemBase item in args.Player.Items)
            {
                if (!(item is Scp330Bag bag))
                    continue;

                int candyCount = bag.Candies.Count - 1;
                CandyKindID id = bag.Candies[candyCount];

                if (id != CandyKindID.Pink)
                {
                    Tuple<string, object>[] pairs = new Tuple<string, object>[]
                    {
                        new Tuple<string, object>("color", ItemConfig.Scp330CandyTranslations[id.ToString().ToUpperInvariant()])
                    };

                    args.Player.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ItemConfig.Scp330CandyPickedUpMessage, '%', pairs));
                }
                else
                {
                    args.Player.SendHintToPlayer(ItemConfig.Scp330PinkCandyPickedUpMessage);
                }
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        bool OnPlayerReceiveEffect(PlayerReceiveEffectEvent args)
        {
            if (CheckPlayerForItemTogglesDisabled(args.Player))
                return true;

            switch (args.Effect)
            {
                case SeveredHands _:
                    args.Player.SendHintToPlayer(ItemConfig.Scp330CandyPickedUpTooManyMessage);
                    break;
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        bool OnPlayerDamage(PlayerDamageEvent args)
        {
            if (!MainConfig.EnableDamageMessages || args.Target == null || args.Player == null)
            {
                return true;
            }

            // Create a list to hold all the tokens to replace (Then replace items in their respective index slot)
            // Order: 0 (Player), 1 (Role), 2 (Hitbox), 3 (Damage)
            Tuple<string, object>[] humanPair = new Tuple<string, object>[4];
            humanPair[0] = new Tuple<string, object>("player", args.Target.Nickname);
            humanPair[1] = new Tuple<string, object>("role", args.Player.Role.ToString());

            switch (args.DamageHandler)
            {
                case FirearmDamageHandler fiHandler:    
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[fiHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", fiHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.FirearmDamageDealt, DamageConfig.FirearmDamageReceived);
                    break;
                case ExplosionDamageHandler exHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[exHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", exHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.ExplosiveDamageDealt, DamageConfig.ExplosiveDamageReceived);
                    break;
                case MicroHidDamageHandler micHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[micHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", micHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.MicroHidDamageDealt, DamageConfig.MicroHidDamageReceived);
                    break;
                case JailbirdDamageHandler jaHandler:
                    if (args.Target != args.Player) // Swinging/using the jailbird on nothing damages the player for 0 HP (Likely a bug)
                    {
                        humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[jaHandler.Hitbox.ToString().ToUpperInvariant()]);
                        humanPair[3] = new Tuple<string, object>("damage", jaHandler.DealtHealthDamage);
                        SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.JailbirdDamageDealt, DamageConfig.JailbirdDamageReceived);
                    }
                    break;
                case DisruptorDamageHandler diHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[diHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", diHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.DisruptorDamageDealt, DamageConfig.DisruptorDamageReceived);
                    break;
                case Scp018DamageHandler scp018Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp018Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp018Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.Scp018DamageDealt, DamageConfig.Scp018DamageReceived);
                    break;
                case Scp049DamageHandler scp049Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp049Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp049Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.Scp049DamageDealt, DamageConfig.Scp049DamageReceived);
                    break;
                case Scp096DamageHandler scp096Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp096Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp096Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.Scp096DamageDealt, DamageConfig.Scp096DamageReceived);
                    break;
                case Scp939DamageHandler scp939Handler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scp939Handler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scp939Handler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.Scp939DamageDealt, DamageConfig.Scp939DamageReceived);
                    break;
                case ScpDamageHandler scpHandler:
                    humanPair[2] = new Tuple<string, object>("hitbox", DamageConfig.HitboxTranslations[scpHandler.Hitbox.ToString().ToUpperInvariant()]);
                    humanPair[3] = new Tuple<string, object>("damage", scpHandler.DealtHealthDamage);
                    SendDamageMessagesToPlayers(args.Target, args.Player, humanPair, DamageConfig.ScpDamageDealt, DamageConfig.ScpDamageReceived);
                    break;
            }

            return true;
        }

        [PluginEvent(ServerEventType.TeamRespawn)]
        bool OnTeamRespawn(TeamRespawnEvent args)
        {
            if (!MainConfig.EnableTeamRespawnMessages)
                return true;

            if (args.Team == SpawnableTeamType.NineTailedFox)
            {
                PeopleRespawnedOnMTF = args.Players;
            }
            else if (args.Team == SpawnableTeamType.ChaosInsurgency)
            {
                foreach (Player ply in args.Players)
                {
                    if (!IndividualUserPreferences[ply.UserId].EnableScpMessages || !IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages)
                        continue;

                    ply.SendHintToPlayer(TeamConfig.ChaosInsurgencySpawnMessage);
                }
            }

            return true;
        }

        void SendDamageMessagesToPlayers(Player ply, Player attacker, Tuple<string, object>[] pair, string dealtMessage, string receivedMessage)
        {
            if (IndividualUserPreferences.ContainsKey(attacker.UserId) 
                && (IndividualUserPreferences[attacker.UserId].EnableScpMessages 
                && IndividualUserPreferences[attacker.UserId].EnableDamageMessages))
            {
                attacker.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(dealtMessage, '%', pair));
            }

            pair[0] = new Tuple<string, object>("player", attacker.Nickname);

            if (IndividualUserPreferences.ContainsKey(ply.UserId) 
                && (IndividualUserPreferences[attacker.UserId].EnableScpMessages 
                && IndividualUserPreferences[attacker.UserId].EnableDamageMessages))
            {
                ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(receivedMessage, '%', pair));
            }
        }

        void SaveTogglesToFile()
        {
            try
            {
                string contents = JsonConvert.SerializeObject(IndividualUserPreferences, Formatting.Indented);
                string filePath = Path.Combine(_toggleDir, "toggles.json");
                File.WriteAllText(filePath, contents);
            }
            catch (Exception)
            {
                Log.Error("There was an error trying to save the playerbase toggle preferences", "ERROR");
            }
        }

        bool CheckPlayerForItemTogglesDisabled(Player ply, bool checkScp = true)
        {
            if (!MainConfig.EnableItemMessages
                || !IndividualUserPreferences.ContainsKey(ply.UserId)
                || !IndividualUserPreferences[ply.UserId].EnableScpMessages
                || !IndividualUserPreferences[ply.UserId].EnableItemMessages
                || checkScp && ply.IsSCP)
            {
                return true;
            }

            return false;
        }
    }
}
