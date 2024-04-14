using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using System.Linq;
using Steamworks;
using AirportCEOModLoader.Core;
using Newtonsoft.Json;

namespace AirportCEOCustomBuildables;

static class SaveSubSystem
{
    public static string SaveProccess()
    {
        SaveLoadSystem.Quicklog("Starting AirportCEOCustomBuildables Save Proccess!", true);
        SaveLoadSystem.GetAllBuildablesArrays();

        try
        {
            foreach (PlaceableItem item in SaveLoadSystem.itemArray.ToList())
            {
                if (!item.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
                {
                    continue;
                }

                if (serializableComponent.itemIndex != serializableComponent.nullInt)
                {
                    SerializeItems(serializableComponent.itemIndex, item);
                }

                if (serializableComponent.tileableIndex != serializableComponent.nullInt)
                {
                    SerializeTileables(serializableComponent.tileableIndex, item);
                }
            }

            SerializeFloors();

            string JSON;
            CustomSerializableWrapper JSONWrapper = new CustomSerializableWrapper(SaveLoadSystem.itemJSONList, SaveLoadSystem.floorJSONList, SaveLoadSystem.tileableJSONList);
            JSON = JsonConvert.SerializeObject(JSONWrapper, Formatting.Indented); // We pretty print :)

            // No longer needed, they've been converted to JSON allready
            SaveLoadSystem.itemJSONList = new List<CustomItemSerializable>();
            SaveLoadSystem.floorJSONList = new List<CustomFloorSerializable>();
            SaveLoadSystem.tileableJSONList = new List<CustomTileableSerializable>();

            return JSON;
        }
        catch (Exception ex)
        {
            SaveLoadSystem.Quicklog($"[Error via Logger] Error in custom save code! {ExceptionUtils.ProccessException(ex)}");
        }

        return null;
    }

    private static void SerializeItems(in int itemIndex, in PlaceableItem item)
    {
        CustomItemSerializable customItemSerializable = SaveLoadSystem.SetItemSerializableInfo(itemIndex, item);
        if (customItemSerializable != null)
        {
            SaveLoadSystem.itemJSONList.Add(customItemSerializable);
            return;
        }
        SaveLoadSystem.Quicklog("Custom Serializable item was null!", false);
    }

    private static void SerializeTileables(in int tileableIndex, in PlaceableItem tileable)
    {
        CustomTileableSerializable customTileableSerializable = SaveLoadSystem.SetTileableSerializableInfo(tileableIndex, tileable);
        if (customTileableSerializable != null)
        {
            SaveLoadSystem.tileableJSONList.Add(customTileableSerializable);
            return;
        }
        SaveLoadSystem.Quicklog("Custom Serializable tileable was null!", false);
    }

    private static void SerializeFloors()
    {
        foreach (MergedTile mergedTile in SaveLoadSystem.zonesArray.ToList())
        {
            if (!mergedTile.gameObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
            {
                continue;
            }

            if (serializableComponent.floorIndex == serializableComponent.nullInt)
            {
                continue;
            }

            // So if it is custom, then...
            CustomFloorSerializable customFloorSerializable = SaveLoadSystem.SetFloorSeializableInfo(serializableComponent.floorIndex, mergedTile);
            if (customFloorSerializable != null)
            {
                SaveLoadSystem.floorJSONList.Add(customFloorSerializable);
                continue;
            }
            SaveLoadSystem.Quicklog("Custom Serializable floor was null!", false);
        }
    }
}