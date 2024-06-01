using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System;
using System.Diagnostics;
using System.IO;
using BESimpleJSON;

using System.Linq;

namespace BEngine
{
    public static class BEMeshWorks
    {

        public static Color BASE_COLOR = Color.black;
        public static Vector3 BASE_VECTOR = Vector3.zero;
        public static Vector2 BASE_UV = Vector2.zero;


        // To Compare Vertices/Points
        public struct VertexItem {

            public List<Vector2> UVList;
            public Vector3 Normal;
            public Color VertexColor;
            public int VertID;


            public VertexItem(Vector3 normal, Color vertColor, List<Vector2> uvList, int vertID)
            {
                Normal = normal;
                UVList = uvList;
                VertID = vertID;
                VertexColor = vertColor;
            }

        }


        public static (Mesh, int[]) CreateMeshFromJSON(JSONNode jsMeshNode, BEngineComponent beComp, float meshCounter) {
            Mesh mesh = new Mesh();
            mesh.name = "BE_" + beComp.name  + "_" + meshCounter.ToString();

            JSONNode jsPolyIndices = jsMeshNode["PolyIndices"];
            JSONNode jsLoopStarts = jsMeshNode["LoopStart"];

            // Get Verts
            List<Vector3> verts = new List<Vector3>();
            var vertsJS = jsMeshNode["Verts"];

            for (int i = 0; i < (vertsJS.Count / 3); i++) {
                verts.Add(new Vector3(vertsJS[i * 3], vertsJS[i * 3 + 2], vertsJS[i * 3 + 1]));
            }

            // Get Normals
            List<Vector3> normalsBlender = new List<Vector3>();
            SortedDictionary<int, Vector3> normalsFinal = new SortedDictionary<int, Vector3>();

            var vertNormalsJS = jsMeshNode["Normals"];

            for (int i = 0; i < (vertNormalsJS.Count / 3); i++) {
                normalsBlender.Add(new Vector3(vertNormalsJS[i * 3], vertNormalsJS[i * 3 + 2], vertNormalsJS[i * 3 + 1]));
            }

            // Get Vertex Colors
            List<Color> vertColorsBlender = new List<Color>();
            SortedDictionary<int, Color> vertColorsFinal = new SortedDictionary<int, Color>();

            if (jsMeshNode.HasKey("VertexColors")) {
                
                var vertColorsJS = jsMeshNode["VertexColors"];

                for (int i = 0; i < (vertColorsJS.Count / 4); i++) {
                    Color col = new Color(vertColorsJS[i * 4], vertColorsJS[i * 4 + 1], vertColorsJS[i * 4 + 2], vertColorsJS[i * 4 + 3]);
                    vertColorsBlender.Add(col);
                }
            }

            // Get UVs from Blender
            List<List<Vector2>> uvMapsBlender = new List<List<Vector2>>();
            List<int> uvMapsBlenderIndeces =  new List<int>();
            List<SortedDictionary<int, Vector2>> uvMapsFinal = new List<SortedDictionary<int, Vector2>>();

            foreach (var jsUVNode in jsMeshNode["UVs"]) {
                List<Vector2> uvMap = new List<Vector2>();

                var jsUV = jsUVNode.Value;

                int uvMult = 2;  // Vector2D

                // Get UV Coordinates
                for (int i = 0; i < (jsUV.Count / uvMult); i++) {
                    uvMap.Add(new Vector2(jsUV[i * uvMult], jsUV[i * uvMult + 1]));
                }

                uvMapsBlender.Add(uvMap);
                uvMapsFinal.Add(new SortedDictionary<int, Vector2>());

                // Setup UV Index
                int uvID = 0;

                switch (jsUVNode.Key){
                    case "uv_map2":
                    uvID = 1;
                    break;

                    case "uv_map3":
                    uvID = 2;
                    break;  

                    case "uv_map4":
                    uvID = 3;
                    break;

                    case "uv_map5":
                    uvID = 4;
                    break;

                    case "uv_map6":
                    uvID = 5;
                    break;

                    case "uv_map7":
                    uvID = 6;
                    break;

                    case "uv_map8":
                    uvID = 7;
                    break;
                }

                uvMapsBlenderIndeces.Add(uvID);

            }

            // Setup Polygons(Triangles) and Materials
            SortedDictionary<int, List<int>> subMeshes = new SortedDictionary<int, List<int>>();
            JSONNode jsMaterials = null;


            if (jsMeshNode.HasKey("Materials")) {
                jsMaterials =  jsMeshNode["Materials"];
            }

            JSONNode matID;

            Dictionary<int, List<VertexItem>> compareVerts = new Dictionary<int, List<VertexItem>>();

            // Materials, Polygons, Submeshes
            for (int i=0; i < jsLoopStarts.Count; i++ ) {
                // var jsPOly = jsPolyIndices[i];

                int loopStart = jsLoopStarts[i];
                int loopEnd;

                if (i < jsLoopStarts.Count - 1) {
                    loopEnd = jsLoopStarts[i + 1] - 1;
                }
                else {
                    loopEnd = jsPolyIndices.Count - 1;
                }

                int vertsCount = loopEnd - loopStart + 1;

                if (vertsCount > 2) {
                    matID = 0;  // 0 index. If no materials or a material = 0

                    if (jsMaterials != null) {
                        matID = jsMaterials[i];
                    }

                    // Add Submeshes and Materials Items
                    if(!subMeshes.ContainsKey(matID)) {
                        subMeshes[matID] = new List<int>();
                    }

                    var submeshList = subMeshes[matID];
                    HashSet<int> checkedVerts = new HashSet<int>();

                    // Get All Triangles of Polygon(Quad, NGon)
                    for (int u = 0; u < vertsCount - 2; u++) {
                        int[] triangleIndices = new int[] {jsPolyIndices[loopStart], jsPolyIndices[u + 1 + loopStart], jsPolyIndices[u + 2 + loopStart]};
                        
                        // Split Unique Points and make final UVs/Colors/Normals
                        for (int j=0; j < 3; j++ ) {
                            if (!checkedVerts.Contains(triangleIndices[j])) {

                                var loopID = 0;
                                if (j == 1) {
                                    loopID = u + 1;
                                }
                                else if (j == 2) {
                                    loopID = u + 2;
                                }

                                List<Vector2> uvCompareList = new List<Vector2>();

                                foreach (var uvMap in uvMapsBlender) {

                                    var uvVec = uvMap[jsLoopStarts[i] + loopID];
                                    uvCompareList.Add(uvVec);
                                }

                                Vector3 normalVec = normalsBlender[jsLoopStarts[i] + loopID];

                                Color vertColor;
                                if (vertColorsBlender.Count > 0) {
                                    vertColor = vertColorsBlender[jsLoopStarts[i] + loopID];
                                }
                                else {
                                    vertColor = BASE_COLOR;
                                }

                                VertexItem vertItem = new VertexItem(normalVec, vertColor, uvCompareList, triangleIndices[j]);

                                if (!compareVerts.ContainsKey(triangleIndices[j])) {
                                    compareVerts[triangleIndices[j]] = new List<VertexItem>() {vertItem};

                                    // Add Final UV Point
                                    for (int k = 0; k < uvMapsFinal.Count; k++) {
                                        uvMapsFinal[k][triangleIndices[j]] = vertItem.UVList[k];
                                    }

                                    // Add Final Normal
                                    normalsFinal[triangleIndices[j]] = vertItem.Normal;

                                    // Add Final Color
                                    if (vertColorsBlender.Count > 0) {
                                        vertColorsFinal[triangleIndices[j]] = vertItem.VertexColor;
                                    }
                                }

                                else {
                                    bool areVertsEqual = false;

                                    foreach (var uniqueVertItem in compareVerts[triangleIndices[j]]) {

                                        areVertsEqual = CompareVertexItems(uniqueVertItem, vertItem);

                                        if (areVertsEqual) {
                                            triangleIndices[j] = uniqueVertItem.VertID;  // Changed Vert ID

                                            break;
                                        }
                                        
                                    }

                                    if(!areVertsEqual) {
                                        int newVertID = verts.Count;

                                        // Add New Unique Final UV Point
                                        for (int k = 0; k < uvMapsFinal.Count; k++) {
                                            uvMapsFinal[k][newVertID] = vertItem.UVList[k];
                                        }

                                        // Add New Unique Final Normal
                                        normalsFinal[newVertID] = vertItem.Normal;

                                        // Add New Unique Final Vertex Color
                                        if (vertColorsBlender.Count > 0) {
                                            vertColorsFinal[newVertID] = vertItem.VertexColor;
                                        }

                                        // Add New Unique Vert
                                        verts.Add( (Vector3)verts[triangleIndices[j]] );

                                        // Add new Vertex item to the Compare List
                                        vertItem.VertID = newVertID;
                                        compareVerts[triangleIndices[j]].Add(vertItem);

                                        triangleIndices[j] = newVertID;  // Changed Vert ID
                                    }
                                }

                                checkedVerts.Add(triangleIndices[j]);
                            }
                        }

                        // Add Polygon to Mesh/Submesh
                        submeshList.Add(triangleIndices[2]);
                        submeshList.Add(triangleIndices[1]);
                        submeshList.Add(triangleIndices[0]);

                    }
                }
            }

            // Fix if Vertices > than Other Data (Normals, UV, Colors)
            // This happens when a Mesh has some Vertices without Polygons
            if(verts.Count != normalsFinal.Count) {
                for(int i = 0; i < verts.Count; i++) {
                    if (!normalsFinal.ContainsKey(i)) {

                        // Set Zero Normal
                        normalsFinal[i] = BASE_VECTOR;

                        // Set Zero UV
                        for (int j = 0; j < uvMapsFinal.Count; j++) {
                            uvMapsFinal[j][i] = BASE_UV;
                        }

                        // Set Zero Color
                        if (vertColorsBlender.Count > 0) {
                            vertColorsFinal[i] = BASE_COLOR;
                        }
                    }
                }
            }


            // Setup Mesh Index Format to 32 Bit
            if(verts.Count >= 65500) {
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            // Setup Vertices
            mesh.vertices = verts.ToArray();

            mesh.subMeshCount = subMeshes.Keys.Count;
            
            // Setup Submeshes Polygons(Triangles)
            int iCounter = 0;
            foreach (var subMesh in subMeshes.Values) {
                // mesh.triangles = tris.ToArray();
                mesh.SetTriangles(subMesh.ToArray(), iCounter);
                iCounter++;
            }

            // Setup Normals
            mesh.normals = normalsFinal.Values.ToArray();

            // Setup Vertex vertColorsFinal
            if (vertColorsFinal.Count > 0) {
                mesh.colors = vertColorsFinal.Values.ToArray();
            }

            // Setup UVs
            for (int i = 0; i < uvMapsFinal.Count; i++) {
                mesh.SetUVs(uvMapsBlenderIndeces[i], uvMapsFinal[i].Values.ToArray());
            }

            mesh.Optimize();
            // MeshUtility.Optimize(mesh);

            mesh.RecalculateBounds();

            // Setup Normals/Tangents
            // mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return ( mesh, subMeshes.Keys.ToArray() );
        }


        public static Mesh MeshFromJSON(JSONNode jsMeshNode, GameObject gameObj, BEngineComponent beComp) {
            // Create Mesh
            if(jsMeshNode["Verts"] != null && jsMeshNode["Verts"].Count > 0) {
                var meshValues = BEMeshWorks.CreateMeshFromJSON(jsMeshNode, beComp, beComp.meshCounter);

                beComp.meshCounter += 1;  // Add +1 to Unique Meshes

                var mesh = meshValues.Item1;
                var materialsIndices = meshValues.Item2;
                // var maxMatsVal = meshValues.Item3;

                // Create Mesh Filter
                var meshFilter = gameObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;

                // Create Mesh Renderer
                var meshRend = gameObj.AddComponent<MeshRenderer>();

                // Set Materials
                List<Material> finalMaterials = new List<Material>();
                foreach (var matIdx in materialsIndices) {
                    if (matIdx < beComp.Outputs.OutputMaterials.Count) {
                        finalMaterials.Add(beComp.Outputs.OutputMaterials[matIdx].Material);
                    }
                    else {
                        finalMaterials.Add(null);
                    }
                }
                meshRend.sharedMaterials = finalMaterials.ToArray();

                return mesh;
            }

            return null;
        }


        public static bool CompareVertexItems(VertexItem item1, VertexItem item2) {
            if (item1.Normal.x != item2.Normal.x) return false;
            else if (item1.Normal.y != item2.Normal.y) return false;
            else if (item1.Normal.z != item2.Normal.z) return false;
    
            if (item1.VertexColor.r != item2.VertexColor.r) return false;
            else if (item1.VertexColor.g != item2.VertexColor.g) return false;
            else if (item1.VertexColor.b != item2.VertexColor.b) return false;
            else if (item1.VertexColor.a != item2.VertexColor.a) return false;

            for (int i = 0; i < item1.UVList.Count; i++) {
                if (item1.UVList[i].x != item2.UVList[i].x) return false;
                else if (item1.UVList[i].y != item2.UVList[i].y) return false;

            }
            
            return true;
        }


        public static void SetTransformFromJSON(GameObject obj, JSONNode jsPos, JSONNode jsRot, JSONNode jsScale) {
            // Setup Position
            Vector3 instaPos = new Vector3();
            instaPos.x = jsPos[0];
            instaPos.y = jsPos[2];
            instaPos.z = jsPos[1];

            obj.transform.localPosition = instaPos;

            // Setup Rotation
            var instaRot_x = -(jsRot[0] * Mathf.Rad2Deg);
            var instaRot_y = -(jsRot[2] * Mathf.Rad2Deg);
            var instaRot_z = -(jsRot[1] * Mathf.Rad2Deg);

            obj.transform.rotation = new Quaternion();

            obj.transform.Rotate(Vector3.up, instaRot_y);
            obj.transform.Rotate(Vector3.forward, instaRot_z);
            obj.transform.Rotate(Vector3.right, instaRot_x);

            // Setup Scale
            Vector3 instaScale = new Vector3();
            instaScale.x = jsScale[0];
            instaScale.y = jsScale[2];
            instaScale.z = jsScale[1];

            obj.transform.localScale = instaScale;
        }


        public static JSONNode MeshToJSON(MeshFilter meshFilter) {
            JSONNode jsMesh = new JSONObject();
            
            var curMesh = meshFilter.sharedMesh;

            // VERTS
            JSONNode jsVerts = new JSONArray();

            foreach (var vert in curMesh.vertices) {
                // jsVerts.Add(PositionToBlenderJSON(vert));
                jsVerts.Add(vert[0]);
                jsVerts.Add(vert[2]);
                jsVerts.Add(vert[1]);
            }

            jsMesh["Verts"] = jsVerts;

            // Colors
            if (curMesh.colors != null && curMesh.colors.Count() > 0) {
                JSONNode jsColors = new JSONArray();

                foreach (var vertColor in curMesh.colors) {
                    jsColors.Add(vertColor[0]);
                    jsColors.Add(vertColor[1]);
                    jsColors.Add(vertColor[2]);
                    jsColors.Add(vertColor[3]);

                }
                jsMesh["VertexColors"] = jsColors;
            }

            // Polygons Indices
            JSONNode jsPolyIndices = new JSONArray();
            var meshTris = curMesh.triangles;

            for ( int i = 0; i < meshTris.Count(); i += 3)
            {
                // jsPolyIndices.Add( new Vector3(meshTris[i+2], meshTris[i+1], meshTris[i]) );
                jsPolyIndices.Add(meshTris[i+2]);
                jsPolyIndices.Add(meshTris[i+1]);
                jsPolyIndices.Add(meshTris[i]);
            }

            jsMesh["PolyIndices"] = jsPolyIndices;

            // Normals
            JSONNode jsNormals = new JSONArray();

            foreach (var narmalVec in curMesh.normals) {
                jsNormals.Add(narmalVec[0]);
                jsNormals.Add(narmalVec[2]);
                jsNormals.Add(narmalVec[1]);
            }

            jsMesh["Normals"] = jsNormals;

            // UVs
            JSONNode jsUVs = new JSONObject();
            Vector2[] uvs = null;
            for (int i = 0; i < 8; i++) {

                switch (i){
                    case 0:
                        uvs = curMesh.uv;
                        break;

                    case 1:
                        uvs = curMesh.uv2;
                        break;

                    case 2:
                        uvs = curMesh.uv3;
                        break;

                    case 3:
                        uvs = curMesh.uv4;
                        break;

                    case 4:
                        uvs = curMesh.uv5;
                        break;

                    case 5:
                        uvs = curMesh.uv6;
                        break;

                    case 6:
                        uvs = curMesh.uv7;
                        break;

                    case 7:
                        uvs = curMesh.uv8;
                        break;
                }

                if (uvs != null && uvs.Count() > 0) {
                    
                    JSONNode jsUV = new JSONArray();

                    // js_uv[idx] for idx in np_poly_indices
                    foreach (JSONNode vertID in jsPolyIndices) {
                        var curUV = uvs[vertID.AsInt];
                        jsUV.Add(curUV[0]);
                        jsUV.Add(curUV[1]);
                    }

                    jsUVs.Add("UV" + (i+1), jsUV);

                }

            }

            jsMesh["UVs"] = jsUVs;

            return jsMesh;
        }


        public static JSONNode CurveToJSON(BEngineCurve curveComp) {
            JSONNode jsCurveObj = new JSONObject();
            
            JSONNode jsCurveElements = new JSONArray();
            jsCurveObj["CurveElements"] = jsCurveElements;
            
            // Set Curve Type
            if (curveComp.CurveSettings.CurveDataType == CurveType.PolyLine) {
                jsCurveObj["Type"] = "PolyLine";
            }
            else {
                jsCurveObj["Type"] = "Spline";
            }

            foreach (var curveData in curveComp.Curves) {
                JSONNode jsCurveElement = new JSONObject();
                JSONNode jsVerts = new JSONArray();

                // VERTS
                if (curveComp.CurveSettings.CurveDataType == CurveType.PolyLine) {
                    foreach (var point in curveData.CurvePoints) {
                        var worldPos = curveComp.transform.InverseTransformPoint(point.position);
                        jsVerts.Add(worldPos[0]);
                        jsVerts.Add(worldPos[2]);
                        jsVerts.Add(worldPos[1]);
                    }
                }
                else {
                    // Generate Curve Segments
                    if (curveData.CurveSplineSegments == null) {
                        curveData.GenerateSplineSegments(curveData.IsClosed, curveComp.CurveSettings);
                    }

                    // Parse Curve Segments
                    if (curveData.CurveSplineSegments != null) {
                        for (int i = 0; i < curveData.CurveSplineSegments.Count; i++) {
                            var splineCurve = curveData.CurveSplineSegments[i];

                            for (int j = 0; j < splineCurve.Points.Count; j++) {
                                if (j == splineCurve.Points.Count - 1) {
                                    // Add Last Point of Last Segment
                                    if (i == curveData.CurveSplineSegments.Count - 1 && !curveData.IsClosed) {
                                        var worldPos2 = curveComp.transform.InverseTransformPoint(splineCurve.Points[j]);
                                        jsVerts.Add(worldPos2[0]);
                                        jsVerts.Add(worldPos2[2]);
                                        jsVerts.Add(worldPos2[1]);
                                    }
                                }
                                else {
                                    // Add Point
                                    var worldPos3 = curveComp.transform.InverseTransformPoint(splineCurve.Points[j]);
                                    jsVerts.Add(worldPos3[0]);
                                    jsVerts.Add(worldPos3[2]);
                                    jsVerts.Add(worldPos3[1]);
                                }
                            }
                        }
                    }
                }

                // Add to CurveElement
                jsCurveElement["Verts"] = jsVerts;
                jsCurveElement["IsClosed"] = curveData.IsClosed;

                jsCurveElements.Add(jsCurveElement);
            }

            return jsCurveObj;
        }


        public static JSONNode TerrainToJSON(BEngineTerrain terrainComp) {
            JSONNode jsTerrainObj = new JSONObject();
            JSONNode jsVerts = new JSONArray();

            var terrPoints = terrainComp.GenerateTerrain();  // Get New Data

            jsTerrainObj["NumberSegmentsX"] = terrainComp.NumberSegmentsX;
            jsTerrainObj["NumberSegmentsY"] = terrainComp.NumberSegmentsY;  // Y is in Blender

            // Add Points
            foreach (var pointVec in terrPoints)
            {
                // var jsPos = PositionToBlenderJSON(pointVec);
                jsVerts.Add(pointVec[0]);
                jsVerts.Add(pointVec[2]);
                jsVerts.Add(pointVec[1]);
            }

            jsTerrainObj["Verts"] = jsVerts;

            return jsTerrainObj;
        }


        // public static Tuple<float, float, float> PositionToBlenderJSON(Vector3 vert) {
        //         return Tuple.Create(vert[0], vert[2], vert[1]);
        // }

    }
}