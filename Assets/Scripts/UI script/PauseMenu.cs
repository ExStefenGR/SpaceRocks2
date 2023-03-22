using UnityEngine;
using static Player;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Player player;

    private void Start()
    {
        // Initially, the pause menu UI should be hidden
        pauseMenuUI.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (player.CurrentState == PlayerState.Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        player.CurrentState = PlayerState.Normal;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        player.CurrentState = PlayerState.Paused;
    }

    // Add any other methods for the Pause menu options, e.g., Restart, Main Menu, etc.
}