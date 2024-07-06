using System.Collections;
using UnityEngine;

public class TradeTransitTicker : MonoBehaviour
{
    [SerializeField] GameObject transitPanel;
    [SerializeField] GameObject tradePanel;
    [SerializeField] AudioClip tradeClick;
    [SerializeField] AudioClip transitClick;

    private bool inTransit = true;
    private Quaternion rotationIncrement = Quaternion.Euler(-90, 0, 0);
    private float rotationTime = 0.5f;
    private bool isRotating = false;


    public void ActivateTicker()
    {
        if (isRotating == false)
        {
            StartCoroutine(RotateTickerForward());
        }
    }


    private IEnumerator RotateTickerForward()
    {
        isRotating = true;

        float elapsedTime = 0f;
        Quaternion initialRotation = transform.localRotation;
        Quaternion targetRotation = initialRotation * rotationIncrement;
        Debug.Log("First target rotation: " + targetRotation.eulerAngles);

        // First -90 degrees rotation
        while (elapsedTime < rotationTime)
        {
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = targetRotation;
        TogglePanels();

        // Reset elapsed time for the second rotation
        elapsedTime = 0f;
        initialRotation = transform.localRotation;
        targetRotation = initialRotation * rotationIncrement;
        Debug.Log("Second target rotation: " + targetRotation.eulerAngles);

        // Second -90 degrees rotation
        while (elapsedTime < rotationTime)
        {
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (inTransit)
        {
            GameManager.Instance.audioManager.PlaySFX(transitClick);
        }
        else
        {
            GameManager.Instance.audioManager.PlaySFX(tradeClick);
        }

        transform.localRotation = targetRotation;
        isRotating = false;
    }

    private void TogglePanels()
    {
        if (inTransit)
        {
            transitPanel.SetActive(false);
            tradePanel.SetActive(true);
            inTransit = false;
        }
        else
        {
            tradePanel.SetActive(false);
            transitPanel.SetActive(true);
            inTransit = true;
        }
    }
}