using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;

namespace BEngine
{

    [CustomEditor(typeof(BEngineComponent))]
    [CanEditMultipleObjects]
    public class BEngineEditor : Editor
    {

        protected BEngineComponent script;
        protected Object curObj;


        void OnEnable() {
            script = (BEngineComponent)target;
            curObj = script.BlenderParams.BlendFile;

            // if (PrefabUtility.GetPrefabInstanceStatus(script.gameObject) != PrefabInstanceStatus.NotAPrefab) {
                BEJSONParser.LoadJSONNodeSystem(script, false);
            // }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // // // History
            // if (GUI.changed) {
                Undo.RecordObject(target, "Changed BEngine Params");
            // }

            /// Some Sanity Checks
            if (script.BlenderParams.BlendFile != curObj) {

                if (script.BlenderParams.BlendFile == null) {
                    script.Inputs.CurrentParams = null;
                }
                else {
                    curObj = script.BlenderParams.BlendFile;
                    BEJSONParser.LoadJSONNodeSystem(script, true);
                }
            }
            // else if ((script.Inputs.CurrentParams == null || script.Inputs.CurrentParams.Count == 0)
            //     && script.BlenderParams.BlendFile != null) {
            //     BEJSONParser.LoadJSONNodeSystem(script, false);
            // }

            // Buttons
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Run Nodes")) {
                var selObjs = Selection.gameObjects;

                foreach (var selObj in selObjs)
                {
                    if (selObj.TryGetComponent<BEngineComponent>(out var beCompOut)) {
                        beCompOut.RunNodes(RunNodesType.RunNodes);
                    }
                }
            }

            if(GUILayout.Button("Update Inputs")) {
                var selObjs = Selection.gameObjects;

                foreach (var selObj in selObjs)
                {
                    if (selObj.TryGetComponent<BEngineComponent>(out var beCompOut)) {
                        beCompOut.RunNodes(RunNodesType.UpdateInputs);
                    }
                }
            }

