using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    private bool playerInputEnabled = true;
    
    private void Start()
    {
        tileArray = new Tile[width,height];
        gamePiecesArray = new GamePiece[width,height];
        SetupTiles();
        SetupCamera();
        FillBoard(10,.5f);
        // HighlightMatches();
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
        if (IsWithinBounds(x,y))
		{
			gamePiecesArray[x,y] = gamePiece;
		}
        gamePiece.SetCoordination(x,y);
    }
    bool IsWithinBounds(int x, int y)
	{
		return (x >= 0 && x < width && y>= 0 && y<height);
	}
    private GamePiece FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        GameObject randomPiece = Instantiate(GetRandomPiece(), Vector3.zero, Quaternion.identity);
        if (randomPiece != null)
        {
            randomPiece.GetComponent<GamePiece>().SetBoard(this);
            PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), x, y);

            if (falseYOffset != 0 )
            {
                randomPiece.transform.position = new Vector3(x, y + falseYOffset ,0);
                randomPiece.GetComponent<GamePiece>().Move(x,y,moveTime);
            }

            randomPiece.transform.parent = transform;
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }
    private void FillBoard(int falseYOffset = 0 , float moveTime = .1f)
    {
        int maxInterations = 100;
        int iterations = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gamePiecesArray[i,j] == null)
                {
                    GamePiece piece = FillRandomAt(i, j, falseYOffset, moveTime);
                    iterations = 0;
                    while (HasMatchOnFill(i,j))
                    {
                        ClearPieceAt(i,j);
                        piece = FillRandomAt(i,j, falseYOffset , moveTime);
                        iterations++;
                        if (iterations >= maxInterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    
    private bool HasMatchOnFill(int x, int y, int minLegth = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x,y, new Vector2(-1,0),minLegth);
        List<GamePiece> downwardMatches = FindMatches(x,y, new Vector2(0,-1),minLegth);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }
        return (leftMatches.Count > 0 || downwardMatches.Count > 0);
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
        StartCoroutine(SwitchTilesRoutine(clickedTile,targetTile));
    }
    private IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (playerInputEnabled)
        {
            if (isNextTo(clickedTile,targetTile))
            {
                GamePiece clickedPiece = gamePiecesArray[clickedTile.xIndex, clickedTile.yIndex];
                GamePiece targetPiece = gamePiecesArray[targetTile.xIndex , targetTile.yIndex];

                if (targetPiece != null && clickedPiece != null)
                {
                    clickedPiece.Move(targetTile.xIndex, targetTile.yIndex , moveSpeed);
                    targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex , moveSpeed);

                    yield return new WaitForSeconds(moveSpeed);

                    List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex,clickedTile.yIndex);
                    List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex,targetTile.yIndex);

                    if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0)
                    {
                        clickedPiece.Move(clickedTile.xIndex,clickedTile.yIndex,moveSpeed);
                        targetPiece.Move(targetTile.xIndex,targetTile.yIndex,moveSpeed);
                    }
                    else
                    {
                        yield return new WaitForSeconds(moveSpeed);
                        ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
                    }

                    
                }
            }
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
    private List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLegth =3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;
        if (IsWithinBounds(startX,startY))
        {
            startPiece = gamePiecesArray[startX,startY];
        }
        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }
        int nextX,nextY;
        int maxValue = (width > height) ? width: height;
        for (int i = 1; i < maxValue; i++)
        {
            nextX = startX + (int) Mathf.Clamp(searchDirection.x,-1,1) * i;
            nextY = startY + (int) Mathf.Clamp(searchDirection.y,-1,1) * i;
            if (!IsWithinBounds(nextX,nextY))
            {
                break;
            }

            GamePiece nextPiece = gamePiecesArray[nextX,nextY];
            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
            
        }
        if (matches.Count >= minLegth)
        {
            return matches;
        }
        return null;

    }
    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLegth =3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX,startY, new Vector2(0,1),2);
        List<GamePiece> downwoardMatches = FindMatches(startX,startY, new Vector2(0,-1),2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwoardMatches == null)
        {
            downwoardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwoardMatches).ToList();

        return (combinedMatches.Count >= minLegth) ? combinedMatches : null;
    }
    private List<GamePiece> FindHorizontalMatches(int startX, int startY,int minLegth =3)
    {
        List<GamePiece> rightMatches = FindMatches(startX,startY, new Vector2(1,0),2);
        List<GamePiece> leftMatches = FindMatches(startX,startY, new Vector2(-1,0),2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLegth) ? combinedMatches : null;
    }
    private void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);

        List<GamePiece> combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HightlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    private void HightlightTileOn(int x, int y, Color color)
    {
        SpriteRenderer spriteRenderer = tileArray[x,y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    private void HighlightTileOff(int x, int y)
    {
        SpriteRenderer spriteRenderer = tileArray[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
    }

    private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y, 3);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y, 3);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }
        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        return combinedMatches;
    }
    private List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLegth = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLegth)).ToList();
        }
        return matches;
    }
    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GamePiece> matches = FindMatchesAt(i,j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }
    private void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = gamePiecesArray[x,y];
        if (pieceToClear != null)
        {
            gamePiecesArray[x,y] = null;
            Destroy(pieceToClear.gameObject);
        }
        HighlightTileOff(x,y);
    }
    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i,j);
            }
        }
    }
    private void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
               ClearPieceAt(piece.xIndex,piece.yIndex); 
            }
        }
    }
    private void HighlightMatches(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            HightlightTileOn(piece.xIndex, piece.yIndex , piece.GetComponent<SpriteRenderer>().color);
        }
    }
    private List<GamePiece> CollapseColumn(int column, float collapseTime = .1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < height - 1; i++)
        {
            if (gamePiecesArray[column,i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (gamePiecesArray[column,j] != null)
                    {
                        gamePiecesArray[column, j].Move(column , i ,collapseTime * (j - i));
                        gamePiecesArray[column, i] = gamePiecesArray[column, j];
                        gamePiecesArray[column, i].SetCoordination(column,i);
                        
                        if (!movingPieces.Contains(gamePiecesArray[column,i]))
                        {
                            movingPieces.Add(gamePiecesArray[column, i]);    
                        }
                        gamePiecesArray[column,j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }
    private List<GamePiece> CollapseColumn(List<GamePiece> gamePieces, float collapseTime = .1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }

    private List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }
        return columns;
    }
    private void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }
    private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        playerInputEnabled = false;
        List<GamePiece> matches = gamePieces;

        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();

            yield return new WaitForSeconds(.5f);
        }
        while (matches.Count != 0);
        playerInputEnabled = true;
    }
    private IEnumerator RefillRoutine()
    {
        FillBoard(10, .5f);
        yield return null;
    }
    private IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        HighlightMatches(gamePieces);

        yield return new WaitForSeconds(.5f);

        bool isFinished = false;
        while (!isFinished)
        {
            ClearPieceAt(gamePieces);
            
            yield return new WaitForSeconds(.2f);

            movingPieces = CollapseColumn(gamePieces);

            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(.2f);

            matches = FindMatchesAt(movingPieces);

            if (matches.Count == 0)
            {
                isFinished = true;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }
    private bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
