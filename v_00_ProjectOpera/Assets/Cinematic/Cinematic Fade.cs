using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicFade : MonoBehaviour
{
    public CanvasGroup cinematic;
    public float fadeOut = 2f;
    public float videoPlay = 28f;

    private float timer;
    private bool isPlaying = true;

    void Start() // Canvas is on
    {
        cinematic.alpha = 1f; 
    }

    void Update()
    {
        if (isPlaying) // Cinematic is playing
        {
            timer += Time.deltaTime;
            if (timer >= videoPlay) // Cinematic plays for 30 seconds
            {
                isPlaying = false;
                timer = 0f;
            }
        }
        else // Fades out canvas
        {
            cinematic.alpha = Mathf.Lerp(1f, 0f, timer / fadeOut);
            timer += Time.deltaTime;

            if (timer >= fadeOut) //Turns off Canvas
            {
                cinematic.alpha = 0f;
                cinematic.gameObject.SetActive(false); 
            }
        }
    }
}
