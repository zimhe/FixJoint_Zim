using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace GraphIntro
{
    public class ShortestPathDebug : MonoBehaviour
    {
        public Transform CameraPivot;
        private float _cameraStiffness = 5.0f;

        private HeGraph3d _graph;
        private int[] _vertexStates;

        private float[] _vertexDistances;
        private int _sourceVertex = 10;

        HeGraph3d.Vertex _currVertex;
        HeGraph3d.Halfedge _currHedge;

        private Color[] _colors = new Color[]
        {
            Color.gray,
            Color.magenta,
            Color.cyan,
            Color.black
        };

        private float[] _radii = new float[] { 0.05f, 0.1f, 0.2f, 0.2f };

        private int _countX = 5;
        private int _countY = 10;

        private bool _initialized = false;


        // Use this for initialization
        void Start()
        {
            // create a graph
            _graph = CreateGrid(_countX, _countY);

            // create array of vertex states
            _vertexStates = new int[_graph.Vertices.Count];

            // set current vertex & halfedge
            _currVertex = _graph.Vertices[0];
            _currHedge = _currVertex.First;

            _initialized = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIndex"></param>
        void CalculateVertexDistancesFrom(int sourceIndex)
        {
            _vertexDistances = new float[_graph.Vertices.Count];
            _vertexDistances.Set(float.PositiveInfinity);

            // create queue
            var queue = new Queue<HeGraph3d.Vertex>();

            // add source vertex/vertices to queue
            queue.Enqueue(_graph.Vertices[sourceIndex]);
            _vertexDistances[sourceIndex] = 0.0f;

            while(queue.Count > 0)
            {
                var v0 = queue.Dequeue();
                var d0 = _vertexDistances[v0]; // distance to v0
                var d1 = d0 + 1.0f; // distance to neighbours from v0 (assumes all edge lengths are 1)

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (d1 < _vertexDistances[v1])
                    {
                        _vertexDistances[v1] = d1;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        HeGraph3d CreateGrid(int countX, int countY)
        {
            HeGraph3d graph = new HeGraph3d();

            // add vertices
            for (int i = 0; i < countY; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    HeGraph3d.Vertex v = graph.AddVertex();
                    v.Position = new Vec3d(i, j, 0);
                    // v.Normal = blah;
                }
            }

            // add edges
            int index = 0;
            for (int i = 0; i < countY; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    if (j > 0)
                        graph.AddEdge(index, index - 1);

                    if (i > 0)
                        graph.AddEdge(index, index - countX);

                    index++;
                }
            }

            return graph;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                _currHedge = _currHedge.PreviousAtStart;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                _currHedge = _currHedge.NextAtStart;
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _currHedge = _currHedge.Twin;
                _currVertex = _currHedge.Start;
            }

            UpdateVertexStates();

            var target = (Vector3)_currVertex.Position;
            CameraPivot.position = Vector3.Lerp(CameraPivot.position, target, Time.deltaTime * _cameraStiffness);
        }


        /// <summary>
        /// 
        /// </summary>
        void UpdateVertexStates()
        {
            _vertexStates.Clear();

            foreach (var v in _currVertex.ConnectedVertices)
                _vertexStates[v] = 1;

            _vertexStates[_currHedge.End] = 2;
            _vertexStates[_currVertex] = 3;
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_initialized) return;

            // draw vertices
            foreach (HeGraph3d.Vertex v in _graph.Vertices)
            {
                int state = _vertexStates[v];
                Gizmos.color = _colors[state];
                Gizmos.DrawSphere((Vector3)v.Position, _radii[state]);
            }

            Gizmos.color = _colors[0];

            // draw edges
            foreach (var he in _graph.Halfedges)
            {
                var p0 = (Vector3)he.Start.Position;
                var p1 = (Vector3)he.End.Position;
                Gizmos.DrawLine(p0, p1);
            }
        }


    }
}
