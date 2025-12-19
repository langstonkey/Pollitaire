using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI movesText;

    [Header("Pause")]
    [SerializeField] GameObject pausePanel;

    [Header("Win Screen")]
    [SerializeField] GameObject winPanel;
    [SerializeField] TextMeshProUGUI winScreenStats;
    [SerializeField] TextMeshProUGUI[] winTitle;


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

        winPanel.SetActive(true);

        winScreenStats.text = $"Moves: {moves}\n" +
                              $"Time: {timeText.text}";
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
