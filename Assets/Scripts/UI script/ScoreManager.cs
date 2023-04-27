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

            GameObject persistentObject = new GameObject("PersistentScoreManager");
            ScoreManager persistentScoreManager = persistentObject.AddComponent<ScoreManager>();
            persistentObject.transform.SetParent(null);
            DontDestroyOnLoad(persistentObject);

            // Copy the properties of the current ScoreManager to the new ScoreManager
            persistentScoreManager.score = score;
            persistentScoreManager.OnScoreChanged = OnScoreChanged;

            // Replace the Instance reference with the new ScoreManager component
            Instance = persistentScoreManager;

            // Destroy the old ScoreManager component
            Destroy(this);
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
