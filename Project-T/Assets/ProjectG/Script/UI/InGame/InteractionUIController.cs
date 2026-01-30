using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Script; // Namespace chứa class Interactable của bạn

public class InteractionUIController : MonoBehaviour
{

    UIDocument uiDocument;
    Player playerContext; // Tham chiếu tới Player hiện tại để thực hiện Action
   
    private VisualElement _container;
    private VisualElement _root;

    void Start()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        _root = uiDocument.rootVisualElement;
        playerContext = GameManager.instance.player;
        playerContext.onMeetInteraction += RenderButtonInteraction;
        // Lấy container từ UXML
        _container = _root.Q<VisualElement>("interaction-list");
    }
    private void OnEnable()
    {
        
    }

    /// <summary>
    /// Hàm Render danh sách nút dựa trên dữ liệu từ Interactable
    /// </summary>
    /// <param name="actions">List lấy từ Interactable.GetList()</param>
    /// <summary>
    /// Hàm Render danh sách nút dựa trên dữ liệu từ Interactable
    /// </summary>
    /// <param name="actions">List lấy từ Interactable.GetList()</param>
    public void RenderButtonInteraction(List<(string action_name, Action<Player> action)> actions)
    {
        // 1. XỬ LÝ LIST NULL: Nếu list null hoặc rỗng -> Tắt toàn bộ UI
        if (actions == null || actions.Count == 0)
        {
            HideInteraction();
            return;
        }

        // Nếu có dữ liệu, đảm bảo container được hiện lên
        if (_container != null)
        {
            _container.Clear(); // Xóa các nút cũ
            _container.style.visibility = Visibility.Visible;
           
            // 2. DUYỆT QUA TỪNG PHẦN TỬ
            foreach (var item in actions)
            {
                Button btn = new Button();
                btn.text = item.action_name;
                btn.AddToClassList("interaction-btn");

                // --- XỬ LÝ PHẦN TỬ NULL ---
                // Kiểm tra xem Action bên trong có null không
                if (item.action == null)
                {
                    // Disable nút: Nút sẽ bị mờ đi và không click được
                    btn.SetEnabled(false); 
                    Debug.Log("Disable");
                    // (Tùy chọn) Bạn có thể đổi text để báo hiệu, ví dụ:
                    // btn.text = $"{item.action_name} (Locked)";
                }
                else
                {
                    // Nếu có action, bật nút và gán sự kiện click
                    btn.SetEnabled(true);
                    Debug.Log("Enable");
                    btn.RegisterCallback<ClickEvent, VisualElement>((evt, args) =>
                    {
                        item.action.Invoke(playerContext);
                        Debug.Log($"Đã chọn hành động: {item.action_name}");

                    },null);
                }

                _container.Add(btn);
            }
        }
    }

    public void HideInteraction()
    {
        if (_container != null)
        {
            _container.Clear(); // Xóa nút để tiết kiệm bộ nhớ UI
            _container.style.visibility = Visibility.Hidden;
        }
    }

    // Hàm phụ trợ: Di chuyển UI đến vị trí của vật thể (World to Screen)
    // Gọi hàm này trong Update nếu bạn muốn menu bay trên đầu nhân vật
    public void MoveMenuToWorldPosition(Vector3 worldPos, Camera mainCamera)
    {
        if (_container.style.visibility == Visibility.Hidden) return;

        Vector2 screenPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            _container.panel, 
            worldPos, 
            mainCamera
        );

        // Cập nhật vị trí (Offset sang phải một chút cho đẹp)
        _container.style.left = screenPos.x + 50; 
        _container.style.top = screenPos.y - 50;
    }
}