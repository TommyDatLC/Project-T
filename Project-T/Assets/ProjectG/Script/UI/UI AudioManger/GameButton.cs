using UnityEngine.UIElements;
using UnityEngine;

// Thêm attribute này để Unity tự động tạo Factory cho UXML
[UxmlElement]
public partial class GameButton : Button
{
    // Bạn có thể xóa dòng UxmlFactory cũ nếu đang dùng Unity 2023/Unity 6 trở lên
    // public new class UxmlFactory : UxmlFactory<GameButton, UxmlTraits> { }

    public GameButton()
    {
        this.AddToClassList("game-button");
        this.clicked += PlayClickSound;
        this.RegisterCallback<MouseEnterEvent>(OnHoverEnter);
    }

    private void PlayClickSound()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayClick();
    }

    private void OnHoverEnter(MouseEnterEvent evt)
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayHover();
    }
}