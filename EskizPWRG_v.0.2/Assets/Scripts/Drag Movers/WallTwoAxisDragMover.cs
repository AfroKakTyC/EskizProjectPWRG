using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTwoAxisDragMover : MonoBehaviour
{

    public Vector3 startMousePosition;
	public bool isIntersect = false;
    public float direction;
    Vector3 startPosition;

	private void Start()
	{
        startPosition = transform.position;
        direction = 1;
	}

	private void OnMouseDown()
    {
        startMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
    }

    private void OnMouseDrag()
    {



		Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
		float deltaUp = newMousePosition.y - startMousePosition.y;
		direction = transform.TransformDirection(Vector3.right).normalized.x + transform.TransformDirection(Vector3.right).normalized.y + transform.TransformDirection(Vector3.right).normalized.z;
		//Debug.LogError(direction);
		float deltaRight = (startMousePosition.x - newMousePosition.x + startMousePosition.z - newMousePosition.z) * -direction;
		transform.Translate(Vector3.right * deltaRight);
		transform.Translate(Vector3.up * deltaUp);
		startMousePosition = newMousePosition;

	}

	private void OnTriggerEnter(Collider other)
	{
        if (other.name != "Wall1")
        {
            isIntersect = true;
            direction = direction * -1;
            //transform.position = startPosition;
            Debug.LogError("Entered trigger " + other.name);
            Debug.LogError("Direction = " + direction);
        }
	}

	private void OnTriggerExit(Collider other)
	{
        if (other.name != "Wall1")
		{
            isIntersect = false;
            direction = direction * -1;
            Debug.LogError("Exited trigger");
            Debug.LogError("Direction = " + direction);

        }
    }

	private void Update()
	{
	    
	}

	void FixedUpdate()
    {

    }
}
