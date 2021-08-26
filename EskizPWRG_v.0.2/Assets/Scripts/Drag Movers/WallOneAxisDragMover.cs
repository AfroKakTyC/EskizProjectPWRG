using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOneAxisDragMover : MonoBehaviour
{

    public Vector3 startMousePosition;

    // Update is called once per frame
    private void OnMouseDown()
    {
        startMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
    }

    private void OnMouseDrag()
    {
        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
        float direction = transform.TransformDirection(Vector3.right).normalized.x + transform.TransformDirection(Vector3.right).normalized.y + transform.TransformDirection(Vector3.right).normalized.z;
        float deltaRight = (startMousePosition.x - newMousePosition.x + startMousePosition.z - newMousePosition.z) * -direction;
        transform.Translate(Vector3.right * deltaRight);
        startMousePosition = newMousePosition;
    }

    void FixedUpdate()
    {

    }
}
