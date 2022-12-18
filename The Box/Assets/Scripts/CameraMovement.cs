using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform LookAt;
    public bool isFollow = true;
    public float boundX;
    public float boundY;
    public float speed;
    public float scroll_speed;
    Vector3 DesiredPosition;
    public Vector2 player;
    private void Start()
    {
        LookAt = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        transform.position = new Vector3(LookAt.position.x, LookAt.position.y, transform.position.z);
    }
    void LateUpdate()
    {
        if (isFollow == true)
        {
            Vector3 delta = Vector3.zero;

            float dx = LookAt.position.x - transform.position.x;

            if (dx > boundX || dx < -boundX)
            {
                if (transform.position.x < LookAt.position.x)
                {
                    delta.x = dx - boundX;
                }
                else
                {
                    delta.x = dx + boundX;
                }

            }

            float dy = LookAt.position.y - transform.position.y;

            if (dy > boundY || dy < -boundY)
            {
                if (transform.position.y < LookAt.position.y)
                {
                    delta.y = dy - boundY;
                }
                else
                {
                    delta.y = dy + boundY;
                }

            }
            DesiredPosition = transform.position + delta;
            transform.position = Vector3.Lerp(transform.position, DesiredPosition, speed);
        }
        

    }


}
