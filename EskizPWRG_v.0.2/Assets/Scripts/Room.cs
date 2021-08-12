using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using static DataTypes;

public class Room : MonoBehaviour
{
	public List<Vector2> RoomCorners = new List<Vector2>();
	public List<Wall> Walls = new List<Wall>();
	public float Height;
	public string floorMaterialId = "";
	public string ceilingMaterialId = "";
	public string baseBoardMaterialId = "";
	public string Name = "Room 1";
	public float Area = 0;
	public float Cost = 0;
	public RoomType Type;

	public float CalculateArea()
	{
		if (Type == RoomType.Rectangle)
		{
			Area = Walls[0].Length * Walls[1].Length;
			return (Area);
		}
		else if (Type == RoomType.G_type)
		{
			Area = (Walls[0].Length * Walls[1].Length) - (Walls[3].Length * Walls[4].Length);
			return (Area);
		}
		else if (Type == RoomType.T_type)
		{
			Area = (Walls[0].Length * Walls[1].Length) +(Walls[4].Length * Walls[5].Length);
			return (Area);
		}
		else if (Type == RoomType.Trapezoidal)
		{
			Area = (Walls[0].Length * Walls[1].Length) + (Walls[4].Length - Walls[1].Length) * ((Walls[0].Length + Walls[3].Length) / 2);
			return (Area);
		}
		else if (Type == RoomType.Z_type)
		{
			Area = ((Walls[0].Length + Walls[2].Length) * (Walls[1].Length + Walls[3].Length)) - (Walls[1].Length * Walls[2].Length + Walls[5].Length * Walls[6].Length);
			return (Area);
		}
		else
			return 0;
	}

	public void AddWall(Wall wall)
	{
		Walls.Add(wall);
	}
}
