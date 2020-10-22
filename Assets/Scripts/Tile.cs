using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public enum TileType
    {
        Normal,
        Obstacle,
        Breakable
    }

    public int yIndex;
    public int xIndex;
    public TileType tileType = TileType.Normal;
    private SpriteRenderer spriteRenderer;

    public Color normalColor;

    public int breakableValue = 0;
    public Sprite[] breakableSprites;
    private Board board;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(int x, int y , Board board)
    {
        xIndex = x;
        yIndex = y;
        this.board = board;

        if (tileType == TileType.Breakable)
        {
            if (breakableSprites[breakableValue] != null)
            {
                spriteRenderer.sprite = breakableSprites[breakableValue];
            }
        }
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
    public void BreakTile()
    {
        if (tileType != TileType.Breakable)
        {
            return;
        }
        StartCoroutine(BreakTileRoutine());
    }
    private IEnumerator BreakTileRoutine()
    {
        breakableValue--; 
        breakableValue = Mathf.Clamp(breakableValue,0,breakableValue);

        yield return new WaitForSeconds(.2f);

        if (breakableSprites[breakableValue] != null)
        {
            spriteRenderer.sprite = breakableSprites[breakableValue];
        }

        if (breakableValue == 0)
        {
            tileType = TileType.Normal;
            spriteRenderer.color = normalColor;
        }
    }
}
