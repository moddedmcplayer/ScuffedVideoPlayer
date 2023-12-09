namespace ScuffedVideoPlayer.Patches
{
    using CentralAuth;
    using HarmonyLib;
    using ScuffedVideoPlayer.Audio;

    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.InstanceMode), MethodType.Setter)]
    public class InstanceModeSetterPatch
    {
        public static bool Prefix(PlayerAuthenticationManager __instance)
            => !AudioNpc.IsNpc(__instance._hub);
    }
}