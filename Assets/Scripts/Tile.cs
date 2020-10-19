using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int yIndex;
    public int xIndex;

    private Board board;
    public void Init(int x, int y , Board board)
    {
        xIndex = x;
        yIndex = y;
        this.board = board;
    }
    private void OnMouseDown()
    {
        if (board !=null)
        {
            board.ClickTile(this);
        }
    }
    private void OnMouseEnter()
    {
        if (board !=null)
        {
            board.DragToTile(this);
        }
    }
    
    private void OnMouseUp()
    {
        if (board !=null)
        {
            board.ReleaseTile();
        }
    }
}
