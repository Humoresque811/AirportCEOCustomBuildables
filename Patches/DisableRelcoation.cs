using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

[HarmonyPatch(typeof(PlaceableObject))]
[HarmonyPatch("CanBeRelocated")]
static class DisableRelcoation
{   
    public static void Postfix(PlaceableObject __instance, ref bool __result)
    {
        // If the item is custom
        if (!__instance.gameObject.transform.TryGetComponent(out CustomItemSerializableComponent mod))
        {
            return;
        }

        try
        {
            __result = false;
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Patch disable relocation failed. {ExceptionUtils.ProccessException(ex)}");
        }
    }
}