            if(GUILayout.Button("Run Server")) {
                BENetworking.RunBEServer();
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            // Draw Default Inspector
            DrawDefaultInspector();


            if (script.Inputs.CurrentParams != null && script.BlenderParams.BlendFile != null) {

                script.FoldoutInputs = EditorGUILayout.Foldout(script.FoldoutInputs, "Inputs");
                
                DrawInputs();
            }

            // EditorGUI.BeginChangeCheck();

            if (GUI.changed && EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            
        }


        void DrawInputs() {
            if (script.Inputs.CurrentParams != null && script.Inputs.CurrentParams.Count > 0 && script.FoldoutInputs) {

                bool isOpenFoldOutEditor = true;

                // Set Tabs Up
                List<BaseParam> tabsList = GetTabs(script.Inputs.CurrentParams);

                if (tabsList.Count > 0) {
                    script.CurrentInputsTab = GUILayout.Toolbar(script.CurrentInputsTab, GetTabParamsNames(tabsList).ToArray());
                }
                
                int curTab = -1;

                // Parse Inputs Params
                foreach (BaseParam baseParam in script.Inputs.CurrentParams) {
                    // IParam iParam = (IParam)baseParam;

                    // Break if that went wrong
                    if (baseParam == null) break;

                    // Check Tabs and Foldouts
                    if (baseParam.Type == ParameterType.Tab) {
                        curTab += 1;
                        isOpenFoldOutEditor = true;
                        continue;
                    }

                    if (tabsList.Count > 0 && curTab != script.CurrentInputsTab && curTab >= 0) continue;

                    if (baseParam.Type == ParameterType.FoldOut) {
                        // FoldOutParam paramFoldOut = (FoldOutParam)baseParam;
                        baseParam.IsOpenFoldout = EditorGUILayout.Foldout(baseParam.IsOpenFoldout, baseParam.Name);
                        isOpenFoldOutEditor = baseParam.IsOpenFoldout;
                        continue;
                    }

                    if (!isOpenFoldOutEditor) continue;

                    switch (baseParam.Type) {
                        case ParameterType.Boolean:
                            // BooleanParam paramBool = (BooleanParam)iParam;
                            // GUILayout.Toggle(paramBool.DefaultValue, paramBool.Name);
                            // DrawValue(baseParam, baseParam.Name);

                            baseParam.BoolValue = EditorGUILayout.Toggle(baseParam.Name, baseParam.BoolValue);

                            break;

                        case ParameterType.Color:
                            // ColorParam paramCol = (ColorParam)iParam;
                            // EditorGUILayout.ColorField(paramCol.Name, paramCol.DefaultValue.gamma);
                            // DrawValue(baseParam, baseParam.Name);

                            baseParam.ColorValue = EditorGUILayout.ColorField(baseParam.Name, baseParam.ColorValue);

                            break;

                        case ParameterType.Float:
                            // FloatParam paramFloat = (FloatParam)baseParam;
                            baseParam.FloatValue = EditorGUILayout.FloatField(baseParam.Name, baseParam.FloatValue);
                            baseParam.FloatValue = Mathf.Clamp(baseParam.FloatValue, baseParam.DefaultFloatMinValue, baseParam.DefaultFloatMaxValue);

                            // DrawValue(baseParam, baseParam.Name);
                            break;

                        case ParameterType.Image:
                            // DrawValue(baseParam, baseParam.Name);

                            baseParam.ImageValue =  (Texture)EditorGUILayout.ObjectField(baseParam.Name, baseParam.ImageValue, typeof(Texture), true);

                            break;

                        case ParameterType.Integer:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.IntValue =  EditorGUILayout.IntField(baseParam.Name, baseParam.IntValue);
                            baseParam.IntValue = Mathf.Clamp(baseParam.IntValue, baseParam.DefaultIntMinValue, baseParam.DefaultIntMaxValue);
                            break;

                        case ParameterType.Label:
                            // LabelParam paramLabel = (LabelParam)baseParam;
                            EditorGUILayout.LabelField(baseParam.Name);
                            break;

                        case ParameterType.Material:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.MaterialValue = (Material)EditorGUILayout.ObjectField(baseParam.Name, baseParam.MaterialValue, typeof(Material), true);
                            break;

                        case ParameterType.Object:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.ObjectValue = (GameObject)EditorGUILayout.ObjectField(baseParam.Name, baseParam.ObjectValue, typeof(GameObject), true);
                            break;

                        case ParameterType.Collection:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.ObjectValue = (GameObject)EditorGUILayout.ObjectField(baseParam.Name, baseParam.ObjectValue, typeof(GameObject), true);
                            break;

                        case ParameterType.Separator:
                            EditorGUILayout.Separator();
                            break;

                        case ParameterType.String:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.StringValue =  EditorGUILayout.TextField(baseParam.Name, baseParam.StringValue);
                            break;

                        case ParameterType.Vector:
                            // DrawValue(baseParam, baseParam.Name);
                            baseParam.Vector3Value =  EditorGUILayout.Vector3Field(baseParam.Name, baseParam.Vector3Value);

                            float vecX = baseParam.Vector3Value.x;
                            vecX = Mathf.Clamp(vecX, baseParam.DefaultFloatMinValue, baseParam.DefaultFloatMaxValue);

                            float vecY = baseParam.Vector3Value.y;
                            vecY = Mathf.Clamp(vecY, baseParam.DefaultFloatMinValue, baseParam.DefaultFloatMaxValue);

                            float vecZ = baseParam.Vector3Value.z;
                            vecZ = Mathf.Clamp(vecZ, baseParam.DefaultFloatMinValue, baseParam.DefaultFloatMaxValue);

                            baseParam.Vector3Value.x = vecX;
                            baseParam.Vector3Value.y = vecY;
                            baseParam.Vector3Value.z = vecZ;

                            break;

                    }
                }
            }
        }


        // void DrawValue(BaseParam scParam, string propUIName, string propName = "_value") {
            // SerializedObject so = new SerializedObject(scParam);
            // SerializedProperty serializedField = so.FindProperty(propName);

            // EditorGUILayout.PropertyField(serializedField, new GUIContent(propUIName));

            // if (GUI.changed && EditorGUI.EndChangeCheck()) {
            //     so.ApplyModifiedProperties();
            // }

            // return so;
        // }


        List<BaseParam> GetTabs(IEnumerable<BaseParam> paramsList){
            List<BaseParam> tabsList = new List<BaseParam>();

            foreach (BaseParam param in paramsList) {
                if (param.Type == ParameterType.Tab) {
                    tabsList.Add(param);
                }
            }

            return tabsList;
        }


        List<string> GetTabParamsNames(IEnumerable<BaseParam> paramsList){
            List<string> namesList = new List<string>();

            foreach (BaseParam param in paramsList) {
                if (param.Type == ParameterType.Tab) {
                    namesList.Add(param.Name);
                }
            }

            return namesList;
        }


        
    }
}
