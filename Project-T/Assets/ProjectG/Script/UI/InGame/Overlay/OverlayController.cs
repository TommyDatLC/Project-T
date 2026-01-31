using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectG.Script.UI.InGame.Overlay
{
    public class OverlayController : MonoBehaviour
    {
        private VisualElement _background;
        private Label _messageLabel;
        private Coroutine _fadeCoroutine;
      
        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _background = root.Q<VisualElement>("Background");
            _messageLabel = root.Q<Label>("MessageLabel");

            // Đảm bảo ban đầu ẩn đi
            _background.RemoveFromClassList("visible");
        }

        // Hàm Show theo yêu cầu
        public void Show(string content, float duration)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeSequence(content, duration));
        }

        private IEnumerator FadeSequence(string content, float duration)
        {
            // Gán nội dung
            _messageLabel.text = content;

            // Fade In bằng cách thêm class .visible
            _background.AddToClassList("visible");

            // Đợi n giây
            yield return new WaitForSeconds(duration);

            // Fade Out bằng cách xóa class .visible
            _background.RemoveFromClassList("visible");
        
            _fadeCoroutine = null;
        }
    }
}