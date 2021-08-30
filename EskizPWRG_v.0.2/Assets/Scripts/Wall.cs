using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;
using Newtonsoft.Json;
using System;

public class Wall : MonoBehaviour
{
	public Material wallMaterial;
	public Material doorMaterial;
	public List<Window> Windows = new List<Window>();
	public List<Door> Doors = new List<Door>();
	public BaseBoardData BaseBoard = null;
	//GameObject BaseBoard = null;
	GameObject Door = null;
	public string Name;
	public string materialId = "";
	public Vector2 StartCoord;
	public Vector2 EndCoord;
	public float Height = 5;
	public float Length;
	
	void UpdateTextureScale()
	{
		Vector2 vanillaTextureScale = wallMaterial.mainTextureScale;
		Vector2 textureScale = new Vector2(Length * vanillaTextureScale.x, Height * vanillaTextureScale.y);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_GlossMap"), textureScale);
	}

	public void CreateWall(Vector2 start, Vector2 end, float height)
	{
		StartCoord = start;
		EndCoord = end;
		Height = height;
		var heading = EndCoord - StartCoord;
		var distance = heading.magnitude;
		Length = distance;
		Mesh mesh = new Mesh();

		Vector3[] vertices = new Vector3[4]
	   {
			new Vector3(start.x, 0, start.y),
			new Vector3(end.x, 0, end.y),
			new Vector3(start.x, height, start.y),
			new Vector3(end.x, height, end.y)
	   };
		mesh.vertices = vertices;

		int[] tris = new int[6]
		{
			// lower left triangle
			0, 2, 1,
			// upper right triangle
			2, 3, 1
		};
		mesh.triangles = tris;

		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1)
		};
		mesh.uv = uv;

		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
		//wallMaterial = Resources.Load<Material>("Default/DefaultMaterial");
		UpdateMaterial(MaterialBuilder.GetMaterial("default"));
		//Debug.LogError(start.x + " " + start.y + " " + end.x + " " + end.y);
		//UpdateMaterial();
	}

	public void UpdateMaterial(Material updateMaterial)
	{
		wallMaterial = updateMaterial;
		materialId = updateMaterial.name;
		gameObject.GetComponent<MeshRenderer>().material = updateMaterial;
		UpdateTextureScale();
	}

	internal void CreateBaseBoard(string materialId)
	{
		float baseBoardOffset = 0.025f;
		float baseBoardHeight = 0.06f;

		var heading = new Vector3(EndCoord.x, 0, EndCoord.y) - new Vector3(StartCoord.x, 0, StartCoord.y);
		GameObject baseBoard = new GameObject("baseBord " + Name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		BaseBoardData baseBoardData = new BaseBoardData();
		baseBoardData.MaterialId = materialId;
		BaseBoard = baseBoardData;
		Mesh mesh = new Mesh();
		heading = heading.normalized;
		//Debug.LogError(heading.x + " " + heading.y + " " + heading.z);
		Vector3[] vertices = new Vector3[4]
	   {
			new Vector3(StartCoord.x + heading.z * baseBoardOffset - heading.x * baseBoardOffset, 0, StartCoord.y - heading.x * baseBoardOffset - heading.z * baseBoardOffset),
			new Vector3(EndCoord.x + heading.z * baseBoardOffset + heading.x * baseBoardOffset, 0, EndCoord.y - heading.x * baseBoardOffset + heading.z * baseBoardOffset),
			new Vector3(StartCoord.x, baseBoardHeight, StartCoord.y),
			new Vector3(EndCoord.x, baseBoardHeight, EndCoord.y)
	   };
		mesh.vertices = vertices;

		int[] tris = new int[6]
		{
			// lower left triangle
			0, 2, 1,
			// upper right triangle
			2, 3, 1
		};
		mesh.triangles = tris;

		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1)
		};
		mesh.uv = uv;

		baseBoard.GetComponent<MeshFilter>().mesh = mesh;
		baseBoard.transform.SetParent(gameObject.transform);
		baseBoard.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Default/DefaultMaterial");
		Material materialToAttach = MaterialBuilder.GetMaterial(materialId, "baseboard");
		materialToAttach.mainTextureScale = new Vector2(1f, 1f);
		baseBoard.GetComponent<MeshRenderer>().material = materialToAttach;
	}

	public void CreateWindow(string windowType)
	{
		Vector3 wallDirection = EndCoord - StartCoord;
		float windowHeight = 0;
		if (windowType == "balcony_left_door" || windowType == "balcony_right_door")
		{
			windowHeight = 1.095f;
		}
		else
		{
			windowHeight = 1.7f;
		}
		Vector3 middleWallCoord = new Vector3((StartCoord.x + EndCoord.x) / 2, windowHeight, (StartCoord.y + EndCoord.y) / 2);
		if (Windows.Count > 1)
		{
			Vector3 newWindowCoords = new Vector3((StartCoord.x + EndCoord.x) / 3, windowHeight, (StartCoord.y + EndCoord.y) / 3);
			Window existedWindowScript = Windows[0].GetComponent<Window>();
			existedWindowScript.UpdatePosition(newWindowCoords);
			middleWallCoord.x *= 2f;
			middleWallCoord.y *= 2f;
		}
		//Debug.LogError(Vector3.Angle(wallDirection, Vector3.left));
		//float windowRotationAngle = Vector3.Angle(wallDirection, Vector3.up) + 90f;
		float windowRotationAngle = Vector3.SignedAngle(wallDirection, Vector3.up, Vector3.forward) + 90f;
		Debug.LogError(gameObject.name + "  angle = " + windowRotationAngle);
		//Debug.LogError(wallDirection + " " + Vector3.left + " " + Vector3.up + "   " + windowRotationAngle);
		PrefabContainer container = GameObject.Find("PrefabContainer").GetComponent<PrefabContainer>();
		GameObject window = null;
		WindowType type = 0;
		if (windowType == "tricuspid_window")
		{
			window = Instantiate(container.GetWindow(WindowType.tricuspid_window), middleWallCoord, Quaternion.AngleAxis(windowRotationAngle, Vector3.up), gameObject.transform);
			type = WindowType.tricuspid_window;
		}
		else if (windowType == "double_leaf_window")
		{
			window = Instantiate(container.GetWindow(WindowType.double_leaf_window), middleWallCoord, Quaternion.AngleAxis(windowRotationAngle, Vector3.up), gameObject.transform);
			type = WindowType.double_leaf_window;
		}
		else if (windowType == "balcony_right_door")
		{
			window = Instantiate(container.GetWindow(WindowType.balcony_right_door), middleWallCoord, Quaternion.AngleAxis(windowRotationAngle, Vector3.up), gameObject.transform);
			type = WindowType.balcony_right_door;
		}
		else if (windowType == "balcony_left_door")
		{
			window = Instantiate(container.GetWindow(WindowType.balcony_left_door), middleWallCoord, Quaternion.AngleAxis(windowRotationAngle, Vector3.up), gameObject.transform);
			type = WindowType.balcony_left_door;
		}
		window.AddComponent<Window>();
		Window windowScript = window.GetComponent<Window>();
		windowScript.Type = type;
		windowScript.Position = window.transform.position;
		windowScript.Rotation = window.transform.rotation;
		Windows.Add(windowScript);
	}

	public void CreateWindow(WindowType type, Vector3 position, Quaternion rotation)
	{
		Vector3 wallDirection = EndCoord - StartCoord;
		Vector3 middleWallCoord = new Vector3((StartCoord.x + EndCoord.x) / 2, 1.7f, (StartCoord.y + EndCoord.y) / 2);

		PrefabContainer container = GameObject.Find("PrefabContainer").GetComponent<PrefabContainer>();
		GameObject window = Instantiate(container.GetWindow(type), position, rotation, gameObject.transform);
		window.AddComponent<Window>();
		Window windowScript = window.GetComponent<Window>();
		windowScript.Type = type;
		windowScript.Position = window.transform.position;
		windowScript.Rotation = window.transform.rotation;
		Windows.Add(windowScript);
	}



	public void CreateDoor(string materialId)
	{
		Vector3 wallDirection = EndCoord - StartCoord;
		Vector3 middleWallCoord = new Vector3((StartCoord.x + EndCoord.x) / 2, 0f, (StartCoord.y + EndCoord.y) / 2);
		if (Windows.Count > 1)
		{
			Vector3 newDoorCoords = new Vector3((StartCoord.x + EndCoord.x) / 3, 1.7f, (StartCoord.y + EndCoord.y) / 3);
			Door oldDoorScript = Doors[0].GetComponent<Door>();
			oldDoorScript.UpdatePosition(newDoorCoords);
			middleWallCoord.x *= 2f;
			middleWallCoord.y *= 2f;
		}
		//Debug.LogError(Vector3.Angle(wallDirection, Vector3.left));
		float doorRotationAngle = Vector3.Angle(wallDirection, Vector3.left);
		PrefabContainer container = GameObject.Find("PrefabContainer").GetComponent<PrefabContainer>();
		GameObject door = Instantiate(container.door, middleWallCoord, Quaternion.AngleAxis(doorRotationAngle, Vector3.up), gameObject.transform);
		door.AddComponent<Door>();
		Door doorScript = door.GetComponent<Door>();
		Door = door;
		Transform doorModel = door.transform.Find("DoorSurface");
		Material materialToAttach = MaterialBuilder.GetMaterial(materialId);
		doorMaterial = materialToAttach;
		materialToAttach.mainTextureScale = new Vector2(1f, 1f);
		doorModel.GetComponent<MeshRenderer>().material = materialToAttach;
		doorScript.Position = door.transform.position;
		doorScript.Rotation = door.transform.rotation;
		doorScript.MaterialId = materialToAttach.name;
		Doors.Add(doorScript);
	}

	public void CreateDoor(string materialId, Vector3 position, Quaternion rotation)
	{
		Vector3 wallDirection = EndCoord - StartCoord;
		Vector3 middleWallCoord = new Vector3((StartCoord.x + EndCoord.x) / 2, 0f, (StartCoord.y + EndCoord.y) / 2);
		//Debug.LogError(Vector3.Angle(wallDirection, Vector3.left));
		float doorRotationAngle = Vector3.Angle(wallDirection, Vector3.left);
		PrefabContainer container = GameObject.Find("PrefabContainer").GetComponent<PrefabContainer>();
		DoorData doorData = new DoorData();
		GameObject door = Instantiate(container.door, position, rotation, gameObject.transform);
		door.AddComponent<Door>();
		Door doorScript = door.GetComponent<Door>();
		Door = door;
		Transform doorModel = door.transform.Find("DoorSurface");
		Material materialToAttach = MaterialBuilder.GetMaterial(materialId);
		doorMaterial = materialToAttach;
		materialToAttach.mainTextureScale = new Vector2(1f, 1f);
		doorModel.GetComponent<MeshRenderer>().material = materialToAttach;
		doorScript.Position = door.transform.position;
		doorScript.Rotation = door.transform.rotation;
		doorScript.MaterialId = materialToAttach.name;
		Doors.Add(doorScript);
	}

}
