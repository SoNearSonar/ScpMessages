# Configuring messages in ScpMessages
ScpMessages supports 4 general types of interactions:
- Damage related events
- Item related events
- Map related events
- Team respawn related events

These general types of interactions have their own configuration file and some of the items within these configurations support tokens! More information below:

## Damage interaction tokens
The following tokens work for all messages within the damage config:
```
- %player | The name that the victim/attacker was
- %role | The role that the attacker was
- %hitbox | The hitbox that was hit
- %damage | The amount of damage dealt
```

## Item interaction tokens
The following token work for specific messages within the item config:
```
- %item | The item name that was thrown (ItemTossed, ItemDropped, ItemPickedUp, AmmoDropped, AmmoPickedUp)
- %color | The color of the SCP-330 candy piece that was picked up (Scp330CandyPickedUpMessage)
```

## Map interaction tokens
The following token is the only one that is in this config and it is only for the ElevatorUsedMessage message within the item config:
```
- %level | The current level the elevator is in
```

## Team respawn tokens
The following token is the only one that is in this config and it is only for the NineTailedFoxSpawnMessage message within the item config:
```
- %team | The MTF unit name that spawned in in the latest wave
```

# Console command
ScpMessages has one console command for individually toggling plugin messages:
```
.scpmsg (all, damage, item, map, list, help)
-> all (Toggles all interaction messages)
-> damage (Toggles damage interaction messages)
-> item (Toggles item interaction messages)
-> map (Toggles map interaction messages)
-> team (Toggles team respawn messages)
-> time (non-negative number, show) (How long in seconds messages appear for)
-> list (Lists all the interactions that are true/false for you)
-> help (Lists all the arguments above)
```

# Console command use
Example usages: 
```
.scpmsg all
.scpmsg Team
.scpmsg list
.scpmsg time 5
.scpmsg time show
```
