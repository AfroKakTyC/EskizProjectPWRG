using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;

public class Window : MonoBehaviour
{
    public Vector3 Position;
    public Quaternion Rotation;
	public WindowType Type;
	//public string Type;
	public float Scale = 1f;
	public float Width;
	public float Height;

    public void UpdatePosition(Vector3 newPosition)
	{
        Position = newPosition;
        gameObject.transform.position = newPosition;
	}

    public void UpdateRotation(Quaternion newRotation)
	{
        gameObject.transform.rotation = newRotation;
	}

	public float GetDistanceLeftBorder(float width)
	{
		Wall parentWall = transform.parent.GetComponent<Wall>();
		Vector2 leftBorder = new Vector2(parentWall.StartCoord.x, parentWall.StartCoord.y);
		Vector2 currentWindowPosition = new Vector2(transform.position.x, transform.position.z);
		Debug.LogError("Distance = " + (Vector2.Distance(leftBorder, currentWindowPosition) - ((width / 2) * Scale)));
		return (Vector2.Distance(leftBorder, currentWindowPosition) - ((width / 2) * Scale));
	}

	public float GetDistanceRightBorder(float width)
	{
		Wall parentWall = transform.parent.GetComponent<Wall>();
		Vector2 rightBorder = new Vector2(parentWall.EndCoord.x, parentWall.EndCoord.y);
		Vector2 currentWindowPosition = new Vector2(transform.position.x, transform.position.z);
		Debug.LogError("Distance = " + (Vector2.Distance(rightBorder, currentWindowPosition) - ((width / 2) * Scale)));
		return (Vector2.Distance(rightBorder, currentWindowPosition) - ((width / 2) * Scale));
	}

	private void Start()
	{
		GetDistances();
	}

	public Vector4 GetDistances()
	{
		
		Vector4 result = new Vector4();
		if (Type == WindowType.double_leaf_window)
		{
			Width = 1.29f;
			Height = 1.33f;
		}
		else if (Type == WindowType.tricuspid_window)
		{
			Width = 1.96f;
			Height = 1.33f;
		}
		else if (Type == WindowType.balcony_left_door || Type == WindowType.balcony_right_door)
		{
			Width = 2.12f;
			Height = 2.19f;
		}
		else
		{
			Debug.LogError("Unknown window type");
		}
		return result;
	}



	private void Update()
	{
		//GetDistanceLeftBorder();
	}

}
