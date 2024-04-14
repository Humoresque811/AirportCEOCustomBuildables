using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;
using System.IO;
using LapinerTools.Steam.Data;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;


[HarmonyPatch(typeof(ModManager), "InitializeMods")]
static class StartWorkshopLoading
{   
    public static void Prefix()
    {
        try
        {
            AirportCEOCustomBuildables.LogInfo("Started Loading workshop mods, cleared previous mod info! - - - - ");
            foreach (Type type in FileManager.Instance.buildableTypes.Keys)
            {
                BuildableClassHelper.GetBuildableSourceCreator(type, out IBuildableSourceCreator buildableSourceCreator);
                buildableSourceCreator.modPaths = new List<string>();
            }
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Error in clearing mods... (how da hell you screwed this up Humoresque?) {ExceptionUtils.ProccessException(ex)}");
        }
    }
}
