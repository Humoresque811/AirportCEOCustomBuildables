using AirportCEOModLoader.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOCustomBuildables;

class TileableCreator : MonoBehaviour, IBuildableCreator
{
    public static TileableCreator Instance { get; private set; }
    public List<GameObject> buildables { get; set; }
    private Dictionary<TileableMod, Texture2D> cachedSprites;

    public void SetUp()
    {
        Instance = this;
        buildables = new List<GameObject>();
        cachedSprites = new Dictionary<TileableMod, Texture2D>();
    }

    public void ClearBuildables()
    {
        buildables = new List<GameObject>();
        cachedSprites = new Dictionary<TileableMod, Texture2D>();
    }

    public void CreateBuildables()
    {
        buildables = new List<GameObject>();
        cachedSprites = new Dictionary<TileableMod, Texture2D>();

        for (int i = 0; i < FileManager.Instance.buildableTypes[typeof(TileableMod)].Item2.buildableMods.Count; i++)
        {
            TileableMod tileableMod = FileManager.Instance.buildableTypes[typeof(TileableMod)].Item2.buildableMods[i] as TileableMod;
            if (tileableMod == null)
            {
                AirportCEOCustomBuildables.LogError($"A buildable mod of TileableCreator is not an tileable mod!");
                continue;
            }

            try
            {
                GameObject template = TemplateManager.TileableTemplate;

                GameObject newTileable = GameObject.Instantiate(template, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

                if (!newTileable.TryGetComponent<DragableItem>(out DragableItem dragableItem))
                {
                    AirportCEOCustomBuildables.LogError($"Failed to get DraggableItem component of tileable!");
                    continue;
                }

                dragableItem.objectName = tileableMod.name;
                dragableItem.objectDescription = tileableMod.description;
                // dragableItem.itemType = (Enums.ItemType)EnumManager.ItemEnumCustomTileable; i wish I could use this, but it interferes
                dragableItem.Quality = (byte)(FileManager.Instance.tileableIndexAddative + i);
                dragableItem.variationIndex = FileManager.Instance.tileableIndexAddative + i;

                if (!newTileable.TryGetComponent<GenericBuilder>(out GenericBuilder genericBuilder))
                {
                    AirportCEOCustomBuildables.LogError($"Failed to get DraggableItem component of tileable!");
                    continue;
                }

                genericBuilder.hasCenterPiece = false;
                genericBuilder.hasDiagonals = false;

                FileManager.Instance.GetIconSprite(tileableMod, out Sprite iconSprite);
                dragableItem.descriptionSpriteOverrider = iconSprite;
                    
                CustomItemSerializableComponent comp = newTileable.AddComponent<CustomItemSerializableComponent>();
                comp.Setup(i, typeof(TileableMod));

                buildables.Add(newTileable);
                cachedSprites.Add(tileableMod, null);
                newTileable.SetActive(false);
            }
            catch (Exception ex)
            {
                AirportCEOCustomBuildables.LogError($"Creating tileable \"{tileableMod.name}\" failed. {ExceptionUtils.ProccessException(ex)}");
            }
        }
    }

    public void ConvertBuildableToCustom(GameObject buildable, int index)
    {
        TexturedBuildableMod texturedBuildableMod = FileManager.Instance.buildableTypes[typeof(TileableMod)].Item2.buildableMods[index];
        TileableMod tileableMod = texturedBuildableMod as TileableMod;
        if (tileableMod == null)
        {
            AirportCEOCustomBuildables.LogError($"Tileable mod not a tileable mod... Skipping conversion");
            return;
        }

        try
        {
            if (!buildable.TryGetComponent<DragableItem>(out DragableItem dragableItem))
            {
                AirportCEOCustomBuildables.LogError($"Failed to get DraggableItem component of tileable!");
                return;
            }

            dragableItem.objectName = tileableMod.name;
            dragableItem.objectDescription = tileableMod.description;
            dragableItem.Quality = (byte)(FileManager.Instance.tileableIndexAddative + index);
            dragableItem.variationIndex = FileManager.Instance.tileableIndexAddative + index;

            if (!buildable.TryGetComponent<GenericBuilder>(out GenericBuilder genericBuilder))
            {
                AirportCEOCustomBuildables.LogError($"Failed to get DraggableItem component of tileable!");
                return;
            }

            genericBuilder.hasCenterPiece = false;
            genericBuilder.hasDiagonals = false;

            FileManager.Instance.GetIconSprite(tileableMod, out Sprite iconSprite);
            dragableItem.descriptionSpriteOverrider = iconSprite;
                    
            CustomItemSerializableComponent comp = buildable.AddComponent<CustomItemSerializableComponent>();
            comp.Setup(index, typeof(TileableMod));

            genericBuilder.UpdatePiece();
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Creating tileable \"{tileableMod.name}\" failed. {ExceptionUtils.ProccessException(ex)}");
        }
    }

