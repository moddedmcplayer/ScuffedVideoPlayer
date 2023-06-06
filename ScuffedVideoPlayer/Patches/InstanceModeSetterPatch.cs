namespace ScuffedVideoPlayer.Patches
{
    using HarmonyLib;
    using ScuffedVideoPlayer.Audio;

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.InstanceMode), MethodType.Setter)]
    public class InstanceModeSetterPatch
    {
        public static bool Prefix(CharacterClassManager __instance)
            => !AudioNpc.IsNpc(__instance._hub);
    }
}