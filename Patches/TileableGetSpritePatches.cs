using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;

namespace AirportCEOCustomBuildables;

[HarmonyPatch(typeof(GenericBuilder), "SetBuilderPiece")]
public static class TileableGetSpritePatches
{
    public static int setValue = -1;

    public static void Prefix(GenericBuilder __instance)
    {
        if (!__instance.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent serializableComponent))
        {
            return;
        }

        setValue = serializableComponent.tileableIndex;
    }
}

[HarmonyPatch(typeof(DataPlaceholderItems), "GetGenericPieceTypeByItem")]
public static class Patch_TileableGetSpriteOverride
{
    public static bool Prefix(Enums.BuilderPieceType pieceType, out Sprite __result)
    {
        if (TileableGetSpritePatches.setValue == -1)
        {
            __result = null;
            return true;
        }
        __result = TileableCreator.Instance.GetSprite(pieceType, TileableGetSpritePatches.setValue);
        TileableGetSpritePatches.setValue = -1;
        return false;
    }
}
