using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal
{
    public override void Start()
    {
        levelCounter = LevelCounter.Moves;
        base.Start();
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
        return(movesLeft == 0);
    }
}
