using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 Position;
    public Quaternion Rotation;
    public string MaterialId;

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
