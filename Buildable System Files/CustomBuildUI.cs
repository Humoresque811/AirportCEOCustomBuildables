using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;
using AirportCEOModLoader.Core;

namespace AirportCEOCustomBuildables;

class CustomBuildUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button assignedButton;
    public GameObject assignedObject;
    public Animator assignedAnimator;
    public Type modType;

    public string buildableName;
    public string buildableDescription;
    public int buildableCost;
    public int buildableOperatingCost;

    public void ConvertButtonToCustom()
    {
        // Make the button custom and remove all things on click, re-add the appropriate ones
        try
        {
            assignedButton.onClick.RemoveAllListeners();
            assignedButton.onClick.AddListener(delegate ()
            {
                CustomBuildingController.SpawnItem(assignedObject, modType);
                Singleton<AudioController>.Instance.PlayAudio(Enums.AudioClip.PointerClick, false, 1f, 1f, false);
                Singleton<PlaceablePanelUI>.Instance.EnableDisableSearchFieldInput(false);
                Singleton<ObjectDescriptionPanelUI>.Instance.HidePanel();
                ObjectPlacementController.hasAttemptedBuild = false;
                EventSystem.current.SetSelectedGameObject(null);
            });
            assignedAnimator = assignedButton.GetComponent<Animator>();
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Error in convert button custom. {ExceptionUtils.ProccessException(ex)}");
        }
    }
    
    public void OnPointerEnter(PointerEventData eventdata)
    {
        Singleton<ObjectDescriptionPanelUI>.Instance.HidePanel();
        Singleton<VariationsHandler>.Instance.TogglePanel(false);
        try
        {
            assignedAnimator.Play("BounceButton");
            Singleton<AudioController>.Instance.PlayAudio(Enums.AudioClip.PointerEnter, true, 1f, 1f, false);
            ObjectDescriptionPanelUI ObjectDescriptionPanel = Singleton<ObjectDescriptionPanelUI>.Instance;
            ObjectDescriptionPanel.ShowTemplatePanel(assignedButton.transform, buildableName, buildableDescription);
            ObjectDescriptionPanel.contractorCostText.text = $"{LocalizationManager.GetLocalizedValue("ObjectDescriptionPanelUI.cs.key.21")} {buildableCost}";
            ObjectDescriptionPanel.operatingCostText.text = $"{LocalizationManager.GetLocalizedValue("ObjectDescriptionPanelUI.cs.key.27")} {buildableOperatingCost}";
            ObjectDescriptionPanel.objectImageInstructionText.text = "No Preview Availible For Custom Buildables...";

            if (this.modType != typeof(FloorMod))
            {
                return;
            }

            if (!assignedObject.TryGetComponent<CustomItemSerializableComponent>(out CustomItemSerializableComponent comp))
            {
                return;
            }

            if (!UIManager.IndexHasVariations(comp.floorIndex))
            {
                return;
            }

            Singleton<VariationsHandler>.Instance.SetVariations(assignedObject.GetComponent<PlaceableObject>(), this.transform, Color.white);
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"PointerEnter Custom Build UI Failed. {ExceptionUtils.ProccessException(ex)}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            assignedAnimator.Play("BounceDown");
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"PointerExit Custom Build UI Failed. {ExceptionUtils.ProccessException(ex)}");
        }
    }
	}
