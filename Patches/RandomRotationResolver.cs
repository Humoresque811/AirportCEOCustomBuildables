using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

[HarmonyPatch(typeof(AttachedObjectPositioningHandler))]
[HarmonyPatch("ExecuteObjectPositioningAdjustment")]
static class RandomRotationResolver
{   
    // Who doesnt love alliteration?
    public static bool Prefix(AttachedObjectPositioningHandler __instance)
    {
        // If user specifies in config for no random rotation.
        if (!AirportCEOCustomBuildablesConfig.useRandomRotation.Value)
        {
            UnityEngine.Object.Destroy(__instance);
            return false;
        }

        // If the item is custom
        if (!__instance.gameObject.transform.parent.parent.TryGetComponent(out CustomItemSerializableComponent mod))
        {
            // Vanilla; go ahead
            return true;
        }

        try
        {
            if (mod.itemIndex == mod.nullInt)
            {
                return true;
            }

            ItemMod itemMod = FileManager.Instance.buildableTypes[typeof(ItemMod)].Item2.buildableMods[mod.itemIndex] as ItemMod;
            if (itemMod == null)
            {
                return true;
            }

            if (itemMod.useRandomRotation)
            {
                return true;
            }

            UnityEngine.Object.Destroy(__instance);
            return false;
        }
        catch (Exception ex)
        {
            // If it fails log and return true (because its vanilla)
            AirportCEOCustomBuildables.LogError($"Random Rotation Patch failed. {ExceptionUtils.ProccessException(ex)}");
            return true;
        }
    }
}