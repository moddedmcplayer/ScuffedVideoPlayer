namespace ScuffedVideoPlayer.Patches
{
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using ScuffedVideoPlayer.Audio;

    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdateFloating))]
    public class UpdateFloating
    {
        public static bool Prefix(FpcMotor __instance)
            => !AudioNpc.IsNpc(__instance.Hub);
    }
}