using AirportCEOModLoader.WatermarkUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportCEOModLoader.WorkshopUtils;
using AirportCEOModLoader.SaveLoadUtils;

namespace AirportCEOCustomBuildables;

internal class ModLoaderInteractionHandler
{
    internal static void SetUpInteractions()
    {
        AirportCEOCustomBuildables.LogInfo("Seting up ModLoader interactions");
        EventDispatcher.NewGameStarted += ModLoader.LoadMods;
        EventDispatcher.EndOfLoad += LoadSubSystem.LoadProccess;

        WatermarkUtils.Register(new WatermarkInfo("CB", PluginInfo.PLUGIN_VERSION, true));
        SetUpWorkshopInteractions();
        SetUpSaveLoadInteractions();
        AirportCEOCustomBuildables.LogInfo("Completed ModLoader interactions!");
    }

    private static void SetUpWorkshopInteractions()
    {
        WorkshopUtils.Register(FileManager.Instance.pathAddativeBase, WorkshopSystem.ProccessWorkshopMod);
    }

    private static void SetUpSaveLoadInteractions()
    {
        SaveLoadUtils.RegisterLoad(SaveLoadSystem.originalSaveFileName);
        SaveLoadUtils.RegisterLoad(SaveLoadSystem.newSaveFileName);

        SaveLoadUtils.RegisterSave(SaveLoadSystem.newSaveFileName, SaveSubSystem.SaveProccess);
    }
}
