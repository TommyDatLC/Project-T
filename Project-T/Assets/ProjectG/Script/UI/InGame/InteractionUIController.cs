using System;
using System.Collections.Generic;
using System.Linq; // Để dùng Last(), First()
using UnityEngine;
using UnityEngine.UIElements;
using Script;

public class InteractionUIController : MonoBehaviour
{
    UIDocument uiDocument;
    Player playerContext;

    private VisualElement _container;
    private VisualElement _root;
    private List<Button> _currentButtons = new List<Button>();

    void Start()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        _root = uiDocument.rootVisualElement;
        playerContext = GameManager.instance.GetCurrentPlayer();
        playerContext.onMeetInteraction = RenderButtonInteraction;
        GameManager.instance.OnChangePlayer += () =>
        {
            Debug.Log("change player");
            playerContext = GameManager.instance.GetCurrentPlayer();
            playerContext.onMeetInteraction = RenderButtonInteraction;
        };
        
        
        _container = _root.Q<VisualElement>("interaction-list");

        // Đăng ký sự kiện bàn phím trên toàn bộ Root
        _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void RenderButtonInteraction(List<(string action_name, Action<Player> action)> actions)
{
    Debug.Log("RenderButtonInteraction");
    if (actions == null || actions.Count == 0)
    {
        HideInteraction();
        return;
    }

    if (_container != null)
    {
        _container.Clear();
        _currentButtons.Clear();
        _container.style.visibility = Visibility.Visible;

        foreach (var item in actions)
        {
            // 1. Tạo Container A (Hàng ngang)
            VisualElement itemContainer = new VisualElement();
            itemContainer.AddToClassList("interaction-item");

            // 2. Tạo Chữ F bên trái
            VisualElement fBox = new VisualElement();
            fBox.AddToClassList("focus-indicator");
            fBox.Add(new Label("F"));

            // 3. Tạo Nút bên phải
            Button btn = new Button();
            btn.text = item.action_name;
            btn.AddToClassList("interaction-btn");
            btn.focusable = true; // Để có thể dùng phím mũi tên

            // Xử lý Action
            if (item.action == null) {
                btn.SetEnabled(false);
            } else {
                btn.clicked += () => item.action.Invoke(playerContext);
            }

            // 4. Lắp ráp: Cho F và Nút vào Container A
            itemContainer.Add(fBox);
            itemContainer.Add(btn);

            // 5. Cho Container A vào danh sách tổng
            _container.Add(itemContainer);
            
            // Lưu lại danh sách nút để điều khiển phím mũi tên
            _currentButtons.Add(btn);
        }

        // Focus vào nút đầu tiên
        if (_currentButtons.Count > 0) _currentButtons[0].Focus();
    }
}
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (_container.style.visibility == Visibility.Hidden || _currentButtons.Count == 0) return;

        // Lấy nút hiện tại đang focus
        VisualElement focusedElement = _root.focusController.focusedElement as VisualElement;
        int currentIndex = _currentButtons.IndexOf(focusedElement as Button);

        switch (evt.keyCode)
        {
            case KeyCode.DownArrow:
                // Nếu chưa focus cái nào hoặc là cái cuối cùng -> Về cái đầu
                // Dùng cái này nếu KHÔNG muốn xoay vòng
                int nextIndex = Mathf.Clamp(currentIndex + 1, 0, _currentButtons.Count - 1);
                _currentButtons[nextIndex].Focus();
                evt.StopPropagation(); // Ngăn sự kiện truyền đi tiếp
                break;

            case KeyCode.UpArrow:
                // Nếu là cái đầu -> Xuống cái cuối
                int prevIndex = Mathf.Clamp(currentIndex - 1, 0, _currentButtons.Count - 1);
                _currentButtons[prevIndex].Focus();
                evt.StopPropagation();
                break;

            case KeyCode.F:
                // Nếu đang focus vào một nút hợp lệ, giả lập click nút đó
                if (currentIndex != -1 && _currentButtons[currentIndex].enabledSelf)
                {
                    // Thực hiện hành động click
                    // UI Toolkit gọi bằng ExecuteClick
                    using (var submitEvt = NavigationSubmitEvent.GetPooled())
                    {
                        submitEvt.target = _currentButtons[currentIndex];
                        _currentButtons[currentIndex].SendEvent(submitEvt);
                    }
                    Debug.Log("Đã bấm phím F để tương tác");
                }
                break;
        }
    }

    public void HideInteraction()
    {
        if (_container != null)
        {
            _container.Clear();
            _currentButtons.Clear();
            _container.style.visibility = Visibility.Hidden;
        }
    }

    // Giữ nguyên hàm MoveMenuToWorldPosition của bạn...
}