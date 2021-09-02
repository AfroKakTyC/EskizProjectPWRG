using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallGridMoverTwoAxis : MonoBehaviour
{
    Vector2Int objectSizeInCells = Vector2Int.zero;
    Vector3 objectSizeInUnits = Vector3.zero;
    Transform parentWall = null;

    void CalculateObjectSizeInCells()
    { 
        Collider collider = GetComponent<Collider>();
        Vector3 minPoint = transform.TransformPoint(collider.bounds.min);
        Vector3 maxPoint = transform.TransformPoint(collider.bounds.max);
        
        objectSizeInUnits = maxPoint - minPoint;
        WallGrid wallGrid = GetComponentInParent<WallGrid>();
        float objectSizeInCellsX = Mathf.Abs(objectSizeInUnits.x / wallGrid.GetCellSize().x);
        float objectSizeInCellsY = Mathf.Abs(objectSizeInUnits.y / wallGrid.GetCellSize().y);
        objectSizeInCells = new Vector2Int((int)Math.Ceiling(objectSizeInCellsX), (int)Math.Ceiling(objectSizeInCellsY));
        if (objectSizeInCells.x % 2 == 0)
            objectSizeInCells.x++;
        if (objectSizeInCells.y % 2 == 0)
            objectSizeInCells.y++;
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

    private Vector3 GetGlobalMousePosition()
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            return (hit.point);
        }
        return (new Vector3(0f, 0f, 0f));
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
        Vector3 globalMousePosition = GetGlobalMousePosition();
        Debug.LogError("Global mouse position = " + globalMousePosition);
        Vector2 celledCoordinates = RoundPositionToCellSize(localMousePosition);
        WallGrid wallGrid = transform.GetComponentInParent<WallGrid>();
        if (wallGrid.CheckPositionAccesibility(celledCoordinates, objectSizeInCells))
        {
            SetParentGridToObject();
            transform.localPosition = new Vector3(celledCoordinates.x, celledCoordinates.y, 0);
            SetParentWallToObject();
        }
        

        Vector2Int downLeftCellIndex = wallGrid.GetObjectDownLeftCellIndex(celledCoordinates, objectSizeInCells);

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
    void SetParentGridToObject()
	{
        transform.SetParent(transform.parent.FindChild("Grid").transform);
	}

    void SetParentWallToObject()
	{
        transform.SetParent(parentWall);
	}

    void Start()
    {
        parentWall = transform.parent;
        CalculateObjectSizeInCells();
        //SetParentGridToObject();
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
