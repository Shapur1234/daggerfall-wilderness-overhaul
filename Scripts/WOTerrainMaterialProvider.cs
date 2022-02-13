// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), Nystul, TheLacus
// Contributors:
//
// Notes:
//

using System;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop
{
    public class WOTerrainMaterialProvider : TerrainMaterialProvider
    {
        Mod mod;
        TextureReader textureReader;

        public WOTerrainMaterialProvider(Mod injectedMod) {
            mod = injectedMod;
            textureReader = new TextureReader(DaggerfallUnity.Instance.Arena2Path);
        }


        //private readonly Shader shader = Shader.Find("WildernessOverhaul/TilemapTextureArray");
        //private readonly Shader shader = Shader.Find("Daggerfall/TilemapTextureArray");

        internal static bool IsSupported
        {
            get { return SystemInfo.supports2DArrayTextures && DaggerfallUnity.Settings.EnableTextureArrays; }
        }

        public sealed override Material CreateMaterial()
        {
            return mod.GetAsset<Material>("WOTilemapTextureArrayMat", true);
        }

        public override void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData)
        {
            Material tileMaterial = GetTerrainTextureArrayMaterial(GetGroundArchive(terrainMaterialData.WorldClimate));

            // Assign textures (propagate material settings from tileMaterial to terrainMaterial)
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileNormalMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileNormalMapTexArr));
            if (tileMaterial.IsKeywordEnabled(KeyWords.NormalMap))
                terrainMaterialData.Material.EnableKeyword(KeyWords.NormalMap);
            else
                terrainMaterialData.Material.DisableKeyword(KeyWords.NormalMap);
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TilemapTex, terrainMaterialData.TileMapTexture);
        }

        public Material GetTerrainTextureArrayMaterial(int archive)
        {
            Material material;

            // Ready check
            /* if (!IsReady)
                return null; */

            // Return from cache if present
            /* int key = MakeTextureKey((short)archive, (byte)0, (byte)0, TileMapKeyGroup);
            if (materialDict.ContainsKey(key))
            {
                CachedMaterial cm = GetMaterialFromCache(key);
                if (cm.filterMode == MainFilterMode)
                {
                    // Properties are the same
                    return cm.material;
                }
                else
                {
                    // Properties don't match, remove material and reload
                    materialDict.Remove(key);
                }
            } */

            // Generate texture array
            Texture2DArray textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("302-TexArray.asset");
            if (archive == 002)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("002-TexArray.asset");
            if (archive == 003)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("003-TexArray.asset");
            if (archive == 004)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("004-TexArray.asset");
            if (archive == 102)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("102-TexArray.asset");
            if (archive == 103)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("103-TexArray.asset");
            if (archive == 104)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("104-TexArray.asset");
            if (archive == 302) {
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("302-TexArray.asset");
            }
            if (archive == 303)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("303-TexArray.asset");
            if (archive == 304)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("304-TexArray.asset");
            if (archive == 402)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("402-TexArray.asset");
            if (archive == 403)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("403-TexArray.asset");
            if (archive == 404)
                textureArrayTerrainTiles = mod.GetAsset<Texture2DArray>("404-TexArray.asset");
            Texture2DArray textureArrayTerrainTilesNormalMap = null; // = GetTerrainNormalMapTextureArray(archive);
            Texture2DArray textureArrayTerrainTilesMetallicGloss = null; // = GetTerrainMetallicGlossMapTextureArray(archive);
            textureArrayTerrainTiles.filterMode =  FilterMode.Point;

            material = CreateMaterial();
            material.name = string.Format("TEXTURE.{0:000} [TilemapTextureArray]", archive);

            material.SetTexture(TileTexArrUniforms.TileTexArr, textureArrayTerrainTiles);
            if (textureArrayTerrainTilesNormalMap != null)
            {
                // if normal map texture array was loaded successfully enable normalmap in shader and set texture
                material.SetTexture(TileTexArrUniforms.TileNormalMapTexArr, textureArrayTerrainTilesNormalMap);
                material.EnableKeyword(KeyWords.NormalMap);
            }
            if (textureArrayTerrainTilesMetallicGloss != null)
            {
                // if metallic gloss map texture array was loaded successfully set texture (should always contain a valid texture array - since it defaults to 1x1 textures)
                material.SetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr, textureArrayTerrainTilesMetallicGloss);
            }

            /* CachedMaterial newcm = new CachedMaterial()
            {
                key = key,
                keyGroup = TileMapKeyGroup,
                material = material,
                filterMode = MainFilterMode,
            };
            materialDict.Add(key, newcm); */

            return material;
        }

        private TextureFormat ParseTextureFormat(SupportedAlphaTextureFormats format)
        {
            switch (format)
            {
                default:
                case SupportedAlphaTextureFormats.RGBA32:
                    return TextureFormat.RGBA32;
                case SupportedAlphaTextureFormats.ARGB32:
                    return TextureFormat.ARGB32;
                case SupportedAlphaTextureFormats.ARGB444:
                    return TextureFormat.ARGB4444;
                case SupportedAlphaTextureFormats.RGBA444:
                    return TextureFormat.RGBA4444;
            }
        }
    }
}