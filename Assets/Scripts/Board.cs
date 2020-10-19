using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int height;
    public int width;
    public int borderSize;
    public float moveSpeed;
    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;
    private Tile[,] tileArray;
    private GamePiece[,] gamePiecesArray;
    private Tile clickedTile,targetTile;
    
    private void Start()
    {
        tileArray = new Tile[width,height];
        gamePiecesArray = new GamePiece[width,height];
        SetupTiles();
        SetupCamera();
        FillRandom();
    }

    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab,new Vector3(i,j,0), Quaternion.identity) as GameObject;
                tile.name = "Tile (" + i + "," + j + ") ";
                tileArray [i,j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;
                tileArray[i,j].Init(i,j,this);

            }
            
        }
    }
    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float) (width - 1 )/2f, (float) (height - 1)/2f, -10f);
        float aspectRatio = (float) Screen.width / (float) Screen.height;
        float verticalSize = (float) height / 2f + (float) borderSize;
        float horizontalSize = ((float) width / 2f + (float) borderSize) / aspectRatio;
        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }
    private GameObject GetRandomPiece()
    {
        int randomPiece = Random.Range(0,gamePiecePrefabs.Length);
        return gamePiecePrefabs[randomPiece];
        
    }
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        gamePiece.transform.position = new Vector3(x,y,0);
        gamePiecesArray[x,y] = gamePiece;
        gamePiece.SetCoordination(x,y);
    }
    private void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomPiece(), Vector3.zero ,Quaternion.identity);
                if (randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().SetBoard(this);
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i ,j);
                    randomPiece.transform.parent = transform;
                }
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (clickedTile == null)
        {
            clickedTile = tile;
        }
    }
    public void DragToTile(Tile tile)
    {
        if (clickedTile != null)
        {
            targetTile = tile;
        }
    }
    public void ReleaseTile()
    {
        if (clickedTile != null && targetTile != null)
        {
            SwitchTiles(clickedTile,targetTile);
        }
        clickedTile = null;
        targetTile = null;
    }
    private void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        if (isNextTo(clickedTile,targetTile))
        {
            GamePiece clickedPiece = gamePiecesArray[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = gamePiecesArray[targetTile.xIndex , targetTile.yIndex];
            clickedPiece.Move(targetTile.xIndex, targetTile.yIndex , moveSpeed);
            targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex , moveSpeed); 
        }
        
    }
    bool isNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }
        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }
        return false;
    }
}
