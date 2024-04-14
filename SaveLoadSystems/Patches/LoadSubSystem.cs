using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using AirportCEOModLoader.SaveLoadUtils;

namespace AirportCEOCustomBuildables;

static class LoadSubSystem
{
    private static CustomSerializableWrapper loadedWrapper = null;

    internal static CustomSerializableWrapper LoadedWrapper 
    { 
        get { return loadedWrapper; } 
        set { loadedWrapper = value; }
    }

    internal static void LoadProccess(SaveLoadGameDataController _)
    {
        // Setup visual log stuff
        Singleton<SceneMessagePanelUI>.Instance.SetLoadingText("Loading Saved Custom Buildables...", 97);
        SaveLoadSystem.Quicklog("Starting Custom Load Round!", true);

        LoadedWrapper = GetCustomSaveData();

        if (LoadedWrapper == null)
        {
            return;
        }
        
        SaveLoadSystem.Quicklog("A total of " + LoadedWrapper.customItemSerializables.Count + " custom items were found and will be loaded/changed. " +
            "A total of " + LoadedWrapper.customFloorSerializables.Count + " custom floors were found and will be loaded/changed. " +
            "A total of " + LoadedWrapper.customTileableSerializables.Count + " custom tileables were found and will be loaded/changed. ", false);


        // Now we know the Item info is valid
        DynamicSimpleArray<PlaceableItem> itemsList = Singleton<BuildingController>.Instance.allItemsArray;
        foreach (PlaceableItem worldItem in itemsList.ToList())
        {
            if (CheckForSameItem(worldItem))
            {
                continue;
            }

            if (CheckForSameTileable(worldItem))
            {
                continue;
            }
        }

        Singleton<SceneMessagePanelUI>.Instance.SetLoadingText(LocalizationManager.GetLocalizedValue("SaveLoadGameDataController.cs.key.almost-done"), 100);
        SaveLoadSystem.Quicklog("Custom items finished loading, without any errors!", true);

        LoadedWrapper = null;
    }

    private static bool CheckForSameTileable(PlaceableItem worldItem)
    {
        foreach (CustomTileableSerializable customTileable in LoadedWrapper.customTileableSerializables)
        {
            Vector3 customPostion = new Vector3(customTileable.position[0], customTileable.position[1], customTileable.position[2]);

            // Required to be the same
            if (!Vector3.Equals(worldItem.gameObject.transform.position, customPostion))
            {
                continue;
            }

            if (!int.Equals(worldItem.Floor, customTileable.floor))
            {
                continue;
            }

            // Auxilary checks, to make sure
            if (!string.Equals(worldItem.ReferenceID, customTileable.referenceID))
            {
                SaveLoadSystem.Quicklog("What? The reference ID is different, but the postion and floor are the same? huh?", true);
            }

            HandleCustomTileable(customTileable, worldItem);
            return true;
        }
        return false;
    }

    private static bool CheckForSameItem(PlaceableItem worldItem)
    {
        foreach (CustomItemSerializable customItem in LoadedWrapper.customItemSerializables)
        {
            Vector3 customPostion = new Vector3(customItem.postion[0], customItem.postion[1], customItem.postion[2]);
            float spriteRotation = customItem.spriteRotation;
            float itemRotation = customItem.itemRotation;

            // Required to be the same
            if (!Vector3.Equals(worldItem.gameObject.transform.position, customPostion))
            {
                continue;
            }

            if (!int.Equals(worldItem.Floor, customItem.floor))
            {
                continue;
            }

            // Auxilary checks, to make sure
            if (!string.Equals(worldItem.ReferenceID, customItem.referenceID))
            {
                SaveLoadSystem.Quicklog("What? The reference ID is different, but the postion and floor are the same? huh?", true);
            }

            HandleCustomItem(customItem, spriteRotation, itemRotation, worldItem);
            return true;
        }
        return false;
    }

