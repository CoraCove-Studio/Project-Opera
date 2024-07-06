using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationTab : MonoBehaviour
{
    [SerializeField] RectTransform notificationTab;
    [SerializeField] TextMeshProUGUI notificationTextBody;

    [SerializeField] float lerpTime = 0.5f;
    [SerializeField] float displayTime = 2f;
    [SerializeField] Vector2 hiddenPosition;
    [SerializeField] Vector2 visiblePosition;
    [SerializeField] float overshootAmount = 10f;

    private Vector2 overshootPosition;
    private bool isShowing = false;
    private bool conditionMet;

    private void Start()
    {
        overshootPosition = visiblePosition + new Vector2(0, overshootAmount);
        notificationTab.anchoredPosition = hiddenPosition;
    }

    public void ShowTimedNotification(string message)
    {
        if (isShowing == false)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTimedNotificationCoroutine(message, displayTime));
        }
    }

    public void ShowConditionalNotification(string message)
    {
        if (isShowing == false)
        {
            conditionMet = false;
            StopAllCoroutines();
            StartCoroutine(ShowConditionalNotificationCoroutine(message));
        }
    }

    public void MeetCondition()
    {
        conditionMet = true;
    }

    private IEnumerator ShowTimedNotificationCoroutine(string message, float displayDuration)
    {
        notificationTextBody.text = message;
        isShowing = true;
        // Lerp into view with overshoot
        yield return StartCoroutine(LerpPosition(hiddenPosition, overshootPosition, lerpTime * 0.8f));
        yield return StartCoroutine(LerpPosition(overshootPosition, visiblePosition, lerpTime * 0.2f));

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Lerp out of view
        yield return StartCoroutine(LerpPosition(visiblePosition, hiddenPosition, lerpTime));
        isShowing = false;
    }

    private IEnumerator ShowConditionalNotificationCoroutine(string message)
    {
        notificationTextBody.text = message;
        isShowing = true;
        // Lerp into view with overshoot
        yield return StartCoroutine(LerpPosition(hiddenPosition, overshootPosition, lerpTime * 0.8f));
        yield return StartCoroutine(LerpPosition(overshootPosition, visiblePosition, lerpTime * 0.2f));

        // Wait for condition to be met
        while (conditionMet == false)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // Lerp out of view
        yield return StartCoroutine(LerpPosition(visiblePosition, hiddenPosition, lerpTime));
        isShowing = false;
    }

    private IEnumerator LerpPosition(Vector2 from, Vector2 to, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            notificationTab.anchoredPosition = Vector2.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        notificationTab.anchoredPosition = to;
    }
}
