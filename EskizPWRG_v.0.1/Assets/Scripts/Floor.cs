using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTypes;

public class Floor : MonoBehaviour
{
	public Material wallMaterial;
	public string Name;
	public string materialId = "";
	//public Vector2[] RoomCorners;
	public float Length;
	public float Width;

	public void CreateFloor(Vector2 downLeftCoord, Vector2 upperRigthCoord, float height)
	{
		//RoomCorners = new Vector2[roomCorners.Length];
		//roomCorners.CopyTo(RoomCorners, 0);
		Vector2 lengthVector = new Vector2(downLeftCoord.x, upperRigthCoord.y);
		var heading = lengthVector - downLeftCoord;
		var distance = heading.magnitude;
		Length = distance;
		heading = upperRigthCoord - lengthVector;
		distance = heading.magnitude;
		Width = distance;
		Mesh mesh = new Mesh();
		//Vector2[] RoomCorners = { new Vector2(0f, 0f), new Vector2(0f, 5f), new Vector2(5f, 5f), new Vector2(5f, 0f) };

		Vector3[] vertices = new Vector3[4]
	   {
			new Vector3(downLeftCoord.x, height, downLeftCoord.y),
			new Vector3(downLeftCoord.x, height, upperRigthCoord.y),
			new Vector3(upperRigthCoord.x, height, upperRigthCoord.y),
			new Vector3(upperRigthCoord.x, height, downLeftCoord.y)
	   };
		mesh.vertices = vertices;

		int[] tris;
		if (height == 0)
		{
			tris = new int[6]
				{
			// lower left triangle
			0, 1, 2,
			// upper right triangle
			0, 2, 3
			};
		}
		else
		{
			tris = new int[6]
			{
				// lower left triangle
				2, 1, 0,
				// upper right triangle
				3, 2, 0
			};
		}

		mesh.triangles = tris;

		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		};
		mesh.uv = uv;

		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		wallMaterial = Resources.Load<Material>("Default/DefaultMaterial");
		UpdateMaterial(wallMaterial);
		//UpdateMaterial();
	}

	public void CreateFloor(Vector2[] roomCorners, float height)
	{
		//RoomCorners = new Vector2[roomCorners.Length];
		//roomCorners.CopyTo(RoomCorners, 0);
		var heading = roomCorners[1] - roomCorners[0];
		var distance = heading.magnitude;
		Length = distance;
		heading = roomCorners[2] - roomCorners[1];
		distance = heading.magnitude;
		Width = distance;
		Mesh mesh = new Mesh();

		Vector2[] RoomCorners = { new Vector2(0f, 0f), new Vector2(0f, 5f), new Vector2(5f, 5f), new Vector2(5f, 0f) };

		Vector3[] vertices = new Vector3[4]
	   {
			new Vector3(roomCorners[0].x, height, roomCorners[0].y),
			new Vector3(roomCorners[1].x, height, roomCorners[1].y),
			new Vector3(roomCorners[2].x, height, roomCorners[2].y),
			new Vector3(roomCorners[3].x, height, roomCorners[3].y)
	   };
		mesh.vertices = vertices;

		int[] tris;
		if (height == 0)
		{
			tris = new int[6]
				{
			// lower left triangle
			0, 1, 2,
			// upper right triangle
			0, 2, 3
			};
		}
		else
		{
			tris = new int[6]
			{
				// lower left triangle
				2, 1, 0,
				// upper right triangle
				3, 2, 0
			};
		}
		mesh.triangles = tris;

		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		};
		mesh.uv = uv;

		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		wallMaterial = Resources.Load<Material>("Default/DefaultMaterial");
		UpdateMaterial(wallMaterial);
		//UpdateMaterial();
	}

	public void UpdateMaterial(Material updateMaterial)
	{
		if (updateMaterial == null)
			return;
		wallMaterial = updateMaterial;
		materialId = updateMaterial.name;
		gameObject.GetComponent<MeshRenderer>().material = updateMaterial;
		UpdateTextureScale();
	}

	void UpdateTextureScale()
	{
		Vector2 vanillaTextureScale = wallMaterial.mainTextureScale;
		Vector2 textureScale = new Vector2(Length * vanillaTextureScale.x, Width * vanillaTextureScale.y);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_MainTex"), textureScale);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_BumpMap"), textureScale);
		gameObject.GetComponent<MeshRenderer>().material.SetTextureScale(Shader.PropertyToID("_GlossMap"), textureScale);
	}

	
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
