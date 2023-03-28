using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public static ScoreManager Instance { get; private set; }

    private int score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }
}
