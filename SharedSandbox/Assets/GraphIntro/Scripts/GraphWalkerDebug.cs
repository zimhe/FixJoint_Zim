using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurUnity;

/*
 * Notes
 */ 

namespace GraphIntro
{
    /// <summary>
    /// 
    /// </summary>
    public class GraphWalkerDebug : MonoBehaviour, IFollowable
    {
        private HeGraph3d _graph;
        private int[] _vertexState;
        private bool _initialized = false;

        private HeGraph3d.Vertex _currVertex;
        private HeGraph3d.Halfedge _currHedge;

        private Color[] _colors = new Color[]
        {
            Color.gray,
            Color.red,
            Color.red,
            Color.black
        };

        private float[] _radii = new float[] { 0.05f, 0.1f, 0.2f, 0.2f };
        private int _countX = 10;
        private int _countY = 10;


        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position
        {
            get { return (Vector3) _currVertex.Position; }
        }


        /// <summary>
        /// 
        /// </summary>
        void Start()
        {
            _graph = CreateGrid(_countX, _countY);
            _vertexState = new int[_graph.Vertices.Count];
            
            _currVertex = _graph.Vertices[0];
            _currHedge = _currVertex.First;
            _initialized = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <returns></returns>
        HeGraph3d CreateGrid(int countX, int countY)
        {
            var graph = new HeGraph3d();

            // add vertices
            for(int i = 0; i < countY; i++)
            {
                for(int j = 0; j < countX; j++)
                {
                    var v = graph.AddVertex();
                    v.Position = new Vec3d(i, j, 0);
                    // v.Normal = blah;
                    // v.Texture = blah;
                }
            }

            // add edges
            int index = 0;
            for (int i = 0; i < countY; i++)
            {
                for (int j = 0; j < countX; j++)
                {
                    if(j > 0)
                        graph.AddEdge(index, index - 1);
                    
                    if(i > 0)
                        graph.AddEdge(index, index - countX);
                    
                    index++;
                }
            }

            return graph;
        }


        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                _currHedge = _currHedge.PreviousAtStart;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                _currHedge = _currHedge.NextAtStart;

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                _currHedge = _currHedge.Twin;
                _currVertex = _currHedge.Start;
            }

            UpdateNeighborhood();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexIndex"></param>
        /// <param name="color"></param>
        void UpdateNeighborhood()
        {
            _vertexState.Clear();
            
            // set state of neighbours
            foreach (var v in _currVertex.ConnectedVertices)
                _vertexState[v.Index] = 1;
            
            _vertexState[_currHedge.End.Index] = 2;
            _vertexState[_currVertex.Index] = 3; // set state of current vertex
        }

    
        /// <summary>
        /// 
        /// </summary>
        void OnDrawGizmos()
        {
            if (!_initialized)
                return;

            Gizmos.matrix = gameObject.transform.localToWorldMatrix;
            
            // draw vertex icons
            foreach(var v in _graph.Vertices)
            {
                Vector3 p = (Vector3)v.Position;
                int state = _vertexState[v.Index];

                Gizmos.color = _colors[state];
                Gizmos.DrawSphere(p, _radii[state]);
            }

            Gizmos.color = _colors[0];

            // draw edge lines
            foreach(var he in _graph.Edges)
            {
                Vector3 p0 = (Vector3)he.Start.Position;
                Vector3 p1 = (Vector3)he.End.Position;
                Gizmos.DrawLine(p0, p1);
            }
        }
    }
}