namespace ScuffedVideoPlayer.Patches
{
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using ScuffedVideoPlayer.Audio;

    [HarmonyPatch(typeof(FpcMouseLook), nameof(FpcMouseLook.UpdateRotation))]
    public class FpcRotationPatch
    {
        public static bool Prefix(FpcMouseLook __instance)
            => !AudioNpc.IsNpc(__instance._hub);
    }
}