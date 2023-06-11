# Configuring messages in ScpMessages
ScpMessages supports 3 general types of interactions:
- Damage related events
- Item related events
- Map related events

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
The following token is the only one that is in this config and it is only for the ItemTossed message within the item config:
```
- %item | The item name that was thrown
```

## Map interaction tokens
The following token is the only one that is in this config and it is only for the ElevatorUsedMessage message within the item config:
```
- %level | The current level the elevator is in
```