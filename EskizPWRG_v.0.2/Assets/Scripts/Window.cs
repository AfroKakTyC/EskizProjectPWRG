using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;

public class Window : MonoBehaviour
{
    public Vector3 Position;
    public Quaternion Rotation;
    public string Type;

    public void UpdatePosition(Vector3 newPosition)
	{
        Position = newPosition;
        gameObject.transform.position = newPosition;
	}

    public void UpdateRotation(Quaternion newRotation)
	{
        gameObject.transform.rotation = newRotation;
	}

}
