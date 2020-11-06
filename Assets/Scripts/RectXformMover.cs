using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectXformMover : MonoBehaviour
{
    public Vector3 startPosition,onScreenPosition,endPosition;

    public float timeToMove = 1f;

    private RectTransform rectTransform;

    private bool isMoving = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Move(Vector3 startPos,Vector3 endPos, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(startPos,endPos,timeToMove));
        }
    }
    private IEnumerator MoveRoutine(Vector3 startPos,Vector3 endPos, float timeToMove)
    {
        if (rectTransform !=null )
        {
            rectTransform.anchoredPosition = startPos;
        }

        bool reachedDestination = false;
        float elapsedTime = 0f;
        isMoving = true;

        while (!reachedDestination)
        {
            if (Vector3.Distance(rectTransform.anchoredPosition,endPos)< 0.010f)
            {
                reachedDestination = true;
                break;
                
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / timeToMove);
            t = t * t * t * (t * (t * 6 - 15) + 10);

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(startPos,endPos,t);
            }
            yield return null;
        }
        isMoving = false;
    }

    public void MoveOn()
    {
        Move(startPosition,onScreenPosition,timeToMove);
    }
    public void MoveOff()
    {
        Move(onScreenPosition,endPosition,timeToMove);
    }
}
