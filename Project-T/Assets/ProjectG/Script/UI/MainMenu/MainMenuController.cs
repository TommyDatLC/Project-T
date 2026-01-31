using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _doc;
    
    // 1. CHỈNH SỬA: Đổi từ Button sang GameButton để khớp với Custom Control của bạn
    private GameButton _btnPlay;
    private GameButton _btnSettings;
    private GameButton _btnQuit;

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null) return;

        var root = _doc.rootVisualElement;

        // 2. CHỈNH SỬA: Tìm đúng kiểu dữ liệu GameButton theo Name bạn đặt trong UI Builder
        _btnPlay = root.Q<GameButton>("btn-play");
        _btnSettings = root.Q<GameButton>("btn-settings");
        _btnQuit = root.Q<GameButton>("btn-quit");

        // Kiểm tra lỗi để đảm bảo không bị NullReferenceException
        if (_btnPlay == null) Debug.LogError("MainMenuController: Không tìm thấy 'btn-play'. Hãy kiểm tra Name trong UI Builder!");
        if (_btnSettings == null) Debug.LogWarning("MainMenuController: Không tìm thấy 'btn-settings'.");
        if (_btnQuit == null) Debug.LogWarning("MainMenuController: Không tìm thấy 'btn-quit'.");

        // 3. Đăng ký sự kiện Click
        if (_btnPlay != null) _btnPlay.clicked += OnPlayClicked;
        if (_btnSettings != null) _btnSettings.clicked += OnSettingsClicked;
        if (_btnQuit != null) _btnQuit.clicked += OnQuitClicked;
    }
    
    private void OnDisable()
    {
        // Hủy đăng ký sự kiện để tránh rò rỉ bộ nhớ
        if (_btnPlay != null) _btnPlay.clicked -= OnPlayClicked;
        if (_btnSettings != null) _btnSettings.clicked -= OnSettingsClicked;
        if (_btnQuit != null) _btnQuit.clicked -= OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        Debug.Log("Đang chuyển sang Scene: Experiment");
        SceneManager.LoadScene("Experiment");
    }

    private void OnSettingsClicked()
    {
        // 4. CHỈNH SỬA: Truy cập trực tiếp qua Singleton Instance
        // Cách này an toàn hơn việc gán biến trong Start vì SettingsController có thể chưa khởi tạo xong
        if (SettingsController.instance != null)
        {
            SettingsController.instance.DisplaySetting();
        }
        else
        {
            Debug.LogError("SettingsController Instance chưa được khởi tạo! Hãy đảm bảo nó có trong Scene.");
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("Thoát ứng dụng...");
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}