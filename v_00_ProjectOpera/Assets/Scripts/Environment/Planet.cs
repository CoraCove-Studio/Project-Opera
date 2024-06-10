using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Planet : MonoBehaviour
{
    public bool autoUpdate = true;
    [Range(2, 256)] public int resolution = 10;

    [Header("HDRP Materials")]
    public Material hdrpMaterial;

    [Header("Settings")]
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [SerializeField, HideInInspector]
    public bool shapeSettingsFoldout;
    [SerializeField, HideInInspector]
    public bool colorSettingsFoldout;
    
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    ShapeGenerator shapeGenerator;

    void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                // Assign the HDRP material
                MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
                if (hdrpMaterial != null)
                {
                    //Debug.Log("Assigning HDRP material to mesh renderer.");
                    meshRenderer.material = hdrpMaterial;
                }
                else
                {
                    Debug.LogError("HDRP Material not assigned!");
                }

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            else
            {
                MeshRenderer meshRenderer = meshFilters[i].GetComponent<MeshRenderer>();
                if (meshRenderer != null && hdrpMaterial != null)
                {
                    //Debug.Log("Reassigning HDRP material to existing mesh renderer.");
                    meshRenderer.material = hdrpMaterial;
                }
            }

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    // to generate the whole planet
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    // if only the shape settings have changed you can call this method
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    // if only the color settings have changed you can call this method
    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColors()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}
