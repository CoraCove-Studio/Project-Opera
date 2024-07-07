using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private PlayerUIHandler playerUI;

    [SerializeField] private float timeCounter;
    private int minutesElapsed;
    private int secondsElapsed;

    public float CountdownTimer { get; private set; }
    public int minutesLeft;
    public int secondsLeft;
    public (int, int) TimeLeft { get; private set; }

    public bool TimerIsRunning { get; private set; } = false;

    private void Update()
    {
        TrackTime();
    }

    public void TrackTime()
    {
        if (GameManager.Instance.GamePaused == false)
        {
            timeCounter += Time.deltaTime;
            minutesElapsed = Mathf.FloorToInt(timeCounter / 60f);
            secondsElapsed = Mathf.FloorToInt(timeCounter - minutesElapsed * 60);


            if (TimerIsRunning && CountdownTimer > 0)
            {
                CountdownTimer -= Time.deltaTime;
                minutesLeft = Mathf.FloorToInt(CountdownTimer / 60f);
                secondsLeft = Mathf.FloorToInt(CountdownTimer - minutesLeft * 60);
                TimeLeft = (minutesLeft, secondsLeft);
            }
            playerUI.UpdateGameTimer(TimeLeft.Item1, TimeLeft.Item2);
        }
        if(TimerIsRunning && CountdownTimer <= 0)
        {
            TimeLeft = (0, 0);
            Debug.Log("GameTimer: TrackTime: Timer up, TimeLeft set to " + TimeLeft.Item1 + TimeLeft.Item2);
            playerUI.UpdateGameTimer(TimeLeft.Item1, TimeLeft.Item2);
        }
    }

    public void SetNewTimer(float timeInSeconds)
    {
        CountdownTimer = timeInSeconds;
        TimeLeft = (10, 0);
        playerUI.UpdateGameTimer(TimeLeft.Item1, TimeLeft.Item2);
        Debug.Log("GameTimer: SetNewTimer: Timer set to: " + TimeLeft.Item1 + ":" + TimeLeft.Item2);
    }

    public void StartTimer()
    {
        TimerIsRunning = true;
    }

    public void StopTimer()
    {
        TimerIsRunning = false;
    }
}
