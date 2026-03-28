using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InnerNet;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles;

public class RoleInfo
{
    public static Dictionary<RoleId, RoleInfo> roleInfoById = new();

    // Impostor
    public static RoleInfo godfather = new("godfather", Godfather.color, RoleId.Godfather);

    public static RoleInfo mafioso = new("mafioso", Mafioso.color, RoleId.Mafioso);

    public static RoleInfo janitor = new("janitor", Janitor.color, RoleId.Janitor);

    public static RoleInfo morphling = new("morphling", Morphling.color, RoleId.Morphling);

    public static RoleInfo camouflager = new("camouflager", Camouflager.color, RoleId.Camouflager);

    public static RoleInfo vampire = new("vampire", Vampire.color, RoleId.Vampire);

    public static RoleInfo eraser = new("eraser", Eraser.color, RoleId.Eraser);

    public static RoleInfo trickster = new("trickster", Trickster.color, RoleId.Trickster);

    public static RoleInfo cleaner = new("cleaner", Cleaner.color, RoleId.Cleaner);

    public static RoleInfo warlock = new("warlock", Warlock.color, RoleId.Warlock);

    public static RoleInfo bountyHunter = new("bountyHunter", BountyHunter.color, RoleId.BountyHunter);

    public static RoleInfo impostor = new("impostor", Palette.ImpostorRed, RoleId.Impostor);

    public static RoleInfo witch = new("witch", Witch.color, RoleId.Witch);

    public static RoleInfo ninja = new("ninja", Ninja.color, RoleId.Ninja);

    public static RoleInfo bomber = new("bomber", Bomber.color, RoleId.Bomber);

    public static RoleInfo yoyo = new("yoyo", Yoyo.color, RoleId.Yoyo);

    public static RoleInfo hunter = new("hunter", Palette.ImpostorRed, RoleId.Impostor);

    public static RoleInfo fraudster = new("fraudster", Fraudster.color, RoleId.Fraudster);

    public static RoleInfo devil = new("devil", Devil.color, RoleId.Devil);


    // Neutral
    public static RoleInfo goodGuesser = new("niceGuesser", Guesser.color, RoleId.NiceGuesser);

    public static RoleInfo badGuesser = new("evilGuesser", Palette.ImpostorRed, RoleId.EvilGuesser);
    
    public static RoleInfo jester = new("jester", Jester.color, RoleId.Jester, true);

    public static RoleInfo arsonist = new("arsonist", Arsonist.color, RoleId.Arsonist, true);
    
    public static RoleInfo jackal = new("jackal", Jackal.color, RoleId.Jackal, true);

    public static RoleInfo sidekick = new("sidekick", Sidekick.color, RoleId.Sidekick, true);

    public static RoleInfo vulture = new("vulture", Vulture.color, RoleId.Vulture, true);
    
    public static RoleInfo lawyer = new("lawyer", Lawyer.color, RoleId.Lawyer, true);

    public static RoleInfo prosecutor = new("prosecutor", Lawyer.color, RoleId.Prosecutor, true);

    public static RoleInfo pursuer = new("pursuer", Pursuer.color, RoleId.Pursuer);

    public static RoleInfo thief = new("thief", Thief.color, RoleId.Thief, true);


    // Crewmate
    public static RoleInfo mayor = new("mayor", Mayor.color, RoleId.Mayor);

    public static RoleInfo portalmaker = new("portalmaker", Portalmaker.color, RoleId.Portalmaker);

    public static RoleInfo securityGuard = new("securityGuard", SecurityGuard.color, RoleId.SecurityGuard);

    public static RoleInfo engineer = new("engineer", Engineer.color, RoleId.Engineer);

    public static RoleInfo sheriff = new("sheriff", Sheriff.color, RoleId.Sheriff);

    public static RoleInfo deputy = new("deputy", Sheriff.color, RoleId.Deputy);

    public static RoleInfo lighter = new("lighter", Lighter.color, RoleId.Lighter);

    public static RoleInfo detective = new("detective", Detective.color, RoleId.Detective);

    public static RoleInfo timeMaster = new("timeMaster", TimeMaster.color, RoleId.TimeMaster);

    public static RoleInfo medic = new("medic", Medic.color, RoleId.Medic);

    public static RoleInfo swapper = new("swapper", Swapper.color, RoleId.Swapper);

    public static RoleInfo seer = new("seer", Seer.color, RoleId.Seer);

    public static RoleInfo hacker = new("hacker", Hacker.color, RoleId.Hacker);

    public static RoleInfo tracker = new("tracker", Tracker.color, RoleId.Tracker);

