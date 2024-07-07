using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHoverEffect : MonoBehaviour
{
    private Renderer _renderer;
    private Material[] _originalMaterials;
    public Material outlineMaterial;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalMaterials = _renderer.materials;
    }

    void OnMouseEnter()
    {
        Material[] materials = new Material[_originalMaterials.Length + 1];
        _originalMaterials.CopyTo(materials, 0);
        materials[_originalMaterials.Length] = outlineMaterial;
        _renderer.materials = materials;
    }

    void OnMouseExit()
    {
        _renderer.materials = _originalMaterials;
    }
}
