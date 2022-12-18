using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        Movement.getInstance().canTurnBack = true;
        Movement.getInstance().lastGhost = gameObject.transform.position;
        Destroy(this.gameObject);
    }
}
