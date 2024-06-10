using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Planet : MonoBehaviour
{
    public bool autoUpdate = true;
    [Range(2, 256)] public int resolution = 10;

    //[Header("HDRP Materials")]
    //public Material hdrpMaterial;

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

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

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
                meshRenderer.material = colorSettings.planetMaterial;

                //if (hdrpMaterial != null)
                //{
                //    //Debug.Log("Assigning HDRP material to mesh renderer.");
                //    meshRenderer.material = hdrpMaterial;
                //}
                //else
                //{
                //    Debug.LogError("HDRP Material not assigned!");
                //}

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            else
            {
                MeshRenderer meshRenderer = meshFilters[i].GetComponent<MeshRenderer>();
                if (meshRenderer != null) //&& hdrpMaterial != null
                {
                    //Debug.Log("Reassigning HDRP material to existing mesh renderer.");
                    meshRenderer.material = colorSettings.planetMaterial;
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
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColors()
    {
        colorGenerator.UpdateColors();

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }

    public void GenerateRandomPlanet()
    {
        if (colorSettings != null)
        {
            colorSettings.RandomizeColors();
            Initialize();
            GenerateMesh();
        }
    }

    public void DestroyPlanet()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter != null)
            {
                Destroy(meshFilter.gameObject);
            }
        }
        Destroy(gameObject);
    }
}
