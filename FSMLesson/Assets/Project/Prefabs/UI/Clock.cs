using System.Collections;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private bool _gameOver = false;
    private bool _gamePause = false;

    [SerializeField] private TextMeshProUGUI _tmp;
    [SerializeField] private float _playTime;

    private float _timer;    
    
    void Awake()
    {
        if (_tmp == null) _tmp = GetComponent<TextMeshProUGUI>();
    }

    IEnumerator Start()
    {
        _timer = Time.time + _playTime;
        while (_gameOver == false)
        {
            if (_timer < Time.time) yield break;

            UpdateTime(_timer - Time.time);

            yield return new WaitForSeconds(0.5f);
        }
    }    

    public void Win()
    {
        _gameOver = true;
        _tmp.text = "You win!";        
    }

    public void GameOver()
    {
        _gameOver = true;
        _tmp.text = "Game Over!";
    }
    public void GamePause()
    {
        _gamePause = true;
        _tmp.text = "Pause!";
    }

    private void UpdateTime(float time)
    {
        float min = Mathf.FloorToInt(time / 60);
        float sec = Mathf.FloorToInt(time % 60);
        _tmp.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}
