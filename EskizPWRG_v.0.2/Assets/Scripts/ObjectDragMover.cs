using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragMover : MonoBehaviour
{

    public Vector3 startMousePosition;
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError(transform.name);
        Debug.LogError(transform.TransformDirection(Vector3.right));
    }

    void GetRaycastHit()
	{
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow, 5);
            //Debug.LogError(hit.distance - 0.98f);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
    }

	// Update is called once per frame
	private void OnMouseDown()
	{
        startMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
        //Debug.DrawRay(transform.position, Vector3.forward * 10, Color.red, 5f);
        
        //Debug.LogError("GOVNO");
    }

	private void OnMouseDrag()
	{
        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
        float deltaUp = newMousePosition.y - startMousePosition.y;
        //Debug.LogError(startMousePosition + "   " + newMousePosition);
        float direction = transform.TransformDirection(Vector3.right).normalized.x + transform.TransformDirection(Vector3.right).normalized.y + transform.TransformDirection(Vector3.right).normalized.z;
        float deltaRight = (startMousePosition.x - newMousePosition.x + startMousePosition.z - newMousePosition.z) * -direction;
        Debug.LogError(transform.TransformDirection(Vector3.right));
        transform.Translate(Vector3.right * deltaRight);
        transform.Translate(Vector3.up * deltaUp);
        startMousePosition = newMousePosition;
    }

	void FixedUpdate()
    {
        GetRaycastHit();

        //if (gameObject.GetComponent<Rigidbody>)
        //transform.Translate(Vector3.right * 0.01f);
    }
}
