namespace ScuffedVideoPlayer.Patches
{
    using HarmonyLib;
    using PlayerRoles.Voice;
    using ScuffedVideoPlayer.Audio;

    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CheckRange))]
    public class CheckRangePatch
    {
        public static bool Prefix(ReferenceHub hub, ref bool __result)
        {
            if (AudioNpc.IsNpc(hub))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}