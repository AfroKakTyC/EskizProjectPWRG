using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallGridMoverTwoAxis : MonoBehaviour
{
    Vector2Int objectSizeInCells = Vector2Int.zero;

    void CalculateObjectSizeInCells()
    { 
        Collider collider = GetComponent<Collider>();
        Vector3 minPoint = transform.TransformPoint(collider.bounds.min);
        Vector3 maxPoint = transform.TransformPoint(collider.bounds.max);
        Vector3 objectSizeInUnits = maxPoint - minPoint;
        WallGrid wallGrid = GetComponentInParent<WallGrid>();
        float objectSizeInCellsX = Mathf.Abs(objectSizeInUnits.x / wallGrid.GetCellSize().x);
        float objectSizeInCellsY = Mathf.Abs(objectSizeInUnits.y / wallGrid.GetCellSize().y);
        objectSizeInCells = new Vector2Int((int)Math.Ceiling(objectSizeInCellsX), (int)Math.Ceiling(objectSizeInCellsY));

        //Debug.LogError(objectSizeInCells);
    }

    private Vector2 GetLocalMousePosition()
    {
        Wall wall = GetComponentInParent<Wall>();
        Mesh mesh = GetComponentInParent<MeshFilter>().mesh;
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
            //Debug.LogError(localMousePosition);
        }
        return (localMousePosition);
    }

    Vector2 RoundPositionToCellSize(Vector2 position)
	{
        WallGrid grid = GetComponentInParent<WallGrid>();
        position.x = position.x / grid.GetCellSize().x;
        position.x = Mathf.Round(position.x);
        position.x = position.x * grid.GetCellSize().x;
        position.y = position.y / grid.GetCellSize().y;
        position.y = Mathf.Round(position.y);
        position.y = position.y * grid.GetCellSize().y;

        return (position);
    }

    void MoveObjectOnCells()
	{
        Vector2 localMousePosition = GetLocalMousePosition();
        Vector2 celledCoordinates = RoundPositionToCellSize(localMousePosition);

        transform.position = new Vector3(celledCoordinates.x, celledCoordinates.y, 4);
        WallGrid wallGrid = transform.GetComponentInParent<WallGrid>();
        //wallGrid.GetObjectDownLeftCellIndex(celledCoordinates, objectSizeInCells);
        Debug.LogError(celledCoordinates);

	}

    void DrawCells()
	{
        WallGrid grid = GetComponentInParent<WallGrid>();
        for (int i = 0; i < objectSizeInCells.x; i++)
		{
            for (int j = 0; j < objectSizeInCells.y; j++)
			{
                Vector3 position = new Vector3(transform.position.x + i * grid.GetCellSize().x + (grid.GetCellSize().x / 2), transform.position.y + j * grid.GetCellSize().y + (grid.GetCellSize().y / 2), transform.position.z);
                Collider collider = GetComponent<Collider>();
                //Vector3 minPoint = transform.TransformPoint(collider.bounds.min);
                Vector3 minPoint = collider.bounds.min;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(gameObject.transform);
                //Vector3 position = new Vector3(collider.bounds.min.x + i * grid.GetCellSize().x - (grid.GetCellSize().x / 2), collider.bounds.min.y + j * grid.GetCellSize().y - (grid.GetCellSize().y / 2), transform.position.z);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(grid.GetCellSize().x - 0.01f, grid.GetCellSize().y - 0.01f, 0.01f);
                cube.GetComponent<Renderer>().material.color = Color.red;
            }
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        CalculateObjectSizeInCells();
        //DrawCells();
    }

	private void OnMouseDrag()
	{
        
        MoveObjectOnCells();
        
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
