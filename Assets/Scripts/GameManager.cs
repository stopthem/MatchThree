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

    private Board m_board;

    private LevelGoal m_LevelGoal;
    // private LevelGoalTimed levelGoalTimed;
    private LevelGoalCollected m_LevelGoalCollected;
    public LevelGoal levelGoal { get { return m_LevelGoal; } }


    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;
    private bool isReadyToReload = true;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

    public override void Awake()
    {
        base.Awake();
        m_LevelGoal = GetComponent<LevelGoal>();
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
        // levelGoalTimed = GetComponent<LevelGoalTimed>();
        m_LevelGoalCollected = GetComponent<LevelGoalCollected>();
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
            if (m_LevelGoalCollected != null)
            {
                UIManager.Instance.EnableCollectionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(m_LevelGoalCollected.collectionGoals);
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
                UIManager.Instance.messageWindow.ShowScoreMessage(levelGoal.scoreGoals[0]);

                if (m_LevelGoal.levelCounter == LevelCounter.Timer)
                {
                    UIManager.Instance.messageWindow.ShowTimeGoal(m_LevelGoal.timeLeft);
                }
                else
                {
                    UIManager.Instance.messageWindow.ShowMovesGoal(m_LevelGoal.movesLeft);
                }
                if (m_LevelGoalCollected != null)
                {
                    UIManager.Instance.messageWindow.ShowCollectionGoal(true);

                    GameObject goalLayout = UIManager.Instance.messageWindow.collectionGoalLayout;

                    if (goalLayout != null)
                    {
                        UIManager.Instance.SetupCollectionGoalLayout(m_LevelGoalCollected.collectionGoals, goalLayout, 70);
                    }
                }
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

        if (m_board != null)
        {
            m_board.SetupBoard();
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

        if (m_board != null)
        {
            yield return new WaitForSeconds(m_board.fillMoveTime);
            while (m_board.isRefilling)
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
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
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
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene("Level 1");
        }
        

    }

    private void ShowLoseScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
        {
            UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.messageWindow.ShowLoseMessage();
            UIManager.Instance.messageWindow.ShowCollectionGoal(false);

            string caption = "";
            if (m_LevelGoal.levelCounter == LevelCounter.Timer)
            {
                caption = "Out of time!";
            }
            else
            {
                caption = "Out of moves!";
            }
            UIManager.Instance.messageWindow.ShowGoalCaption(caption, 0, 70);

            if (UIManager.Instance.messageWindow.goalFailedIcon != null)
            {
                UIManager.Instance.messageWindow.ShowGoalImage(UIManager.Instance.messageWindow.goalFailedIcon);
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLoseSound();
        }
    }

    private void ShowWinScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
        {
            UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.messageWindow.ShowWinMessage();
            UIManager.Instance.messageWindow.ShowCollectionGoal(false);

            if (ScoreManager.Instance != null)
            {
                string scoreStr = "YOU SCORED \n" + ScoreManager.Instance.GetCurrentScore().ToString() + " POINTS!";
                UIManager.Instance.messageWindow.ShowGoalCaption(scoreStr, 0, 70);
            }

            if (UIManager.Instance.messageWindow.goalCompleteIcon != null)
            {
                UIManager.Instance.messageWindow.ShowGoalImage(UIManager.Instance.messageWindow.goalCompleteIcon);
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWinSound();
        }
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
        if (m_LevelGoalCollected != null)
        {
            m_LevelGoalCollected.UpdateGoals(pieceToCheck);
        }
    }
}
