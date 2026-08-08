using UnityEngine;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    void Awake()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        _button.onClick.AddListener(PlaySound);
    }

    public void PlaySound()
    {
        if (_audioSource == null || _audioClip == null) return;
        
        _audioSource.PlayOneShot(_audioClip);
    }    
    
}
