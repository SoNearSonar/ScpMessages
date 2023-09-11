using CommandSystem;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using System;
using System.Text;

namespace ScpMessages.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ScpMessage : ICommand
    {
        public string Command => "scpmessages";

        public string[] Aliases => new string[] { "scpmsg" };

        public string Description => "Manages displaying messages for the ScpMessages plugin";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender);
            if (ply == null)
            {
                response = Environment.NewLine + "<color=red>[ScpMessages] Only players in the game can use this command</color>";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = Environment.NewLine + "<color=yellow>[ScpMessages] Usage: .scpmsg (all, damage, item, map, team, time, list, help)</color>";
                return true;
            }

            string additionalText = string.Empty;
            if (!ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages)
            {
                if (arguments.At(0).Equals("time", StringComparison.InvariantCultureIgnoreCase))
                    additionalText = ", however you have all interactions disabled so changing the message display time will have no effect";
                else
                    additionalText = ", however you have all interactions disabled so toggling this interaction type will have no effect";
            }

            switch (arguments.At(0).ToLowerInvariant())
            {
                case "all":
                    if (!ScpMessages.Instance.MainConfig.EnableScpMessages)
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] All messages cannot be toggled as they are disabled for all users</color>";
                        break;
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages = !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages;
                    if (ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages)
                    {
                        response = Environment.NewLine + "<color=green>[ScpMessages] All messages will display at the bottom for interactions</color>";
                    }
                    else
                    {
                        response = Environment.NewLine + "<color=red>[ScpMessages] All messages will not display at the bottom for interactions</color>";
                    }
                    break;
                case "damage":
                    if (!ScpMessages.Instance.MainConfig.EnableDamageMessages)
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Damage messages cannot be toggled as they are disabled for all users</color>";
                        break;
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableDamageMessages = !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableDamageMessages;
                    if (ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableDamageMessages)
                    {
                        response = Environment.NewLine + $"<color=green>[ScpMessages] Damage messages will display at the bottom for interaction{additionalText}</color>";
                    }
                    else
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Damage messages will not display at the bottom for interactions{additionalText}</color>";
                    }
                    break;
                case "item":
                    if (!ScpMessages.Instance.MainConfig.EnableItemMessages)
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Item messages cannot be toggled as they are disabled for all users</color>";
                        break;
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableItemMessages = !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableItemMessages;
                    if (ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableItemMessages)
                    {
                        response = Environment.NewLine + $"<color=green>[ScpMessages] Item messages will display at the bottom for interactions{additionalText}</color>";
                    }
                    else
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Item messages will not display at the bottom for interactions{additionalText}</color>";
                    }
                    break;
                case "map":
                    if (!ScpMessages.Instance.MainConfig.EnableMapMessages)
                    {
                        response = Environment.NewLine + "<color=red>[ScpMessages] Map messages cannot be toggled as they are disabled for all users</color>";
                        break;
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableMapMessages = !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableMapMessages;
                    if (ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableMapMessages)
                    {
                        response = Environment.NewLine + $"<color=green>[ScpMessages] Map messages will display at the bottom for interactions{additionalText}</color>";
                    }
                    else
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Map messages will not display at the bottom for interactions{additionalText}</color>";
                    }
                    break;
                case "team":
                    if (!ScpMessages.Instance.MainConfig.EnableTeamRespawnMessages)
                    {
                        response = Environment.NewLine + "<color=red>[ScpMessages] Team messages cannot be toggled as they are disabled for all users</color>";
                        break;
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages = !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages;
                    if (ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages)
                    {
                        response = Environment.NewLine + $"<color=green>[ScpMessages] Team messages will display at the bottom for interactions{additionalText}</color>";
                    }
                    else
                    {
                        response = Environment.NewLine + $"<color=red>[ScpMessages] Team messages will not display at the bottom for interactions{additionalText}</color>";
                    }
                    break;
                case "time":
                    if (!ScpMessages.Instance.MainConfig.EnableScpMessages)
                    {
                        response = Environment.NewLine + "<color=red>[ScpMessages] Message display time cannot be modified as messages are disabled for all users</color>";
                        break;
                    }

                    if (arguments.Count < 2)
                    {
                        response = Environment.NewLine + "<color=yellow>[ScpMessages] Usage: .scpmsg time (non-negative number (default 5), show)</color>";
                        break;
                    }

                    if (arguments.At(1).Equals("show", StringComparison.InvariantCultureIgnoreCase))
                    {
                        response = Environment.NewLine + $"<color=green>[ScpMessages] Message display time is set to {ScpMessages.Instance.IndividualUserPreferences[ply.UserId].MessageDisplayTime} seconds</color>";
                        break;
                    }

                    if (!float.TryParse(arguments.At(1), out float time))
                    {
                        response = Environment.NewLine + $"<color=yellow>[ScpMessages] \"{arguments.At(1)}\" is not a valid number for time in seconds</color>";
                        break;
                    }
                    
                    if (time < 0)
                    {
                        time = Math.Abs(time);
                        additionalText += ", and the number was less than 0 so it was adjusted to be more than 0";
                    }

                    ScpMessages.Instance.IndividualUserPreferences[ply.UserId].MessageDisplayTime = time;
                    response = Environment.NewLine + $"<color=green>[ScpMessages] Message display time has been set to {time} seconds{additionalText}</color>";
                    break;
                case "list":
                    StringBuilder builder1 = StringBuilderPool.Shared.Rent();
                    builder1.Append(Environment.NewLine + "[ScpMessages] These are the messages that are toggled for you:" + Environment.NewLine);
                    builder1.Append("- Plugin messages (all): " + ConvertValueToOnOff(ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages) + Environment.NewLine);
                    builder1.Append("- Damage messages (damage): " + ConvertValueToOnOff(ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableDamageMessages) + Environment.NewLine);
                    builder1.Append("- Item messages (item): " + ConvertValueToOnOff(ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableItemMessages) + Environment.NewLine);
                    builder1.Append("- Map messages (map): " + ConvertValueToOnOff(ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableMapMessages) + Environment.NewLine);
                    builder1.Append("- Team messages (team): " + ConvertValueToOnOff(ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages) + Environment.NewLine);
                    response = builder1.ToString();
                    break;
                case "help":
                    StringBuilder builder2 = StringBuilderPool.Shared.Rent();
                    builder2.Append(Environment.NewLine + "[ScpMessages] These are the commands you can do" + Environment.NewLine);
                    builder2.Append("- .scpmsg all (Toggle all plugin messages)" + Environment.NewLine);
                    builder2.Append("- .scpmsg damage (Toggle damage messages)" + Environment.NewLine);
                    builder2.Append("- .scpmsg item (Toggle item messages)" + Environment.NewLine);
                    builder2.Append("- .scpmsg map (Toggle map messages)" + Environment.NewLine);
                    builder2.Append("- .scpmsg team (Toggle team messages)" + Environment.NewLine);
                    builder2.Append("- .scpmsg time (non-negative number, show) (How long in seconds messages appear for)" + Environment.NewLine);
                    builder2.Append("- .scpmsg list (List all message toggles on your account)" + Environment.NewLine);
                    builder2.Append("- .scpmsg help (Displays this message again)" + Environment.NewLine);
                    response = builder2.ToString();
                    break;
                default:
                    response = Environment.NewLine + "<color=yellow>[ScpMessages] Usage: .scpmsg (all, damage, item, map, team, time, list, help)</color>";
                    break;
            }
            return true;
        }

        string ConvertValueToOnOff(bool value)
        {
            return value ? "<color=green>Yes</color>" : "<color=red>No</color>";
        }
    }
}
