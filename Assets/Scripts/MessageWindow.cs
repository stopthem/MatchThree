using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{
    public Image messageIcon;
    public TextMeshProUGUI messageText,buttonText;

    public void ShowMessage(Sprite sprite, string message = "", string buttonMessage = "")
    {
        if (messageIcon != null)
        {
            messageIcon.sprite = sprite;
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
}
