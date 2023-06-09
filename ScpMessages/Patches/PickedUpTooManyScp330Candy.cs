using CustomPlayerEffects;
using HarmonyLib;
using Hints;
using ScpMessages.Configs;

namespace ScpMessages.Patches
{
    [HarmonyPatch(typeof(SeveredHands), "ChangeHandsState")]
    public class PickedUpTooManyScp330Candy
    {
        public static void Postfix(SeveredHands __instance, bool handsCut)
        {
            MainConfig mainConfig = ScpMessages.Instance.MainConfig;
            ItemConfig itemConfig = ScpMessages.Instance.ItemConfig;

            if (!mainConfig.EnableItemMessages || !handsCut)
            {
                return;
            }

            __instance.Hub.hints.Show(new TextHint($"\n\n\n\n\n\n\n\n{itemConfig.Scp330CandyPickedUpTooManyMessage}", new HintParameter[1]
            {
                new StringHintParameter($"\n\n\n\n\n\n\n\n{itemConfig.Scp330CandyPickedUpTooManyMessage}")
            }, HintEffectPresets.FadeInAndOut(0.25f), 5f));
        }
    }
}
