using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    private VisualElement _root;
    
    // Thay đổi kiểu dữ liệu tại đây
    private GameButton _continueBtn;
    private GameButton _exitBtn;
    private GameButton _openPauseBtn;
    
    private bool _isPaused = false;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var rootElement = uiDocument.rootVisualElement;

        // Truy vấn với kiểu GameButton
        _root = rootElement.Q<VisualElement>("Root");
        _openPauseBtn = rootElement.Q<GameButton>("OpenPauseBtn");
        _continueBtn = rootElement.Q<GameButton>("ContinueButton");
        _exitBtn = rootElement.Q<GameButton>("ExitButton");

        // Đăng ký sự kiện (GameButton kế thừa từ Button nên .clicked vẫn dùng bình thường)
        if (_openPauseBtn != null) _openPauseBtn.clicked += TogglePause;
        if (_continueBtn != null) _continueBtn.clicked += TogglePause;
        if (_exitBtn != null) _exitBtn.clicked += ExitGame;

        ResumeGame();
    }

    void OnDisable()
    {
        // Hủy đăng ký để tránh lỗi bộ nhớ
        if (_openPauseBtn != null) _openPauseBtn.clicked -= TogglePause;
        if (_continueBtn != null) _continueBtn.clicked -= TogglePause;
        if (_exitBtn != null) _exitBtn.clicked -= ExitGame;
    }

    void Update()
    {
        // Cách viết mới cho Input System Package
        if (Keyboard.current.escapeKey.wasPressedThisFrame) 
        {
            TogglePause();
            
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused) PauseGame();
        else ResumeGame();
    }

    private void PauseGame()
    {
        _isPaused = true;
        if (_root != null) _root.style.display = DisplayStyle.Flex;
        if (_openPauseBtn != null) _openPauseBtn.style.display = DisplayStyle.None;
        
        Time.timeScale = 0f;

        // Dừng Task thời gian nếu có
        if (TimerController.Instance != null)
            TimerController.Instance.TogglePause(true);
    }

    private void ResumeGame()
    {
        _isPaused = false;
        if (_root != null) _root.style.display = DisplayStyle.None;
        if (_openPauseBtn != null) _openPauseBtn.style.display = DisplayStyle.Flex;
        
        Time.timeScale = 1f;

        // Chạy lại Task thời gian
        if (TimerController.Instance != null)
            TimerController.Instance.TogglePause(false);
    }

    private async void ExitGame()
    {
        Time.timeScale = 1f;
        GridLoadingController.Instance.TransitionIn();
        
        SceneManager.LoadSceneAsync("MainMenu");
    }
}