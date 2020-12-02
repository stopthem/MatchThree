using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{

    public Sprite loseIcon, winIcon, goalIcon;
    public Sprite collectIcon, timeIcon, movesIcon;

    public Sprite goalCompleteIcon, goalFailedIcon;
    public Image goalImage;
    public TextMeshProUGUI goalText;

    public GameObject collectionGoalLayout;

    public Image messageImage;
    public TextMeshProUGUI messageText, buttonText;

    public void ShowMessage(Sprite sprite, string message = "", string buttonMessage = "")
    {
        if (messageImage != null)
        {
            messageImage.sprite = sprite;
        }
        if (messageText != null)
        {
            messageText.text = message;
        }
        if (buttonText != null)
        {
            buttonText.text = buttonMessage;
        }
    }

    public void ShowScoreMessage(int scoreGoal)
    {
        string message = "SCORE GOAL \n" + scoreGoal.ToString();
        ShowMessage(goalIcon, message, "START");
    }

    public void ShowWinMessage()
    {
        ShowMessage(winIcon, "LEVEL\nCOMPLETE", "CONTINUE");
    }
    public void ShowLoseMessage()
    {
        ShowMessage(loseIcon, "LEVEL\nFAILED", "AGANE");
    }

    public void ShowGoal(string caption = "", Sprite icon = null)
    {
        if (caption != "")
        {
            ShowGoalCaption(caption);
        }
        if (icon != null)
        {
            ShowGoalImage(icon);
        }
    }

    public void ShowGoalCaption(string caption = "", int offsetX = 0, int offsetY = 0)
    {
        if (goalText != null)
        {
            goalText.text = caption;
            RectTransform rectXform = goalText.GetComponent<RectTransform>();
            rectXform.anchoredPosition += new Vector2(offsetX, offsetY);
        }
    }

    public void ShowGoalImage(Sprite icon = null)
    {
        if (goalImage != null)
        {
            goalImage.gameObject.SetActive(true);
            goalImage.sprite = icon;
        }
        if (icon == null)
        {
            goalImage.gameObject.SetActive(false);
        }
    }

    public void ShowTimeGoal(int time)
    {
        string caption = time.ToString() + " SECONDS";
        ShowGoal(caption, timeIcon);
    }

    public void ShowMovesGoal(int moves)
    {
        string caption = moves.ToString() + " MOVES";
        ShowGoal(caption, movesIcon);
    }

    public void ShowCollectionGoal(bool state = true)
    {
        if (collectionGoalLayout != null)
        {
            collectionGoalLayout.SetActive(state);
        }
        if (state)
        {
            ShowGoal("", collectIcon);
        }
    }
}
