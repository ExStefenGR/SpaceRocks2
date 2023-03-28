using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float updateDuration = 0.5f;

    private ScoreManager scoreManager;
    private int displayedScore;
    private Coroutine updateCoroutine;

    private void Start()
    {
        scoreManager = ScoreManager.Instance;
        displayedScore = scoreManager.GetScore();
        scoreText.text = displayedScore.ToString();
        scoreManager.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= HandleScoreChanged;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (displayedScore != newScore)
        {
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }
            updateCoroutine = StartCoroutine(UpdateScoreSmoothly(displayedScore, newScore));
        }
    }

    private IEnumerator UpdateScoreSmoothly(int fromScore, int toScore)
    {
        float progress = 0f;
        float startTime = Time.time;

        while (progress < 1f)
        {
            progress = (Time.time - startTime) / updateDuration;
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(fromScore, toScore, progress));
            scoreText.text = displayedScore.ToString();
            yield return null;
        }

        displayedScore = toScore;
        scoreText.text = displayedScore.ToString();
    }
}