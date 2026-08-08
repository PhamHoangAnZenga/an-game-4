using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Clock _clock;
    [SerializeField] private Goal _goal;
    [SerializeField] private GameObject _pausePannel;

    private bool _gameOn = true;

    void Awake()
    {
        _pausePannel.SetActive(false);
    }

    public void ChangeState()
    {
        _gameOn = !_gameOn;

        if (_gameOn) ResumeGame();
        else PauseGame();
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0f;
        _clock.GamePause();
        _pausePannel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _pausePannel.SetActive(false);
    }

    public void GameOver()
    {
        _clock.GameOver();
        _goal.GameOver();
    }

    public void Win()
    {
        Time.timeScale = 0f;
        _clock.Win();
    }
}
