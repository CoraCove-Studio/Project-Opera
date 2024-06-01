using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace BEngine {

    [CustomEditor(typeof(BEngineTerrain))]
    public class BEngineTerrainEditor : Editor
    {
        BEngineTerrain _terrain;

        void OnEnable() {
            _terrain = target as BEngineTerrain;

            if(_terrain.DrawPositions) {
                _terrain.UpdateEditorTerrain();
            }
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if(GUILayout.Button("Update")) {
                _terrain.UpdateEditorTerrain();
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmo(BEngineTerrain scr, GizmoType gizmoType)
        {
            if (scr.DrawPositions) {
                foreach(var vec in scr.terrainPoints) {
                    Gizmos.DrawCube(vec, scr.DrawPointsSize);
                }
            }
        }

    }
}
