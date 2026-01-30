using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // Cần thiết để chuyển Scene

public class MainMenuController : MonoBehaviour
{

    SettingsController settingsController;
    private UIDocument _doc;
    private Button _btnPlay;
    private Button _btnSettings;
    private Button _btnQuit;

    void Start()
    {
        settingsController =  SettingsController.instance;
    }
    private void OnEnable()
    {
        
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;
        // 1. Tìm các nút
        _btnPlay = root.Q<Button>("btn-play");
        _btnSettings = root.Q<Button>("btn-settings");
        _btnQuit = root.Q<Button>("btn-quit");

        // 2. Gán sự kiện
        _btnPlay.clicked += OnPlayClicked;
        _btnSettings.clicked += OnSettingsClicked;
        _btnQuit.clicked += OnQuitClicked;
    }
    
    private void OnDisable()
    {
        // Luôn hủy đăng ký sự kiện
        _btnPlay.clicked -= OnPlayClicked;
        _btnSettings.clicked -= OnSettingsClicked;
        _btnQuit.clicked -= OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        // Load Scene GamePlay
        Debug.Log("Loading GamePlay Scene...");
        SceneManager.LoadScene("GamePlay");
    }

    private void OnSettingsClicked()
    {
        // Mở bảng Settings đã làm trước đó
        if (settingsController != null)
        {
            settingsController.DisplaySetting();
        }
        else
        {
            Debug.LogError("Chưa gán SettingsController vào MainMenuController!");
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();
        
        // Dòng này chỉ để test trong Unity Editor (vì Application.Quit không chạy trong Editor)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}