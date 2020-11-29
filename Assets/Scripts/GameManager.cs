using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    // public int movesLeft = 30;
    // public int scoreGoal = 10000;

    private Board board;

    private LevelGoal m_LevelGoal;
    // private LevelGoalTimed levelGoalTimed;
    private LevelGoalCollected levelGoalCollected;
    public LevelGoal levelGoal { get { return m_LevelGoal; } }


    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;
    private bool isReadyToReload = true;


    public Sprite loseIcon, winIcon, goalIcon;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

    public override void Awake()
    {
        base.Awake();
        m_LevelGoal = GetComponent<LevelGoal>();
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
        // levelGoalTimed = GetComponent<LevelGoalTimed>();
        levelGoalCollected = GetComponent<LevelGoalCollected>();
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.scoreMeter != null)
            {
                UIManager.Instance.scoreMeter.SetupStars(m_LevelGoal);
            }

            if (UIManager.Instance.levelNameText != null)
            {
                Scene scene = SceneManager.GetActiveScene();
                UIManager.Instance.levelNameText.text = scene.name;
            }
            if (levelGoalCollected != null)
            {
                UIManager.Instance.EnableCollectionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(levelGoalCollected.collectionGoals);
            }
            else
            {
                UIManager.Instance.EnableCollectionGoalLayout(false);
            }
            bool useTimer = (m_LevelGoal.levelCounter == LevelCounter.Timer);
            UIManager.Instance.EnableTimer(useTimer);
            UIManager.Instance.EnableMovesCounter(!useTimer);
        }

        m_LevelGoal.movesLeft++;
        UpdateMoves();
        StartCoroutine(ExecuteGameLoop());
    }
    public void UpdateMoves()
    {
        if (levelGoal.levelCounter == LevelCounter.Moves)
        {
            m_LevelGoal.movesLeft--;
            if (UIManager.Instance != null && UIManager.Instance.movesLeftText != null)
            {
                UIManager.Instance.movesLeftText.text = m_LevelGoal.movesLeft.ToString();
            }
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
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowMessage(goalIcon, "SCORE GOAL \n" + m_LevelGoal.scoreGoals[0].ToString(), "START");
            }
        }
        while (!isReadyToBegin)
        {
            yield return null;
        }

        if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
        {
            UIManager.Instance.screenFader.FadeOff();
        }

        yield return new WaitForSeconds(1f);

        if (board != null)
        {
            board.SetupBoard();
        }
    }
    private IEnumerator PlayGameRoutine()
    {
        if (m_LevelGoal.levelCounter == LevelCounter.Timer)
        {
            m_LevelGoal.StartCountDown();
        }
        while (!IsGameOver)
        {
            isGameOver = m_LevelGoal.IsGameOver();
            isWinner = m_LevelGoal.IsWinner();
            yield return null;
        }
    }

    private IEnumerator WaitForBoardRoutine(float delay = .5f)
    {

        if (m_LevelGoal.levelCounter == LevelCounter.Timer && UIManager.Instance != null && UIManager.Instance.timer != null)
        {
            if (UIManager.Instance.timer != null)
            {
                UIManager.Instance.timer.FadeOff();
                UIManager.Instance.timer.paused = true;
            }
        }

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
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowMessage(winIcon, "YOU WIN", "OK");
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        }
        else
        {
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowMessage(loseIcon, "YOU LOSE", "OK");
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
        }
        yield return new WaitForSeconds(1.5f);
        if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
        {
            UIManager.Instance.screenFader.FadeOn();
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

    public void ScorePoints(GamePiece piece, int multipler = 1, int bonus = 0)
    {
        if (UIManager.Instance != null && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(piece.scoreValue * multipler + bonus);
            m_LevelGoal.UpdateScoreStars(ScoreManager.Instance.GetCurrentScore());

            if (UIManager.Instance.scoreMeter != null)
            {
               UIManager.Instance.scoreMeter.UpdateScoreMeter(ScoreManager.Instance.GetCurrentScore(), m_LevelGoal.scoreStars);
            }
        }
        if (SoundManager.Instance != null && piece.clearSound != null)
        {
            SoundManager.Instance.PlayClipAtPoint(piece.clearSound, Vector3.zero, SoundManager.Instance.fxVolume);
        }
    }

    public void AddTime(int timeValue)
    {
        if (m_LevelGoal.levelCounter == LevelCounter.Timer)
        {
            m_LevelGoal.AddTime(timeValue);
        }
    }

    public void UpdateCollectionGoals(GamePiece pieceToCheck)
    {
        if (levelGoalCollected != null)
        {
            levelGoalCollected.UpdateGoals(pieceToCheck);
        }
    }
}
