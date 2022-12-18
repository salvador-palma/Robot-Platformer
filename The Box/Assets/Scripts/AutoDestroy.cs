using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float Timer;
    void Start()
    {
        StartCoroutine(AD());
    }

    IEnumerator AD()
    {
        yield return new WaitForSeconds(Timer);
        Destroy(this.gameObject);
    }
}
