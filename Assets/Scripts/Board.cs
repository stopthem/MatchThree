using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int height;
    public int width;
    public int borderSize;
    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;
    private Tile[,] tileArray;
    private GamePiece[,] gamePiecesArray;
    
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
    private void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        gamePiece.transform.position = new Vector3(x,y,0);
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
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i ,j);
                }
            }
        }
    }

}
