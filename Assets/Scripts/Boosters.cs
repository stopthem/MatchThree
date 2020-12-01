using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class Boosters : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    private Image m_image;
    private RectTransform m_rectXform;
    private Vector3 m_startPosition;
    private Board m_board;
    private Tile m_tileTarget;

    public static GameObject ActiveBooster;
    public TextMeshProUGUI instructionsText;
    public string instructions = "drag over game piece to remove";

    public bool isEnabled = false;
    public bool isDraggable = true;
    public bool isLocked = false;

    public List<CanvasGroup> canvasGroups;
    public UnityEvent boostEvent;
    public int boostTime = 15;

    private void Awake()
    {
        m_image = GetComponent<Image>();
        m_rectXform = GetComponent<RectTransform>();
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }
    private void Start()
    {
        EnableBooster(false);
    }

    public void EnableBooster(bool state)
    {
        isEnabled = state;

        if (state)
        {
            DisableOtherBoosters();
            Boosters.ActiveBooster = gameObject;
        }
        else if (gameObject == Boosters.ActiveBooster)
        {
            Boosters.ActiveBooster = null;
        }

        m_image.color = (state) ? Color.white : Color.gray;

        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(Boosters.ActiveBooster != null);

            if (gameObject == Boosters.ActiveBooster)
            {
                instructionsText.text = instructions;
            }
        }
    }

    private void DisableOtherBoosters()
    {
        Boosters[] allBoosters = Object.FindObjectsOfType<Boosters>();

        foreach (Boosters booster in allBoosters)
        {
            if (booster != this)
            {
                booster.EnableBooster(false);
            }
        }
    }

    public void ToggleBooster()
    {
        EnableBooster(!isEnabled);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked)
        {
            m_startPosition = gameObject.transform.position;
            EnableCanvasGroups(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked && Camera.main != null)
        {
            Vector3 onScreenPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rectXform, eventData.position, Camera.main, out onScreenPosition);

            gameObject.transform.position = onScreenPosition;

            RaycastHit2D hit2D = Physics2D.Raycast(onScreenPosition, Vector3.forward, Mathf.Infinity);

            if (hit2D.collider != null)
            {
                m_tileTarget = hit2D.collider.GetComponent<Tile>();
            }
            else
            {
                m_tileTarget = null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked)
        {
            gameObject.transform.position = m_startPosition;
            EnableCanvasGroups(true);

            if (m_board != null && m_board.isRefilling)
            {
                return;
            }

            if (m_tileTarget != null)
            {
                if (boostEvent != null)
                {
                    boostEvent.Invoke();
                }
                EnableBooster(false);

                m_tileTarget = null;
                Boosters.ActiveBooster = null;
            }
        }
    }

    private void EnableCanvasGroups(bool state)
    {
        if (canvasGroups != null && canvasGroups.Count > 0)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = state;
                }
            }
        }
    }

    public void ClearPieceAtBoard()
    {
        if (m_board != null && m_tileTarget != null)
        {
            m_board.ClearAndRefillBoard(m_tileTarget.xIndex, m_tileTarget.yIndex);
        }
    }

    public void AddTime()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(boostTime);
        }
    }

    public void DropColorBomb()
    {
        if (m_board != null && m_tileTarget != null)
        {
            m_board.MakeColorBombBooster(m_tileTarget.xIndex, m_tileTarget.yIndex);
        }
    }
}
