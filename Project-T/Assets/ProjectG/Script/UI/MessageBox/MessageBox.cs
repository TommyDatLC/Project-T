using System;
using UnityEngine;
using UnityEngine.UIElements;
[RequireComponent(typeof(UIDocument))]
public class MessageBox : MonoBehaviour
{
    // Singleton để gọi static
    private static MessageBox _instance;

 
    private UIDocument _uiDoc;

    // Các phần tử UI
    private VisualElement _root;
    private VisualElement _overlay;
    private VisualElement _msgBox;
    private Label _lblContent;
    private Button _btnOk;
    private Button _btnClose;

    // Callbacks
    private Action _onOkAction;
    private Action _onCancelAction;

    private void Awake()
    {
        // Khởi tạo Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // Giữ lại khi chuyển scene (tuỳ chọn)
    }

    private void Start()
    {
        if (_uiDoc == null) _uiDoc = GetComponent<UIDocument>();
        _root = _uiDoc.rootVisualElement;

        // Query các phần tử
        _overlay = _root.Q<VisualElement>("msg-overlay");
        _msgBox = _root.Q<VisualElement>("msg-box");
        _lblContent = _root.Q<Label>("lbl-content");
        _btnOk = _root.Q<Button>("btn-ok");
        _btnClose = _root.Q<Button>("btn-close");

        // Đăng ký sự kiện
        _btnOk.clicked += OnOkClicked;
        _btnClose.clicked += OnCancelClicked;
        
        // Đảm bảo ẩn lúc đầu
        _overlay.style.display = DisplayStyle.None;
    }

    // --- STATIC METHOD ---
    public static void Show(string msg, Action ok = null, Action btn_cancel = null)
    {
        if (_instance == null)
        {
            Debug.LogError("Chưa có MessageBox instance trong scene!");
            return;
        }
        
        _instance.ShowInternal(msg, ok, btn_cancel);
    }

    // --- INTERNAL LOGIC ---
    private void ShowInternal(string msg, Action ok, Action cancel)
    {
        // 1. Setup dữ liệu
        _lblContent.text = msg;
        _onOkAction = ok;
        _onCancelAction = cancel;

        // 2. Bật hiển thị (Display Flex)
        _overlay.style.display = DisplayStyle.Flex;

        // 3. Kích hoạt Animation (Sau 1 frame nhỏ để CSS nhận diện thay đổi display)
        _overlay.schedule.Execute(() =>
        {
            _overlay.AddToClassList("overlay--show"); // Fade in nền
            _msgBox.AddToClassList("msg-box--show");  // Scale up hộp
        }).StartingIn(10); // Delay 10ms
    }

    private void OnOkClicked()
    {
        _onOkAction?.Invoke();
        CloseBox();
    }

    private void OnCancelClicked()
    {
        _onCancelAction?.Invoke();
        CloseBox();
    }

    private void CloseBox()
    {
        // 1. Xóa class animation để kích hoạt hiệu ứng đóng (Scale down)
        _overlay.RemoveFromClassList("overlay--show");
        _msgBox.RemoveFromClassList("msg-box--show");

        // 2. Đợi animation chạy xong (0.3s = 300ms) rồi mới ẩn hoàn toàn
        _overlay.schedule.Execute(() =>
        {
            _overlay.style.display = DisplayStyle.None;
            
            // Reset actions để tránh memory leak
            _onOkAction = null;
            _onCancelAction = null;
            
        }).StartingIn(300); // Thời gian khớp với transition trong CSS
    }
}