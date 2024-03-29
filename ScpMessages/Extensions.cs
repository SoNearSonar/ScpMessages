﻿using Hints;
using PluginAPI.Core;

namespace ScpMessages
{
    public static class Extensions
    {
        public static void SendHintToPlayer(this Player ply, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = "\n\n\n\n\n\n\n\n" + message;
            ply.ReceiveHint(message, HintEffectPresets.FadeInAndOut(0.25f), ScpMessages.Instance.IndividualUserPreferences[ply.UserId].MessageDisplayTime);
        }
    }
}
