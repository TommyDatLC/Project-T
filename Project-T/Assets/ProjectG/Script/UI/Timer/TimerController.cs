using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading;
using System.Threading.Tasks;

public class TimerController : MonoBehaviour
{
    public static TimerController Instance;

    [Header("Timer Settings")]
    [SerializeField] private float maxTime = 120f; // 2 phút
    private float _currentTime;
    
    [Header("UI References")]
    private UIDocument _uiDoc;
    private VisualElement _timerBar;
    private Label _timerLabel;
    
    private CancellationTokenSource _cts;
    private bool _isPaused = false;

    private void Awake()
    {
        // Khởi tạo Singleton để các script Entity có thể gọi AddBonusTime()
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        _uiDoc = GetComponent<UIDocument>();
        var root = _uiDoc.rootVisualElement;

        // Tìm các phần tử theo Name trong UXML
        _timerBar = root.Q<VisualElement>("timer-bar");
        _timerLabel = root.Q<Label>("timer-label");
        
        _currentTime = maxTime;
        StartTimerTask();
    }

    private void OnDisable()
    {
        StopTimerTask();
    }

    private async void StartTimerTask()
    {
        StopTimerTask(); // Đảm bảo không có task cũ chạy đè
        _cts = new CancellationTokenSource();

        try
        {
            while (_currentTime > 0)
            {
                // Kiểm tra hủy task (khi out game hoặc disable)
                if (_cts.Token.IsCancellationRequested) break;

                if (!_isPaused)
                {
                    _currentTime -= 0.1f; // Giảm 100ms mỗi vòng lặp
                    UpdateTimerUI();
                }

                // Tương đương Task.Delay trong thư viện System.Threading.Tasks
                await Task.Delay(100, _cts.Token);
            }

            if (_currentTime <= 0)
            {
                _currentTime = 0;
                UpdateTimerUI();
                OnTimerEnd();
            }
        }
        catch (TaskCanceledException) { /* Task bị hủy an toàn */ }
    }

    private void UpdateTimerUI()
    {
        if (_timerBar == null) return;

        // 1. Tính toán phần trăm
        float percent = (_currentTime / maxTime) * 100f;
        _timerBar.style.width = Length.Percent(Mathf.Clamp(percent, 0, 100));

        // 2. Đổi màu theo yêu cầu
        if (percent > 50f)
            _timerBar.style.backgroundColor = new StyleColor(Color.green); // 100% -> 50%
        else if (percent > 20f)
            _timerBar.style.backgroundColor = new StyleColor(Color.yellow); // 50% -> 20%
        else
            _timerBar.style.backgroundColor = new StyleColor(Color.red); // < 20%

        // 3. Cập nhật nhãn thời gian (mm:ss)
        if (_timerLabel != null)
        {
            int minutes = Mathf.FloorToInt(_currentTime / 60);
            int seconds = Mathf.FloorToInt(_currentTime % 60);
            _timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // --- CÁC HÀM PUBLIC ---

    // Gọi hàm này khi Entity nhặt đồ về đích: TimerController.Instance.AddBonusTime();
    public void AddBonusTime()
    {
        float bonus = maxTime * 0.1f; // 10% của 120s = 12s
        _currentTime = Mathf.Min(_currentTime + bonus, maxTime);
        UpdateTimerUI();
        Debug.Log("Bonus +10% thời gian!");
    }

    public void TogglePause(bool pause)
    {
        _isPaused = pause;
    }

    private void StopTimerTask()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("HẾT GIỜ! Game Over.");
        // Gọi logic Game Over của bạn tại đây
    }

    private void OnApplicationQuit()
    {
        StopTimerTask();
    }
}