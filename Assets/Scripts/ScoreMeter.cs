using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    public Slider slider;
    public ScoreStar[] scoreStars = new ScoreStar[3];
    LevelGoal levelGoal;
    int maxScore;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    public void SetupStars(LevelGoal levelGoal)
    {
        if (levelGoal == null)
        {
            return;
        }
        this.levelGoal = levelGoal;
        maxScore = levelGoal.scoreGoals[levelGoal.scoreGoals.Length - 1 ];
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

        if (maxScore > 0)
        {
            for (int i = 0; i < levelGoal.scoreGoals.Length; i++)
            {
                if (scoreStars[i] != null)
                {
                    float newX = (sliderWidth * levelGoal.scoreGoals[i] / maxScore) - (sliderWidth * .5f);
                    RectTransform starRectXform = scoreStars[i].GetComponent<RectTransform>();
                    if (starRectXform != null)
                    {
                        starRectXform.anchoredPosition = new Vector2(newX,starRectXform.anchoredPosition.y);
                    }
                }
            }
        }
    }

    public void UpdateScoreMeter(int score, int starCount)
    {
        if (levelGoal != null)
        {
            slider.value = (float)score / (float)maxScore;
        }

        for (int i = 0; i < starCount; i++)
        {
            if (scoreStars[i] != null)
            {
                scoreStars[i].Activate();
            }
        }
    }
}
