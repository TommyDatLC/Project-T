using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SettingsController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    
    private Slider _musicSlider;
    private DropdownField _resDropdown; // Thay đổi từ Toggle sang Dropdown
    private Button _saveBtn;
    private Button _closeBtn;
    private VisualElement _overlay;

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _musicSlider = _root.Q<Slider>("music-slider");
        _resDropdown = _root.Q<DropdownField>("resolution-dropdown"); // Lấy Dropdown
        _saveBtn = _root.Q<Button>("save-btn");
        _closeBtn = _root.Q<Button>("close-btn");
        _overlay = _root.Q<VisualElement>("overlay");

        _saveBtn.clicked += OnSaveClicked;
        _closeBtn.clicked += OnCloseClicked;

        // Xử lý sự kiện thay đổi độ phân giải ngay lập tức (hoặc để trong nút Save tùy bạn)
        _resDropdown.RegisterValueChangedCallback(OnResolutionChanged);
    }

    private void OnDisable()
    {
        _saveBtn.clicked -= OnSaveClicked;
        _closeBtn.clicked -= OnCloseClicked;
        _resDropdown.UnregisterValueChangedCallback(OnResolutionChanged);
    }

    // Hàm xử lý logic độ phân giải
    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        string selectedOption = evt.newValue;
        Debug.Log("Đổi độ phân giải sang: " + selectedOption);

        switch (selectedOption)
        {
            case "Max (Native)":
                // Lấy độ phân giải gốc của màn hình hiện tại
                Resolution maxRes = Screen.currentResolution;
                Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.FullScreenWindow);
                break;

            case "FullHD (1920x1080)":
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;

            case "HD (720p)":
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed); // Thường 720p thì để cửa sổ hoặc full tùy game
                break;
        }
    }

    private void OnSaveClicked()
    {
        // --- BƯỚC 1: LƯU DỮ LIỆU VÀO PLAYERPREFS ---
        float currentVol = _musicSlider.value;
        string currentRes = _resDropdown.value;

        PlayerPrefs.SetFloat("MusicVol", currentVol);
        PlayerPrefs.SetString("Resolution", currentRes);
        PlayerPrefs.Save();

        // --- BƯỚC 2: ÁP DỤNG ÂM THANH (GLOBAL) ---
        // AudioListener.volume nhận giá trị từ 0.0 đến 1.0
        // Slider của bạn chạy từ 0 đến 100 nên cần chia cho 100
        AudioListener.volume = currentVol / 100f;
        Debug.Log($"Đã chỉnh âm lượng toàn cục: {AudioListener.volume}");

        // --- BƯỚC 3: ÁP DỤNG ĐỘ PHÂN GIẢI ---
        ApplyResolution(currentRes);

        // --- BƯỚC 4: ĐÓNG PANEL ---
        OnCloseClicked();
    }

// Hàm tách riêng để xử lý logic đổi độ phân giải
// Giúp code gọn hơn và có thể tái sử dụng khi Load game
    private void ApplyResolution(string selection)
    {
        Debug.Log($"Đang đổi độ phân giải sang: {selection}");

        switch (selection)
        {
            case "Max (Native)":
                // Lấy độ phân giải gốc cao nhất của màn hình
                Resolution maxRes = Screen.currentResolution;
                Screen.SetResolution(maxRes.width, maxRes.height, FullScreenMode.FullScreenWindow);
                break;

            case "FullHD (1920x1080)":
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;

            case "HD (720p)":
                // 720p thường dùng chế độ cửa sổ (Windowed) để test hoặc cho máy yếu
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
            
            default:
                Debug.LogWarning("Không tìm thấy độ phân giải phù hợp!");
                break;
        }
    }

    private void OnCloseClicked()
    {
        _overlay.style.display = DisplayStyle.None;
    }

    [ContextMenu("DisplaySetting")]
    void DisplaySetting()
    {
        _overlay.style.display = DisplayStyle.Flex;
    }
}