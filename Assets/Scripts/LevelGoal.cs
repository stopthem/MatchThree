using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelGoal : Singleton<LevelGoal>
{
    public int scoreStars = 0;
    public int[] scoreGoals = new int[3] {1000,2000,3000};

    public int movesLeft = 20;
    private void Start()
    {
        
    }

    private int UpdateScore(int score)
    {
        for (int i = 0; i < scoreGoals.Length; i++)
        {
            if (score < scoreGoals[i])
            {
                return i;
            }
        }
        return scoreGoals.Length;
    }
    public void UpdateScoreStars(int score)
    {
        scoreStars = UpdateScore(score);
    }
    public abstract bool IsWinner();
    public abstract bool IsGameOver();
}
