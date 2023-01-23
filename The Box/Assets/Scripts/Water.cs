using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    List<WaterNode> springs = new List<WaterNode>();
    int NodeAmount;

    [Header("Dimensions and Density")]
    public int NodesPerUnit;
    public Vector2Int meshSize;


    [Header("Force Wave Settings")]
    public static float default_stiffness = 0.05f;
    public static float default_decay = 0.1f;
    public float spredForce = 0.9f;

    public GameObject WaterNode;

    Mesh mesh;
    Vector3[] vertices;
    
    int[] triangles;
    MeshFilter filter;

    [Header("Sin Wave Settings")]
    public bool wavy = false;
    public float waveSpeed;
    public float waveLength;
    public float waveHeight;

    Bounds bounds;
    public void Start()
    {
        filter = GetComponent<MeshFilter>();
        Vector2 vec = transform.localScale;
        vec.x *= 1 / (float)NodesPerUnit;
        transform.localScale = vec;
        NodeAmount = meshSize.x * NodesPerUnit;
        NodeAmount+=2;
        meshSize = new Vector2Int(NodeAmount, meshSize.y);
        for(int i = 0; i!=NodeAmount; i++)
        {
            GameObject go = Instantiate(WaterNode, transform.position, Quaternion.identity);
            go.transform.parent = gameObject.transform;
            springs.Add(go.GetComponent<WaterNode>());
        }
        
        SetUpMesh(filter.mesh.bounds);
        SetUpWaterNodes();
    }
    
    public void Update()
    {

        SetUpMesh(bounds);
        SetUpWaterNodes();
        UpdateNodes();
        UpdateNeighbours();
       


        

    }
    void UpdateNodes()
    {
        foreach(WaterNode node in springs)
        {
            node.UpdateHeight();
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
    void UpdateMesh(Bounds b)
    {
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
            
            mesh.vertices = vertices;
            filter.mesh = mesh;

        }
    }
    void SetUpMesh(Bounds r)
    {
        Vector2 min = (Vector2)(transform.worldToLocalMatrix * r.min) - (Vector2)transform.position;
        Vector2 max = (Vector2)(transform.worldToLocalMatrix * r.max) - (Vector2)transform.position;

        mesh = new Mesh(); 
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
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = new Vector2[4 * 8 * 2];
        filter.mesh = mesh;
        bounds = mesh.bounds;
    }

    void SetUpWaterNodes()
    {
        WaterNode[] nodes = springs.ToArray();

        for (int i = 0; i < springs.Count; i++)
        {
            int topRow = (meshSize.x * (meshSize.y + 1)) - meshSize.x + 1;
            nodes[i].transform.localPosition = vertices[topRow + i];
        }
    }
    float GetWaveOffset(int i)
    {
        return Mathf.Sin((Time.time * waveSpeed) + (i * (1f / waveLength))) * waveHeight;
    }

}
