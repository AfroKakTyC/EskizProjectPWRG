using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTiling : MonoBehaviour
{
    MeshRenderer ren;
    // Start is called before the first frame update
    void Start()
    {
        ren = GetComponent<MeshRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ren.material.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
        //Debug.LogError(ren.material.mainTextureOffset + "   " + ren.material.mainTextureScale);
        //Debug.LogError(transform.localScale.x + " " + transform.localScale.y);
    }
}
