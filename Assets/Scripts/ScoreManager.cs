using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>
{
    int currentScore = 0;
    int counterValue = 0;
    int increment = 5;

    public TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScoreText(currentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        currentScore += value;
        StartCoroutine(CountScoreRoutine());
    }
    private IEnumerator CountScoreRoutine()
    {
        int iterations = 0;

        while (counterValue < currentScore && iterations < 10000)
        {
            counterValue += increment;
            UpdateScoreText(counterValue);
            iterations++;
            yield return null;
        }

        counterValue = currentScore;
        UpdateScoreText(currentScore);
    }
    public int GetCurrentScore()
    {
        return currentScore;
    }
}
