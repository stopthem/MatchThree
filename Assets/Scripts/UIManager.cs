using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public ScreenFader screenFader;

    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI movesLeftText;
    public MessageWindow messageWindow;
    public ScoreMeter scoreMeter;

    public GameObject collectionGoalLayout;
    public GameObject movesCounter;

    public Timer timer;

    public int collectionGoalBaseWidth = 125;
    CollectionGoalPanel[] collectionGoalPanels;

    public override void Awake()
    {
        base.Awake();

        if (messageWindow != null)
        {
            messageWindow.gameObject.SetActive(true);
        }
        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);
        }
    }

    public void SetupCollectionGoalLayout(CollectionGoal[] collectionGoals)
    {
        if (collectionGoalLayout != null && collectionGoals != null && collectionGoals.Length != 0)
        {
            RectTransform rectXFrom = collectionGoalLayout.GetComponent<RectTransform>();
            rectXFrom.sizeDelta = new Vector2(collectionGoals.Length * collectionGoalBaseWidth, rectXFrom.sizeDelta.y);
            collectionGoalPanels = collectionGoalLayout.gameObject.GetComponentsInChildren<CollectionGoalPanel>();

            for (int i = 0; i < collectionGoalPanels.Length; i++)
            {
                if (i < collectionGoals.Length && collectionGoals[i] != null)
                {
                    collectionGoalPanels[i].gameObject.SetActive(true);
                    collectionGoalPanels[i].collectionGoal = collectionGoals[i];
                    collectionGoalPanels[i].SetupPanel();
                }
                else
                {
                    collectionGoalPanels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateCollectionGoalLayout()
    {
        foreach (CollectionGoalPanel panel in collectionGoalPanels)
        {
            panel.UpdatePanel();
        }
    }

    public void EnableTimer(bool state)
    {
        if (timer != null)
        {
            timer.gameObject.SetActive(state);
        }
    }

    public void EnableMovesCounter(bool state)
    {
        if (movesCounter != null)
        {
            movesCounter.gameObject.SetActive(state);
        }
    }

    public void EnableCollectionGoalLayout(bool state)
    {
        if (collectionGoalLayout != null)
        {
            movesCounter.gameObject.SetActive(state);
        }
    }
}
