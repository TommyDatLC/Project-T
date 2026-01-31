using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance = null;
    private UIDocument _doc;
    private VisualElement _root;
    
    private Slider _musicSlider;
    private DropdownField _resDropdown;
    
    // Sử dụng đúng kiểu Custom Control GameButton
    private GameButton _saveBtn;
    private GameButton _closeBtn;
    private VisualElement _overlay;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) return;
        
        _root = _doc.rootVisualElement;

        // Tìm các thành phần UI
        _musicSlider = _root.Q<Slider>("music-slider");
        _resDropdown = _root.Q<DropdownField>("resolution-dropdown");
        _saveBtn = _root.Q<GameButton>("save-btn");
        _closeBtn = _root.Q<GameButton>("close-btn");
        _overlay = _root.Q<VisualElement>("overlay");

        // Kiểm tra lỗi tìm kiếm
        if (_saveBtn == null) Debug.LogError("Không tìm thấy save-btn. Kiểm tra Name trong UI Builder!");
        if (_closeBtn == null) Debug.LogError("Không tìm thấy close-btn. Kiểm tra Name trong UI Builder!");

        // Khởi tạo trạng thái
        if (_overlay != null) _overlay.style.display = DisplayStyle.None;

        // Đăng ký sự kiện
        if (_saveBtn != null) _saveBtn.clicked += OnSaveClicked;
        if (_closeBtn != null) _closeBtn.clicked += OnCloseClicked;
        if (_musicSlider != null) _musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        if (_resDropdown != null) _resDropdown.RegisterValueChangedCallback(OnResolutionChanged);

        LoadSettings();
    }

    private void OnDisable()
    {
        if (_saveBtn != null) _saveBtn.clicked -= OnSaveClicked;
        if (_closeBtn != null) _closeBtn.clicked -= OnCloseClicked;
        if (_musicSlider != null) _musicSlider.UnregisterValueChangedCallback(OnMusicVolumeChanged);
        if (_resDropdown != null) _resDropdown.UnregisterValueChangedCallback(OnResolutionChanged);
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue / 100f;
    }

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        ApplyResolution(evt.newValue);
    }

    private void OnSaveClicked()
    {
        Debug.Log("Nút Save đã được bấm!"); // Kiểm tra trong Console
        float currentVol = _musicSlider.value;
        string currentRes = _resDropdown.value;

        PlayerPrefs.SetFloat("MusicVol", currentVol);
        PlayerPrefs.SetString("Resolution", currentRes);
        PlayerPrefs.Save();

        OnCloseClicked();
    }

    private void OnCloseClicked()
    {
        Debug.Log("Nút Close đã được bấm!"); // Kiểm tra trong Console
        if (_overlay != null) _overlay.style.display = DisplayStyle.None;
    }

    private void LoadSettings()
    {
        float savedVol = PlayerPrefs.GetFloat("MusicVol", 80f);
        string savedRes = PlayerPrefs.GetString("Resolution", "Max (Native)");

        if (_musicSlider != null) _musicSlider.value = savedVol;
        if (_resDropdown != null) _resDropdown.value = savedRes;
        
        AudioListener.volume = savedVol / 100f;
        ApplyResolution(savedRes);
    }

    private void ApplyResolution(string selection)
    {
        switch (selection)
        {
            case "Max (Native)":
                Resolution maxRes = Screen.currentResolution;
                Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.FullScreenWindow);
                break;
            case "FullHD (1920x1080)":
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case "HD (720p)":
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
        }
    }

    [ContextMenu("DisplaySetting")]
    public void DisplaySetting()
    {
        if (_overlay != null) _overlay.style.display = DisplayStyle.Flex;
    }
}