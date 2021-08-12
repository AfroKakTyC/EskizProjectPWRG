using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using static DataTypes;


public class DataCacher : MonoBehaviour
{
	//static JsonSerializerSettings settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
	private static string _cachePath;

	public static List<DownloadedMaterial> GetCachedMaterialsJSONs()
	{
		List<DownloadedMaterial> cachedMaterialsJSONs = new List<DownloadedMaterial>();
		Debug.Log(_cachePath + "MaterialDataJSONS/");
		if (Directory.Exists(_cachePath + "MaterialDataJSONS/"))
		{
			string[] cachedFiles = Directory.GetFiles(_cachePath + "MaterialDataJSONS/");
			for (int i = 0; i < cachedFiles.Length; i++)
			{
				if (!cachedFiles[i].EndsWith(".meta"))
				{
					cachedMaterialsJSONs.Add(JsonConvert.DeserializeObject<DownloadedMaterial>(File.ReadAllText(cachedFiles[i])));
				}
			}
		}
		return cachedMaterialsJSONs;
	}

	public static string[] GetCachedMaterialsFoldersNames()
	{
		string[] _foldersPaths = Directory.GetDirectories(_cachePath);
		
		for (int i = 0; i < _foldersPaths.Length; i++)
		{

			int lastSlashIndex = _foldersPaths[i].LastIndexOf('/');
			_foldersPaths[i] = _foldersPaths[i].Substring(lastSlashIndex + 1);
		}
		return _foldersPaths;
	}

	public static void CacheTextures(DownloadedMaterial material)
	{
		var dirPath = _cachePath + "Textures/" + material.id + "/";
		if (Directory.Exists(dirPath))
		{
			Directory.Delete(dirPath, true);
			Debug.Log("Directory '" + dirPath + "' recreated");
		}
		Directory.CreateDirectory(dirPath);
		Debug.Log("Directory created '" + dirPath + "'");
		if (material.type == "paint" || material.type == "ceiling")
			material.MakeMaterialPreview();
		byte[] bytes;
		if (material.tex.tex_diffuse != "")
		{
			try
			{
				bytes = material.applyedTextures.tex_diffuse.EncodeToPNG();
				File.WriteAllBytes(dirPath + material.tex.tex_diffuse, bytes);
			}
			catch (System.Exception e)
			{
				material.tex.tex_diffuse = "";
				material.isValid = false;
				Debug.LogError(e.Message);

			}
		}
		if (material.tex.tex_normal != "")
		{
			try
			{
				bytes = material.applyedTextures.tex_normal.EncodeToPNG();
				File.WriteAllBytes(dirPath + material.tex.tex_normal, bytes);
			}
			catch (System.Exception e)
			{
				material.tex.tex_normal = "";
				Debug.LogError(e.Message);

			}
		}
		if (material.tex.tex_roughness != "")
		{
			try
			{
				bytes = material.applyedTextures.tex_roughness.EncodeToPNG();
				File.WriteAllBytes(dirPath + material.tex.tex_roughness, bytes);
			}
			catch (System.Exception e)
			{
				material.tex.tex_roughness = "";
				Debug.LogError(e.Message);

			}
		}
		if (material.preview_icon != "")
		{
			try
			{
				bytes = material.applyedTextures.tex_preview_icon.EncodeToPNG();
				File.WriteAllBytes(dirPath + material.preview_icon, bytes);
			}
			catch (System.Exception e)
			{
				material.preview_icon = "";
				material.isValid = false;
				Debug.LogError(e.Message);
			}
		}
		material.ClearApplyedTextures();
		Debug.LogWarning("texture saved to '" + dirPath + "'");
	}

	public static void CacheJSONMaterialData(DownloadedMaterial material)
	{
		var dirPath = _cachePath + "MaterialDataJSONS/";
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
			Debug.Log("Directory created '" + dirPath + "'");
		}
		File.WriteAllText(dirPath + material.id, JsonConvert.SerializeObject(material));
		Debug.Log(material.id + "Json writted");
	}

	public static void CacheJsonRoomData(string roomData, string roomName)
	{
		var dirPath = _cachePath + "RoomDataJSONS/";
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
			Debug.Log("Directory created '" + dirPath + "'");
		}
		string[] filesWithSameName = Directory.GetFiles(dirPath, roomName + "*");
		for (int i = 1; i < 100; i++)
		{
			if (Array.IndexOf(filesWithSameName, dirPath + roomName + i.ToString()) < 0)
			{ 
				File.WriteAllText(dirPath + roomName + i.ToString(), roomData);
				Debug.Log("Room " + roomName + " Json writted");
				return;
			}
		}
		Debug.LogError("Room " + roomName + " cannot be writed due to limit files with dame name");
	}

	public static void GetTexturesFromCache(DownloadedMaterial material)
	{
		var dirPath = _cachePath + "Textures/" + material.id + "/";
		if (material.tex.tex_diffuse != "")
		{
			Texture2D cachedTexture = new Texture2D(2, 2);
			if (File.Exists(dirPath + material.tex.tex_diffuse))
			{
				cachedTexture.LoadImage(File.ReadAllBytes(dirPath + material.tex.tex_diffuse));
			}
			material.applyedTextures.tex_diffuse = cachedTexture;
		}
		if (material.tex.tex_normal != "")
		{
			Texture2D cachedTexture = new Texture2D(2, 2);
			if (File.Exists(dirPath + material.tex.tex_diffuse))
			{
				cachedTexture.LoadImage(File.ReadAllBytes(dirPath + material.tex.tex_normal));
			}
			material.applyedTextures.tex_normal = cachedTexture;
		}
		if (material.tex.tex_roughness != "")
		{
			Texture2D cachedTexture = new Texture2D(2, 2);
			if (File.Exists(dirPath + material.tex.tex_diffuse))
			{
				cachedTexture.LoadImage(File.ReadAllBytes(dirPath + material.tex.tex_roughness));
			}
			material.applyedTextures.tex_roughness = cachedTexture;
		}
	}

	public static DownloadedMaterial GetMaterialById(string materialToFindId)
	{
		List<DownloadedMaterial> materials = GetCachedMaterialsJSONs();
		foreach (var material in materials)
		{
			if (material.id == materialToFindId)
				return (material);
		}
		return (null);
	}

	public static Texture2D GetPreviewFromCache(DownloadedMaterial material)
	{
		var dirPath = _cachePath + "Textures/" + material.id + "/";
		if (material.preview_icon != "")
		{
			Texture2D cachedTexture = new Texture2D(2, 2);
			cachedTexture.LoadImage(File.ReadAllBytes(dirPath + material.preview_icon));
			return (cachedTexture);
		}
		return (null);
	}

	public static void DeleteAllMaterialDataFromCache(DownloadedMaterial material)
	{
		var texturesPath = _cachePath + "Textures/" + material.id + "/";
		var jsonPath = _cachePath + "MaterialDataJSONS/";
		if (Directory.Exists(texturesPath))
		{
			Debug.Log(texturesPath + " Directiry deleted");
			Directory.Delete(texturesPath, true);
		}
		if (Directory.Exists(jsonPath))
		{
			string[] files = Directory.GetFiles(jsonPath, material.id + "*");
			foreach (string file in files)
			{
				Debug.Log(file + " Deleted");
				File.Delete(file);
			}
		}

	}


	private void Awake()
	{
#if UNITY_EDITOR
		_cachePath = Application.dataPath + "/Resources/";
#else
		_cachePath = Path.Combine(Application.persistentDataPath, "Resources");                                
#endif
		Debug.LogWarning(_cachePath);
	}
}
