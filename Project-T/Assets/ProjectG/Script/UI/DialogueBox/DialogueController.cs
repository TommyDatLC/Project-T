using UnityEngine;
using UnityEngine.UIElements;

public class DialogueFollow : MonoBehaviour
{
    public Transform targetCharacter; 
    public Vector3 offset = new Vector3(0, 2.5f, 0); 

    private VisualElement _container;
    private Camera _mainCamera;

    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;
        // Đảm bảo tên "dialogue-container" khớp chính xác với tên trong UI Builder
        _container = root.Q<VisualElement>("dialogue-container");
        _mainCamera = Camera.main;

        if (_container == null) {
            Debug.LogError("Không tìm thấy VisualElement tên 'dialogue-container' trong UXML!");
        }
    }

    void LateUpdate()
    {
        // Kiểm tra an toàn để tránh lỗi NullReferenceException
        if (targetCharacter == null || _container == null || _mainCamera == null) return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(targetCharacter.position + offset);

        if (screenPos.z < 0)
        {
            _container.style.display = DisplayStyle.None;
            return;
        }
        
        _container.style.display = DisplayStyle.Flex;

        // Chuyển đổi tọa độ sang UI Toolkit (gốc tọa độ trên-trái)
        float x = screenPos.x;
        float y = Screen.height - screenPos.y;

        // Căn lề để tâm hộp thoại nằm giữa nhân vật
        _container.style.left = x - (_container.layout.width / 2);
        _container.style.top = y - _container.layout.height;
    }
}