    public Sprite GetSprite(Enums.BuilderPieceType builderPieceType, int modIndexWithoutAddative)
    {
        try
        {
            TexturedBuildableMod TBMod = FileManager.Instance.buildableTypes[typeof(TileableMod)].Item2.buildableMods[modIndexWithoutAddative];
            TileableMod tileableMod = TBMod as TileableMod;
            if (tileableMod == null)
            {
                AirportCEOCustomBuildables.LogError($"Tileable mod is not tileable mod when getting sprite...");
                return null;
            }

            if (cachedSprites[tileableMod] == null)
            {
                if (!FileManager.Instance.GetTexture2DFromPath(tileableMod, out Texture2D texture2D))
                {
                    AirportCEOCustomBuildables.LogError($"File Manager failed to return a texture2D...");
                    return null;
                }

                cachedSprites[tileableMod] = texture2D;
            }

            Vector2 imageSizes = new Vector2(cachedSprites[tileableMod].width, cachedSprites[tileableMod].height);

            if (tileableMod.tileSize > imageSizes.y / 2)
            {
                AirportCEOCustomBuildables.LogWarning($"[Buildable Non-Critical Issue] Tileable mod \"{tileableMod.name}\" has an invalid tileSize. Check the modding docs or contact Humoresque (y)");
                tileableMod.tileSize = Mathf.FloorToInt(imageSizes.y / 2);
            }

            if (tileableMod.tileSize > imageSizes.x / 3)
            {
                AirportCEOCustomBuildables.LogWarning($"[Buildable Non-Critical Issue] Tileable mod \"{tileableMod.name}\" has an invalid tileSize. Check the modding docs or contact Humoresque (x)");
                tileableMod.tileSize = Mathf.FloorToInt(imageSizes.x / 3);
            }


            Rect rect = new Rect(0, 0, 1, 1);
            if (tileableMod.originalTexturePattern)
            {
                rect = CreateOriginalTextureRect(imageSizes, SpriteCuttingHelper.GetOriginalSpriteCuttingPoints(builderPieceType), tileableMod.tileSize);
            }
            else
            {
                AirportCEOCustomBuildables.LogError($"Non original rect used. Invalid option. ");
            }

            return Sprite.Create(cachedSprites[tileableMod], rect, new Vector2(0.5f, 0.5f), tileableMod.tileSize);
        }
        catch (Exception ex)
        {
            AirportCEOCustomBuildables.LogError($"Error occured in GetTileableSprite. {ExceptionUtils.ProccessException(ex)}");
            return null;
        }
    }

    private Rect CreateOriginalTextureRect(Vector2 imageSizes, Vector2 spriteCuttingPostions, int tileSize)
    {
        Rect rect = new Rect();

        rect.x = spriteCuttingPostions.x * tileSize;
        rect.y = spriteCuttingPostions.y * tileSize;
        rect.width = tileSize;
        rect.height = tileSize;

        return rect;
    }
}
