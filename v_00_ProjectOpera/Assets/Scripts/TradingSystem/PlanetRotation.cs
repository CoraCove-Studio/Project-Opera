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
        StartCoroutine(Rotate(180));
    }
    IEnumerator Rotate(float duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        float endRotation = startRotation.z + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            if (!GameManager.Instance.GamePaused)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation.z, endRotation, t / duration) % 360.0f;
                transform.eulerAngles = new Vector3(startRotation.x, yRotation, startRotation.z);
            }
            yield return null;
        }
    }
}
