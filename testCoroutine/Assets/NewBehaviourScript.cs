using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public IEnumerator testCoroutine()
	{
        Debug.Log("Coroutine 1");
        yield break;
        Debug.Log("Coroutine 2");

        Debug.Log("Coroutine 3");
        //yield return new WaitForSeconds(1);
        Debug.Log("Coroutine 4");
        yield return new WaitForSeconds(1);
       // Debug.Log("Coroutine 5");
        yield return new WaitForSeconds(1);
       // Debug.Log("Coroutine 6");
        yield return new WaitForSeconds(1);
        Debug.Log("Coroutine 7");
    }

    public IEnumerator testCoroutine2()
	{
        Debug.Log("Coroutine2 1");
        yield return new WaitForSeconds(2);
        Debug.Log("Coroutine2 2");
        yield return new WaitForSeconds(2);
        Debug.Log("Coroutine2 3");
        yield return new WaitForSeconds(2);
        Debug.Log("Coroutine2 4");
    }
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(testCoroutine());
        //StartCoroutine(testCoroutine2());



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
