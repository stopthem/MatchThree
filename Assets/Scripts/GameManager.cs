using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;

    public ScreenFader screenFader;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI movesLeftText;
    private Board board;

    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;
    private bool isReadyToReload = true;

    public MessageWindow messageWindow;
    public Sprite loseIcon,winIcon,goalIcon;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

    private void Start()
    {
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();

        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }
        UpdateMoves();
        StartCoroutine(ExecuteGameLoop());
    }
    public void UpdateMoves()
    {
        if (movesLeftText !=null)
        {
            movesLeftText.text = movesLeft.ToString();
        }
    }
    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());
        yield return StartCoroutine(WaitForBoardRoutine());
        yield return StartCoroutine(EndGameRoutine());
    }
    public void BeginGame()
    {
        isReadyToBegin = true;
    }
    private IEnumerator StartGameRoutine()
    {
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectXformMover>().MoveOn();
            messageWindow.ShowMessage(goalIcon,"SCORE GOAL \n"+ scoreGoal.ToString(),"START");
        }
        while (!isReadyToBegin)
        {
            yield return null;
        }
        if (screenFader != null)
        {
            screenFader.FadeOff();
        }
        yield return new WaitForSeconds(1f);
        if (board != null)
        {
            board.SetupBoard();
        }
        
    }
    private IEnumerator PlayGameRoutine()
    {
        while (!IsGameOver)
        {
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.GetCurrentScore() >= scoreGoal)
                {
                    IsGameOver = true;
                    isWinner = true;
                }
            }
            if (movesLeft <= 0)
            {
                IsGameOver = true;
                isWinner = false;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForBoardRoutine(float delay = .5f)
    {
        if (board != null)
        {
            yield return new WaitForSeconds(board.fillMoveTime);
            while (board.isRefilling)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(delay);
    }
    private IEnumerator EndGameRoutine()
    {
        isReadyToReload = false;

        if (isWinner)
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(winIcon,"YOU WIN","OK");
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon,"YOU LOSE","OK");
            }
            
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
        }
        yield return new WaitForSeconds(1.5f);
        if (screenFader != null)
        {
            screenFader.FadeOn();
        }
        while (!isReadyToReload)
        {
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
    public void ReloadScene()
    {
        isReadyToReload = true;
    }
}
