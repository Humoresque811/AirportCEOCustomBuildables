using BepInEx;
using AirportCEOModLoader.WatermarkUtils;
using AirportCEOModLoader.Core;
using HarmonyLib;
using System.Diagnostics;
using System.Web;
using System;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace AirportCEOCustomBuildables;

[BepInPlugin("org.airportceocustombuidlables.humoresque",  "AirporCEO Custom Buildables", "1.3.0")]
[BepInDependency("org.airportceomodloader.humoresque")]
public class AirportCEOCustomBuildables : BaseUnityPlugin
{
    public static AirportCEOCustomBuildables Instance { get; private set; }
    internal static Harmony Harmony { get; private set; }
    internal static ManualLogSource CBLogger { get; private set; }
    internal static ConfigFile ConfigReference {  get; private set; }

    private void Awake()
    {
        Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(); 

        // Plugin startup logic
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        Instance = this;
        CBLogger = Logger;
        ConfigReference = Config;

        // Config
        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is setting up config.");
        AirportCEOCustomBuildablesConfig.SetUpConfig();
        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} finished setting up config.");
    }


    private void Start()
    {
        try
        {
            Logger.LogInfo("[Init] Setting up creators!");
            FileManager fileManager = this.gameObject.AddComponent<FileManager>();
            fileManager.SetUp();

            ItemModSourceCreator itemModSourceCreator = this.gameObject.AddComponent<ItemModSourceCreator>();
            itemModSourceCreator.SetUp();
            FloorModSourceCreator floorModSourceCreator = this.gameObject.AddComponent<FloorModSourceCreator>();
            floorModSourceCreator.SetUp();
            TileableSourceCreator tileableSourceCreator = this.gameObject.AddComponent<TileableSourceCreator>();
            tileableSourceCreator.SetUp();

            ItemCreator itemManager = this.gameObject.AddComponent<ItemCreator>();
            itemManager.SetUp();
            FloorCreator floorManager = this.gameObject.AddComponent<FloorCreator>();
            floorManager.SetUp();
            TileableCreator tileableCreator = this.gameObject.AddComponent<TileableCreator>();
            tileableCreator.SetUp();

            fileManager.SetUpBuildableTypes();
            fileManager.SetUpBasePaths();
            Logger.LogInfo("[Init] Completed creator setup!");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to set up buildable creators! {ExceptionUtils.ProccessException(ex)}");
        }

        try
        {
            EnumManager.ValidateEnums(new Action<string>(Logger.LogWarning));
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while validating enums. Error: {ExceptionUtils.ProccessException(ex)}");
        }

        ModLoaderInteractionHandler.SetUpInteractions();
    }

    /// <summary>
    /// For message logging only
    /// </summary>
    /// <param name="message">String to log</param>
    internal static void Log(string message) => LogInfo(message);

    internal static void LogInfo(string message) => CBLogger.LogInfo(message);
    internal static void LogError(string message) => CBLogger.LogError(message);
    internal static void LogWarning(string message) => CBLogger.LogWarning(message);
    internal static void LogDebug(string message) => CBLogger.LogDebug(message);
}