using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AxisRotation : MonoBehaviour
{
    public static AxisRotation INSTANCE;
    
    public float RotationSpeed;

    Transform player;

    public int curFace = 0;
    int prevFace = 0;
    float playerPlacementZ = 0.5f;
    float playerPlacementX = 0.7f;

    public TilemapCollider2D[] colliders = new TilemapCollider2D[4];
    public float offset;

    public float TimeRotation;
    float TimeRotationTimer;
    bool RotationTimer = false;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        INSTANCE = this;
    }
    
    public static AxisRotation getInstance()
    {
        return INSTANCE;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateAxis(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateAxis(-1);
        }
        if (TimeRotationTimer >0)
        {
            TimeRotationTimer -= Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(curFace * 90, transform.up), RotationSpeed * Time.deltaTime);
        }
        else if(RotationTimer)
        {
            RotationTimer = false;
            
            transform.eulerAngles = new Vector3(0, curFace * 90, 0);
            
            player.gameObject.GetComponent<Movement>().stop_player = false;
            
            player.GetComponent<TrailRenderer>().time = 1.2f;
        }

    }

    public void RotateAxis(int dir)
    {
        //CHANGE TO FALSE
        prevFace = curFace;
        colliders[curFace].enabled = false;
        curFace+=dir;
        curFace = curFace == 4 ? 0 : curFace == -1 ? 3 : curFace;
        colliders[curFace].enabled = true;
        
        
        player.gameObject.GetComponent<Movement>().stop_player = true;
        player.localRotation = Quaternion.Euler(0, curFace * 90, 0);
        switch (prevFace * 4 + curFace) //0,0   0,1   0,2   0,3   1,0   1,1   1,2   1,3   2,0   2,1   2,2   2,3   3,0   3,1   3,2   3,3
        {
            //0,1   1,0
            case 1:
                player.localPosition = new Vector3(offset, player.position.y, player.position.z + playerPlacementX);
                Debug.Log(player.position);
                break;
            case 4:
                player.localPosition = new Vector3(-player.position.x - playerPlacementZ, player.position.y, -offset);
                break;

            //1,2   2,1
            case 6:
                player.localPosition = new Vector3(player.position.x - playerPlacementZ, player.position.y, offset);
                break;
            case 9:
                player.localPosition = new Vector3(offset, player.position.y, -player.position.z - playerPlacementX);
                break;

            //2,3   3,2
            case 11:
                player.localPosition = new Vector3(-offset, player.position.y, -player.position.z - playerPlacementX);
                break;
            case 14:
                player.localPosition = new Vector3(player.position.x + playerPlacementZ, player.position.y, offset);
                break;

            //0,3   3,0
            case 3:
                player.localPosition = new Vector3(-offset, player.position.y, player.position.z + playerPlacementX);
                break;
            case 12:
                player.localPosition = new Vector3(-player.position.x + playerPlacementZ, player.position.y, -offset);
                break;

        }


        TimeRotationTimer = TimeRotation;
        RotationTimer = true;
        Movement m = Movement.getInstance();
        m.canTurnBack = false;
        GameObject[] clones = GameObject.FindGameObjectsWithTag("Ghost");
        foreach(GameObject g in clones)
        {
            Destroy(g);
        }
        player.GetComponent<TrailRenderer>().time = 0f;


    }
    
}
