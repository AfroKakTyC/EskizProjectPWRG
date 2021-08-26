using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMoverTest : MonoBehaviour
{
    float cellSizeX = 0.1f;
    float cellSizeY = 0.1f;
    int sizeInCellsX;
    int sizeInCellsY;
    float wallLength;
    float wallHeight;
    // Start is called before the first frame update
    void Start()
    {
        wallLength = transform.parent.GetComponent<Wall>().Length;
        wallHeight = transform.parent.GetComponent<Wall>().Height;
        cellSizeX = wallLength / 10;
        cellSizeY = wallHeight / 10;
        sizeInCellsX = (int)System.Math.Ceiling(gameObject.GetComponent<Window>().Width / cellSizeX);
        sizeInCellsY = (int)System.Math.Ceiling(gameObject.GetComponent<Window>().Height / cellSizeY);
        Debug.LogError("cellSizeX = " + cellSizeX + " cellSizeY = " + cellSizeY);
        Debug.LogError("sizeInCellsX = " + sizeInCellsX + " sizeInCellsY = " + sizeInCellsY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
