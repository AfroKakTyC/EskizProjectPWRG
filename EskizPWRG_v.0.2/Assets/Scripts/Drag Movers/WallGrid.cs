using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrid : MonoBehaviour
{
    int gridSizeX = 20;
    int gridSizeY = 20;
    public Transform[,] WallGridArray;
    float CellSizeX;
    float CellSizeY;

    public Vector2 GetCellSize()
	{
        return new Vector2(CellSizeX, CellSizeY);
	}

    void CalculateCellSize()
	{
        Wall wall = GetComponent<Wall>();
        CellSizeX = wall.Length / gridSizeX;
        CellSizeY = wall.Height / gridSizeY;
	}

    public Vector2Int GetObjectDownLeftCellIndex(Vector2 objectCoords, Vector2Int objectSizeInCells)
	{
        Debug.LogError("Size x = " + CellSizeX + " Size y = " + CellSizeY);
        Debug.LogError("Object coords = " + objectCoords);
        Debug.LogError(string.Format("{0} / {1} = {2}   {3} / {4} = {5}  {6}", objectCoords.x, CellSizeX, objectCoords.x / CellSizeX, objectCoords.y.ToString("F5"), CellSizeY.ToString("F5"), objectCoords.y / CellSizeY, (int)((objectCoords.y / CellSizeY))));
        float centreCoordX = (objectCoords.x / CellSizeX);
        float centreCoordY = (objectCoords.y / CellSizeY);
        Debug.LogError("Centre coordY (float)= " + centreCoordY.ToString("F10") + " (int)=" + (int)centreCoordY);
        Vector2Int centreOfObjectCellIndex = new Vector2Int((int)Mathf.RoundToInt(centreCoordX), (int)Mathf.RoundToInt(centreCoordY));
        //Vector2Int centreOfObjectCellIndex = new Vector2Int((int)(objectCoords.x / CellSizeX), (int)(objectCoords.y / CellSizeY));
        //Vector2 centreOfObjectCellIndex = new Vector2((objectCoords.x / CellSizeX), (objectCoords.y / CellSizeY));
        Debug.LogError("Centre of object = " + centreOfObjectCellIndex);
        Vector2Int downLeftCellIndex = new Vector2Int(centreOfObjectCellIndex.x - (objectSizeInCells.x / 2), centreOfObjectCellIndex.y - (objectSizeInCells.y / 2));
        DrawObjectGrid(downLeftCellIndex, objectSizeInCells);
        //WallGridArray[downLeftCellIndex.x, downLeftCellIndex.y].GetComponent<Renderer>().material.color = Color.black;
        return downLeftCellIndex;
        //return new Vector2Int(0, 0);
	}

    public void DrawObjectGrid(Vector2Int objectDownLeftCellIndex, Vector2Int objectSizeInCells)
	{
        RestoreGridColors();
        for (int i = objectDownLeftCellIndex.x; i < objectSizeInCells.x + objectDownLeftCellIndex.x; i++)
		{
            for (int j = objectDownLeftCellIndex.y; j < objectSizeInCells.y + objectDownLeftCellIndex.y; j++)
			{
                WallGridArray[i, j].GetComponent<Renderer>().material.color = Color.red * 0.5f;
			}
		}
	}

    public void RestoreGridColors()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if ((i + j) % 2 != 0)
                {
                    WallGridArray[i, j].GetComponent<MeshRenderer>().material.color = Color.green * 0.3f;
                }
                else
                {
                    WallGridArray[i, j].GetComponent<Renderer>().material.color = Color.blue * 0.3f;
                }
            }
        }
    }

 //   public bool CheckCellsAcessibility(Vector2 objectCoords, Vector2Int objectSizeInCells)
	//{
 //       Vector2 downLeftCoord = new Vector2(objectCoords.x / 2, objectCoords.y / 2);
 //       for (float i = downLeftCoord.x; i < downLeftCoord.x + (objectSizeInCells.x * CellSizeX); i += CellSizeX)
	//	{
 //           for (float j = downLeftCoord.y; j< downLeftCoord.y + (objectSizeInCells.y * CellSizeY); j += CellSizeY)
	//		{
 //               if (Grid[i, j] != null)
	//		}
	//	}
	//}

    private void CreateBoxGrid()
	{
        float z = 0;
        GameObject BoxGrid = new GameObject("Grid");
        Wall wall = gameObject.GetComponent<Wall>();
        BoxGrid.transform.SetParent(wall.transform);
        for (int i = 0; i < gridSizeX; i++)
		{
            for (int j = 0; j < gridSizeY; j++)
			{
                Vector3 position = new Vector3(i * CellSizeX, j * CellSizeY, z);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(CellSizeX - 0.01f, CellSizeY - 0.01f, 0.01f);
                cube.transform.SetParent(BoxGrid.transform);
                cube.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
                cube.GetComponent<Renderer>().material.color = Color.blue * 0.3f;
                WallGridArray[i, j] = cube.transform;
                GameObject textObj = new GameObject("Text");
                //textObj.GetComponent<Transform>().position = position;
                textObj.AddComponent(typeof(TextMesh));
                TextMesh text = textObj.GetComponent<TextMesh>();
                text.text = (i.ToString() +"|"+ j.ToString());
                text.characterSize = CellSizeX / 3f;
                text.color = Color.black;
                text.anchor = TextAnchor.MiddleCenter;
                textObj.transform.SetParent(cube.transform);
                textObj.transform.localPosition = Vector3.zero;
                //textObj.transform.localPosition = cube.transform.position;

                
                if ((i + j) % 2 != 0)
				{

                    cube.GetComponent<MeshRenderer>().material.color = Color.green * 0.3f;
				}
			}
		}

        Window window = wall.transform.GetComponentInChildren<Window>();
        Vector3 rotation = window.transform.rotation.eulerAngles;

        if ((Mathf.Abs(rotation.y) / 90) % 2 == 0)
		{
            if (Mathf.Abs(rotation.y) < 90 || Mathf.Abs(rotation.y) > 180)
                BoxGrid.transform.position = new Vector3(wall.StartCoord.x - (CellSizeX / 2), CellSizeY / 2, wall.StartCoord.y);
            else
                BoxGrid.transform.position = new Vector3(wall.StartCoord.x + (CellSizeX / 2), CellSizeY / 2, wall.StartCoord.y);

        }
        else
		{
            if (Mathf.Abs(rotation.y) < 90 || Mathf.Abs(rotation.y) > 180)
                BoxGrid.transform.position = new Vector3(wall.StartCoord.x, CellSizeY / 2, wall.StartCoord.y - (CellSizeX / 2));
            else
                BoxGrid.transform.position = new Vector3(wall.StartCoord.x, CellSizeY / 2, wall.StartCoord.y + (CellSizeX / 2));

        }
        Debug.LogError(rotation);
        Quaternion newRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y + 180f, rotation.z));
        BoxGrid.transform.rotation = newRotation;
        //Grid.transform.rotation.eulerAngles.Set(rotation);
        
	}

    // Start is called before the first frame update
    void Start()
    {
        WallGridArray = new Transform[gridSizeX, gridSizeY];
        CalculateCellSize();
        CreateBoxGrid();
    }

	// Update is called once per frame
	void Update()
    {

    }
}
