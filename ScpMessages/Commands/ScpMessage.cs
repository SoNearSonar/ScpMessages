using CommandSystem;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
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
            Player ply = Player.Get(((CommandSender)sender).SenderId);
            if (ply == null)
            {
                response = Environment.NewLine + "<color=red>[ScpMessages] Only players in the game can use this command</color>";
                return false;
            }

            if (ply.DoNotTrack)
            {
               response = Environment.NewLine + "<color=red>[ScpMessages] You cannot use ScpMessage console commands because your account has do not track enabled. Messages are disabled by default</color>";
               return true;
            }

            string additionalText = string.Empty;
            if (!ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableScpMessages)
            {
                additionalText = ", however you have all interactions disabled so toggling this interaction type will have no effect";
            }

            switch (arguments.Count)
            {
                case 1:
                    switch (arguments.At(0).ToLowerInvariant())
                    {
                        case "all":
                            if (!ScpMessages.Instance.MainConfig.EnableScpMessages)
                            {
                                response = Environment.NewLine + $"<color=red>[ScpMessages] All messages cannot be toggled as they are disabled for all users</color>";
                                break;
                            }

                            ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableScpMessages = !ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableScpMessages;
                            if (ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableScpMessages)
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

                            ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableDamageMessages = !ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableDamageMessages;
                            if (ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableDamageMessages)
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

                            ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableItemMessages = !ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableItemMessages;
                            if (ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableItemMessages)
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

                            ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableMapMessages = !ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableMapMessages;
                            if (ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableMapMessages)
                            {
                                response = Environment.NewLine + $"<color=green>[ScpMessages] Map messages will display at the bottom for interactions{additionalText}</color>";
                            }
                            else
                            {
                                response = Environment.NewLine + $"<color=red>[ScpMessages] Map messages will not display at the bottom for interactions{additionalText}</color>";
                            }
                            break;
                        case "list":
                            StringBuilder builder1 = StringBuilderPool.Shared.Rent();
                            builder1.Append(Environment.NewLine + "[ScpMessages] These are the messages that are toggled for you:" + Environment.NewLine);
                            builder1.Append("- Plugin messages (all): <color=yellow>" + ConvertValueToOnOff(ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableScpMessages) + "</color>" + Environment.NewLine);
                            builder1.Append("- Damage messages (damage): <color=yellow>" + ConvertValueToOnOff(ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableDamageMessages) + "</color>" + Environment.NewLine);
                            builder1.Append("- Item messages (item): <color=yellow>" + ConvertValueToOnOff(ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableItemMessages) + "</color>" + Environment.NewLine);
                            builder1.Append("- Map messages (map): <color=yellow>" + ConvertValueToOnOff(ScpMessages.Instance.ToggleScpMessages[ply.UserId].EnableMapMessages) + "</color>" + Environment.NewLine);
                            response = builder1.ToString();
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
                            response = builder2.ToString();
                            break;
                        default:
                            response = Environment.NewLine + "<color=yellow>[ScpMessages] Usage: .scpmsg (all, damage, item, map, list, help)</color>";
                            break;
                    }
                    break;
                default:
                    response = Environment.NewLine + "<color=yellow>[ScpMessages] Usage: .scpmsg (all, damage, item, map, list, help)</color>";
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
