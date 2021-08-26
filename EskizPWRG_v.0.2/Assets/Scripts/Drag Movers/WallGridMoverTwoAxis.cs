using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallGridMoverTwoAxis : MonoBehaviour
{
    Vector2 objectSizeInCells = Vector2.zero;

    void CalculateObjectSizeInCells()
    { 
        Collider collider = GetComponent<Collider>();
        Vector3 minPoint = transform.TransformPoint(collider.bounds.min);
        Vector3 maxPoint = transform.TransformPoint(collider.bounds.max);
        Vector3 objectSizeInUnits = maxPoint - minPoint;
        WallGrid wallGrid = GetComponentInParent<WallGrid>();
        float objectSizeInCellsX = Mathf.Abs(objectSizeInUnits.x / wallGrid.GetCellSize().x);
        float objectSizeInCellsY = Mathf.Abs(objectSizeInUnits.y / wallGrid.GetCellSize().y);
        objectSizeInCells = new Vector2((int)Math.Ceiling(objectSizeInCellsX), (int)Math.Ceiling(objectSizeInCellsY));

        Debug.LogError(objectSizeInCells);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void OnMouseDown()
	{
        CalculateObjectSizeInCells();	
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
