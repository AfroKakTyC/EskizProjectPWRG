using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;

public class MaterialBuilder : MonoBehaviour
{

    public static Material GetMaterial(DownloadedMaterial material)
    {
        Material materialFromTextures = new Material(Shader.Find("Pavel/CustomPBS"));

        DataCacher.GetTexturesFromCache(material);
        Vector2 textureScale = new Vector2(material.texture_dimensions.x, material.texture_dimensions.y);
        if (material != null)
        {
            materialFromTextures.name = material.id;

            DataCacher.GetTexturesFromCache(material);
            if (material.tex.tex_diffuse != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_MainTex"), material.applyedTextures.tex_diffuse);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
            }
            if (material.tex.tex_normal != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_BumpMap"), material.applyedTextures.tex_normal);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
            }
            if (material.tex.tex_roughness != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_GlossMap"), material.applyedTextures.tex_roughness);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_GlossMap"), textureScale);
            }
            material.ClearApplyedTextures();
        }
        return (materialFromTextures);
    }

    public static Material GetMaterial(string materialId)
    {
        Material materialFromTextures = new Material(Shader.Find("Pavel/CustomPBS"));
        if (materialId == "default" || materialId == "DefaultMaterial")
        {
            materialFromTextures = Resources.Load<Material>("Default/DefaultMaterial");
            return materialFromTextures;
        }
        DownloadedMaterial material = DataCacher.GetMaterialById(materialId);
        if (material != null)
        {
            materialFromTextures.name = materialId;

            Vector2 textureScale = new Vector2(material.texture_dimensions.x, material.texture_dimensions.y);
            DataCacher.GetTexturesFromCache(material);
            if (material.tex.tex_diffuse != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_MainTex"), material.applyedTextures.tex_diffuse);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
            }
            if (material.tex.tex_normal != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_BumpMap"), material.applyedTextures.tex_normal);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
            }
            if (material.tex.tex_roughness != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_GlossMap"), material.applyedTextures.tex_roughness);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_GlossMap"), textureScale);
            }
            material.ClearApplyedTextures();
        }
        return (materialFromTextures);
    }

    public static Material GetMaterial(string materialId, string objectType)
    {
        Material materialFromTextures;
        if (objectType == "baseboard")
        {
            materialFromTextures = new Material(Shader.Find("Custom/CustomPBSBaseBoard"));

        }
        else
		{
            materialFromTextures = new Material(Shader.Find("Pavel/CustomPBS"));

        }
        if (materialId == "default" || materialId == "DefaultMaterial")
        {
            materialFromTextures = Resources.Load<Material>("Default/DefaultMaterial");
            return materialFromTextures;
        }
        DownloadedMaterial material = DataCacher.GetMaterialById(materialId);
        if (material != null)
        {
            materialFromTextures.name = materialId;

            Vector2 textureScale = new Vector2(material.texture_dimensions.x, material.texture_dimensions.y);
            DataCacher.GetTexturesFromCache(material);
            if (material.tex.tex_diffuse != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_MainTex"), material.applyedTextures.tex_diffuse);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
            }
            if (material.tex.tex_normal != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_BumpMap"), material.applyedTextures.tex_normal);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
            }
            if (material.tex.tex_roughness != "")
            {
                materialFromTextures.SetTexture(Shader.PropertyToID("_GlossMap"), material.applyedTextures.tex_roughness);
                materialFromTextures.SetTextureScale(Shader.PropertyToID("_GlossMap"), textureScale);
            }
            material.ClearApplyedTextures();
        }
        return (materialFromTextures);
    }

    #region backup
    //public static Material GetMaterial(DownloadedMaterial material)
    //{
    //       Material materialFromTextures = new Material(Shader.Find("Pavel/CustomPBS"));
    //       materialFromTextures.name = material.id;
    //       DataCacher.GetTexturesFromCache(material);
    //       Vector2 textureScale = new Vector2(material.texture_dimensions.x, material.texture_dimensions.y);
    //       if (material != null)
    //       {
    //           DataCacher.GetTexturesFromCache(material);
    //           if (material.tex.tex_diffuse != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_MainTex"), material.applyedTextures.tex_diffuse);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
    //           }
    //           if (material.tex.tex_normal != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_BumpMap"), material.applyedTextures.tex_normal);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
    //           }
    //           if (material.tex.tex_roughness != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_SmoothnessTexture"), material.applyedTextures.tex_roughness);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_SmoothnessTexture"), textureScale);
    //           }
    //           material.ClearApplyedTextures();
    //       }
    //       return (materialFromTextures);
    //}

    //   public static Material GetMaterial(string materialId)
    //   {
    //       Material materialFromTextures = new Material(Shader.Find("Pavel/CustomPBS"));
    //       materialFromTextures.name = materialId;
    //       DownloadedMaterial material = DataCacher.GetMaterialById(materialId);
    //       if (material != null)
    //       {
    //           Vector2 textureScale = new Vector2(material.texture_dimensions.x, material.texture_dimensions.y);
    //           DataCacher.GetTexturesFromCache(material);
    //           if (material.tex.tex_diffuse != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_MainTex"), material.applyedTextures.tex_diffuse);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
    //           }
    //           if (material.tex.tex_normal != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_BumpMap"), material.applyedTextures.tex_normal);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
    //           }
    //           if (material.tex.tex_roughness != "")
    //           {
    //               materialFromTextures.SetTexture(Shader.PropertyToID("_SmoothnessTexture"), material.applyedTextures.tex_roughness);
    //               materialFromTextures.SetTextureScale(Shader.PropertyToID("_SmoothnessTexture"), textureScale);
    //           }
    //           material.ClearApplyedTextures();
    //       }
    //       return (materialFromTextures);
    //   }
    #endregion
}
