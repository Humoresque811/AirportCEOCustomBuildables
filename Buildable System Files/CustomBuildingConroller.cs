using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

class CustomBuildingController : MonoBehaviour
{
    /// <summary>
    /// Begins item placement
    /// </summary>
    /// <param name="item">The item itself</param>
    /// <param name="type">Mod Type</param>
    public static void SpawnItem(GameObject item, Type type)
    {
        // Get template
        ObjectPlacementController placementController = TemplateManager.objectPlacementControllerTemplate;

        if (item == null || placementController == null)
        {
            AirportCEOCustomBuildables.LogError("Hmm, Humoresque failed to code correctly. The item was null or the placment controller was. Fix it!!!");
            return;
        }

        try
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
            }

            if (Equals(type, typeof(FloorMod)))
            {
                if (!item.TryGetComponent(out PlaceableFloor placeableFloor))
                {
                    AirportCEOCustomBuildables.LogError("Failed to get placeableFloor component on item...?");
                    return;
                }

                //item.transform.GetChild(0).localScale = ItemCreator.Instance.calculateScale(item.transform.GetChild(0).gameObject, 1f, 1f);                        

                //VariationsHandler.CurrentVariationSizeType = placeableFloor.objectSize;
                //VariationsHandler.CurrentVariationQualityType = placeableFloor.objectQuality;
                VariationsHandler.currentVariationIndex = placeableFloor.variationIndex;
                placementController.SetObject(item, 0);
                return;
            }

            placementController.SetObject(item, 0);
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Failed to set object! {ExceptionUtils.ProccessException(ex)}");
        }
    }
}
