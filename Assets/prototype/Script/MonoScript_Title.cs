using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoScript_Title : MonoBehaviour
{
    private void Awake()
    {
        float randx = UnityEngine.Random.Range(-7.9f, 7.9f);
        float randy = UnityEngine.Random.Range(10f, 15f);
        float rand_size = UnityEngine.Random.Range(1f, 2f);
        this.gameObject.transform.localScale = new Vector3(rand_size, rand_size, 1);
        this.gameObject.transform.position = new Vector3(randx, randy, 0);
    }
}
