using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfication : MonoBehaviour
{

    public float mass = 1f;
    public float stiffness = 1f;
    public float intensity = 1f;
    public float damping = 0.75f;

    MeshRenderer render;
    Mesh StartingMesh;
    Mesh MeshClone;
    JellyficationVertex[] jVertex;
    Vector3[] vertexArray;

    // Start is called before the first frame update
    void Start()
    {
        StartingMesh = GetComponent<MeshFilter>().sharedMesh;
        MeshClone = Instantiate(StartingMesh);
        GetComponent<MeshFilter>().sharedMesh = MeshClone;
        render = GetComponent<MeshRenderer>();

        jVertex = new JellyficationVertex[MeshClone.vertices.Length];

        for (int i = 0; i < MeshClone.vertices.Length; i++)
        {
            jVertex[i] = new JellyficationVertex(i, transform.TransformPoint(MeshClone.vertices[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        vertexArray = StartingMesh.vertices;
        for (int i = 0; i < jVertex.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jVertex[i].vertexId]);
            float _intensity = (1 - (render.bounds.max.y - target.y) / render.bounds.size.y) * intensity;
            jVertex[i].MoveABit(target, mass, stiffness, damping);
            target = transform.InverseTransformPoint(jVertex[i].position);

            vertexArray[jVertex[i].vertexId] = Vector3.Lerp(vertexArray[jVertex[i].vertexId], target, _intensity);

        }

        MeshClone.vertices = vertexArray;
    }

    public class JellyficationVertex
    {
        public int vertexId;
        public Vector3 force;
        public Vector3 velocity;
        public Vector3 position;

        public JellyficationVertex(int _vertexId, Vector3 _position)
        {
            vertexId = _vertexId;
            position = _position;
        }

        public void MoveABit(Vector3 target, float m, float s, float d)
        {
            force = (target - position) * s;
            velocity = (velocity + force / m) * d;
            position += velocity;

            if ((velocity + force + force / m).magnitude < 0.001f)
            {
                position = target;
            }
        }
    }
}