    private static void HandleCustomTileable(CustomTileableSerializable customTileable, PlaceableItem worldItem)
    {
        for (int i = 0; i < TileableSourceCreator.Instance.buildableMods.Count; i++)
        {
            TileableMod tileableMod = TileableSourceCreator.Instance.buildableMods[i] as TileableMod;
            if (tileableMod == null)
            {
                continue;
            }

            if (!string.Equals(tileableMod.id, customTileable.modId))
            {
                if (i != ItemModSourceCreator.Instance.buildableMods.Count - 1)
                {
                    continue;
                }

                // Now the problem is there isn't a matching ID!
                SaveLoadSystem.Quicklog("[Buildable Problem] A custom item is in the processs of being loaded, but a custom item with the id in the save file doesn't exist. " +
                    "This probably means that a mod was uninstalled or its ID was changed. To fix this, either re-install the mod or revert any changes made to a mods id. " +
                    "The id being looked for is " + customTileable.modId + ". For help, contact Humoresque.", false);
                continue;
            }

            // The id is the same!
            TileableCreator.Instance.ConvertBuildableToCustom(worldItem.gameObject, i);
            return;
        }
    }

    private static void HandleCustomItem(CustomItemSerializable customItem, float spriteRotation, float itemRotation, PlaceableItem worldItem)
    {
        for (int i = 0; i < ItemModSourceCreator.Instance.buildableMods.Count; i++)
        {
            ItemMod itemMod = ItemModSourceCreator.Instance.buildableMods[i] as ItemMod;
            if (itemMod == null)
            {
                continue;
            }

            if (!string.Equals(itemMod.id, customItem.modId))
            {
                if (i != ItemModSourceCreator.Instance.buildableMods.Count - 1)
                {
                    continue;
                }

                // Now the problem is there isn't a matching ID!
                SaveLoadSystem.Quicklog("[Buildable Problem] A custom item is in the processs of being loaded, but a custom item with the id in the save file doesn't exist. " +
                    "This probably means that a mod was uninstalled or its ID was changed. To fix this, either re-install the mod or revert any changes made to a mods id. " +
                    "The id being looked for is " + customItem.modId + ". For help, contact Humoresque.", false);
                continue;
            }

            // The id is the same!
            if (itemMod.useRandomRotation)
            {
                ItemCreator.Instance.ConvertItemToCustom(worldItem.gameObject, i, true, spriteRotation, itemRotation);
                return;
            }

            ItemCreator.Instance.ConvertItemToCustom(worldItem.gameObject, i, false, spriteRotation, itemRotation);
            return;
        }
    }

    public static CustomSerializableWrapper GetCustomSaveData()
    {
        if (LoadedWrapper != null)
        {
            return LoadedWrapper;
        }

        string JSON = null;

        SaveLoadSystem.Quicklog("Trying to get save json");
        if (SaveLoadUtils.TryGetJSONFile(SaveLoadSystem.newSaveFileName, out JSON))
        {
            SaveLoadSystem.Quicklog("Got save json from new file");
        }
        else
        {
            SaveLoadSystem.Quicklog("Failed to get save json from new path... Trying old file!");
            if (SaveLoadUtils.TryGetJSONFile(SaveLoadSystem.originalSaveFileName, out JSON))
            {
                SaveLoadSystem.Quicklog("Got save json from old file");
            }
            else
            {
                SaveLoadSystem.Quicklog("Failed to get save json from old path as well... The CustomSaveData.json file does not exist. Skipped loading.", false);
                return null;
            }
        }

        // Now we know that the string has something...
        CustomSerializableWrapper customItemSerializableWrapper = null;
        try
        {
            customItemSerializableWrapper = JsonConvert.DeserializeObject<CustomSerializableWrapper>(JSON);
        }
        catch (Exception ex)
        {
            SaveLoadSystem.Quicklog("JSON deserialized failed! Error: " + ex.Message, true);
            return null;
        }

        if (customItemSerializableWrapper == null)
        {
            SaveLoadSystem.Quicklog("JSON deserialized object is null!!", true);
            return null;
        }

        return customItemSerializableWrapper;
    }
}