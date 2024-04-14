using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Nodes;
using System.Net;
using Newtonsoft.Json;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

class SaveLoadSystem : MonoBehaviour
{
    // Settings
    public static readonly string originalSaveFileName = "CustomSaveData";
    public static readonly string newSaveFileName = "CustomBuildablesData";

    // Buildable Arrays
    //public static List<PlaceableStructure> structureArray; Sructure array not fetched for performance reasons
    public static List<TaxiwayNode> taxiwayNodesArray;
    public static DynamicSimpleArray<PlaceableItem> itemArray;
    public static MergedTile[] zonesArray;
    public static DynamicArray<PlaceableRoom> roomArray;
    public static AircraftController[] aircraftArray;
    public static VehicleController[] vehicleArray;
    public static TrailerController[] trailerArray;
    public static PersonController[] personArray;
    public static DynamicSimpleArray<AssetController> assetArray;

    // JSON vars
    [SerializeField] public static List<CustomItemSerializable> itemJSONList = new List<CustomItemSerializable>();
    [SerializeField] public static List<CustomFloorSerializable> floorJSONList = new List<CustomFloorSerializable>();
    [SerializeField] public static List<CustomTileableSerializable> tileableJSONList = new List<CustomTileableSerializable>();

    // Function Utilities
    public static void Quicklog(string message, bool logAsUnityError = false)
    {
        if (logAsUnityError)
        {
            Debug.LogError("[SaveLoadUtility] " + message);
        }
        AirportCEOCustomBuildables.LogInfo("[SaveLoadUtility] " + message);
    }

    public static void GetAllBuildablesArrays()
    {
        try
        {
            // Structure array still needs to be added, its more complicated
            taxiwayNodesArray = Singleton<TaxiwayController>.Instance.GetSerializableTaxiwayNodeList();
            zonesArray = TileMerger.mergedTiles.ToArray();
            roomArray = Singleton<BuildingController>.Instance.allRoomsArray;
            itemArray = Singleton<BuildingController>.Instance.allItemsArray;
            aircraftArray = Singleton<AirTrafficController>.Instance.GetAircraftList();
            vehicleArray = Singleton<TrafficController>.Instance.GetVehicleArray();
            trailerArray = Singleton<TrafficController>.Instance.GetTrailersArray();
            personArray = Singleton<AirportController>.Instance.GetAllPersons();
            assetArray = Singleton<AirportController>.Instance.allAssetsArray;
        }
        catch (Exception ex)
        {
            Quicklog($"[Error via Logger] Error getting buildable arrays! {ExceptionUtils.ProccessException(ex)}", true);
        }
    }

    public static CustomItemSerializable SetItemSerializableInfo(int index, PlaceableItem item)
    {
        try
        {
            // Transfer vars and nessesary info
            CustomItemSerializable returnItem = new CustomItemSerializable();
            returnItem.modId = ItemModSourceCreator.Instance.buildableMods[index].id;
            returnItem.spriteRotation = Mathf.Round(item.transform.GetChild(0).GetChild(0).transform.eulerAngles.z);
            returnItem.itemRotation = Mathf.Round(item.transform.eulerAngles.z);

            returnItem.postion[0] = item.transform.position.x;
            returnItem.postion[1] = item.transform.position.y;
            returnItem.postion[2] = item.transform.position.z;

            returnItem.floor = item.Floor;
            returnItem.referenceID = item.ReferenceID;
            return returnItem;
        }
        catch (Exception ex)
        {
            Quicklog($"[Error via Logger] Error occured in setting a serializable item class. {ExceptionUtils.ProccessException(ex)}", true);
            return null;
        }
    }

    public static CustomFloorSerializable SetFloorSeializableInfo(int index, MergedTile mergedTile)
    {
        try
        {
            CustomFloorSerializable returnFloor = new CustomFloorSerializable();
            returnFloor.modId = FloorModSourceCreator.Instance.buildableMods[index].id;

            returnFloor.position[0] = mergedTile.transform.position.x;
            returnFloor.position[1] = mergedTile.transform.position.y;
            returnFloor.position[2] = mergedTile.transform.position.z;

            returnFloor.size[0] = mergedTile.spriteRenderer.size[0];
            returnFloor.size[1] = mergedTile.spriteRenderer.size[1];

            returnFloor.tileType = mergedTile.TileType.ToString();
            returnFloor.floor = mergedTile.Floor;

            return returnFloor;
        }
        catch (Exception ex)
        {
            Quicklog($"[Error via Logger] Error occured in setting a serializable floor class. {ExceptionUtils.ProccessException(ex)}", true);
            return null;
        }
    }

    public static CustomTileableSerializable SetTileableSerializableInfo(int index, PlaceableItem item)
    {
        try
        {
            // Transfer vars and nessesary info
            CustomTileableSerializable returnItem = new CustomTileableSerializable();
            returnItem.modId = TileableSourceCreator.Instance.buildableMods[index].id;

            returnItem.position[0] = item.transform.position.x;
            returnItem.position[1] = item.transform.position.y;
            returnItem.position[2] = item.transform.position.z;

            returnItem.floor = item.Floor;
            returnItem.referenceID = item.ReferenceID;
            return returnItem;
        }
        catch (Exception ex)
        {
            Quicklog($"[Error via Logger] Error occured in setting a serializable tileable class. {ExceptionUtils.ProccessException(ex)}", true);
            return null;
        }
    }

    public static void CreateJSON(string path, string fileName = "CustomSaveData.json")
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        // Make sure the array isn't empty!
        if (!((itemJSONList != null && itemJSONList.Count != 0) || (floorJSONList != null && floorJSONList.Count != 0) || (tileableJSONList != null && tileableJSONList.Count != 0)))
        {
            return;
        }

        // Get basepath, add it if not allready there
        string basepath = Singleton<SaveLoadGameDataController>.Instance.GetUserSavedDataSearchPath();
        if (!string.Equals(path.SafeSubstring(0, 2), "C:"))
        {
            path = Path.Combine(basepath.Remove(basepath.Length - 1), path);
        }

        // Make sure the directory does exist
        if (!Directory.Exists(path))
        {
            Quicklog("The directory for saving does not exist! The path was \"" + path + "\".", true);
            return;
        }

        // Add the filename itself
        path = Path.Combine(path, fileName);
        Quicklog("Full path is \"" + path + "\"", false);

        // Make sure the file doesn't allready exist
        if (File.Exists(path))
        {
            Quicklog("The file allready exists!", true);
            return;
        }


        // JSON creation vars and systems
        string JSON;
        CustomSerializableWrapper JSONWrapper = new CustomSerializableWrapper(itemJSONList, floorJSONList, tileableJSONList);
        try
        {
            JSON = JsonConvert.SerializeObject(JSONWrapper, Formatting.Indented); // We pretty print :)
        }
        catch (Exception ex)
        {
            Quicklog("Error converting classes to JSON. Error: " + ex.Message, true);
            return;
        }


        // Saving
        string exception;
        try
        {
            Utils.TryWriteFile(JSON, path, out exception);
            if (!string.IsNullOrEmpty(exception))
            {
                Quicklog("An exception occured! It was: " + exception, true);
                return;
            }
        }
        catch (Exception ex)
        {
            Quicklog("Outer error writing file to JSON. Error: " + ex.Message, true);
            return;
        }

        Quicklog("JSON creation succesfull, and JSON saving finished! Yay", true);
    }
}