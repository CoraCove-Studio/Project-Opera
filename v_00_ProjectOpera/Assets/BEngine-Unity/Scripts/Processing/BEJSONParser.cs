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
    public class BEJSONParser
    {

        #if UNITY_EDITOR

        public static JSONNode LoadJSON(string jsPath) {
            if (File.Exists(jsPath)) {
                StreamReader reader = new StreamReader(jsPath);
                string jsString = reader.ReadToEnd();
                reader.Close();

                JSONNode data = JSONNode.Parse(jsString);

                return data;
            }

            return null;
        }


        public static void LoadJSONNodeSystem(BEngineComponent beComp, bool resetOldParams) {

            var inputs = beComp.Inputs;
            var blenderParams = beComp.BlenderParams;

            JSONNode jsData = null;
            string jsPath = "";

            // If Blend file is set
            if (blenderParams.BlendFile != null) {
                var blenderPaths = BEPaths.GetBlenderPaths(blenderParams);

                jsPath = GetNodeJSONPath(blenderParams, blenderPaths.BlendFolderPath, blenderPaths.BlendFilePath);

                jsData = LoadJSON(jsPath);

            }


            if (jsData != null) {

                Undo.RecordObject(beComp, "BEngine Update");  // Record BEngine Update

                // Get Previous Params
                Dictionary<string, BaseParam> prevParamsDict = null;

                if (inputs.CurrentParams != null) {
                    prevParamsDict = inputs.CurrentParams.ToDictionary(keySelector: m => m.Identifier, elementSelector: m => m);
                }

                inputs.CurrentParams = new List<BaseParam>();

                foreach (var input in jsData["Inputs"]) {
                    string inputType = input.Value[BECommon.jsType];
                    string identifier = input.Value[BECommon.jsIdentifier];

                    switch (inputType) {
                        case "BOOLEAN":
                            BaseParam boolParm = new BaseParam(identifier, ParameterType.Boolean);

                            boolParm.DefaultBoolValue = input.Value[BECommon.jsDefaultValue] != 0;  // Was Bug in SimpleJSON
                            boolParm.BoolValue = boolParm.DefaultBoolValue;
                            boolParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(boolParm);
                            break;

                        case "RGBA":
                            BaseParam colParm = new BaseParam(identifier, ParameterType.Color);

                            // Here we apply Gamma to Color
                            colParm.DefaultColorValue = ((Color)input.Value[BECommon.jsDefaultValue]).gamma;
                            colParm.ColorValue = colParm.DefaultColorValue;
                            colParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(colParm);
                            break;

                        case "VALUE":
                            BaseParam floatParm = new BaseParam(identifier, ParameterType.Float);

                            floatParm.DefaultFloatMinValue = input.Value[BECommon.jsMinValue];
                            floatParm.DefaultFloatMaxValue = input.Value[BECommon.jsMaxValue];
                            floatParm.DefaultFloatValue = input.Value[BECommon.jsDefaultValue];
                            floatParm.FloatValue= floatParm.DefaultFloatValue;
                            floatParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(floatParm);
                            break;

                        case "INT":
                            BaseParam intParm = new BaseParam(identifier, ParameterType.Integer);

                            intParm.DefaultIntMinValue = input.Value[BECommon.jsMinValue];
                            intParm.DefaultIntMaxValue = input.Value[BECommon.jsMaxValue];
                            intParm.DefaultIntValue = input.Value[BECommon.jsDefaultValue];
                            intParm.IntValue = intParm.DefaultIntValue;
                            intParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(intParm);
                            break;

                        case "IMAGE":
                            BaseParam imageParm = new BaseParam(identifier, ParameterType.Image);
                            imageParm.Name = input.Value[BECommon.jsName];
                            
                            inputs.CurrentParams.Add(imageParm);
                            break;

                        case "MATERIAL":
                            BaseParam matParm = new BaseParam(identifier, ParameterType.Material);
                            matParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(matParm);
                            break;
 
                        case "OBJECT":
                            BaseParam objParm = new BaseParam(identifier, ParameterType.Object);
                            objParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(objParm);
                            break;

                        case "COLLECTION":
                            BaseParam collParm = new BaseParam(identifier, ParameterType.Collection);
                            collParm.Name = input.Value[BECommon.jsName];

                            inputs.CurrentParams.Add(collParm);
                            break;

                        case "STRING":
                            if(input.Value[BECommon.jsName] == "TAB"){
                                BaseParam tabParm = new BaseParam(identifier, ParameterType.Tab);
                                tabParm.Name = input.Value[BECommon.jsDefaultValue];

                                inputs.CurrentParams.Add(tabParm);
                            }

                            else if(input.Value[BECommon.jsName] == "FOLDOUT"){
                                BaseParam foldOutParm = new BaseParam(identifier, ParameterType.FoldOut);
                                foldOutParm.Name = input.Value[BECommon.jsDefaultValue];

                                inputs.CurrentParams.Add(foldOutParm);
                            }

                            else if(input.Value[BECommon.jsName] == "LABEL"){
                                BaseParam labelParm = new BaseParam(identifier, ParameterType.Label);
                                labelParm.Name = input.Value[BECommon.jsDefaultValue];

                                inputs.CurrentParams.Add(labelParm);
                            }

                            else if(input.Value[BECommon.jsName] == "SEPARATOR"){
                                BaseParam separatorParm = new BaseParam(identifier, ParameterType.Separator);

                                inputs.CurrentParams.Add(separatorParm);
                            }

                            else {
                                BaseParam strParm = new BaseParam(identifier, ParameterType.String);
                                strParm.DefaultStringValue = input.Value[BECommon.jsDefaultValue];
                                strParm.StringValue = strParm.DefaultStringValue;
                                strParm.Name = input.Value[BECommon.jsName];

                                inputs.CurrentParams.Add(strParm);
                            }

                            break;

                        case "VECTOR":
                            BaseParam vecParm = new BaseParam(identifier, ParameterType.Vector);
                            vecParm.DefaultVector3Value = input.Value[BECommon.jsDefaultValue];

                            vecParm.DefaultFloatMinValue = input.Value[BECommon.jsMinValue];
                            vecParm.DefaultFloatMaxValue = input.Value[BECommon.jsMaxValue];

                            vecParm.Vector3Value = vecParm.DefaultVector3Value;
                            vecParm.Name = input.Value[BECommon.jsName];

                            // vecParm.Init((Vector3)input.Value[_jsDefaultValue], identifier, input.Value[_jsName]);
                            
                            inputs.CurrentParams.Add(vecParm);
                            break;
                    }
                }

                if (prevParamsDict != null && !resetOldParams) beComp.Inputs.ReplacePreviousParamsValues(prevParamsDict);

            }
        }


        public static void LoadBlenderOutputs(BEngineComponent beComp, JSONNode jsData) {

            if (jsData != null) {
                // Undo.RecordObject(beComp.gameObject, "BEngine Run Nodes");  // Record BEngine Update
                // Undo.RegisterFullObjectHierarchyUndo(beComp.gameObject, "BEngine Run Nodes");
                // Undo.RegisterCreatedObjectUndo(beComp.gameObject, "BEngine Run Nodes");
                // Undo.RegisterCompleteObjectUndo(beComp.gameObject, "BEngine Run Nodes");

                GameObject missedInstance = null;
                BEngineOutputs beCompOutputs = beComp.Outputs;

                beComp.meshCounter = 0;  // Reset Mesh Counter

                // Delete old Objects
                int childs = beComp.transform.childCount;
                for (int i = childs - 1; i >= 0; i--) {
                    GameObject delObj = beComp.transform.GetChild( i ).gameObject;

                    // Undo.DestroyObjectImmediate(delObj);
                    UnityEngine.Object.DestroyImmediate(delObj);
                }

                // Create Instances
                foreach (var instanceJSONData in jsData["ObjectsSets"]) {
                    int bengine_instance = -1;
                    
                    if (instanceJSONData.Value.HasKey(BECommon.BEInstanceName)) {
                        bengine_instance = instanceJSONData.Value[BECommon.BEInstanceName];
                    }

                    // Create Instance in a case if the Mesh is from Blender
                    GameObject instaNewObj = null;

                    foreach (var instTransforms in instanceJSONData.Value["Transforms"]) {

                        GameObject newInsta = null;

                        if (bengine_instance > -1) {

                            if (beCompOutputs.OutputInstances.Count > bengine_instance && beCompOutputs.OutputInstances[bengine_instance] != null) {
                                // GameObject newInsta = Instantiate(BEngineOutputs.OutputInstances[bengine_instance].Instance);
                                newInsta = (GameObject)PrefabUtility.InstantiatePrefab(beCompOutputs.OutputInstances[bengine_instance].Instance);
                            }

                            else {
                                // MISSED INSTANCE
                                if (missedInstance == null) {
                                    newInsta = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    newInsta.GetComponent<MeshRenderer>().sharedMaterials = new Material[] {null};

                                    missedInstance = newInsta;
                                }
                                else {
                                    newInsta = GameObject.Instantiate(missedInstance);
                                }

                            }
                        }

                        else {
                            if (instaNewObj == null) {
                                // New GameObject
                                newInsta = new GameObject();
                                newInsta.name = "BE_" + beComp.name;

                                if (instanceJSONData.Value["Mesh"] != null) {
                                    int meshID = instanceJSONData.Value["Mesh"];
                                    BEMeshWorks.MeshFromJSON(jsData["Meshes"][meshID], newInsta, beComp);
                                }
                                
                                instaNewObj = newInsta;
                            }
                            else {
                                newInsta = GameObject.Instantiate(instaNewObj);
                            }
                        }

                        BEMeshWorks.SetTransformFromJSON(newInsta, instTransforms.Value[0], 
                                            instTransforms.Value[1],  instTransforms.Value[2]);


                        newInsta.transform.SetParent(beComp.transform, true);


                        // Undo.RegisterCreatedObjectUndo(newInsta, "BEngine Run Nodes");
                    }
                }
            }
        }


        public static void SaveJSON(string pathJSON, JSONNode jsData) {
            File.WriteAllText(pathJSON, jsData.ToString());
        }


        public static JSONNode GetUnityBaseValuesJSON(BlenderPaths blenderPaths,
                                                       BESettingsBlender BlenderParams, BESettings beSettings,
                                                       RunNodesType runType) {
            JSONNode jsData = new JSONObject();
            
            jsData["BEngineType"] = "Unity";
            jsData["BlendFile"] = blenderPaths.BlendFilePath;
            jsData["BlendFolder"] = blenderPaths.BlendFolderPath;
            jsData["NodeSysName"] = BlenderParams.NodeSystemName;

            jsData["RunNodesType"] = runType.ToString();

            // jsData["RunBlenderType"] = beSettings.runBlenderType.ToString();
            // jsData["Host"] = beSettings.Host;
            // jsData["Port"] = beSettings.Port;
            // jsData["MaxPackageBytes"] = beSettings.MaxPackageBytes;

            // string pathJSON = tmpFolderPath_Fixed + "BEngineBaseFromEngine.json";
            // File.WriteAllText(pathJSON, jsData.ToString());

            return jsData;
        }


        public static JSONNode GetUnityValuesJSON(BEngineComponent beComp) {

            var inputs = beComp.Inputs;

            JSONNode jsData = new JSONObject();

            // TRANSFORM OF BEngine Object
            var beCompObjPos = beComp.transform.position;
            jsData["Pos"] = new Vector3(beCompObjPos.x, beCompObjPos.z, beCompObjPos.y);

            var beCompObjRot = beComp.transform.eulerAngles;
            jsData["Rot"] = new Vector3((-beCompObjRot.x) * Mathf.Deg2Rad, (-beCompObjRot.z) * Mathf.Deg2Rad, (-beCompObjRot.y) * Mathf.Deg2Rad);

            var beCompObjScale = beComp.transform.lossyScale;
            jsData["Scale"] = new Vector3(beCompObjScale.x, beCompObjScale.z, beCompObjScale.y);

            JSONNode jsBEngineInputs = new JSONObject();
            jsData["BEngineInputs"] = jsBEngineInputs;

            List<Mesh> meshesList = new List<Mesh>();
            JSONArray meshesJSON = new JSONArray();

            foreach (BaseParam baseParam in inputs.CurrentParams) {

                switch (baseParam.Type) {
                    case ParameterType.Boolean:
                        JSONNode boolNode = new JSONObject();
                        boolNode["Type"] = "BOOLEAN";
                        boolNode["Value"] = baseParam.BoolValue;

                        jsBEngineInputs[baseParam.Identifier] = boolNode;
                        break;

                    case ParameterType.Color:
                        // var colVal = (ColorParam)baseParam;

                        JSONNode colNode = new JSONObject();
                        colNode["Type"] = "RGBA";

                        // if (baseParam != null && baseParam != null) {
                        colNode["Value"] = baseParam.ColorValue.linear;
                        // }

                        jsBEngineInputs[baseParam.Identifier] = colNode;
                        break;

                    case ParameterType.Float:
                        JSONNode floatNode = new JSONObject();
                        floatNode["Type"] = "VALUE";
                        floatNode["Value"] = baseParam.FloatValue;

                        jsBEngineInputs[baseParam.Identifier] = floatNode;
                        break;

                    case ParameterType.Image:
                        // var imageVal = baseParam;

                        JSONNode imageNode = new JSONObject();
                        imageNode["Type"] = "IMAGE";

                        if (baseParam.ImageValue != null) {
                            imageNode["Value"] = BEPaths.GetProjectPath_2() + AssetDatabase.GetAssetPath(baseParam.ImageValue);
                        }

                        jsBEngineInputs[baseParam.Identifier] = imageNode;
                        break;

                    case ParameterType.Integer:
                        JSONNode intNode = new JSONObject();
                        intNode["Type"] = "INT";
                        intNode["Value"] = baseParam.IntValue;

                        jsBEngineInputs[baseParam.Identifier] = intNode;
                        break;

                    case ParameterType.Material:
                        // var matVal = baseParam;

                        JSONNode matNode = new JSONObject();
                        matNode["Type"] = "MATERIAL";

                        if (baseParam.MaterialValue != null) {
                            matNode["Value"] = AssetDatabase.GetAssetPath(baseParam.MaterialValue);
                        }

                        jsBEngineInputs[baseParam.Identifier] =  matNode;    
                        break;

                    case ParameterType.Object:
                        // var objParam = baseParam;
                        var objVal = baseParam.ObjectValue;

                        JSONNode objNode = new JSONObject();
                        objNode["Type"] = "OBJECT";

                        ParseObjectToJSON(objVal, objNode, false, meshesList, meshesJSON);
                        
                        jsBEngineInputs[baseParam.Identifier] = objNode;    
                        break;

                    case ParameterType.Collection:
                        // var objParam = baseParam;
                        var collVal = baseParam.ObjectValue;

                        JSONNode collNode = new JSONObject();
                        collNode["Type"] = "COLLECTION";

                        ParseObjectToJSON(collVal, collNode, true, meshesList, meshesJSON);
                        
                        jsBEngineInputs[baseParam.Identifier] = collNode;    
                        break;

                    case ParameterType.String:
                        JSONNode strNode = new JSONObject();
                        strNode["Type"] = "STRING";
                        strNode["Value"] = baseParam.StringValue;

                        jsBEngineInputs[baseParam.Identifier] = strNode;
                        break;

                    case ParameterType.Vector:
                        // var vecVal = baseParam;

                        JSONNode vecNode = new JSONObject();
                        vecNode["Type"] = "VECTOR";

                        if (baseParam.Vector3Value != null) {
                            vecNode["Value"] = baseParam.Vector3Value;
                        }

                        jsBEngineInputs[baseParam.Identifier] = vecNode;    
                        break;
                }
            }

            jsData["Meshes"] = meshesJSON;

            // string pathJSON = tmpFolderPath_Fixed + "BEngineInputs.json";
            // File.WriteAllText(pathJSON, jsData.ToString());jsData

            return jsData;
        }


        protected static void RecordTransformJSON(Transform objTrans, JSONNode objNode) {
            var objPos1 = objTrans.position;
            objNode["Pos"] = new Vector3(objPos1.x, objPos1.z, objPos1.y);

            var objRot1 = objTrans.eulerAngles;
            objNode["Rot"] = new Vector3((-objRot1.x) * Mathf.Deg2Rad, (-objRot1.z) * Mathf.Deg2Rad, (-objRot1.y) * Mathf.Deg2Rad);

            var objScale1 = objTrans.lossyScale;
            objNode["Scale"] = new Vector3(objScale1.x, objScale1.z, objScale1.y);
        }


        protected static void ParseObjectToJSON(GameObject objVal, JSONNode objNode,
                                                bool isCollection, List<Mesh> meshes,
                                                JSONArray meshesJSON) {
            if (objVal != null) {
                JSONArray objValueNode = new JSONArray();

                var transComps = objVal.GetComponentsInChildren<Transform>();

                for (int i = 0; i < transComps.Count(); i++) {
                    var transComp = transComps[i];

                    JSONNode subObject = new JSONObject();

                    subObject["Name"] = transComp.gameObject.name;

                    // TRANSFORM OF MAIN MESH
                    RecordTransformJSON(transComp, subObject);

                    var meshComp = transComp.GetComponent<MeshFilter>();
                    var curveComp = transComp.GetComponent<BEngineCurve>();
                    var terrainComp = transComp.GetComponent<BEngineTerrain>();

                    // Get Mesh
                    if(meshComp != null && meshComp.sharedMesh != null) {
                        if (meshes.Contains(meshComp.sharedMesh) == false) {
                            meshesJSON.Add(BEMeshWorks.MeshToJSON(meshComp));

                            subObject["Mesh"] = meshes.Count;
                            meshes.Add(meshComp.sharedMesh);

                            // subObject["Mesh"] = BEMeshWorks.MeshToJSON(meshComp);
                        }
                        else {
                            subObject["Mesh"] = meshes.IndexOf(meshComp.sharedMesh);
                        }
                    }

                    // Get Curves
                    if(curveComp != null) {
                        subObject["Curves"] = BEMeshWorks.CurveToJSON(curveComp);
                    }

                    // Get Terrain
                    if(terrainComp != null) {
                        subObject["Terrain"] = BEMeshWorks.TerrainToJSON(terrainComp);
                    }                    

                    // Add Object
                    if(isCollection && i == 0
                     && (meshComp == null || meshComp.sharedMesh == null)
                     && curveComp == null 
                     && terrainComp == null) {
                        // DO NOTHING
                    }
                    else {
                        objValueNode.Add(subObject);
                    }

                }

                objNode["Value"] = objValueNode;
            }
        }


        public static string GetNodeJSONPath(BESettingsBlender blenderParams, string blendFolderPath, string blendFilePath) {
            return blendFolderPath + Path.GetFileNameWithoutExtension(blendFilePath) + "_" + blenderParams.NodeSystemName + ".json";
        }

    #endif

    }
}