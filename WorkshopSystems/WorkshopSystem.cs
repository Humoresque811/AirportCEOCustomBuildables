using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using TMPro;
using PlacementStrategies;
using System.IO;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

static class WorkshopSystem
{
    private static int modAmount = 0;

    internal static void ProccessWorkshopMod(string path)
    {
        AirportCEOCustomBuildables.LogInfo("Found workshop mod, loading!");

        modAmount++;

        if (CheckIfFolderValid(path, out List<Type> modTypes, out List<string> fullPaths))
        {
            for (int k = 0; k < modTypes.Count; k++)
            {
                BuildableClassHelper.GetBuildableSourceCreator(modTypes[k], out IBuildableSourceCreator buildableSourceCreator);
                buildableSourceCreator.modPaths.Add(fullPaths[k]);
                AirportCEOCustomBuildables.LogInfo($"Added mod to {modTypes[k].Name}s pile");
            }
        }
    }


    public static bool CheckIfFolderValid(string path, out List<Type> modTypes, out List<string> fullPaths)
    {
        try
        {
            modTypes = new List<Type>();
            fullPaths = new List<string>();
            bool success = false;

            foreach (Type type in FileManager.Instance.buildableTypes.Keys.ToArray())
            {
                string specificPath = Path.Combine(path, FileManager.Instance.buildableTypes[type].Item1);
                if (!Directory.Exists(specificPath))
                {
                    continue;
                }

                string[] jsonFiles = Directory.GetFiles(specificPath, "*.json");
                if (jsonFiles.Length < 1)
                {
                    continue;
                }

                modTypes.Add(type);
                fullPaths.Add(specificPath);
                success = true;
                continue;
            }

            if (success)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"The code to check if a workshop folder is valid failed. {ExceptionUtils.ProccessException(ex)}");
            modTypes = new List<Type>();
            fullPaths = new List<string>();
            return false;
        }
    }
}
