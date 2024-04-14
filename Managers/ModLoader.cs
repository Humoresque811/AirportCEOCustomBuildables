using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using AirportCEOCustomBuildables;
using HarmonyLib;

namespace AirportCEOCustomBuildables;

// 1.3-Bep RETIRED

//[HarmonyPatch(typeof(SaveLoadGameDataController))]
//static class NewGameModPostix
//{
//    [HarmonyPatch("StartNewGame")]
//    public static void Prefix(SaveLoadGameDataController __instance)
//    {
//        ModLoader.LoadMods();
//    }
//}

internal class ModLoader : MonoBehaviour
{
    public static void LoadMods(SaveLoadGameDataController _)
    {
        AirportCEOCustomBuildables.LogInfo("Started loading mod info!");
        Singleton<SceneMessagePanelUI>.Instance.SetLoadingText("Creating Custom Buildables...", 5);
        if (!TemplateManager.GetAllTemplates())
        {
            AirportCEOCustomBuildables.LogError("Template manager did not get all templates. Aborted mod loading!");
            return;
        }

        // Clear out last load's mods!
        foreach (Type type in FileManager.Instance.buildableTypes.Keys)
        {
            BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
            buildableSourceCreator.ClearBuildableMods(true); 
        }

        // Load the JSON files
        foreach (Type type in FileManager.Instance.buildableTypes.Keys)
        {
            BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
            buildableSourceCreator.ImportMods();
        }

        // Create buildables
        foreach (Type type in FileManager.Instance.buildableTypes.Keys)
        {
            BuildableClassHelper.GetBuildableCreator(type, out IBuildableCreator buildableCreator);
            buildableCreator.ClearBuildables();
            buildableCreator.CreateBuildables();
            AirportCEOCustomBuildables.LogInfo($"[Success] {buildableCreator.GetType().Name} finished creating buildables, creating {buildableCreator.buildables.Count} buildable(s)");
        }

        UIManager.ClearUI();
        UIManager.CreateAllUI();
        if (UIManager.UIFailed)
        {
            AirportCEOCustomBuildables.LogError("There was an error creating an UI button (see above). Mod loading will countinue.");
        }
        else
        {
            AirportCEOCustomBuildables.LogInfo($"[Success] Ended creating UI. Created {UIManager.floorIcons.Count + UIManager.itemIcons.Count} UI button(s)");
        }
    }
}