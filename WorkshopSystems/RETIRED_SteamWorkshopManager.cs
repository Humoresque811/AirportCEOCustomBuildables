using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LapinerTools.Steam.Data;
using Steamworks;
using UnityEngine;

namespace AirportCEOCustomBuildables
{
    // 1.3-Bep RETIRED (Moved to mod loader)

    /*
    [HarmonyPatch(typeof(SteamWorkshopManager))]
    static class Patch_SteamWorkshopManager
    {
        // This code is from Zekew (all credits to him), and is needed till a game patch!


        static uint page = 1U; // 1-indexed so initilize as 1                // 
        static WorkshopItemList workshopItemList = new WorkshopItemList();   // OnAvailableItemsCallCompleted is iterated now, so this all needs to be external

        [HarmonyPatch("InitializeWorkshop")]
        [HarmonyPrefix]
        public static bool Patch_InitializeWorkshop()          // replace InitilizeWorkshop() with InitilizeWorkshopAtPage(1U)
        {
            InitilizeWorkshopAtPage(1U);
            return false;
        }
        
        private static void InitilizeWorkshopAtPage(uint page = 1U)
        {
                SteamAPICall_t hAPICall =
                    SteamUGC.SendQueryUGCRequest(
                        SteamUGC.CreateQueryUserUGCRequest(
                            SteamUser.GetSteamID().GetAccountID(),
                            EUserUGCList.k_EUserUGCList_Subscribed,
                            EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                            EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc,
                            AppId_t.Invalid,
                            SteamUtils.GetAppID(),
                            page));// -----------------------------Changed from 1U

                CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(OnAvailableItemsCallCompleted)).Set(hAPICall, null);
        }

        private static void OnAvailableItemsCallCompleted(SteamUGCQueryCompleted_t p_callback, bool p_bIOFailure)
        {
            //WorkshopItemList workshopItemList = new WorkshopItemList()      //
            //{                                                               //
            //    PagesItems = SteamWorkshopManager.GetPageCount(p_callback)  // Is this important?
            //};                                                              //
            
            uint num = 0U;
            for (num = 0U; num < p_callback.m_unNumResultsReturned; num += 1U)
            {
                SteamUGCDetails_t p_itemDetails;
                if (SteamUGC.GetQueryUGCResult(p_callback.m_handle, num, out p_itemDetails))
                {
                    WorkshopItem workshopItem = SteamWorkshopManager.ParseItem(p_callback.m_handle, num, p_itemDetails);
                    if (!SteamUGC.DownloadItem(workshopItem.SteamNative.m_nPublishedFileId, true))
                    {
                        Debug.Log("Could not update workshop item: " + workshopItem.Name);
                    }
                    workshopItemList.Items.Add(workshopItem);
                }
            }
            if (num >= 50U) //API returns max 50 results at a time, so if 50 ther might be more pages...
            {
                page++;
                InitilizeWorkshopAtPage(page);
            }
            else   // else this is the last page
            {
                ModManager.InitalizeSteamWorkshopMods(workshopItemList); //now external, so contains all pages
            }
        }
    }*/
}
