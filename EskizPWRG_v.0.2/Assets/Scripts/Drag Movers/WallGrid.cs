using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrid : MonoBehaviour
{
    public Transform[,] Grid = new Transform[10, 10];
    float CellSizeX;
    float CellSizeY;

    public Vector2 GetCellSize()
	{
        return new Vector2(CellSizeX, CellSizeY);
	}

    void CalculateCellSize()
	{
        Wall wall = GetComponent<Wall>();
        CellSizeX = wall.Length / 10;
        CellSizeY = wall.Height / 10;
	}

    private Vector2 GetLocalMousePosition()
	{
        Wall wall = GetComponent<Wall>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2 localMousePosition = Vector2.zero;
        Vector3 a = mesh.vertices[0];
        Vector3 b = mesh.vertices[1];
        Vector3 c = mesh.vertices[2];
        Plane plane = new Plane(a, b, c); //Making virtual plane parallel to wall
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            var hitPoint = ray.GetPoint(distance);
            Vector3 wallPosition = new Vector3(wall.StartCoord.x, 0f, wall.StartCoord.y);
            Vector3 position = wallPosition - hitPoint;
            localMousePosition = new Vector2(Mathf.Abs(position.x + position.z), Mathf.Abs(position.y));
            Debug.LogError(localMousePosition);
        }
        return (localMousePosition);
    }

    private void CreateBoxGrid()
	{
        float z = 0;
        GameObject Grid = new GameObject("Grid");
        Wall wall = gameObject.GetComponent<Wall>();
        Grid.transform.SetParent(wall.transform);
        for (int i = 0; i < 10; i++)
		{
            for (int j = 0; j < 10; j++)
			{
                Vector3 position = new Vector3(i * CellSizeX, j * CellSizeY, z);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(CellSizeX - 0.01f, CellSizeY - 0.01f, 0.1f);
                cube.transform.SetParent(Grid.transform);
                GameObject textObj = new GameObject("Text");
                //textObj.GetComponent<Transform>().position = position;
                textObj.AddComponent(typeof(TextMesh));
                TextMesh text = textObj.GetComponent<TextMesh>();
                text.text = (i.ToString() + j.ToString());
                text.characterSize = 0.2f;
                text.color = Color.black;
                text.anchor = TextAnchor.MiddleCenter;
                textObj.transform.SetParent(cube.transform);
                textObj.transform.localPosition = Vector3.zero;
                //textObj.transform.localPosition = cube.transform.position;

                
                if ((i + j) % 2 != 0)
				{

                    cube.GetComponent<MeshRenderer>().material.color = Color.red;
				}
			}
		}

        Window window = wall.transform.GetComponentInChildren<Window>();
        Vector3 rotation = window.transform.rotation.eulerAngles;

        if ((Mathf.Abs(rotation.y) / 90) % 2 == 0)
		{
            if (Mathf.Abs(rotation.y) < 90 || Mathf.Abs(rotation.y) > 180)
                Grid.transform.position = new Vector3(wall.StartCoord.x - (CellSizeX / 2), CellSizeY / 2, wall.StartCoord.y);
            else
                Grid.transform.position = new Vector3(wall.StartCoord.x + (CellSizeX / 2), CellSizeY / 2, wall.StartCoord.y);

        }
        else
		{
            if (Mathf.Abs(rotation.y) < 90 || Mathf.Abs(rotation.y) > 180)
                Grid.transform.position = new Vector3(wall.StartCoord.x, CellSizeY / 2, wall.StartCoord.y - (CellSizeX / 2));
            else
                Grid.transform.position = new Vector3(wall.StartCoord.x, CellSizeY / 2, wall.StartCoord.y + (CellSizeX / 2));

        }
        Debug.LogError(rotation);
        Quaternion newRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y + 180f, rotation.z));
        Grid.transform.rotation = newRotation;
        //Grid.transform.rotation.eulerAngles.Set(rotation);
        
	}

    // Start is called before the first frame update
    void Start()
    {
        CalculateCellSize();
        CreateBoxGrid();
    }

	private void OnMouseDown()
	{
        GetLocalMousePosition();
	}

	// Update is called once per frame
	void Update()
    {

    }
}
