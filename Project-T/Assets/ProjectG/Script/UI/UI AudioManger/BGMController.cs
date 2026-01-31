using UnityEngine;

public class BGMController : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    void Start()
    {
        if (bgmSource == null) bgmSource = GetComponent<AudioSource>();

        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true; // Luôn lặp lại nhạc nền
            bgmSource.playOnAwake = true; 
            bgmSource.Play();
        }
    }

    // Hàm để các script khác có thể tắt/mở nhạc (Ví dụ từ menu Settings)
    public void SetVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }
}