using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperCubebe
{
    public class VertexList
    {
        List<Vector3> vertices = new List<Vector3>();
        Dictionary<Vector3, int> indices = new Dictionary<Vector3, int>();

        public int AddOrReuse(Vector3 vertex)
        {
            if (indices.TryGetValue(vertex, out int index))
            {
                return index;
            }
            else
            {
                index = vertices.Count;
                vertices.Add(vertex);
                return index;
            }
        }

        public Vector3[] ToArray() => vertices.ToArray();
    }

    public class NaiveVoxelMesher
    {
        public static Mesh GetMesh(VoxelWorld world)
        {
            var vertices = new VertexList();
            var triangles = new List<int>();

            foreach (var face in world.FaceViews())
            {
                // D --- C
                // |   / |
                // |  /  |
                // | /   |
                // A --- B                
                
                var (A, B, C, D) = face.Vertices;
                int iA = vertices.AddOrReuse(A);
                int iB = vertices.AddOrReuse(B);
                int iC = vertices.AddOrReuse(C);
                int iD = vertices.AddOrReuse(D);

                triangles.Add(iA);
                triangles.Add(iB);
                triangles.Add(iC);

                triangles.Add(iA);
                triangles.Add(iC);
                triangles.Add(iD);
            }

            var mesh = new Mesh();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
