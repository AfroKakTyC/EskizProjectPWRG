using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperementalMouseDrag : MonoBehaviour
{
    public Rigidbody rb2 = null;
    float speed1 = 10f;
    float speed2 = 2000f;
    // Start is called before the first frame update
    void Start()
    {
        rb2 = gameObject.GetComponent<Rigidbody>();
        //rb2.constraints = RigidbodyConstraints.FreezeAll;
    }

	//private void OnMouseDrag()
	//{
 //       float h = Input.GetAxis("Mouse X");
 //       float v = Input.GetAxis("Mouse Y");
 //       Debug.LogError(h + " " + v);
 //       Vector3 velocity = ((transform.up * v) + (transform.right * (-h))) * speed2 * Time.fixedDeltaTime;
 //       Rigidbody rb = gameObject.GetComponent<Rigidbody>();
 //       rb.velocity = velocity;


 // //      //rb2.constraints = RigidbodyConstraints.None;
 // //      //rb2.constraints = RigidbodyConstraints.FreezeRotation;
 // //      Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
	//	////gameObject.transform.position = newMousePosition;
	//	//rb2.MovePosition(newMousePosition);
	//}

	private void OnMouseUp()
	{
        //rb2.constraints = RigidbodyConstraints.FreezeAll;
        //rb2.constraints = RigidbodyConstraints.None;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
		if (Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				Transform currentObject = hit.transform;
				Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3));
				currentObject.GetComponent<Rigidbody>().MovePosition(newMousePosition);
				//currentObject.transform.position = Vector3.Lerp(transform.position, newMousePosition, 1 * Time.deltaTime);
			}
		}
	}
}
