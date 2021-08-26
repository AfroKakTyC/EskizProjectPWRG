using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spizjeniymover : MonoBehaviour
{
	public string[] tags; // массив тегов, объекты которых можно двигать
	public Camera _camera; // основная камера сцены
	public enum ProjectMode { Project3D = 0, Project2D = 1 };
	public ProjectMode mode = ProjectMode.Project3D;
	public float step = 5; // шаг для изменения высоты в 3D
	private Transform curObj;
	private float mass;

	void Start()
	{
		if (mode == ProjectMode.Project2D) _camera.orthographic = true;
	}

	bool GetTag(string curTag)
	{
		bool result = false;
		foreach (string t in tags)
		{
			if (t == curTag) result = true;
		}
		return result;
	}

	void Update()
	{
		if (Input.GetMouseButton(1)) // Удерживать правую кнопку мыши
		{
			if (mode == ProjectMode.Project3D)
			{
				RaycastHit hit;
				Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit))
				{
					if (GetTag(hit.transform.tag) && hit.rigidbody && !curObj)
					{
						curObj = hit.transform;
						mass = curObj.GetComponent<Rigidbody>().mass; // запоминаем массу объекта
						curObj.GetComponent<Rigidbody>().mass = 0.0001f; // убираем массу, чтобы не сбивать другие объекты
						curObj.GetComponent<Rigidbody>().useGravity = false; // убираем гравитацию
						curObj.GetComponent<Rigidbody>().freezeRotation = true; // заморозка вращения
						curObj.position += new Vector3(0, 0.5f, 0); // немного приподымаем выбранный объект
					}
				}

				if (curObj)
				{
					Vector3 mousePosition = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.y));
					curObj.GetComponent<Rigidbody>().MovePosition(new Vector3(mousePosition.x, curObj.position.y + Input.GetAxis("Mouse ScrollWheel") * step, mousePosition.z));

				}
			}
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if (hit.transform != null)
				{
					if (GetTag(hit.transform.tag) && hit.rigidbody && !curObj)
					{
						curObj = hit.transform;
						mass = curObj.GetComponent<Rigidbody2D>().mass;
						curObj.GetComponent<Rigidbody2D>().mass = 0.0001f;
						curObj.GetComponent<Rigidbody2D>().gravityScale = 0;
						curObj.GetComponent<Rigidbody2D>().freezeRotation = true;
						curObj.position += new Vector3(0, 0.5f, 0);
					}
				}

				if (curObj)
				{
					Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
					curObj.GetComponent<Rigidbody2D>().MovePosition(new Vector3(mousePosition.x, mousePosition.y, curObj.position.z));
				}
			}
		}
		else if (curObj)
		{
			if (curObj.GetComponent<Rigidbody>())
			{
				curObj.GetComponent<Rigidbody>().freezeRotation = false;
				curObj.GetComponent<Rigidbody>().useGravity = true;
				curObj.GetComponent<Rigidbody>().mass = mass;
			}
			else
			{
				curObj.GetComponent<Rigidbody2D>().freezeRotation = false;
				curObj.GetComponent<Rigidbody2D>().mass = mass;
				curObj.GetComponent<Rigidbody2D>().gravityScale = 1;
			}
			curObj = null;
		}
	}
}