    public static RoleInfo snitch = new("snitch", Snitch.color, RoleId.Snitch);

    public static RoleInfo spy = new("spy", Spy.color, RoleId.Spy);

    public static RoleInfo medium = new("medium", Medium.color, RoleId.Medium);

    public static RoleInfo trapper = new("trapper", Trapper.color, RoleId.Trapper);

    public static RoleInfo prophet = new("prophet", Prophet.color, RoleId.Prophet);

    public static RoleInfo peacedove = new("peacedove", PeaceDove.color, RoleId.PeaceDove);

    public static RoleInfo crewmate = new("crewmate", Color.white, RoleId.Crewmate);

    public static RoleInfo hunted = new("hunted", Color.white, RoleId.Crewmate);

    public static RoleInfo prop = new("prop", Color.white, RoleId.Crewmate);


    // Modifier
    public static RoleInfo bloody = new("modifierBloody", Color.yellow, RoleId.Bloody, false, true);

    public static RoleInfo antiTeleport = new("modifierAntiTeleport", Color.yellow, RoleId.AntiTeleport, false, true);

    public static RoleInfo tiebreaker = new("modifierTieBreaker", Color.yellow, RoleId.Tiebreaker, false, true);

    public static RoleInfo bait = new("modifierBait", Color.yellow, RoleId.Bait,
        false, true);

    public static RoleInfo sunglasses = new("modifierSunglasses", Color.yellow, RoleId.Sunglasses, false, true);

    public static RoleInfo lover = new("modifierLover", Lovers.color, RoleId.Lover, false,
        true);

    public static RoleInfo mini = new("modifierMini", Color.yellow, RoleId.Mini, false, true);

    public static RoleInfo vip = new("modifierVip", Color.yellow, RoleId.Vip, false, true);

    public static RoleInfo invert = new("modifierInvert", Color.yellow, RoleId.Invert, false, true);

    public static RoleInfo chameleon = new("modifierChameleon", Color.yellow, RoleId.Chameleon, false, true);

    public static RoleInfo armored = new("modifierArmored", Color.yellow, RoleId.Armored, false, true);

    public static RoleInfo shifter = new("modifierShifter", Color.yellow, RoleId.Shifter, false, true);


    public static List<RoleInfo> allRoleInfos = new()
    {
        impostor,
        godfather,
        mafioso,
        janitor,
        morphling,
        camouflager,
        vampire,
        eraser,
        trickster,
        cleaner,
        warlock,
        bountyHunter,
        witch,
        ninja,
        bomber,
        yoyo,
        fraudster,
        devil,
        goodGuesser,
        badGuesser,
        lover,
        jester,
        arsonist,
        jackal,
        sidekick,
        vulture,
        pursuer,
        lawyer,
        thief,
        prosecutor,
        crewmate,
        mayor,
        portalmaker,
        engineer,
        sheriff,
        deputy,
        lighter,
        detective,
        timeMaster,
        medic,
        swapper,
        seer,
        hacker,
        tracker,
        snitch,
        spy,
        securityGuard,
        bait,
        medium,
        trapper,
        prophet,
        peacedove,
        bloody,
        antiTeleport,
        tiebreaker,
        sunglasses,
        mini,
        vip,
        invert,
        chameleon,
        armored,
        shifter
    };


    private static string ReadmePage = "";
    public Color color;
    public string introDescription;
    public bool isModifier;
    public bool isNeutral;
    public string name;
    public RoleId roleId;
    public string shortDescription;

    public RoleInfo(string name, Color color, RoleId roleId,
    bool isNeutral = false, bool isModifier = false)
    {
        this.color = color;
        switch (name)
        {
            case "impostor":
                this.name = name.Translate();
                this.introDescription = Helpers.cs(
                    Palette.ImpostorRed,
                    (name + "IntroDesc").Translate()
                );
                this.shortDescription = (name + "ShortDesc").Translate();
                break;

            case "crewmate":
                this.name = name.Translate();
                this.introDescription = Helpers.cs(
                    Color.white,
                    (name + "IntroDesc").Translate()
                );
                this.shortDescription = (name + "ShortDesc").Translate();
                break;

            default:
                this.name = name.Translate();
                this.introDescription = (name + "IntroDesc").Translate();
                this.shortDescription = (name + "ShortDesc").Translate();
                break;
        }
        this.roleId = roleId;
        this.isNeutral = isNeutral;
        this.isModifier = isModifier;
        roleInfoById.TryAdd(roleId, this);
    }

    public bool isImpostor => color == Palette.ImpostorRed && !(roleId == RoleId.Spy);

