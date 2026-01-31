using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental; // Cần cho Animation

public class GridLoadingController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument _uiDoc;

    public static GridLoadingController Instance;
    [Header("Grid Settings")]
    [SerializeField] private float _gridSpacing = 50f;
    [SerializeField] private float _minThickness = 1f;
    [SerializeField] private float _thicknessAmplitude = 2f;
    [SerializeField] private float _speed = 4f;

    [Header("Animation Settings")]
    [SerializeField] private float _animDuration = 0.5f; // Thời gian slide/scale
    [SerializeField] private float _fadeDuration = 0.5f; // Thời gian fade in/out tổng

    // Các phần tử UI
    private VisualElement _root; // Cần cái này để fade toàn bộ màn hình
    private VisualElement _mask;
    private Label _label;
    private GridElement _gridDrawer;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        var rootElement = _uiDoc.rootVisualElement;

        // 1. Tìm các phần tử
        _root = rootElement.Q<VisualElement>("Root"); // Đảm bảo trong UXML root có tên là "Root"
        _mask = rootElement.Q<VisualElement>("Mask");
        _label = rootElement.Q<Label>("Label");

        // 2. Tạo Grid Element
        _gridDrawer = new GridElement(_gridSpacing, _minThickness, _thicknessAmplitude, _speed);
        _gridDrawer.style.width = Length.Percent(100);
        _gridDrawer.style.height = Length.Percent(100);
        _mask.Add(_gridDrawer);

        // Mặc định ẩn root đi để chờ TransitionIn
        _root.style.opacity = 0;
    }

    // --- LOGIC TRANSITION ---

    [ContextMenu("Transition in")]
    public void TransitionIn()
    {
        // QUAN TRỌNG: Đảm bảo object được bật lên trước khi làm bất cứ gì
        gameObject.SetActive(true);

        int slideDurationMs = (int)(_animDuration * 1000);
        int fadeDurationMs = (int)(_fadeDuration * 1000);

        // BƯỚC 1: Setup trạng thái ban đầu
        _root.style.opacity = 0;           
        _mask.style.height = Length.Percent(0); 
        _label.style.scale = new Scale(Vector3.one * 1.25f); 
        _label.style.opacity = 1;          

        // BƯỚC 2: Fade In Root (0 -> 1)
        _root.experimental.animation
            .Start(0f, 1f, fadeDurationMs, (e, v) => e.style.opacity = v)
            .Ease(Easing.OutSine);

        // BƯỚC 3: Sau khi Fade xong thì mới chạy Slide Mask & Scale Text
        _root.schedule.Execute(() => 
        {
            // Mask Slide Down
            _mask.experimental.animation
                .Start(0f, 100f, slideDurationMs, (e, v) => e.style.height = Length.Percent(v))
                .Ease(Easing.OutCubic);

            // Text Scale In
            _label.experimental.animation
                .Start(1.25f, 1f, slideDurationMs, (e, v) => e.style.scale = new Scale(Vector3.one * v))
                .Ease(Easing.OutCubic);

        }).StartingIn(fadeDurationMs);
 
    }

    [ContextMenu("Transition out")]
    public void TransitionOut()
    {
        int slideDurationMs = (int)(_animDuration * 1000);
        int fadeDurationMs = (int)(_fadeDuration * 1000);

        // BƯỚC 1: Chạy Slide Mask Up & Scale Text Out trước
        _mask.experimental.animation
            .Start(100f, 0f, slideDurationMs, (e, v) => e.style.height = Length.Percent(v))
            .Ease(Easing.InCubic);

        _label.experimental.animation
            .Start(1f, 1.25f, slideDurationMs, (e, v) => e.style.scale = new Scale(Vector3.one * v))
            .Ease(Easing.InCubic);

        // BƯỚC 2: Sau khi Slide xong -> Chạy Fade Out
        _root.schedule.Execute(() => 
        {
            _root.experimental.animation
                .Start(1f, 0f, fadeDurationMs, (e, v) => e.style.opacity = v)
                .Ease(Easing.InSine);

            // BƯỚC 3 (MỚI): Sau khi Fade Out xong -> Disable Object
            // Lồng thêm một schedule nữa để đợi fadeDurationMs kết thúc
            _root.schedule.Execute(() => 
            {
                gameObject.SetActive(false);
            }).StartingIn(fadeDurationMs);

        }).StartingIn(slideDurationMs); 
    }

    // --- TEST INPUT ---
 
    // --- CLASS VẼ LƯỚI GIỮ NGUYÊN ---
    public class GridElement : VisualElement
    {
        float spacing, minThick, ampThick, speed;
        public GridElement(float spacing, float min, float amp, float speed)
        {
            this.spacing = spacing; this.minThick = min; this.ampThick = amp; this.speed = speed;
            generateVisualContent += DrawGrid;
            schedule.Execute(() => MarkDirtyRepaint()).Every(16);
        }
        void DrawGrid(MeshGenerationContext context)
        {
            var paint = context.painter2D;
            float time = Time.realtimeSinceStartup;
            float currentThickness = minThick + ampThick * Mathf.Sin(time * speed);
            if (currentThickness < 0.1f) currentThickness = 0.1f;

            paint.lineWidth = currentThickness;
            paint.strokeColor = new Color(1f, 1f, 1f, 0.15f);
            paint.BeginPath();
            float width = contentRect.width;
            float height = contentRect.height;
            for (float x = 0; x <= width; x += spacing) { paint.MoveTo(new Vector2(x, 0)); paint.LineTo(new Vector2(x, height)); }
            for (float y = 0; y <= height; y += spacing) { paint.MoveTo(new Vector2(0, y)); paint.LineTo(new Vector2(width, y)); }
            paint.Stroke();
        }
    }
}