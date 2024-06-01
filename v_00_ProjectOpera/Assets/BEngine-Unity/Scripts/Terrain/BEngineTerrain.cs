using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace BEngine {

    #if UNITY_EDITOR

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [CanEditMultipleObjects]
    #endif

    public class BEngineTerrain : MonoBehaviour
    {
        
        [Tooltip("Terrain Component. Just Drag'n'Drop a Terain Object into this Input.")]
        public Terrain terrain;

        [Tooltip("Size of a Terrain Patch.")]
        public Vector3 TerrainSize = new Vector3(100, 100, 100);

        [HideInInspector]
        public int NumberSegmentsX = 0;
        [HideInInspector]
        public int NumberSegmentsY = 0;

        public bool DrawPositions = true;
        public Vector3 DrawPointsSize = new Vector3(0.3f, 0.3f, 0.3f);

        [Tooltip("Step Between Every Point (Meters). The More Step the Less Points.")]
        [Min(0.01f)]
        public float Step = 10;


        #if UNITY_EDITOR
        [HideInInspector]
        public List<Vector3> terrainPoints = new List<Vector3>();


        public virtual void UpdateEditorTerrain() {
            if (terrain != null) {
                terrainPoints = GenerateTerrain();
                
            }
        }


        void OnDrawGizmos()
        {


            Gizmos.DrawWireCube(this.transform.position, TerrainSize);
        }


            #endif


        public virtual void GenerateSegments() {

            int stepsX = Mathf.Max( (int)Mathf.Floor(TerrainSize.x / Step), 1 );
            int stepsZ = Mathf.Max( (int)Mathf.Floor(TerrainSize.z / Step), 1 );

            // Set Points Lengths
            NumberSegmentsX = stepsX;
            NumberSegmentsY = stepsZ;

        }


        public virtual List<Vector3> GenerateTerrain() {

            if (terrain != null) {
                List<Vector3> newPoints = new List<Vector3>();

                Vector3 thisPos = this.transform.position;

                GenerateSegments();

                float subX = ((NumberSegmentsX * Step) / 2.0f);
                float subZ = ((NumberSegmentsY * Step) / 2.0f);

                Vector3 defaultZeroPos = new Vector3(thisPos.x - subX, 0, thisPos.z - subZ);

                for (int i = 0; i < NumberSegmentsY + 1; i++)
                {
                    float vertZ = (i * Step) + defaultZeroPos.z;

                    for (int j = 0; j < NumberSegmentsX + 1; j++)
                    {
                        var getPos = new Vector3( (j * Step) + defaultZeroPos.x, 0,  vertZ);

                        float height = terrain.SampleHeight(getPos) + terrain.transform.position.y;
                        getPos.y = height;

                        newPoints.Add(getPos);
                    }

                }

                return newPoints;
            }

            return null;
        }


    }

}
