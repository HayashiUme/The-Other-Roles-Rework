using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.Data.Player;
using Assets.InnerNet;
using HarmonyLib;
using TheOtherRoles.Modules;

namespace TheOtherRoles.Patch;

// ##https://github.com/Yumenopai/TownOfHost_Y
public class ModNews
{
    public int Number;
    public int BeforeNumber;
    public string Title;
    public string SubTitle;
    public string ShortTitle;
    public string Text;
    public string Date;

    public Announcement ToAnnouncement()
    {
        var result = new Announcement
        {
            Number = Number,
            Title = Title,
            SubTitle = SubTitle,
            ShortTitle = ShortTitle,
            Text = Text,
            Language = (uint)DataManager.Settings.Language.CurrentLanguage,
            Date = Date,
            Id = "ModNews"
        };

        return result;
    }
}
[HarmonyPatch]
public class ModNewsHistory
{
    public static List<ModNews> AllModNews = new();
    public static void Init()
    {

        // 创建新公告时，不能删除旧公告   
        {
            //var news110 = new ModNews
            //{
            //    Number = 100005,
            //    Title = ModTranslation.GetString("TORR110TitleTop"),
            //    SubTitle = ModTranslation.GetString("TORR110Title"),
            //    ShortTitle = ModTranslation.GetString("TORR110Lans"),
            //    Text = ModTranslation.GetString("TORR110Text"),
            //    Date = "2025-4-12T00:00:00Z"
            //};
            //AllModNews.Add(news110);

            //var news1031 = new ModNews
            //{
            //    Number = 100004,
            //    Title = ModTranslation.GetString("TORR1031TitleTop"),
            //    SubTitle = ModTranslation.GetString("TORR1031Title"),
            //    ShortTitle = ModTranslation.GetString("TORR1031Lans"),
            //    Text = ModTranslation.GetString("TORR1031Text"),
            //    Date = "2025-2-10T00:00:00Z"
            //};
            //AllModNews.Add(news1031);

            //var news103 = new ModNews
            //{
            //    Number = 100003,
            //    Title = ModTranslation.GetString("TORR103TitleTop"),
            //    SubTitle = ModTranslation.GetString("TORR103Title"),
            //    ShortTitle = ModTranslation.GetString("TORR103Lans"),
            //    Text = ModTranslation.GetString("TORR103Text")            ,
            //    Date = "2025-2-5T00:00:00Z"
            //};
            //AllModNews.Add(news103);
        }
    }

    [HarmonyPatch(typeof(PlayerAnnouncementData), nameof(PlayerAnnouncementData.SetAnnouncements)), HarmonyPrefix]
    public static bool SetModAnnouncements(PlayerAnnouncementData __instance, [HarmonyArgument(0)] ref Il2CppReferenceArray<Announcement> aRange)
    {
        if (AllModNews.Count < 1)
        {
            Init();
            AllModNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });
        }

        List<Announcement> FinalAllNews = new();
        AllModNews.Do(n => FinalAllNews.Add(n.ToAnnouncement()));
        foreach (var news in aRange)
        {
            if (!AllModNews.Any(x => x.Number == news.Number))
                FinalAllNews.Add(news);
        }
        FinalAllNews.Sort((a1, a2) => { return DateTime.Compare(DateTime.Parse(a2.Date), DateTime.Parse(a1.Date)); });

        aRange = new(FinalAllNews.Count);
        for (int i = 0; i < FinalAllNews.Count; i++)
            aRange[i] = FinalAllNews[i];

        return true;
    }
}