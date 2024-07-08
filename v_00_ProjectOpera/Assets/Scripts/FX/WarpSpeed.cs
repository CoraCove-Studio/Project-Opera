using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class WarpSpeed : MonoBehaviour
{
    public VisualEffect warpSpeedVFX;
    public Volume volume;
    public MeshRenderer movingSwirlFX;

    private bool warpActive;
    private Coroutine warpCoroutine;
    private Coroutine warpCoroutine_Mesh;

    [SerializeField] private float rate = 0.01f;
    [SerializeField] private float delay = 0.1f;

    void Start()
    {
        gameObject.SetActive(true);
        warpSpeedVFX.Stop();
        warpSpeedVFX.SetFloat("WarpAmount", 0);
        movingSwirlFX.material.SetFloat("_Active", 0f);
    }

    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            Debug.Log("you are holding it down");
            warpActive = true;
            warpCoroutine = StartCoroutine(ActivateWarp());
            warpCoroutine_Mesh = StartCoroutine(ActivateShader());
        }

        if (Input.GetKeyUp("j"))
        {
            Debug.Log("And you let it go!");
            warpActive = false;
            warpCoroutine = StartCoroutine(ActivateWarp());
            warpCoroutine_Mesh = StartCoroutine(ActivateShader());
        }
    }

    public void EnteringWarp()
    {
        warpActive = true;
        warpCoroutine = StartCoroutine(ActivateWarp());
        warpCoroutine_Mesh = StartCoroutine(ActivateShader());
    }

    public void LeavingWarp()
    {
        warpActive = false;
        warpCoroutine = StartCoroutine(ActivateWarp());
        warpCoroutine_Mesh = StartCoroutine(ActivateShader());
    }


    IEnumerator ActivateWarp()
    {
        if (warpActive)
        {
            warpSpeedVFX.Play();

            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            while (amount < 1 && warpActive)
            {
                amount += rate;
                warpSpeedVFX.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(delay);

                if (volume.profile.TryGet<PhysicallyBasedSky>(out var physicallyBasedSky))
                {
                    // Modify the intensityMultiplier parameter
                    physicallyBasedSky.multiplier.value = 0.5f * -rate; // Set your desired intensity multiplier value
                }
            }
        }
        else
        {
            float amount = warpSpeedVFX.GetFloat("WarpAmount");
            while (amount > 0 && !warpActive)
            {
                amount -= rate;
                warpSpeedVFX.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(delay);

                if (volume.profile.TryGet<PhysicallyBasedSky>(out var physicallyBasedSky))
                {
                    // Modify the intensityMultiplier parameter
                    physicallyBasedSky.multiplier.value = 40f * rate * 15f; // Set your desired intensity multiplier value
                }

                if (amount <= 0)
                {
                    amount = 0;
                    warpSpeedVFX.SetFloat("WarpAmount", amount);
                    warpSpeedVFX.Stop();
                }
            }
        }
    }

    IEnumerator ActivateShader()
    {
        if (warpActive)
        {
            float amount = movingSwirlFX.material.GetFloat("_Active");
            while (amount < 1 && warpActive)
            {
                amount += rate;
                movingSwirlFX.material.SetFloat("_Active", amount);
                yield return new WaitForSeconds(delay);

                if (volume.profile.TryGet<PhysicallyBasedSky>(out var physicallyBasedSky))
                {
                    // Modify the intensityMultiplier parameter
                    physicallyBasedSky.multiplier.value = 0.5f * -rate; // Set your desired intensity multiplier value
                }
            }
        }
        else
        {
            float amount = movingSwirlFX.material.GetFloat("_Active");
            while (amount > 0 && !warpActive)
            {
                amount -= rate;
                movingSwirlFX.material.SetFloat("_Active", amount);
                yield return new WaitForSeconds(delay);

                if (volume.profile.TryGet<PhysicallyBasedSky>(out var physicallyBasedSky))
                {
                    // Modify the intensityMultiplier parameter
                    physicallyBasedSky.multiplier.value = 40f * rate * 15f; // Set your desired intensity multiplier value
                }

                if (amount <= 0)
                {
                    amount = 0;
                    movingSwirlFX.material.SetFloat("_Active", amount);
                }
            }
        }
    }
}