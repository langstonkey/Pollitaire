using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI movesText;

    [Header("Pause")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] Image pauseButton;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TextMeshProUGUI winScreenStats;
    [SerializeField] TextMeshProUGUI[] winTitle;
    [SerializeField] TextMeshProUGUI restartButton;
    [SerializeField] TextMeshProUGUI mainMenuButton;


    [SerializeField, ReadOnly] float currentTime;
    [SerializeField, ReadOnly] int moves;

    private void Start()
    {
        StartCoroutine(UpdateTime());
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;

            int mintues = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);

            string minuteText = mintues > 9 ? $"{mintues}" : $"0{mintues}";
            string secondsText = seconds > 9 ? $"{seconds}" : $"0{seconds}";

            timeText.text = $"{minuteText}:{secondsText}";
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void ResetStats()
    {
        currentTime = 0;
        moves = 0;
    }

    public void ShowWinScreen()
    {
        StopAllCoroutines();


        string winText = "";

        StatsManager.AddWin();

        if (moves > 9)
        {
            winText = $"{StatsManager.Wins}";
        }
        else
        {
            winText = $"0{StatsManager.Wins}";
        }

        winScreenStats.text = $"Total Wins: {StatsManager.Wins}\n" +
                              $"Moves: {moves}\n" +
                              $"Time: {timeText.text}";

        StartCoroutine(WinScreenRoutine());
    }


    IEnumerator WinScreenRoutine()
    {
        timeText.DOFade(0, 0.2f);
        movesText.DOFade(0, 0.2f);
        pauseButton.DOFade(0, 0.2f);
        ClearWinScreen();
        winPanel.SetActive(true);

        foreach (TextMeshProUGUI letter in winTitle)
        {
            letter.transform.DOPunchScale(Vector3.one, 0.2f);
            letter.transform.DOPunchRotation(Vector3.one * 45, 0.2f);
            letter.DOFade(1, 0.2f);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        winScreenStats.transform.DOPunchRotation(Vector3.one * 25, 0.2f);
        winScreenStats.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        winScreenStats.DOFade(1, 0.2f);
        yield return new WaitForSeconds(0.2f);

        restartButton.transform.DOPunchRotation(Vector3.one * 25, 0.2f);
        restartButton.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        restartButton.DOFade(1, 0.2f);

        yield return new WaitForSeconds(0.1f);
        mainMenuButton.transform.DOPunchRotation(Vector3.one * 25, 0.2f);
        mainMenuButton.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
        mainMenuButton.DOFade(1, 0.2f);

    }

    public void ClearWinScreen()
    {
        foreach (TextMeshProUGUI letter in winTitle)
        {
            letter.color = Color.clear;
        }

        Color clearWhite = new Color(1, 1, 1, 0);
        winScreenStats.color = clearWhite;
        restartButton.color = clearWhite;
        mainMenuButton.color = clearWhite;
    }

    public void MakeMove(Card card, bool autoAdded)
    {
        if (autoAdded) return;

        moves++;

        if (moves > 99)
        {
            movesText.text = $"{moves}";
        }
        else if (moves > 9)
        {
            movesText.text = $"0{moves}";
        }
        else
        {
            movesText.text = $"00{moves}";
        }
    }
}
