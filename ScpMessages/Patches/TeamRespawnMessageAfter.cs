using HarmonyLib;
using PluginAPI.Core;
using Respawning.NamingRules;
using System;

namespace ScpMessages.Patches
{
    [HarmonyPatch(typeof(UnitNameMessageHandler), nameof(UnitNameMessageHandler.ProcessMessage), new Type[] { typeof(UnitNameMessage) })]
    public class TeamRespawnMessageAfter
    {
        public static void Postfix(UnitNameMessage msg)
        {
            Tuple<string, object>[] teamName = 
            { 
                new Tuple<string, object>("team", msg.UnitName)
            };

            foreach (Player ply in ScpMessages.Instance.PeopleRespawnedOnMTF)
            {
                if (!ScpMessages.Instance.MainConfig.EnableTeamRespawnMessages || !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableScpMessages || !ScpMessages.Instance.IndividualUserPreferences[ply.UserId].EnableTeamRespawnMessages)
                    continue;

                ply.SendHintToPlayer(TokenReplacer.ReplaceAfterToken(ScpMessages.Instance.TeamConfig.NineTailedFoxSpawnMessage, '%', teamName));
            }
        }
    }
}
