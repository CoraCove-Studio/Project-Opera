using System.Collections;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField] private GameObject planetRotator;

    private void Start()
    {
        StartRotationCoroutine();
    }
    public void StartRotationCoroutine()
    {
        StartCoroutine(Rotate(600));
    }
    IEnumerator Rotate(float duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        Debug.Log("PlanetRotation: Rotate: starting rotation " + startRotation);
        float endRotation = startRotation.y + 1080.0f;
        float t = 0.0f;
        while (t < duration)
        {
            if (!GameManager.Instance.GamePaused)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation.y, endRotation, t / duration) % 1080.0f;
                transform.eulerAngles = new Vector3(startRotation.x, yRotation, startRotation.z);
            }
            yield return null;
        }
    }
}
