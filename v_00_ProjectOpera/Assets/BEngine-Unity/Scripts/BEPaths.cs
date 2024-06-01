using UnityEngine;

using UnityEditor;

using System.IO;

namespace BEngine
{

    #if UNITY_EDITOR

    public struct BlenderPaths {
        public string BlendFilePath;
        public string BlendFileName;
        public string BlendFolderPath;

        public BlenderPaths(string blendFilePath, string blendFileName, string blendFolderPath) {
            BlendFilePath = blendFilePath;
            BlendFileName = blendFileName;
            BlendFolderPath = blendFolderPath;
        }
    }

    public static class BEPaths
    {

        public static string GetProjectPath() {
            var projectPath = Application.dataPath;
            projectPath = FixPath(projectPath, true);

            return projectPath;
        }


        public static string GetProjectPath_2() {
            var projectPath_2 = Directory.GetParent(GetProjectPath()).ToString();
            projectPath_2 = Directory.GetParent(projectPath_2).ToString();
            projectPath_2 = FixPath(projectPath_2, true);

            return projectPath_2;
        }


        public static string FixPath(string path, bool isDir) {
            string newPath = path;

            newPath = newPath.Replace("\\", "/");

            if(!newPath.EndsWith("/") && isDir) {
                newPath += "/";
            }
            
            return newPath;
        }


        public static string GetBlenderOutputsJSONPath(string tmpFolderPath) {
            return tmpFolderPath + BECommon.BlenderOutputsFileName;
        }


        public static BlenderPaths GetBlenderPaths(BESettingsBlender blenderParams) {
            string blendFilePath = GetProjectPath_2() + AssetDatabase.GetAssetPath(blenderParams.BlendFile);
            blendFilePath = FixPath(blendFilePath, false);

            string blendFileName = Path.GetFileName(blendFilePath).Replace(".blend", "");

            // blend1 and blend2 are Optional
            blendFileName = blendFileName.Replace(".blend1", "");
            blendFileName = blendFileName.Replace(".blend2", "");

            string blendFolderPath = Directory.GetParent(blendFilePath).ToString();

            blendFolderPath = FixPath(blendFolderPath, true);

            BlenderPaths blenderPaths = new BlenderPaths(blendFilePath, blendFileName, blendFolderPath);

            return blenderPaths;
        }


    }

    #endif
}