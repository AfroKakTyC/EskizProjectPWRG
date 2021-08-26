using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using static DataTypes;

public class RoomCreator : MonoBehaviour
{
	private Vector2[] roomCorners = { new Vector2(0f, 0f), new Vector2(0f, 3f), new Vector2(-3f, 3f), new Vector2(-3f, 6f), new Vector2(3f, 6f), new Vector2(3f, 0f) };
	static GameObject Room = null;

	public void CreateRoom(RoomType type, string roomName, Vector2[] roomCorners, float height)
	{
		//RoomData roomData = new RoomData();
		//roomData.Type = type;
		GameObject room = new GameObject(roomName, typeof(Room));
		Room = room;

		Room roomScript = room.GetComponent<Room>();
		roomScript.Type = type;
		roomScript.Name = roomName;
		roomScript.Height = height;
		for (int i = 0; i < roomCorners.Length; i++)
		{
			roomScript.RoomCorners.Add(roomCorners[i]);
		}
		for (int i = 0; i < roomCorners.Length; i++)
		{
			int j = i + 1;
			if (j >= roomCorners.Length)
				j = 0;

			string name = "Wall" + i;
			GameObject wall = new GameObject(name, typeof(Wall), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
			wall.transform.SetParent(room.transform);
			Wall wallScript = wall.GetComponent<Wall>();
			wallScript.Name = name;
			//roomData.Walls.Add(new WallData(roomCorners[i], roomCorners[j]));
			wallScript.CreateWall(roomCorners[i], roomCorners[j], height);
			wallScript.CreateBaseBoard("default");
			roomScript.AddWall(wallScript);
		}
		GameObject floor = new GameObject("floor", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		floor.transform.SetParent(room.transform);
		Floor floorScript = floor.GetComponent<Floor>();
		floorScript.CreateFloor(FindDownLeftCoord(roomCorners), FindUpperRigthCoord(roomCorners), 0);
		roomScript.floorMaterialId = "default";

		GameObject ceiling = new GameObject("ceiling", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		ceiling.transform.SetParent(room.transform);
		Floor ceilingScript = ceiling.GetComponent<Floor>();
		ceilingScript.CreateFloor(FindDownLeftCoord(roomCorners), FindUpperRigthCoord(roomCorners), height);
		roomScript.ceilingMaterialId = "default";

		string text = ConvertRoomToJSON(roomScript);
		DataCacher.CacheJsonRoomData(text, roomScript.Name);
		DataCacher.CacheJsonRoomData(text, roomScript.Name);
		RoomData test = JsonConvert.DeserializeObject<RoomData>(text);
		CreateRoom(test);

		//PrintDataToFile(roomScript);
	}

	public static void CreateRoom(RoomData roomData)
	{

		GameObject room = new GameObject(roomData.Name, typeof(Room));
		Room = room;
		Room roomScript = room.GetComponent<Room>();
		for (int i = 0; i < roomData.RoomCorners.Count; i++)
		{
			roomScript.RoomCorners.Add(roomData.RoomCorners[i]);
		}
		for (int i = 0; i < roomData.RoomCorners.Count; i++)
		{
			int j = i + 1;
			if (j >= roomData.RoomCorners.Count)
				j = 0;

			string name = "Wall" + i;
			GameObject wall = new GameObject(name, typeof(Wall), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
			wall.transform.SetParent(room.transform);
			Wall wallScript = wall.GetComponent<Wall>();
			wallScript.Name = name;
			wallScript.CreateWall(roomData.RoomCorners[i], roomData.RoomCorners[j], roomData.Height);
			foreach(WindowData window in roomData.Walls[i].Windows)
			{
				wallScript.CreateWindow(window.Type, window.Position, window.Rotation);
			}
			foreach(DoorData door in roomData.Walls[i].Doors)
			{
				wallScript.CreateDoor(door.MaterialId, door.Position, door.Rotation);
			}
			wallScript.UpdateMaterial(MaterialBuilder.GetMaterial(roomData.Walls[i].MaterialId));
			if (roomData.BaseBoard != null)
			{
				wallScript.CreateBaseBoard(roomData.BaseBoard.MaterialId);
				roomScript.baseBoardMaterialId = roomData.BaseBoard.MaterialId;
			}
			roomScript.AddWall(wallScript);
		}
		GameObject floor = new GameObject("floor", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		floor.transform.SetParent(room.transform);
		Floor floorScript = floor.GetComponent<Floor>();
		floorScript.CreateFloor(FindDownLeftCoord(roomData.RoomCorners.ToArray()), FindUpperRigthCoord(roomData.RoomCorners.ToArray()), 0);
		if (roomData.Floor != null)
		{
			floorScript.UpdateMaterial(MaterialBuilder.GetMaterial(roomData.Floor.MaterialId));
		}
		roomScript.floorMaterialId = floorScript.materialId;


		GameObject ceiling = new GameObject("ceiling", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		ceiling.transform.SetParent(room.transform);
		Floor ceilingScript = ceiling.GetComponent<Floor>();
		ceilingScript.CreateFloor(FindDownLeftCoord(roomData.RoomCorners.ToArray()), FindUpperRigthCoord(roomData.RoomCorners.ToArray()), roomData.Height);
		if (roomData.Ceiling != null)
		{
			ceilingScript.UpdateMaterial(MaterialBuilder.GetMaterial(roomData.Ceiling.MaterialId));
		}
		roomScript.ceilingMaterialId = ceilingScript.materialId;
		//PrintDataToFile(roomScript);
	}


	public static void CreateRoomFromLegacy(LegacyRoomData legacyRoomData)
	{
		PrintDataToFile(JsonConvert.SerializeObject(legacyRoomData));
		float length = legacyRoomData.length;
		float width = legacyRoomData.width;
		float height = legacyRoomData.height;
		string baseBoardMaterialId = "";
		
		Vector2[] roomCorners = { new Vector2(0f, 0f), new Vector2(0f, width), new Vector2(length, width), new Vector2(length, 0f) };
		//Debug.LogError(length);
		GameObject room = new GameObject(legacyRoomData.name, typeof(Room));
		Room = room;
		Room roomScript = room.GetComponent<Room>();
		roomScript.Name = legacyRoomData.name;
		roomScript.Height = legacyRoomData.height;
		roomScript.Type = RoomType.Rectangle;
		for (int i = 0; i < roomCorners.Length; i++)
		{
			roomScript.RoomCorners.Add(roomCorners[i]);
		}
		if (legacyRoomData.plinth != null)
		{
			baseBoardMaterialId = legacyRoomData.plinth.id;
			roomScript.baseBoardMaterialId = baseBoardMaterialId;
		}
		for (int i = 0; i < roomCorners.Length; i++)
		{
			int j = i + 1;
			if (j >= roomCorners.Length)
				j = 0;

			string name = "Wall" + i;
			GameObject wall = new GameObject(name, typeof(Wall), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
			wall.transform.SetParent(room.transform);
			Wall wallScript = wall.GetComponent<Wall>();
			wallScript.Name = name;
			wallScript.CreateWall(roomCorners[i], roomCorners[j], height);


			if (baseBoardMaterialId != "")
			{
				wallScript.CreateBaseBoard(baseBoardMaterialId);
				BaseBoardData baseBoardData = new BaseBoardData();
				baseBoardData.MaterialId = legacyRoomData.plinth.id;
				wallScript.BaseBoard = baseBoardData;
			}
			if (legacyRoomData.walls[i].window_type != "none")
			{
				wallScript.CreateWindow(legacyRoomData.walls[i].window_type);
			}
			if (legacyRoomData.walls[i].door != null)
			{
				if (legacyRoomData.walls[i].door.name != "none")
				{
					wallScript.CreateDoor(legacyRoomData.walls[i].door.id);
				}
			}
			if (legacyRoomData.walls[i].material != null)
			{
				wallScript.UpdateMaterial(MaterialBuilder.GetMaterial(legacyRoomData.walls[i].material.id));

			}
			roomScript.AddWall(wallScript);
		}
		GameObject floor = new GameObject("floor", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		floor.transform.SetParent(room.transform);
		Floor floorScript = floor.GetComponent<Floor>();
		floorScript.CreateFloor(roomCorners, 0);
		if (legacyRoomData.floor != null)
		{
			floorScript.UpdateMaterial(MaterialBuilder.GetMaterial(legacyRoomData.floor.material.id));

		}
		roomScript.floorMaterialId = floorScript.materialId;

		GameObject ceiling = new GameObject("ceiling", typeof(Floor), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
		ceiling.transform.SetParent(room.transform);
		Floor ceilingScript = ceiling.GetComponent<Floor>();
		ceilingScript.CreateFloor(roomCorners, height);
		if (legacyRoomData.ceiling != null)
		{
			ceilingScript.UpdateMaterial(MaterialBuilder.GetMaterial(legacyRoomData.ceiling.id));
		}
		roomScript.ceilingMaterialId = ceilingScript.materialId;

		Debug.LogError("Room created");
		//string text = JsonConvert.SerializeObject(roomData, settings);
		//RoomData test = JsonConvert.DeserializeObject<RoomData>(text);
		//PrintDataToFile(ConvertRoomToJSON(Room.GetComponent<Room>()));
		//string test = ConvertRoomToJSON(Room.GetComponent<Room>());
		////PrintDataToFile(test);
		//Debug.LogError("1 room created");
		//RoomCostCalculator.CalculateRoomCost(Room.GetComponent<Room>());
		//DestroyRoom();
		//RoomData test2 = JsonConvert.DeserializeObject<RoomData>(test);
		//CreateRoom(test2);
		//Debug.LogError("2 room created");
		//RoomCostCalculator.CalculateRoomCost(Room.GetComponent<Room>());

		//PrintDataToFile(text);
	}

	public static void DestroyRoom()
	{
		Destroy(Room);
		Room = null;
		Debug.LogError("Room destroyed");
	}

	static Vector2 FindDownLeftCoord(Vector2[] roomCorners)
	{
		float x = 0f;
		float y = 0f;
		for (int i = 0; i < roomCorners.Length; i++)
		{
			if (roomCorners[i].x < x)
				x = roomCorners[i].x;
			if (roomCorners[i].y < y)
				y = roomCorners[i].y;
		}
		return (new Vector2(x, y));
	}

	static Vector2 FindUpperRigthCoord(Vector2[] roomCorners)
	{
		float x = 0f;
		float y = 0f;
		for (int i = 0; i < roomCorners.Length; i++)
		{
			if (roomCorners[i].x > x)
				x = roomCorners[i].x;
			if (roomCorners[i].y > y)
				y = roomCorners[i].y;
		}
		return (new Vector2(x, y));
	}

	public static void PrintDataToFile(string text)
	{
		using (StreamWriter sw = new StreamWriter("D:\\test\\logRoom.txt", true, System.Text.Encoding.Default))
		{
			sw.WriteLine(text);
			sw.WriteLine();
		}
	}

	public static string ConvertRoomToJSON(Room room)
	{
		RoomData roomData = new RoomData();
		roomData.Name = room.Name;
		roomData.Cost = room.Cost;
		roomData.Type = room.Type;
		roomData.Height = room.Height;
		roomData.RoomCorners = room.RoomCorners;
		foreach(Wall wall in room.Walls)
		{
			WallData wallData = new WallData(wall.StartCoord, wall.EndCoord);
			wallData.MaterialId = wall.materialId;
			foreach (Window window in wall.Windows)
			{
				WindowData windowData = new WindowData();
				windowData.Position = window.Position;
				windowData.Rotation = window.Rotation;
				windowData.Type = window.Type;
				wallData.Windows.Add(windowData);
			}
			foreach (Door door in wall.Doors)
			{
				DoorData doorData = new DoorData();
				doorData.Position = door.Position;
				doorData.Rotation = door.Rotation;
				doorData.MaterialId = door.MaterialId;
				wallData.Doors.Add(doorData);
			}
			if (wall.BaseBoard != null)
			{
				BaseBoardData baseBoard = new BaseBoardData();
				baseBoard.MaterialId = wall.BaseBoard.MaterialId;
				roomData.BaseBoard = baseBoard;
			}
			roomData.Walls.Add(wallData);
		}
		FloorData floor = new FloorData();
		floor.MaterialId = room.floorMaterialId;
		roomData.Floor = floor;
		CeilingData ceiling = new CeilingData();
		ceiling.MaterialId = room.ceilingMaterialId;
		roomData.Ceiling = ceiling;
		var settings = new Newtonsoft.Json.JsonSerializerSettings();
		settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		return (JsonConvert.SerializeObject(roomData, settings));
	}

	void Start()
	{
		//CreateRoom(RoomType.Rectangle, "testRoom", roomCorners, 5);
		
	}
}
