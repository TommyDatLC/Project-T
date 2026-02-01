using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
    private VisualElement _overlay;
    private Button _openBtn;
    private Button _closeBtn;

    void OnEnable()
    {
        // 1. Lấy Root của UI
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 2. Tìm các thành phần theo tên đã đặt trong UXML
        _overlay = root.Q<VisualElement>("Tutorial_Overlay");
        _openBtn = root.Q<Button>("Btn_OpenTutorial");
        _closeBtn = root.Q<Button>("Btn_CloseTutorial");

        // 3. Đăng ký sự kiện Click
        _openBtn.clicked += () => {
            _overlay.style.display = DisplayStyle.Flex; // Hiện bảng
            Time.timeScale = 0; // Tạm dừng game
        };

        _closeBtn.clicked += () => {
            _overlay.style.display = DisplayStyle.None; // Ẩn bảng
            Time.timeScale = 1; // Chạy tiếp game
        };
    }
}