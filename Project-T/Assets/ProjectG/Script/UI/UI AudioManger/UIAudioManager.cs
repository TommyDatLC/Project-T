using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    [SerializeField] private AudioSource _audioSource;
    
    public AudioClip clickSound;
    public AudioClip hoverSound;

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

        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayClick()
    {
        if (clickSound != null) _audioSource.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        if (hoverSound != null) _audioSource.PlayOneShot(hoverSound);
    }
}