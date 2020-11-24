using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    // public int movesLeft = 30;
    // public int scoreGoal = 10000;

    public ScreenFader screenFader;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI movesLeftText;
    private Board board;

    private LevelGoal levelGoal;
    public ScoreMeter scoreMeter;

    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;
    private bool isReadyToReload = true;

    public MessageWindow messageWindow;
    public Sprite loseIcon,winIcon,goalIcon;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

    public override void Awake()
    {
        base.Awake();
        levelGoal = GetComponent<LevelGoal>();
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start()
    {
        if (scoreMeter != null)
        {
            scoreMeter.SetupStars(levelGoal);
        }
        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }
        levelGoal.movesLeft++;
        UpdateMoves();
        StartCoroutine(ExecuteGameLoop());
    }
    public void UpdateMoves() 
    {
        levelGoal.movesLeft--;
        if (movesLeftText !=null)
        {
            movesLeftText.text = levelGoal.movesLeft.ToString();
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
            messageWindow.ShowMessage(goalIcon,"SCORE GOAL \n"+ levelGoal.scoreGoals[0].ToString(),"START");
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
            isGameOver = levelGoal.IsGameOver();
            isWinner = levelGoal.IsWinner();
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

    public void ScorePoints(GamePiece piece,int multipler = 1, int bonus = 0)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(piece.scoreValue * multipler + bonus);
            levelGoal.UpdateScoreStars(ScoreManager.Instance.GetCurrentScore());
            
            if (scoreMeter != null)
            {
                scoreMeter.UpdateScoreMeter(ScoreManager.Instance.GetCurrentScore(),levelGoal.scoreStars);
            }
        }
        if (SoundManager.Instance !=null && piece.clearSound != null)
        {
            SoundManager.Instance.PlayClipAtPoint(piece.clearSound,Vector3.zero,SoundManager.Instance.fxVolume);
        }
    }
}
