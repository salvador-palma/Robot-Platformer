using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public List<WaterNode> springs = new List<WaterNode>();
    public int NodeAmount = 5;
    public float default_stiffness = 0.5f;
    public float default_decay = 0.9f;
    public float spredForce;

    public Vector2Int meshSize;

    Mesh mesh;
    public Vector3[] vertices;
    
    public int[] triangles;
    public MeshFilter filter;

    public bool wavy = false;
    public float waveSpeed;
    public float waveLength;
    public float waveHeight;

    Bounds bounds;
    public void Start()
    {
        
        for(int i = 0; i!=NodeAmount; i++)
        {
            springs.Add(new WaterNode(default_stiffness, default_decay));
        }
        SetUpMesh(filter.mesh.bounds);
    }
    public void Update()
    {
        //UpdateNeighbours();
        if(bounds != null)
        {
            SetUpMesh(bounds);
        }
        

    }
    void UpdateNeighbours()
    {
        for (int i = 0; i < springs.Count; i++)
        {
            if (i > 0)
            {
                springs[i - 1].velocity += spredForce * (springs[i].height - springs[i - 1].height);
            }
            if (i < springs.Count - 1)
            {
                springs[i + 1].velocity += spredForce * (springs[i].height - springs[i + 1].height);
            }
        }
    }

    void SetUpMesh(Bounds r)
    {
        Vector2 min = (Vector2)(transform.worldToLocalMatrix * r.min) - (Vector2)transform.position;
        Vector2 max = (Vector2)(transform.worldToLocalMatrix * r.max) - (Vector2)transform.position;

        mesh = new Mesh(); //defined\
        vertices = new Vector3[(meshSize.x + 1) * (meshSize.y + 1)];
        for(int i =0, y=0; y <= meshSize.y; y++)
        {
            for(int x = 0; x <= meshSize.x; x++)
            {
                vertices[i] = new Vector3(x, y, 0);
                i++;
            }
        }

        if (Application.isPlaying)
        {
            if (wavy)
            {
                for (int i = 0; i < springs.Count; i++)
                {
                    int topRow = (meshSize.x * (meshSize.y + 1)) - meshSize.x + 1;
                    vertices[topRow + i] += Vector3.up * (springs[i].height + GetWaveOffset(i));
                }
            }
            else
            {
                for (int i = 0; i < springs.Count; i++)
                {
                    int topRow = (meshSize.x * (meshSize.y + 1)) - meshSize.x + 1;
                    vertices[topRow + i] += Vector3.up * (springs[i].height);
                }
            }
                
            

        }

        triangles = new int[meshSize.x * meshSize.y * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < meshSize.y; z++)
        {
            for (int x = 0; x < meshSize.x; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + meshSize.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + meshSize.x + 1;
                triangles[tris + 5] = vert + meshSize.x + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.Clear();
        foreach(Vector2 v in vertices)
        {
           // Debug.Log(v);
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        filter.mesh = mesh;
        bounds = mesh.bounds;
    }

    float GetWaveOffset(int i)
    {
        return Mathf.Sin((Time.time * waveSpeed) + (i * (1f / waveLength))) * waveHeight;
    }

}
