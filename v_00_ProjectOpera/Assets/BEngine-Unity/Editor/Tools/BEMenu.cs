using UnityEditor;
using UnityEngine;


namespace BEngine {

	public static class BEMenu {

		[MenuItem("BEngine/Run Blender Server")]
		public static void RunServer (MenuCommand menuCommand) {
			BENetworking.RunBEServer();
		}

		[MenuItem("BEngine/Tools/Save Mesh")]
		public static void SaveMeshInPlace (MenuCommand menuCommand) {
			GameObject selGO = Selection.activeGameObject;
			MeshFilter mf = selGO.GetComponent<MeshFilter>();
			Mesh m = mf.sharedMesh;
			SaveMesh(m, m.name, false, true);
		}

		[MenuItem("BEngine/Tools/Save Mesh as Instance")]
		public static void SaveMeshNewInstanceItem (MenuCommand menuCommand) {
			GameObject selGO = Selection.activeGameObject;
			MeshFilter meshFilter = selGO.GetComponent<MeshFilter>();
			Mesh mesh = meshFilter.sharedMesh;
			SaveMesh(mesh, mesh.name, true, true);
		}

		public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
			string filePath = EditorUtility.SaveFilePanel("Save Mesh", "Assets/", name, "asset");
			if (string.IsNullOrEmpty(filePath)) return;
			
			filePath = FileUtil.GetProjectRelativePath(filePath);

			Mesh meshToSave = (makeNewInstance) ? UnityEngine.Object.Instantiate(mesh) as Mesh : mesh;
			
			if (optimizeMesh)
				MeshUtility.Optimize(meshToSave);
			
			AssetDatabase.CreateAsset(meshToSave, filePath);
			AssetDatabase.SaveAssets();
		}
	}
	
}
