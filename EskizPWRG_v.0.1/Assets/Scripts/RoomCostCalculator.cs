using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static DataTypes;

public class RoomCostCalculator : MonoBehaviour
{

    public static float CalculateRoomCost(Room room)
	{
        Dictionary<string, int> materials = new Dictionary<string, int>();
        float perimeter = 0;
        room.CalculateArea();
        foreach (Wall wall in room.Walls)
		{
            perimeter += wall.Length;
            DownloadedMaterial materialDataJson = DataCacher.GetMaterialById(wall.materialId);
            if (materialDataJson != null)
            {
                float wallpaperWidth = materialDataJson.pack_dimensions.x;
                float totalNeededWallpaperLength = wall.Length / wallpaperWidth * wall.Height;
                int totalNeededWallpaperRolls = (int)Math.Ceiling((totalNeededWallpaperLength / materialDataJson.pack_dimensions.y));
                if (!materials.ContainsKey(materialDataJson.id))
                {
                    materials.Add(materialDataJson.id, totalNeededWallpaperRolls + 1);
                }
                else
                {
                    materials[materialDataJson.id] += totalNeededWallpaperRolls;
                }
            }
            foreach (Door door in wall.Doors)
			{
                DownloadedMaterial doorDataJson = DataCacher.GetMaterialById(door.MaterialId);
                if (doorDataJson != null)
                {
                    if (!materials.ContainsKey(doorDataJson.id))
                    {
                        materials.Add(doorDataJson.id, 1);
                    }
                    else
                    {
                        materials[doorDataJson.id] += 1;
                    }
                }
            }
		}

        if (room.floorMaterialId != "DefaultMaterial") //TODO: Если в админке не добавят цену за упаковку  -- переделать
		{
            DownloadedMaterial materialDataJson = DataCacher.GetMaterialById(room.floorMaterialId);
            if (materialDataJson != null)
			{
                int totalNeededFloorMaterial = (int)Math.Ceiling(room.Area / materialDataJson.pack_area);
                materials.Add(materialDataJson.id, totalNeededFloorMaterial + 1);
			}

        }
        if (room.ceilingMaterialId != "DefaultMaterial") //TODO: Добавить подсчёт цены потолка и красок
		{
            //DownloadedMaterial materialDataJson = DataCacher.GetMaterialById(room.floorMaterialId);
            //if (materialDataJson != null)
            //{
            //    int totalNeededCeilingMaterial = (int)Math.Ceiling(room.Area / materialDataJson.pack_area);
            //    materials.Add(materialDataJson.id, totalNeededCeilingMaterial + 1);
            //}
        }
        if (room.baseBoardMaterialId != "")
		{
            DownloadedMaterial materialDataJson = DataCacher.GetMaterialById(room.baseBoardMaterialId);
            if (materialDataJson != null)
            {
                int totalNeededBaseboardsCount = (int)Math.Ceiling(perimeter / (materialDataJson.custom_properties.length / 100));
                materials.Add(materialDataJson.id, totalNeededBaseboardsCount + 1);
            }
        }
        float totalCost = 0;
        foreach (var material in materials)
		{
            DownloadedMaterial materialDataJson = DataCacher.GetMaterialById(material.Key);
            float cost = materialDataJson.cost * material.Value;
           // Debug.LogError((String.Format("{0} #{1} {2}р.", materialDataJson.name, material.Value, cost)));
            totalCost += cost;
		}
        room.Cost = totalCost;
        //Debug.LogError("Total cost = " + totalCost);
        return 0;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
