using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using static DataTypes;

public class DataFromServerDownloader : MonoBehaviour
{
	public Material testMaterial;
	public GameObject cube;

	[SerializeField]
	int countCoroutunesPerCycle = 10;

	public static string devUri = @"https://apidev.ezkiz.ru";
	public static string prodUri = @"https://api.ezkiz.ru";

	const string get_products = @"/api/products";
	const string get_categories = @"/api/categories";
	const string get_ceilings = @"/api/ceilings";
	const string get_shops = @"/api/stores";
	const string post_orders = @"/api/orders";
	const string api = @"/api/";
	const string get_banner = @"/api/banner";
	const string get_rooms = @"/api/rooms?paginate=false";

	bool isCeilingsOn = false;

	static int totalAmountOfMaterials = 0;
	static int totalAmountOfCeilings = 0;
	static int downloadedMaterialsCount = 0;
	static int vallidTexturesCount = 0;
	public static List<DownloadedMaterial> cachedMaterials = new List<DownloadedMaterial>();
	public static List<DownloadedMaterial> downloadedMaterials = new List<DownloadedMaterial>();
	public static List<DownloadedMaterial> accesibleMaterials = new List<DownloadedMaterial>();
	public static List<LegacyRoomData> downloadedLegacyRooms = new List<LegacyRoomData>();
	public static List<Material> testMaterials = new List<Material>();

	static string _cachePath;




