using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField]private int yIndex;
    [SerializeField]private int xIndex;

    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Init(int x, int y , Board board)
    {
        xIndex = x;
        yIndex = y;
        board = this.board;
    }
}
