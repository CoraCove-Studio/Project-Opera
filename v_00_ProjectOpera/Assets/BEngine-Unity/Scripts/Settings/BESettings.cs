using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace BEngine
{
    #if UNITY_EDITOR

    // Create a new type of Settings Asset.
    public class BESettings : ScriptableSingleton<BESettings>
    {
        public const string BEProjectSettingsPath = "Assets/BEngineSettings.asset";

        public RunBlenderType runBlenderType = RunBlenderType.RunNetwork;

        // Networking
        public string Host = "127.0.0.10";
        public uint Port = 55555;
        [Range (512, 65535)]
        public int MaxPackageBytes = 65535;

        // Main Paths
        public string BlenderExecFile = "";
        public string TMPFolderPath = "";
        public string BEPythonModulePath = "";

        internal static BESettings GetOrCreateSettings()
        {
            // var settings = instance;
            var settings = AssetDatabase.LoadAssetAtPath<BESettings>(BEProjectSettingsPath);

            if (settings == null) {

                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<BESettings>();
                    AssetDatabase.CreateAsset(settings, BEProjectSettingsPath);
                    AssetDatabase.SaveAssets();
                }

            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }


    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class BESettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/BEngineSettings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "BEngine",

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settingsSerial = BESettings.GetSerializedSettings();
                    settingsSerial.Update();
                    var settings = (BESettings)settingsSerial.targetObject;

                    EditorGUI.BeginChangeCheck();

                    GUILayout.Space(10);

                    var runBlenderTypeProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("runBlenderType"), new GUIContent("Run Blender Type"));

                    GUILayout.Space(10);

                    GUILayout.Label("Networking");
                    var hostProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("Host"), new GUIContent("Host"));
                    var portProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("Port"), new GUIContent("Port"));
                    var maxPackageBytesProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("MaxPackageBytes"), new GUIContent("Max Package Bytes"));

                    GUILayout.Space(10);

                    GUILayout.Label("Main Paths");

                    // MAIN PATHS
                    var blenderPathProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("BlenderExecFile"), new GUIContent("Blender Path"));

                    if(GUILayout.Button("Blender Executable File")) {
                        string blenderPath = EditorUtility.OpenFilePanel("Blender Executable File", "", "*");

                        // var beSettingsAsset = AssetDatabase.LoadAssetAtPath<BESettings>(BESettings.BEProjectSettingsPath);
                        // var beSettingsAsset = (BESettings)settings.targetObject;

                        settings.BlenderExecFile = blenderPath;
                    }

                    EditorGUILayout.Space();

                    var bePythonPathProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("BEPythonModulePath"), new GUIContent("BEngine Python Folder"));

                    if(GUILayout.Button("BEngine Python Folder Path")) {
                        string bePythonModulePath = EditorUtility.OpenFolderPanel("BEngine Python Folder Path", "", "");

                        // var beSettingsAsset = AssetDatabase.LoadAssetAtPath<BESettings>(BESettings.BEProjectSettingsPath);

                        settings.BEPythonModulePath = bePythonModulePath;
                    }

                    EditorGUILayout.Space();

                    var tmpPathProp = EditorGUILayout.PropertyField(settingsSerial.FindProperty("TMPFolderPath"), new GUIContent("TMP Folder"));

                    if(GUILayout.Button("TMP Folder Path")) {
                        string tmpPath = EditorUtility.OpenFolderPanel("TMP Folder Path", "", "");

                        // var beSettingsAsset = (BESettings)AssetDatabase.LoadMainAssetAtPath(BESettings.BEProjectSettingsPath);

                        settings.TMPFolderPath = tmpPath;
                    }

                    if (EditorGUI.EndChangeCheck() && GUI.changed) {
                        EditorUtility.SetDirty(settings);
                        settingsSerial.ApplyModifiedProperties();
                        // AssetDatabase.SaveAssetIfDirty(settings);
                        AssetDatabase.SaveAssets();
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "BEngine", "Blender" })
            };

            return provider;
        }
    }

    #endif
}
