﻿using Hints;
using PluginAPI.Core;

namespace ScpMessages
{
    public static class Extensions
    {
        public static void SendHintToPlayer(this Player ply, string message)
        {
            if (ScpMessages.Instance.ToggleScpMessages[ply.UserId])
            {
                message = "\n\n\n\n\n\n\n\n" + message;
                ply.ReceiveHint(message, HintEffectPresets.FadeInAndOut(0.25f), 5);
            }
        }
    }
}
