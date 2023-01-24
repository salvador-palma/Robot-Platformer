using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    List<WaterNode> springs = new List<WaterNode>();
    int NodeAmount;

    [Header("Dimensions and Density")]
    public int NodesPerUnit;
    public Vector2Int Size;
    public float WaterLevelOffset;

    [Header("Force Wave Settings")]
    public static float default_stiffness = 0.05f;
    public static float default_decay = 0.1f;
    public float spredForce = 0.9f;

    GameObject WaterNode;

    [Header("Foam Settings")]
    public bool hasFoam = true;
    public float foamHeight;
    public GameObject foam;

    Mesh mesh;
    Vector3[] vertices;
    
    int[] triangles;
    MeshFilter waterFilter;
    MeshFilter foamFilter;
    [Header("Sin Wave Settings")]
    public bool wavy = false;
    public float waveSpeed;
    public float waveLength;
    public float waveHeight;

    Bounds bounds;
    public void Start()
    {
        WaterNode = Resources.Load<GameObject>("WaterNode");
        if (hasFoam)
        {
            GameObject go = Instantiate(foam, transform);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + foamHeight, go.transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - foamHeight + WaterLevelOffset, transform.position.z);
            foamFilter = transform.GetChild(0).GetComponent<MeshFilter>();
        }
        waterFilter = GetComponent<MeshFilter>();
        Vector2 vec = transform.localScale;
        vec.x *= 1 / (float)NodesPerUnit;
        transform.localScale = vec;
        NodeAmount = Size.x * NodesPerUnit;
        NodeAmount+=2;
        Size = new Vector2Int(NodeAmount, Size.y);
        for(int i = 0; i!=NodeAmount; i++)
        {
            GameObject go = Instantiate(WaterNode, transform.position, Quaternion.identity);
            go.transform.parent = gameObject.transform;
            springs.Add(go.GetComponent<WaterNode>());
        }
        
        UpdateMesh();
        UpdateWaterNodes();
    }
    
    public void Update()
    {
        UpdateMesh();
        UpdateWaterNodes();
        UpdateNodes();
        UpdateNeighbourNodes();
    }
    void UpdateNodes()
    {
        foreach(WaterNode node in springs)
        {
            node.UpdateHeight();
        }
    }
    void UpdateNeighbourNodes()
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
    
    void UpdateMesh()
    {
        mesh = new Mesh(); 
        vertices = new Vector3[(Size.x + 1) * (Size.y + 1)];
        for(int i =0, y=0; y <= Size.y; y++)
        {
            for(int x = 0; x <= Size.x; x++)
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
                    int topRow = (Size.x * (Size.y + 1)) - Size.x + 1;
                    vertices[topRow + i] += Vector3.up * (springs[i].height + SinWaveOffset(i));
                }
            }
            else
            {
                for (int i = 0; i < springs.Count; i++)
                {
                    int topRow = (Size.x * (Size.y + 1)) - Size.x + 1;
                    vertices[topRow + i] += Vector3.up * (springs[i].height);
                }
            }
                
            

        }

        triangles = new int[Size.x * Size.y * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < Size.y; z++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + Size.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + Size.x + 1;
                triangles[tris + 5] = vert + Size.x + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        waterFilter.mesh = mesh;
        if (hasFoam) { foamFilter.mesh = mesh; }
        bounds = mesh.bounds;
    }

    void UpdateWaterNodes()
    {
        WaterNode[] nodes = springs.ToArray();

        for (int i = 0; i < springs.Count; i++)
        {
            int topRow = (Size.x * (Size.y + 1)) - Size.x + 1;
            nodes[i].transform.localPosition = vertices[topRow + i];
        }
    }
    float SinWaveOffset(int i)
    {
        return Mathf.Sin((Time.time * waveSpeed) + (i * (1f / waveLength))) * waveHeight;
    }

}
