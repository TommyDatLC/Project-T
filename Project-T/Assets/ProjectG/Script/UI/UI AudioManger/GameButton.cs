using UnityEngine.UIElements;
using UnityEngine;

public class GameButton : Button
{
    public new class UxmlFactory : UxmlFactory<GameButton, UxmlTraits> { }

    public GameButton()
    {
        // Thêm class để nhận style từ USS
        this.AddToClassList("game-button");

        // Đăng ký âm thanh trực tiếp vào sự kiện clicked của class cha
        this.clicked += PlayClickSound;
        
        // Đăng ký hover qua Callback
        this.RegisterCallback<MouseEnterEvent>(OnHoverEnter);
    }

    private void PlayClickSound()
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayClick();
        }
    }

    private void OnHoverEnter(MouseEnterEvent evt)
    {
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayHover();
        }
    }
}