using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchValue{

        Yellow,
        Blue,
        Red,
        Magenta,
        Cyan,
        Teal,
        Green,
        Indigo,
        Wild,
        None
    }
public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    private Board board;
    
    public MatchValue matchValue;
    public int scoreValue = 20;
    
    public AudioClip clearSound;
    public void SetCoordination(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
    public void Move (int destX, int destY, float timeToMove)
    {
        StartCoroutine(MoveRotuine(new Vector3(destX, destY, 0), timeToMove));
    }
    public void SetBoard(Board board)
    {
        this.board = board;
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
                if (board !=null)
                {
                    board.PlaceGamePiece(this, (int)destination.x , (int)destination.y);    
                }
                transform.position = destination;
                SetCoordination((int)destination.x,(int)destination.y);
                break;
            }
            float t = Mathf.Clamp01(elapsedTime / timeToMove);
            t = t * t * t * (t * (t * 6 - 15) + 10);
            transform.position = Vector3.Lerp(startingPosition, destination , t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void ChangeColor(GamePiece pieceToMatch)
    {
        SpriteRenderer renderertoChange = GetComponent<SpriteRenderer>();

        Color colorToMatch = Color.clear;

        if (pieceToMatch !=null)
        {
            SpriteRenderer rendererToMatch = pieceToMatch.GetComponent<SpriteRenderer>();

            if (rendererToMatch != null && renderertoChange != null)
            {
                renderertoChange.color = rendererToMatch.color;
            }
            matchValue = pieceToMatch.matchValue;
        }
    }
}
