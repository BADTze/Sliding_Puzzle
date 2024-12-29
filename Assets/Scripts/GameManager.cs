using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject panelIdle;
    [SerializeField] private GameObject panelGameplay;
    [SerializeField] private GameObject panelEnd;
    [SerializeField] public GameObject Tiles;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI playerTimeText;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private float bestTime = Mathf.Infinity;

    private void Start()
    {
        LoadBestTime();
        ShowPanelIdle();
        Tiles.SetActive(false);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = FormatTime(elapsedTime); 
        }
    }

    private void LoadBestTime()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
            bestTimeText.text = FormatTime(bestTime);
        }
        else
        {
            bestTime = Mathf.Infinity;
            bestTimeText.text = "00:00";
        }
    }

    private void SaveBestTime(float time)
    {
        PlayerPrefs.SetFloat("BestTime", time);
        PlayerPrefs.Save();
        Debug.Log("Best time saved: " + time); // Log untuk memastikan penyimpanan
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartGame()
    {
        if (isTimerRunning) return;
        Tiles.SetActive(true);

        elapsedTime = 0f;
        isTimerRunning = true;

        panelIdle.SetActive(false);
        panelEnd.SetActive(false);
        panelGameplay.SetActive(true);
        FindObjectOfType<PuzzleManager>().Shuffle();
    }

    public void EndGame()
    {
        if (!isTimerRunning) return;

        isTimerRunning = false;

        // Tampilkan waktu bermain player
        playerTimeText.text = FormatTime(elapsedTime);

        if (elapsedTime < bestTime)
        {
            bestTime = elapsedTime;
            SaveBestTime(bestTime);
            bestTimeText.text = FormatTime(bestTime); 
        }

        // Tampilkan panel end
        panelGameplay.SetActive(false);
        panelEnd.SetActive(true);
        StartCoroutine(ReturnToIdleAfterDelay(5f));
    }

    private IEnumerator ReturnToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPanelIdle();
    }

    public void ShowPanelIdle()
    {
        Tiles.SetActive(false);
        panelIdle.SetActive(true);
        panelGameplay.SetActive(false);
        panelEnd.SetActive(false);
    }
}