	public static IEnumerator GetDataByLink<T>(string link)
	{
		using (var www = UnityWebRequest.Get(link))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			yield return www.SendWebRequest();

			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{
#if DEBUG_QUERIES
				Debug.LogWarning(www.error);
#endif
				Debug.LogError(www.error);
				yield break;
			}

			var text = www.downloadHandler.text;
			Debug.Log(text);
#if DEBUG_QUERIES
			MyLogger.Log("Loading from " + link + " completed.\nLoadedRawData:\n" + text);
#endif
			DownloadedMaterialsWithHeader downloadedMaterialsWithHeader = JsonConvert.DeserializeObject<DownloadedMaterialsWithHeader>(text);
			Debug.Log(downloadedMaterialsWithHeader.data.Count);
			//PrintDataToFile(downloadedMaterialsWithHeader, skip);
			SaveMaterialsToList(downloadedMaterialsWithHeader);
		}
		yield break;
	}


	public IEnumerator GetLegacyRoomsData()
	{
		string url = devUri + get_rooms;
		using (var www = UnityWebRequest.Get(url))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			yield return www.SendWebRequest();

			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{

#if DEBUG_QUERIES
				Debug.LogWarning(www.error);
#endif
				Debug.LogError(www.error);
				Debug.LogError(url);
				yield break;
			}
			var text = www.downloadHandler.text;

#if DEBUG_QUERIES
			MyLogger.Log("Loading from " + link + " completed.\nLoadedRawData:\n" + text);
#endif
			try
			{
				LegacyRoomData[] roomsData = JsonConvert.DeserializeObject<LegacyRoomData[]>(text);
				for (int i = 0; i < roomsData.Length; i++)
				{
					downloadedLegacyRooms.Add(roomsData[i]);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.Message + "\nJSON converting error on link " + url);
			}
		}
		yield break;
	}

	public IEnumerator GetDataByLinkWithSkip<T>(string link, int skip, int ammount)
	{
		string url = link + "?query={\"$skip\":" + skip + ",\"$limit\":" + ammount + "}";
		using (var www = UnityWebRequest.Get(url))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			yield return www.SendWebRequest();

			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{

#if DEBUG_QUERIES
				Debug.LogWarning(www.error);
#endif

				yield break;
			}
			var text = www.downloadHandler.text;

#if DEBUG_QUERIES
			MyLogger.Log("Loading from " + link + " completed.\nLoadedRawData:\n" + text);
#endif
			if (link == devUri + get_ceilings)
				Debug.Log(string.Format("Downloading {0} of {1} ceiling data", skip, totalAmountOfCeilings));
			else
				Debug.Log(string.Format("Downloading {0} of {1} material data", skip, totalAmountOfMaterials));
			try
			{
				DownloadedMaterialsWithHeader downloadedMaterialsWithHeader = JsonConvert.DeserializeObject<DownloadedMaterialsWithHeader>(text);

				if (isCeilingsOn)
				{
					if (link == devUri + get_ceilings)
					{
						for (int i = 0; i < downloadedMaterialsWithHeader.data.Count; i++)
						{
							downloadedMaterialsWithHeader.data[i].type = "ceiling";
						}
					}
				}
				//PrintDataToFile(downloadedMaterialsWithHeader, skip);
				SaveMaterialsToList(downloadedMaterialsWithHeader);

			}
			catch (System.Exception e)
			{
				PrintDataToFile(String.Format("JSON convertion error on link '{0}'\n{1}\n********\n", url, e.Message));
				
				Debug.LogError(e.Message + "\nJSON converting error on link " + url);
			}
		}
		yield break;
	}

	public static void SaveMaterialsToList(DownloadedMaterialsWithHeader downloadedMaterialsWithHeader)
	{
		for (int i = 0; i < downloadedMaterialsWithHeader.data.Count; i++)
		{
			DownloadedMaterial cachedMaterial = FindDownloadedMaterialInList(downloadedMaterialsWithHeader.data[i], cachedMaterials);
			if (cachedMaterial != null)
			{
				Debug.Log(string.Format("Material {0} is found in cache", cachedMaterial.id));

				DateTime newMaterialUpdateTime;
				DateTime cachedMaterialUpdateTime;
				if (DateTime.TryParse(downloadedMaterialsWithHeader.data[i].updatedAt, out newMaterialUpdateTime)
					&& DateTime.TryParse(cachedMaterial.updatedAt, out cachedMaterialUpdateTime))
				{
					if (newMaterialUpdateTime > cachedMaterialUpdateTime)
					{
						Debug.Log(string.Format("Updated version of material is added to predownload list"));
						downloadedMaterials.Add(downloadedMaterialsWithHeader.data[i]);
					}
				}
				else
				{
					Debug.LogError(string.Format("Can't parse update time on material '{0}'\nMaterial is not added", downloadedMaterialsWithHeader.data[i].id));
				}
				cachedMaterials.Remove(cachedMaterial);
			}
			else
			{
				Debug.Log(string.Format("New material '{0}' is added to predownload list", downloadedMaterialsWithHeader.data[i].id));
				downloadedMaterials.Add(downloadedMaterialsWithHeader.data[i]);
			}
		}
	}

	public IEnumerator CheckTextureServerAccesibility(string url, DownloadedMaterial downloadedMaterial, string textureType)
	{
		//using (var www = UnityWebRequestTexture.GetTexture(url))
		using (var www = UnityWebRequest.Get(url))
		{


			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");

			long responseCode = 0;
			www.SendWebRequest();

			while (responseCode == 0)
			{
				responseCode = www.responseCode;
				yield return null;
			}

			if (responseCode != 200)
			{
				Debug.LogError("Invalid texture on link " + url + "\n" + www.error);
				PrintDataToFile("Invalid texture on link " + url + "\n" + www.error);
				PrintDataToFile(string.Format("Material {0} '{1}' '{2}'\n********\n", downloadedMaterial.id, downloadedMaterial.name, downloadedMaterial.type));
				if (textureType == "diffuse")
					downloadedMaterial.tex.tex_diffuse = "";
				else if (textureType == "normal")
					downloadedMaterial.tex.tex_normal = "";
				else if (textureType == "roughness")
					downloadedMaterial.tex.tex_roughness = "";
				else if (textureType == "preview")
					downloadedMaterial.preview_icon = "";
				yield break;
			}
			else
			{
				vallidTexturesCount++;
			}
			www.downloadHandler.Dispose();
			www.Dispose();
		}
		yield break;
	}

	public IEnumerator LoadTextureRoutine(string url, DownloadedMaterial material, string textureType, string loadingText = "", bool shouldCompress = true)
	{
		using (var www = UnityWebRequestTexture.GetTexture(url))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			UnityWebRequestAsyncOperation sending = www.SendWebRequest();

			yield return sending;
			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError("Download texture Error on link '" + url + "'\n" + www.error);
				PrintDataToFile("Download texture Error on link '" + url + "'\n" + www.error);
				PrintDataToFile(string.Format("Material {0} '{1}' '{2}'\n********\n", material.id, material.name, material.type));

				material.isValid = false;
				yield break;
			}
			else
			{
				Texture2D tex = DownloadHandlerTexture.GetContent(www);
				//if (shouldCompress)
				//{
				//	float scale = Mathf.Max(1f, tex.width / (float)maxSize, tex.height / (float)maxSize);
				//	if (scale > 1f) tex = Resize(tex, Mathf.RoundToInt(tex.width / scale), Mathf.RoundToInt(tex.height / scale));
				//	tex.Compress(false);
				//}
				tex.Apply(true);
				if (textureType == "diffuse")
				{
					material.applyedTextures.tex_diffuse = tex;
				}
				else if (textureType == "normal")
				{
					material.applyedTextures.tex_normal = tex;
				}
				else if (textureType == "roughness")
				{
					material.applyedTextures.tex_roughness = tex;
				}
				else if (textureType == "preview")
				{
					material.applyedTextures.tex_preview_icon = tex;
				}
				//textureToApply = tex;
				var renderer = cube.GetComponent<Renderer>();
				////testMaterial.SetTexture("")
				//renderer.material.mainTexture = tex;
				renderer.material.mainTexture = material.applyedTextures.tex_diffuse;
				downloadedMaterialsCount++;
				Debug.Log(string.Format("{0} of {1} texture loaded", downloadedMaterialsCount, vallidTexturesCount));
#if DEBUG_LOAD_TEXTURES
				Debug.Log($"Texture loaded!\nurl: {url}");
#endif
				//onLoad(tex);
			}
			www.downloadHandler.Dispose();
			www.Dispose();
		}
		yield break;
	}

	public static void PrintData(DownloadedMaterialsWithHeader downloadedMaterialsWithHeader)
	{
		Debug.Log("count = " + downloadedMaterialsWithHeader.data.Count);
		Debug.Log("count = " + downloadedMaterialsWithHeader.data[0].cost);
		for (int i = 0; i < downloadedMaterialsWithHeader.data.Count; i++)
		{
			Debug.Log(string.Format("name = {0} type = {1}", downloadedMaterialsWithHeader.data[i].name,
			downloadedMaterialsWithHeader.data[i].type));
		}
	}

	public static void PrintDataToFile(DownloadedMaterialsWithHeader downloadedMaterialsWithHeader, int index)
	{
		using (StreamWriter sw = new StreamWriter("D:\\test\\log.txt", true, System.Text.Encoding.Default))
		{
			for (int i = 0; i < downloadedMaterialsWithHeader.data.Count; i++)
			{
				sw.WriteLine(string.Format("{0} name = {1} type = {2} '{3}' '{4}' '{5}' '{6}'", downloadedMaterialsWithHeader.data[i].id, downloadedMaterialsWithHeader.data[i].name,
				downloadedMaterialsWithHeader.data[i].type, downloadedMaterialsWithHeader.data[i].tex.tex_diffuse,
				downloadedMaterialsWithHeader.data[i].tex.tex_normal, downloadedMaterialsWithHeader.data[i].tex.tex_roughness,
				downloadedMaterialsWithHeader.data[i].preview_icon));
				sw.WriteLine(string.Format("texture dimensions:  x = {0}  y = {1}  z = {2}", downloadedMaterialsWithHeader.data[i].texture_dimensions.x,
					downloadedMaterialsWithHeader.data[i].texture_dimensions.y, downloadedMaterialsWithHeader.data[i].texture_dimensions.z));
			}
		}
	}

	public static void PrintDataToFile(string data)
	{
		using (StreamWriter sw = new StreamWriter("D:\\test\\log.txt", true, System.Text.Encoding.Default))
		{

			sw.WriteLine(data);

		}
	}

	public static IEnumerator GetTotalAmountOfMaterials()
	{
		using (var www = UnityWebRequest.Get(devUri + get_products))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			yield return www.SendWebRequest();

			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{
#if DEBUG_QUERIES
				Debug.LogWarning(www.error);
#endif
				Debug.LogError(www.error);
				yield break;
			}

			var text = www.downloadHandler.text;
			//Debug.Log(text);
#if DEBUG_QUERIES
			MyLogger.Log("Loading from " + link + " completed.\nLoadedRawData:\n" + text);
#endif
			DownloadedMaterialsWithHeader downloadedMaterialsWithHeader = JsonConvert.DeserializeObject<DownloadedMaterialsWithHeader>(text);
			totalAmountOfMaterials = downloadedMaterialsWithHeader.total;
		}
		yield break;
	}

	public static IEnumerator GetTotalAmountOfCeilings()
	{
		using (var www = UnityWebRequest.Get(devUri + get_ceilings))
		{
			www.SetRequestHeader("Authorization", "Token ezkiz-jwt-secret");
			yield return www.SendWebRequest();

			while (!www.isDone) yield return null;
			if (www.isNetworkError || www.isHttpError)
			{
#if DEBUG_QUERIES
				Debug.LogWarning(www.error);
#endif
				Debug.LogError(www.error);
				yield break;
			}

			var text = www.downloadHandler.text;
			//Debug.Log(text);
#if DEBUG_QUERIES
			MyLogger.Log("Loading from " + link + " completed.\nLoadedRawData:\n" + text);
#endif
			DownloadedMaterialsWithHeader downloadedMaterialsWithHeader = JsonConvert.DeserializeObject<DownloadedMaterialsWithHeader>(text);
			totalAmountOfCeilings = downloadedMaterialsWithHeader.total;
		}
		yield break;
	}

	public static void DeleteInvalidMaterials()
	{
		for (int i = 0; i < accesibleMaterials.Count; i++)
		{
			if (!accesibleMaterials[i].isValid)
			{
				accesibleMaterials.RemoveAt(i);
				i--;
			}
		}
	}

	IEnumerator DownloadTextureForOneMaterial(DownloadedMaterial downloadedMaterial)
	{
		if (downloadedMaterial.tex.tex_diffuse != "")
		{
			string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterial.id + "_" + downloadedMaterial.tex.tex_diffuse;
			LoadTextureRoutine(url, downloadedMaterial, "diffuse", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
		}
		if (downloadedMaterial.tex.tex_normal != "")
		{
			string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterial.id + "_" + downloadedMaterial.tex.tex_normal;
			LoadTextureRoutine(url, downloadedMaterial, "normal", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
		}
		if (downloadedMaterial.tex.tex_roughness != "")
		{
			string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterial.id + "_" + downloadedMaterial.tex.tex_roughness;
			LoadTextureRoutine(url, downloadedMaterial, "roughness", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
		}
		if (downloadedMaterial.preview_icon != "")
		{
			string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterial.id + "_" + downloadedMaterial.preview_icon;
			LoadTextureRoutine(url, downloadedMaterial, "preview", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
		}
		while (CoroutineExtension.GroupProcessing("ApplyingTexturesToMaterials"))
		{
			yield return null;
		}
		DataCacher.CacheTextures(downloadedMaterial);
		DataCacher.CacheJSONMaterialData(downloadedMaterial);
		yield break;
	}

	IEnumerator MakeRoomsWithDelay()
	{
		for (int i = 0; i < downloadedLegacyRooms.Count; i++)
		{
			RoomCreator.CreateRoomFromLegacy(downloadedLegacyRooms[i]);
			yield return new WaitForSeconds(5f);
			RoomCreator.DestroyRoom();
		}
	}

	IEnumerator GlobalCoroutine()
	{
		GetLegacyRoomsData().ParallelCoroutinesGroup(this, "GettingLegacyRoomData");
		while (CoroutineExtension.GroupProcessing("GettingLegacyRoomData"))
			yield return null;
		//RoomCreator.CreateRoomFromLegacy(downloadedLegacyRooms[0]);
		RoomCreator.CreateRoomFromLegacy(downloadedLegacyRooms[0]);

		//StartCoroutine(MakeRoomsWithDelay());

		GetTotalAmountOfMaterials().ParallelCoroutinesGroup(this, "GettingAmountOfMaterials");
		while (CoroutineExtension.GroupProcessing("GettingAmountOfMaterials"))
			yield return null;
		Debug.LogWarning("total materials count = " + totalAmountOfMaterials);

		if (isCeilingsOn)
		{
			GetTotalAmountOfCeilings().ParallelCoroutinesGroup(this, "GettingAmountOfCeilings");
			while (CoroutineExtension.GroupProcessing("GettingAmountOfCeilings"))
				yield return null;
			Debug.LogWarning("total ceilings count = " + totalAmountOfCeilings);
		}

		//totalAmountOfMaterials = 10;
		for (int i = 0; i < totalAmountOfMaterials; i += countCoroutunesPerCycle)
		{
			for (int j = i; j < i + countCoroutunesPerCycle; j++)
			{
					GetDataByLinkWithSkip<DownloadedMaterial>(devUri + get_products, j, 1).ParallelCoroutinesGroup(this, "FromServerDownloads");
			}
			while (CoroutineExtension.GroupProcessing("FromServerDownloads"))
				yield return null;
		}

		if (isCeilingsOn)
		{
			for (int i = 0; i < totalAmountOfCeilings; i += countCoroutunesPerCycle)
			{
				for (int j = i; j < i + countCoroutunesPerCycle; j++)
				{
					GetDataByLinkWithSkip<DownloadedMaterial>(devUri + get_ceilings, j, 1).ParallelCoroutinesGroup(this, "CeilingsFromServerDownloads");
				}
				while (CoroutineExtension.GroupProcessing("CeilingsFromServerDownloads"))
					yield return null;
			}
		}

		Debug.LogWarning("Materials has been downloaded from server\nDownloaded materials count = " + downloadedMaterials.Count);

		string[] _cachedDirectoriesNames = DataCacher.GetCachedMaterialsFoldersNames();
		for (int i = 0; i < downloadedMaterials.Count; i += countCoroutunesPerCycle)
		{
			for (int j = i; j < i + countCoroutunesPerCycle && j < downloadedMaterials.Count; j++)
			{
				if (downloadedMaterials[j].tex.tex_diffuse != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterials[j].id + "_" + downloadedMaterials[j].tex.tex_diffuse;
					CheckTextureServerAccesibility(url, downloadedMaterials[j], "diffuse").ParallelCoroutinesGroup(this, "CheckingAccesibility");

				}
				if (downloadedMaterials[j].tex.tex_normal != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterials[j].id + "_" + downloadedMaterials[j].tex.tex_normal;
					CheckTextureServerAccesibility(url, downloadedMaterials[j], "normal").ParallelCoroutinesGroup(this, "CheckingAccesibility");
				}
				if (downloadedMaterials[j].tex.tex_roughness != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterials[j].id + "_" + downloadedMaterials[j].tex.tex_roughness;
					CheckTextureServerAccesibility(url, downloadedMaterials[j], "roughness").ParallelCoroutinesGroup(this, "CheckingAccesibility");
				}
				if (downloadedMaterials[j].preview_icon != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + downloadedMaterials[j].id + "_" + downloadedMaterials[j].preview_icon;
					CheckTextureServerAccesibility(url, downloadedMaterials[j], "preview").ParallelCoroutinesGroup(this, "CheckingAccesibility");
				}
			}
			while (CoroutineExtension.GroupProcessing("CheckingAccesibility"))
				yield return null;

			

			for (int j = i; j < i + countCoroutunesPerCycle && j < downloadedMaterials.Count; j++)
			{
				if ((downloadedMaterials[j].tex.tex_diffuse == "" || downloadedMaterials[j].preview_icon == "") && (downloadedMaterials[j].type != "paint" && downloadedMaterials[j].type != "ceiling"))
				{
					PrintDataToFile(string.Format("Material {0} '{1}' '{2}' is invalid\nHave no diffuse or preview\n********\n", downloadedMaterials[j].id, downloadedMaterials[j].name, downloadedMaterials[j].type));
					Debug.LogError(string.Format("Material {0} '{1}' '{2}' is invalid\nHave no diffuse", downloadedMaterials[j].id, downloadedMaterials[j].name, downloadedMaterials[j].type));
				}
				else
				{
					accesibleMaterials.Add(downloadedMaterials[j]);
					Debug.Log(string.Format("Material {0} '{1}' '{2}' is valid", downloadedMaterials[j].id, downloadedMaterials[j].name, downloadedMaterials[j].type));
				}	
			}
		}
		Debug.LogWarning(string.Format("Valid materials are saved. Count valid materials = {0}", accesibleMaterials.Count));

		for (int i = 0; i < accesibleMaterials.Count; i += countCoroutunesPerCycle)
		{
			for (int j = i; j < i + countCoroutunesPerCycle && j < accesibleMaterials.Count; j++)
			{
				if (accesibleMaterials[j].tex.tex_diffuse != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + accesibleMaterials[j].id + "_" + accesibleMaterials[j].tex.tex_diffuse;
					LoadTextureRoutine(url, accesibleMaterials[j], "diffuse", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
				}
				if (accesibleMaterials[j].tex.tex_normal != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + accesibleMaterials[j].id + "_" + accesibleMaterials[j].tex.tex_normal;
					LoadTextureRoutine(url, accesibleMaterials[j], "normal", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
				}
				if (accesibleMaterials[j].tex.tex_roughness != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + accesibleMaterials[j].id + "_" + accesibleMaterials[j].tex.tex_roughness;
					LoadTextureRoutine(url, accesibleMaterials[j], "roughness", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
				}
				if (accesibleMaterials[j].preview_icon != "")
				{
					string url = "https://apidev.ezkiz.ru/api/__uploads/" + accesibleMaterials[j].id + "_" + accesibleMaterials[j].preview_icon;
					LoadTextureRoutine(url, accesibleMaterials[j], "preview", "asda", true).ParallelCoroutinesGroup(this, "ApplyingTexturesToMaterials");
				}

			}
			while (CoroutineExtension.GroupProcessing("ApplyingTexturesToMaterials"))
				yield return null;
			DeleteInvalidMaterials();
			Debug.LogWarning("Cache has started");
			for (int j = i; j < i + countCoroutunesPerCycle && j < accesibleMaterials.Count; j++)
			{
				DataCacher.CacheTextures(accesibleMaterials[j]);
				DataCacher.CacheJSONMaterialData(accesibleMaterials[j]);
			}
			Debug.LogWarning("Cache has ended");
		}

		Debug.LogWarning(string.Format("Textures are applied to materials"));
		Debug.LogWarning(string.Format("Total count of valid materials = {0}", accesibleMaterials.Count));
		Debug.LogWarning(string.Format("Amount of uniq cached materials = {0}", cachedMaterials.Count));
		for (int i = 0; i < cachedMaterials.Count; i++)
		{
				Debug.Log("Material " + cachedMaterials[i].id + " is not found");
				DataCacher.DeleteAllMaterialDataFromCache(cachedMaterials[i]);
		}

		StartCoroutine(TransofrmTexturesIntoMaterial());

		yield break;
	}

	public IEnumerator TransofrmTexturesIntoMaterial()
	{
		for (int i = 0; i < cachedMaterials.Count; i++)
		{
			var renderer = cube.GetComponent<Renderer>();
			////testMaterial.SetTexture("")
			//renderer.material.mainTexture = tex;
			Material material = renderer.material;
			if (cachedMaterials[i].type == "paint")
			{
				renderer.material = MaterialBuilder.GetMaterial(cachedMaterials[i]);
				Debug.Log(cachedMaterials[i].id + " construct material");
				yield return new WaitForSeconds(2f);

			}
		}
	}

	public static DownloadedMaterial FindDownloadedMaterialInList(DownloadedMaterial materialToFind, List<DownloadedMaterial> materials)
	{
		for (int i = 0; i < materials.Count; i++)
		{
			if (materials[i].id == materialToFind.id)
			{
				return (materials[i]);
			}
		}
		return (null);
	}

	void Start()
	{
		cachedMaterials = DataCacher.GetCachedMaterialsJSONs();
		Debug.LogWarning("Cached material count = " + cachedMaterials.Count);
		StartCoroutine(GlobalCoroutine());

	}
	
	// Update is called once per frame
	void Update()
	{

	}
}
