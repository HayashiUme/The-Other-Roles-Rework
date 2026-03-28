using AmongUs.GameOptions;
using HarmonyLib;
using TheOtherRoles.Modules;
using UnityEngine;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
    public static class IntroCutscene_ShowTeam__d_MoveNext
    {
        public static void Prefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek) return;

            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && TORMapOptions.gameMode == CustomGamemodes.Classic || TORMapOptions.gameMode == CustomGamemodes.Guesser && ExileController.Instance.initData.remainingImpostorCount != 1)
            {
                __instance.__4__this.ImpostorText.text = ModTranslation.GetString("impostorShowTeamText");
                __instance.__4__this.ImpostorText.gameObject.SetActive(true);
                __instance.__4__this.ImpostorText.transform.localScale = new Vector3(0.7f, 0.7f);

            }

        }
    }
}
