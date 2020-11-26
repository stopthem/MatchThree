using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
    public Timer timer;

    private void Start()
    {
        if (timer != null)
        {
            timer.InitTimer(timeLeft);
        }
    }
    public void StartCountDown()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (timeLeft > 0 )
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;

            if (timer != null)
            {
                timer.UpdateTimer(timeLeft);
            }
        }
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return(ScoreManager.Instance.GetCurrentScore() >= scoreGoals[0]);
        }
        return false;
    }

    public override bool IsGameOver()
    {
        int maxScore = scoreGoals[scoreGoals.Length - 1 ];

        if (ScoreManager.Instance.GetCurrentScore() >= maxScore)
        {
            return true;
        }
        return(timeLeft <= 0);
    }
}
