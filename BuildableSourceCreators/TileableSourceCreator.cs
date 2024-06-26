﻿using AirportCEOModLoader.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOCustomBuildables;

class TileableSourceCreator : MonoBehaviour, IBuildableSourceCreator
{
    public static TileableSourceCreator Instance { get; private set; }

    public List<TexturedBuildableMod> buildableMods { get; set; }
    public List<string> modPaths { get; set; }

    public void SetUp()
    {
        Instance = this;

        buildableMods = new List<TexturedBuildableMod>();
        modPaths = new List<string>();
    }

    public void ClearBuildableMods(bool clearAllCreators)
    {
        buildableMods = new List<TexturedBuildableMod>();

        if (!clearAllCreators)
        {
            return;
        }
        if (!BuildableClassHelper.GetBuildableCreator(typeof(ItemMod), out IBuildableCreator buildableCreator))
        {
            return;
        }

        buildableCreator.ClearBuildables();
        UIManager.ClearUI();
    }

    public void ImportMods()
    {
        ClearBuildableMods(true);

        ImportModsFromPath();

        if (modPaths.Count <= 0)
        {
            AirportCEOCustomBuildables.LogInfo($"[Success] TileableSourceCreator (re-)Imported {buildableMods.Count} JSON file(s) from just the buildables folder");
            return;
        }

        for (int i = 0; i < modPaths.Count; i++)
        {
            ImportModsFromPath(modPaths[i]);
        }
        AirportCEOCustomBuildables.LogInfo($"[Success] TileableSourceCreator (re-)Imported {buildableMods.Count} JSON file(s) from just the buildables folder");

    }

    private void ImportModsFromPath(string path = "")
    {
        string internalLog = "";
        Action<string> logAction = new Action<string>(AirportCEOCustomBuildables.LogInfo);
        bool giveUsePath = FileManager.Instance.GetCorrectPath(ref path, typeof(TileableMod), logAction);

        if (!Directory.Exists(path))
        {
            AirportCEOCustomBuildables.LogError("There is no directory at the path...");
            return;
        }
        internalLog += "\nFinished pre-work";

        try
        {
            if (!FileManager.Instance.GetDirectories(path, out string[] directories, logAction))
            {
                return;
            }

            for (int i = 0; i < directories.Length; i++)
            {
                string JSONFileContent = FileManager.Instance.GetJSONFileContent(directories[i]);

                if (string.IsNullOrEmpty(JSONFileContent))
                {
                    AirportCEOCustomBuildables.LogError("JSON file is emtpy/null...");
                    continue;
                }

                TileableMod tileableMod = JsonUtility.FromJson<TileableMod>(JSONFileContent);

                if (tileableMod == null)
                {
                    AirportCEOCustomBuildables.LogError("Mod class is null...");
                    continue;
                }

                if (!tileableMod.enabled)
                {
                    continue;
                }

                buildableMods.Add(tileableMod);
                internalLog += "\nFinished modLoading";

                if (giveUsePath)
                {
                    buildableMods[buildableMods.Count - 1].pathToUse = path;
                }
                else
                {
                    buildableMods[buildableMods.Count - 1].pathToUse = "";
                }

                BogusInputHelper.CheckTileableMod(tileableMod, logAction);

                FileManager.Instance.CrossCheckIds(logAction);
                internalLog += "\nFinished final checks";
            }
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError("An error occured while doing a JSON file scan. Info: " +
                "\nError: " + ExceptionUtils.ProccessException(ex) + 
                "\nPath: " + path + 
                "\nError Debug Log: " + internalLog);
        }
    }
}
