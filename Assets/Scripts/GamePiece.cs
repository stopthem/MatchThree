using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int xIndex;
    private int yIndex;
    public void SetCoordination(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
    public void Move (int destX, int destY, float timeToMove)
    {
        StartCoroutine(MoveRotuine(new Vector3(destX, destY, 0), timeToMove));
    }
    private IEnumerator MoveRotuine(Vector3 destination,float timeToMove)
    {
        Vector3 startingPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;
        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination)< 0.01f)
            {
                reachedDestination = true;
                transform.position = destination;
                SetCoordination((int)destination.x,(int)destination.y);
                break;
            }
            float t = elapsedTime / timeToMove;
            Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(startingPosition, destination , t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
