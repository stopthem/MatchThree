﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalCollected : LevelGoal
{

    public CollectionGoal[] collectionGoals;

    public void UpdateGoals(GamePiece pieceToCheck)
    {
        if (pieceToCheck != null)
        {
            foreach (CollectionGoal goal in collectionGoals)
            {
                if (goal != null)
                {
                    goal.CollectPiece(pieceToCheck);
                }
            }
        }
    }

    private bool AreGoalsCompleted(CollectionGoal[] goals)
    {
        foreach (CollectionGoal g in goals)
        {
            if (g.numberToCollect != 0)
            {
                return false;
            }
        }
        return true;
    }
    public override bool IsGameOver()
    {
        if (AreGoalsCompleted(collectionGoals) && ScoreManager.Instance != null)
        {
            int maxScore = scoreGoals[scoreGoals.Length - 1];
            if (ScoreManager.Instance.GetCurrentScore() >= maxScore)
            {
                return true;
            }
        }
        return (movesLeft <= 0);
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.GetCurrentScore() >= scoreGoals[0] && AreGoalsCompleted(collectionGoals));
        }
        return false;
    }
}
