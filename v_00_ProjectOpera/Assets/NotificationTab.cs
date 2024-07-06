using System.Collections;
using System.Collections.Generic;
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
    private Coroutine notificationCoroutine;
    private Queue<IEnumerator> notificationQueue = new Queue<IEnumerator>();

    private void Start()
    {
        overshootPosition = visiblePosition + new Vector2(0, overshootAmount);
        notificationTab.anchoredPosition = hiddenPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowConditionalNotification("This is a conditional notification. Press 'M' to meet the condition.");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MeetCondition();
        }
    }

    public void ShowTimedNotification(string message)
    {
        var notificationCoroutine = ShowTimedNotificationCoroutine(message, displayTime);
        if (!isShowing)
        {
            StartNotification(notificationCoroutine);
        }
        else
        {
            notificationQueue.Enqueue(notificationCoroutine);
        }
    }

    public void ShowConditionalNotification(string message)
    {
        var notificationCoroutine = ShowConditionalNotificationCoroutine(message);
        if (!isShowing)
        {
            StartNotification(notificationCoroutine);
        }
        else
        {
            notificationQueue.Enqueue(notificationCoroutine);
        }
    }

    public void MeetCondition()
    {
        conditionMet = true;
    }

    private void StartNotification(IEnumerator coroutine)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        notificationCoroutine = StartCoroutine(coroutine);
    }

    private void OnNotificationComplete()
    {
        isShowing = false;
        conditionMet = false;
        notificationCoroutine = null;
        if (notificationQueue.Count > 0)
        {
            var nextNotification = notificationQueue.Dequeue();
            StartNotification(nextNotification);
        }
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
        OnNotificationComplete();
    }

    private IEnumerator ShowConditionalNotificationCoroutine(string message)
    {
        notificationTextBody.text = message;
        isShowing = true;
        // Lerp into view with overshoot
        yield return StartCoroutine(LerpPosition(hiddenPosition, overshootPosition, lerpTime * 0.8f));
        yield return StartCoroutine(LerpPosition(overshootPosition, visiblePosition, lerpTime * 0.2f));

        // Wait for condition to be met
        while (!conditionMet)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // Lerp out of view
        yield return StartCoroutine(LerpPosition(visiblePosition, hiddenPosition, lerpTime));
        OnNotificationComplete();
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
