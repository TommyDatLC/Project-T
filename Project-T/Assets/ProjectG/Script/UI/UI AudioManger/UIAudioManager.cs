using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _bgmSource;
    
    [Header("Clips")]
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public AudioClip bgmClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Tự động thiết lập AudioSource nếu chưa gán
            if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
            if (_bgmSource == null) 
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;
                _bgmSource.clip = bgmClip;
            }
        }
        else
        {
            // Nếu đã có Instance rồi, phải kiểm tra để phát lại nhạc trước khi xóa bản phụ
            if (Instance.bgmClip != null && !Instance._bgmSource.isPlaying)
            {
                Instance.PlayBGM();
            }
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kiểm tra chính xác tên Scene từ Project [image_11e8dc.png]
        // Nếu tên scene là "MainMenu" thì phát nhạc, các scene khác thì dừng
        if (scene.name == "MainMenu" || scene.name == "GamePlay" || scene.name == "EndGame")
        {
            PlayBGM();
        }
        else
        {
            StopBGM();
        }
    }

    public void PlayClick() => _sfxSource.PlayOneShot(clickSound);
    public void PlayHover() => _sfxSource.PlayOneShot(hoverSound);

    public void PlayBGM()
    {
        if (_bgmSource != null && !_bgmSource.isPlaying)
        {
            _bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (_bgmSource != null) _bgmSource.Stop();
    }
}