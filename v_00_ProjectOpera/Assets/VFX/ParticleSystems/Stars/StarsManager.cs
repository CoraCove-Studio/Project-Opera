// Rachel Huggins
// Environment Tech Artist

using UnityEngine.Rendering;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class StarsManager : MonoBehaviour
{
    //grabbing main camera
    private Camera povPlayerCam;

    void Start()
    {
        povPlayerCam = Camera.main;
        transform.position = povPlayerCam.transform.position;
        transform.parent = povPlayerCam.transform;

        var starsRenderer = GetComponent<ParticleSystemRenderer>();
        starsRenderer.material.renderQueue = (int) RenderQueue.Background;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
