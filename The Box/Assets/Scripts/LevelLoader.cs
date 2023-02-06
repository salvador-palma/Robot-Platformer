using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour
{
    static public Vector2 Spawn_Coords;
    static public Animator panel;
    // Start is called before the first frame update

    
        
    
    public string LevelToLoad;
    public Vector2 CoordsToSpawn;

    public static Vector2 nextSpawn;
    public static bool toLoad;
    public static string nextLevel;
    public void Start()
    {
        panel = GameObject.Find("SceneTransitor").GetComponent<Animator>();
        Spawn_Coords = new Vector2(-10, -1);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nextSpawn = CoordsToSpawn;
            nextLevel = LevelToLoad;
            toLoad = !gameObject.CompareTag("Reset");
            
            panel.Play("SceneEnd");
             
        }
    }
    public void ReturnScene()
    {
        if (toLoad)
        {
            SceneManager.LoadScene(nextLevel);
            Movement.getInstance().gameObject.transform.position = nextSpawn;
        }
        else
        {
            GameObject p = Movement.getInstance().gameObject;
            p.transform.position = nextSpawn;
            p.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        panel.Play("SceneStart");
    }
}