    public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true)
    {
        var infos = new List<RoleInfo>();
        if (p == null) return infos;

        // Modifier
        if (showModifier)
        {
            // after dead modifier
            if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead ||
                AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
            {
                if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
            }

            if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
            if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
            if (p == Mini.mini) infos.Add(mini);
            if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
            if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
            if (p == Armored.armored) infos.Add(armored);
            if (p == Shifter.shifter) infos.Add(shifter);
        }

        var count = infos.Count; // Save count after modifiers are added so that the role count can be checked

        // Special roles
        if (p == Jester.jester) infos.Add(jester);
        if (p == Mayor.mayor) infos.Add(mayor);
        if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
        if (p == Engineer.engineer) infos.Add(engineer);
        if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
        if (p == Deputy.deputy) infos.Add(deputy);
        if (p == Lighter.lighter) infos.Add(lighter);
        if (p == Godfather.godfather) infos.Add(godfather);
        if (p == Mafioso.mafioso) infos.Add(mafioso);
        if (p == Janitor.janitor) infos.Add(janitor);
        if (p == Morphling.morphling) infos.Add(morphling);
        if (p == Camouflager.camouflager) infos.Add(camouflager);
        if (p == Vampire.vampire) infos.Add(vampire);
        if (p == Eraser.eraser) infos.Add(eraser);
        if (p == Trickster.trickster) infos.Add(trickster);
        if (p == Cleaner.cleaner) infos.Add(cleaner);
        if (p == Warlock.warlock) infos.Add(warlock);
        if (p == Witch.witch) infos.Add(witch);
        if (p == Ninja.ninja) infos.Add(ninja);
        if (p == Bomber.bomber) infos.Add(bomber);
        if (p == Yoyo.yoyo) infos.Add(yoyo);
        if (p == Fraudster.fraudster) infos.Add(fraudster);
        if (p == Devil.devil) infos.Add(devil);
        if (p == Detective.detective) infos.Add(detective);
        if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
        if (p == Medic.medic) infos.Add(medic);
        if (p == Swapper.swapper) infos.Add(swapper);
        if (p == Seer.seer) infos.Add(seer);
        if (p == Hacker.hacker) infos.Add(hacker);
        if (p == Tracker.tracker) infos.Add(tracker);
        if (p == Snitch.snitch) infos.Add(snitch);
        if (p == Jackal.jackal ||
            (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId)))
            infos.Add(jackal);
        if (p == Sidekick.sidekick) infos.Add(sidekick);
        if (p == Spy.spy) infos.Add(spy);
        if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
        if (p == Arsonist.arsonist) infos.Add(arsonist);
        if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
        if (p == Guesser.evilGuesser) infos.Add(badGuesser);
        if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
        if (p == Vulture.vulture) infos.Add(vulture);
        if (p == Medium.medium) infos.Add(medium);
        if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
        if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
        if (p == Trapper.trapper) infos.Add(trapper);
        if (p == Pursuer.pursuer) infos.Add(pursuer);
        if (p == Thief.thief) infos.Add(thief);
        if (p == Prophet.prophet) infos.Add(prophet);
        if (p == PeaceDove.peacedove) infos.Add(peacedove);

        // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
        if (infos.Count == count)
        {
            if (p.Data.Role.IsImpostor)
                infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ||
                          TORMapOptions.gameMode == CustomGamemodes.PropHunt
                    ? hunter
                    : impostor);
            else
                infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? hunted :
                    TORMapOptions.gameMode == CustomGamemodes.PropHunt ? prop : crewmate);
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true,
        bool suppressGhostInfo = false)
    {
        string roleName;
        roleName = string.Join(" ",
            getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name)
                .ToArray());
        if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target)
            roleName += useColors ? Helpers.cs(Pursuer.color, " §") : " §";
        if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId))
        {
            var remainingShots = HandleGuesser.remainingShots(p.PlayerId);
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(p.Data);
            if ((!Helpers.isEvil(p) && playerCompleted < HandleGuesser.tasksToUnlock) || remainingShots == 0)
                roleName += Helpers.cs(Color.gray, $" ({"guesser".Translate()})");
            else
                roleName += Helpers.cs(Color.white, $" ({"guesser".Translate()})");
        }

        if (!suppressGhostInfo && p != null)
        {
            if (p == Shifter.shifter &&
                (PlayerControl.LocalPlayer == Shifter.shifter || Helpers.shouldShowGhostInfo()) &&
                Shifter.futureShift != null)
                roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
            if (p == Vulture.vulture && (PlayerControl.LocalPlayer == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                roleName = roleName + Helpers.cs(Vulture.color, string.Format("roleInfoVulture".Translate(), Vulture.vultureNumberToWin - Vulture.eatenBodies));
            if (Helpers.shouldShowGhostInfo())
            {
                if (Eraser.futureErased.Contains(p))
                    roleName = Helpers.cs(Color.gray, "roleInfoEraser") + roleName;
                if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                    roleName = Helpers.cs(Vampire.color, string.Format("roleInfoVampire", (int)HudManagerStartPatch.vampireKillButton.Timer + 1)) + roleName;
                if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                    roleName = Helpers.cs(Color.gray, "roleInfoDeputy") + roleName;
                if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId))  // Active cuff
                    roleName = Helpers.cs(Deputy.color, "roleInfoDeputy") + roleName;
                if (p == Warlock.curseVictim)
                    roleName = Helpers.cs(Warlock.color, "roleInfoWarlock") + roleName;
                if (p == Ninja.ninjaMarked)
                    roleName = Helpers.cs(Ninja.color, "roleInfoNinja") + roleName;
                if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                    roleName = Helpers.cs(Pursuer.color, "roleInfoPursuer") + roleName;
                if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                    roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                if (BountyHunter.bounty == p)
                    roleName = Helpers.cs(BountyHunter.color, "roleInfoBountyHunter") + roleName;
                if (Arsonist.dousedPlayers.Contains(p))
                    roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                if (p == Arsonist.arsonist)
                    roleName = roleName + Helpers.cs(Arsonist.color, string.Format("roleInfoArsonist".Translate(), PlayerControl.AllPlayerControls.ToArray().Count(x => { return x != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })));
                if (p == Jackal.fakeSidekick)
                    roleName = Helpers.cs(Sidekick.color, "roleInfoJackal") + roleName;

                // Death Reason on Ghosts
                if (p.Data.IsDead)
                {
                    var deathReasonString = "";
                    var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                    Color killerColor = new();
                    if (deadPlayer != null && deadPlayer.killerIfExisting != null)
                        killerColor = getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;

                    if (deadPlayer != null)
                    {
                        switch (deadPlayer.deathReason)
                        {
                            case DeadPlayer.CustomDeathReason.Disconnect:
                                deathReasonString = ModTranslation.GetString("roleSummaryDisconnected");
                                break;
                            case DeadPlayer.CustomDeathReason.Exile:
                                deathReasonString = ModTranslation.GetString("roleSummaryExiled");
                                break;
                            case DeadPlayer.CustomDeathReason.Kill:
                                deathReasonString = string.Format(ModTranslation.GetString("roleSummaryKilled"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                break;
                            case DeadPlayer.CustomDeathReason.Guess:
                                if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                    deathReasonString = ModTranslation.GetString("roleSummaryFailedGuess");
                                else
                                    deathReasonString = string.Format(ModTranslation.GetString("roleSummaryGuess"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                break;
                            case DeadPlayer.CustomDeathReason.Shift:
                                deathReasonString = $" - {Helpers.cs(Color.yellow, ModTranslation.GetString("roleSummaryShift"))} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.WitchExile:
                                deathReasonString = string.Format(ModTranslation.GetString("roleSummarySpelled"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                break;
                            case DeadPlayer.CustomDeathReason.LoverSuicide:
                                deathReasonString = $" - {Helpers.cs(Lovers.color, ModTranslation.GetString("roleSummaryLoverDied"))}";
                                break;
                            case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                deathReasonString = $" - {Helpers.cs(Lawyer.color, "roleSummaryLawyerSuicide")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Bomb:
                                deathReasonString = string.Format(ModTranslation.GetString("roleSummaryBombed"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                break;
                            case DeadPlayer.CustomDeathReason.Arson:
                                deathReasonString = string.Format(ModTranslation.GetString("roleSummaryTorched"), Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName));
                                break;
                        }
                        roleName = roleName + deathReasonString;
                    }
                }
            }
        }

        return roleName;
    }

    public static async Task loadReadme()
    {
        if (ReadmePage == "")
        {
            var client = new HttpClient();
            var response =
                await client.GetAsync("http://api.fangkuai.fun:22022/linmeideli/The-Other-Roles-Rework/main/README.md");
            response.EnsureSuccessStatusCode();
            var httpres = await response.Content.ReadAsStringAsync();
            ReadmePage = httpres;
        }
    }

    public static string GetRoleDescription(RoleInfo roleInfo)
    {
        while (ReadmePage == "")
        {
        }

        var index = ReadmePage.IndexOf($"## {roleInfo.name}");
        var endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
        return ReadmePage.Substring(index, endindex);
    }
}