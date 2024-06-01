// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System;
using System.Diagnostics;




namespace BEngine
{

    public enum RunNodesType {
        UpdateInputs,
        RunNodes
    }


    #if UNITY_EDITOR
        [ExecuteInEditMode]
        [DisallowMultipleComponent]
        [CanEditMultipleObjects]
    #endif

    public class BEngineComponent : MonoBehaviour
    {

        [Space]

        public BEComponentSettings Settings = new BEComponentSettings();
        public BESettingsBlender BlenderParams = new BESettingsBlender();
    
        [HideInInspector] public BEngineInputs Inputs = new BEngineInputs();

        // [Space]
        public BEngineOutputs Outputs = new BEngineOutputs();

        [HideInInspector] public int CurrentInputsTab = 0;

        [HideInInspector] public bool FoldoutInputs = true;

        [HideInInspector] public int meshCounter = 0;  // Just to count unique meshes from Blender


        #if !UNITY_EDITOR
        void Awake() {
            if (Settings.RemoveComponent == true) {
                Destroy(this);
            }
        }
        #endif


        #if UNITY_EDITOR


        public void RunNodes(RunNodesType runType) {
            BEJSONParser.LoadJSONNodeSystem(this, false);  // Update Inputs Before Running Nodes

            BESettings beSettings = (BESettings)AssetDatabase.LoadMainAssetAtPath(BESettings.BEProjectSettingsPath);

            RunBlender.CheckBeforeRun(beSettings, BlenderParams);

            BlenderPaths blenderPaths = BEPaths.GetBlenderPaths(BlenderParams);

            var tmpFolder_Fixed = BEPaths.FixPath(beSettings.TMPFolderPath, true);
            var pyModulePath_Fixed = BEPaths.FixPath(beSettings.BEPythonModulePath, true);
            string pyFilePath = pyModulePath_Fixed + BECommon.BERunNodes_PY;

            // Run Blender Process
            if (beSettings.runBlenderType == RunBlenderType.RunBlender) {

                // Save Unity Base Values
                var jsUnityBaseValues = BEJSONParser.GetUnityBaseValuesJSON(blenderPaths, BlenderParams, beSettings, runType);
                string basePathJSON = tmpFolder_Fixed + "BEngineBaseFromEngine.json";
                BEJSONParser.SaveJSON(basePathJSON, jsUnityBaseValues);

                bool backgroundMode = true;  // True is for "Update Nodes" Always

                // Only for "Run Nodes"
                if (runType == RunNodesType.RunNodes) {
                    var jsUnityValues = BEJSONParser.GetUnityValuesJSON(this);
                    string pathJSON = tmpFolder_Fixed + "BEngineInputs.json";
                    BEJSONParser.SaveJSON(pathJSON, jsUnityValues);

                    backgroundMode = Settings.BackgroundMode;
                }

                // Sart Process
                Process process = RunBlender.CreateBlenderProcess(pyFilePath, null, tmpFolder_Fixed, backgroundMode);
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine("Output:");
                Console.WriteLine(output);

                UnityEngine.Debug.Log(output);

                // Next Step after Blender
                if (output.Contains("!PYTHON DONE!")) {
                    if (runType == RunNodesType.RunNodes) {
                        string blenderOutput = BEPaths.GetBlenderOutputsJSONPath(tmpFolder_Fixed);
                        var jsData = BEJSONParser.LoadJSON(blenderOutput);

                        BEJSONParser.LoadBlenderOutputs(this, jsData);
                    }
                    else {
                        
                    }
                }
            }
            else {
                BENetworking.RunClient(this, runType);
            }
        }


    void OnDrawGizmos() {
        if (Settings.icon.IconSize > 0.001f) {
            Gizmos.color = Settings.icon.IconColor;
    
            //draw force application point
            Gizmos.DrawSphere(this.transform.position + Settings.icon.IconOffset, Settings.icon.IconSize);
            // Gizmos.DrawMesh();
    
            // Gizmos.color = Color.white;
        }
    }


        #endif


    }

}