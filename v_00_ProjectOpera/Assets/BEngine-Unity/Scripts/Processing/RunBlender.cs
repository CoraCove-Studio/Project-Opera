using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System;
using System.Diagnostics;
using System.IO;

namespace BEngine
{
    public static class RunBlender
    {
        #if UNITY_EDITOR

        public static void CheckBeforeRun(BESettings beSettings, BESettingsBlender BlenderParams) {
            if (BlenderParams != null) {
                if (BlenderParams.BlendFile == null) {
                    throw new Exception("Blend File is Null!!!");
                }
            }
            if (beSettings.BlenderExecFile == null || !File.Exists(beSettings.BlenderExecFile) ) {
                throw new Exception("Please, Setup Blender EXE Path in Project Settings (BEngine)");
            }
            if (beSettings.BEPythonModulePath == null || !Directory.Exists(beSettings.BEPythonModulePath) ) {
                throw new Exception("Please, Setup Python Module Folder Path in Project Settings (BEngine)");
            }
            if (beSettings.TMPFolderPath == null || !Directory.Exists(beSettings.TMPFolderPath) ) {
                throw new Exception("Please, Setup TMP Folder Path in Project Settings (BEngine)");
            }
        }

        public static Process CreateBlenderProcess(string pyScript, List<string> arguments, string tmpFolderPath, 
                                                   bool isBackground = true) {

            Process process = new Process();

            var beSettings = (BESettings)AssetDatabase.LoadMainAssetAtPath(BESettings.BEProjectSettingsPath);
            
            if (beSettings == null) throw new Exception("Please Setup Blender Executable Path in Project Settings(BEngine).");

            string blenderPath = BEPaths.FixPath(beSettings.BlenderExecFile, false);
            blenderPath = Path.Combine(blenderPath);

            if (!File.Exists(blenderPath)) throw new Exception("Please Setup Blender Executable Path in Project Settings(BEngine).");
            
            process.StartInfo.FileName = @blenderPath;
            // p.StartInfo.Arguments = "/c \"" + @"D:/Soft/blender/blender.exe" + "\"" + " --background --python " + "D:/Projects/Blender/B-Engine/B-Engine/Assets/BEngine/BEngine-Py/Test.py";

            // process.StartInfo.Arguments = "";

            if (isBackground) {
                process.StartInfo.Arguments += " --background";
            }

            process.StartInfo.Arguments += " --python " + '"' + @pyScript + '"';
            // process.StartInfo.Arguments += " --python-exit-code ";
            process.StartInfo.Arguments += " -- ";

            process.StartInfo.Arguments += "\"" + "BE_TMP_FOLDER=" + BEPaths.FixPath(tmpFolderPath, true) + "\""  + " ";
            // process.StartInfo.Arguments += "\"" + "PROJECT_PATH=" + BEPaths.GetProjectPath() + "\""  + " ";
            // process.StartInfo.Arguments += "\"" + "PROJECT_PATH_2=" + BEPaths.GetProjectPath_2() + "\""  + " ";

            // Networking
            process.StartInfo.Arguments += "\"" + "RunBlenderType=" + beSettings.runBlenderType.ToString() + "\""  + " ";
            process.StartInfo.Arguments += "\"" + "Host=" + beSettings.Host + "\""  + " ";
            process.StartInfo.Arguments += "\"" + "Port=" + beSettings.Port + "\""  + " ";
            process.StartInfo.Arguments += "\"" + "MaxPackageBytes=" + beSettings.MaxPackageBytes + "\""  + " ";

            if (arguments != null && arguments.Count > 0) {
                foreach (string argument in arguments) {
                    process.StartInfo.Arguments += "\"" + argument + "\""  + " ";
                }
            }

            if (isBackground) {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                process.StartInfo.CreateNoWindow = true;
            }

            return process;
        }

        #endif

    }
}