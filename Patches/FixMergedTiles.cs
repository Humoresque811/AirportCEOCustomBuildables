using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

[HarmonyPatch(typeof(MergedTile))]
[HarmonyPatch("SetTile")]
static class FixMergedTiles
{   
    public static void Prefix(MergedTile __instance, ITileable tile, out ITileable __state)
    {
        __state = tile;
    }

    public static void Postfix(MergedTile __instance, ITileable __state)
    {
        if (__state.Quality < FileManager.Instance.floorIndexAddative)
        {
            if (__instance.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
            {
                GameObject.Destroy(serializableComponent);
            }

            return;
        }

        __instance.spriteRenderer.tileMode = SpriteTileMode.Continuous;

        try
        {
            CustomItemSerializableComponent serializableComponent = __instance.gameObject.AddComponent<CustomItemSerializableComponent>();
            serializableComponent.Setup(__state.Quality - FileManager.Instance.floorIndexAddative, typeof(FloorMod));
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Failed to add custom component to Merged Tile. {ExceptionUtils.ProccessException(ex)}");
            return;
        }
    }
}