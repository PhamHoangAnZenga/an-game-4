using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameManager _gameManager;

    void Awake()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        _button.onClick.AddListener(PauseGame);
    }

    public void PauseGame()
    {
        _gameManager.ChangeState();
    }   
}
