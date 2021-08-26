using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;


public static class DataTypes
{
	[System.Serializable]
	public enum RoomType
	{
		Rectangle,
		G_type,
		T_type,
		Trapezoidal,
		Z_type
	}

	[System.Serializable]
	public enum WindowType
	{
		double_leaf_window,
		tricuspid_window,
		balcony_right_door,
		balcony_left_door
	}

	[System.Serializable]
	public class RoomData
	{
		public string Name;
		public RoomType Type;
		public float Height;
		public float Area;
		public float Cost;
		public List<Vector2> RoomCorners = new List<Vector2>();
		public List<WallData> Walls = new List<WallData>();
		public BaseBoardData BaseBoard;
		public FloorData Floor;
		public CeilingData Ceiling;
	}

	[System.Serializable]
	public class WallData
	{
		Vector2 startCoord;
		Vector2 endCoord;
		public string MaterialId;
		public List<WindowData> Windows = new List<WindowData>();
		public List<DoorData> Doors = new List<DoorData>();

		public WallData(Vector2 startCoord, Vector2 endCoord)
		{
			this.startCoord = startCoord;
			this.endCoord = endCoord;
		}
	}

	[System.Serializable]
	public class WindowData
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public WindowType Type;

	}

	[System.Serializable]
	public class DoorData
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public string MaterialId;
	}

	[System.Serializable]
	public class BaseBoardData
	{
		public string MaterialId;
	}

	[System.Serializable]
	public class FloorData
	{
		public string MaterialId;
	}

	[System.Serializable]
	public class CeilingData
	{
		public string MaterialId;
	}


	[System.Serializable]
	public class LegacyRoomData
	{
		public LegacyFloorData floor = new LegacyFloorData();
		public LegacyWallData[] walls = new LegacyWallData[4];
		public LegacyPlinthData plinth = new LegacyPlinthData();
		public LegacyCeilingData ceiling = new LegacyCeilingData();

		public string name;
		public float length;
		public float width;
		public float height;
		public float estimate;
		public string image_normal;
		public string id;
	}

	[System.Serializable]
	public class LegacyFloorData
	{
		public DownloadedMaterial material;
	}

	[System.Serializable]
	public class LegacyWallData
	{
		public LegacyDoorData door;
		public string window_type;
		public DownloadedMaterial material;
	}

	[System.Serializable]
	public class LegacyDoorData
	{
		public string name;
		public string id;
	}

	[System.Serializable]
	public class LegacyPlinthData
	{
		public DownloadedMaterial material;
		public string id;
	}

	[System.Serializable]
	public class LegacyCeilingData
	{
		public DownloadedMaterial material;
		public string id;

	}

	[System.Serializable]
	public class DownloadedMaterialsWithHeader
	{
		public int total = 0;
		public int limit = 0;
		public int skip = 0;
		public List<DownloadedMaterial> data = new List<DownloadedMaterial>();
	}

	[System.Serializable]
	public class MetaData
	{

	}

	[System.Serializable]
	public class Custom_properties
	{
		public float[] width_list;
		public string collection;
		public float total_thickness;
		public float zs_thickness;
		public float weight;
		public float length;
		public string use;
		public string manufacturer_country;
		public string manufacturer_company;
		public string design_type;
		public string usage_class;
		public string basis;
		public string vendor_code;
		public string[] color;
		public string tinting_color;
		public float tinting_price;
	}

	[System.Serializable]
	public class Textures
	{
		public string tex_diffuse = "";
		public string tex_normal = "";
		public string tex_roughness = "";
	}

	[System.Serializable]
	public class Pack_dimensions
	{
		public float x;
		public float y;
	}

	[System.Serializable]
	public class Texture_dimensions
	{
		public float x = 0;
		public float y = 0;
		public float z = 0;
	}

	//[System.Serializable]
	public class AplliedTextures
	{
		public Texture2D tex_diffuse = null;
		public Texture2D tex_normal = null;
		public Texture2D tex_roughness = null;
		public Texture2D tex_preview_icon = null;
	}

	[System.Serializable]
	public class DownloadedMaterial
	{
		[JsonIgnore]
		public AplliedTextures applyedTextures = new AplliedTextures();
		public Sprite previewSprite = null;

		public Textures tex = new Textures();
		public Pack_dimensions pack_dimensions = new Pack_dimensions();
		public Texture_dimensions texture_dimensions = new Texture_dimensions();
		public Custom_properties custom_properties = new Custom_properties();
		public MetaData metadata = new MetaData();

		public bool isValid = true;

		public string _id;
		public float[] width_list;
		public string name;
		public string coating;
		public string color;
		public string type;
		public float cost;
		public string units;

		public string some_new_field;
		public string preview_icon = "";
		public float shininess_scale;
		public float normal_scale;
		public float roughness_scale;
		public float pack_area;

		public string createdAt;
		public string updatedAt;
		public float __v;
		public string category;
		public string id;

		public void MakeMaterialPreview()
		{
			Texture2D previewTexture = new Texture2D(200, 200);
			Color paintColor = new Color();
			string name = "preview_icon.png";
			if (type == "paint")
			{
				if (ColorUtility.TryParseHtmlString(custom_properties.tinting_color, out paintColor))
				{
					for (int y = 0; y < previewTexture.height; y++)
					{
						for (int x = 0; x < previewTexture.width; x++)
						{
							previewTexture.SetPixel(x, y, paintColor);
						}
					}

				}
			}
			else if (type == "ceiling")
			{
				if (ColorUtility.TryParseHtmlString(color, out paintColor))
				{
					for (int y = 0; y < previewTexture.height; y++)
					{
						for (int x = 0; x < previewTexture.width; x++)
						{
							previewTexture.SetPixel(x, y, paintColor);
						}
					}
				}
			}
			preview_icon = name;
			tex.tex_diffuse = name;
			applyedTextures.tex_preview_icon = previewTexture;
			applyedTextures.tex_diffuse = previewTexture;
		}

		public void ClearApplyedTextures()
		{
			applyedTextures.tex_diffuse = null;
			applyedTextures.tex_normal = null;
			applyedTextures.tex_roughness = null;
			applyedTextures.tex_preview_icon = null;
		}
	}


}
