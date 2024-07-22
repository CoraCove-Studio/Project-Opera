using System.Collections;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField] private GameObject planetRotator;
    private Vector3 newGamePosition;

    private void Start()
    {
        newGamePosition = transform.eulerAngles;
    }

    public void StartRotationCoroutine()
    {
        StartCoroutine(Rotate(600));
    }

    public void StartTutorialRotationCoroutine()
    {
        StartCoroutine(TutorialRotation(0.1f));
    }

    public void StartReturnToStartingRotationCoroutine()
    {
        // I think this is what needs to be changed to fix the "Sell All" bug
        StartCoroutine(ReturnToStartingPosition(0.1f));
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

    IEnumerator TutorialRotation(float duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        Debug.Log("PlanetRotation: Rotate: starting rotation " + startRotation);
        float endRotation = startRotation.y - 20.0f;
        float t = 0.0f;
        while (t < duration)
        {
            if (GameManager.Instance.InTutorial)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation.y, endRotation, t / duration) % 20.0f;
                transform.eulerAngles = new Vector3(startRotation.x, yRotation, startRotation.z);
            }
            yield return null;
        }
    }

    IEnumerator ReturnToStartingPosition(float duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        Debug.Log("PlanetRotation: Rotate: starting rotation " + startRotation);
        float endRotation = newGamePosition.y;
        float t = 0.0f;
        while (t < duration)
        {
            if (GameManager.Instance.InTutorial)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation.y, endRotation, t / duration) % 20.0f;
                transform.eulerAngles = new Vector3(startRotation.x, yRotation, startRotation.z);
            }
            yield return null;
        }
    }
}
