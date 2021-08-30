using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToMousePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


	private void OnMouseDrag()
	{
        Debug.LogError("Dragging");
		LayerMask layerMask = LayerMask.GetMask("Wall");
		Vector3 mouse = Input.mousePosition;
		Ray castPoint = Camera.main.ScreenPointToRay(mouse);
		RaycastHit hit;
		if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, layerMask))
		{
			Debug.LogError(hit.collider.name);
			gameObject.transform.position = hit.point;
		}
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
