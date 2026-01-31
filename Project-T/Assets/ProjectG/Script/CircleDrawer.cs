using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class CircleDrawer : MonoBehaviour
{
    private bool hasStartedDrawing = false;
    [Header("Settings")]
    public Transform centerPoint;
    public float minDistance = 0.1f;
    
    [Header("Result")]
    public float lastScore = 0f;

    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private bool isDrawing = false;
    [SerializeField] private Camera camera_for_drawing;
    private bool isDrawed;
    private GameManager gm;

    public void GameManagerInit(GameManager gm)
    {
        this.gm = gm;
    }
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        // 1. Lấy thông tin chuột từ Input System mới
        var mouse = Mouse.current;
        if (mouse == null) return;

        // 2. Kiểm tra nhấn chuột trái (wasPressedThisFrame thay cho GetMouseButtonDown)
        if (mouse.leftButton.wasPressedThisFrame)
        {
            StartDrawing(mouse.position.ReadValue());
        }

        // 3. Kiểm tra giữ chuột (isPressed thay cho GetMouseButton)
        if (isDrawing && mouse.leftButton.isPressed)
        {
            ContinuingDrawing(mouse.position.ReadValue());
        }

        // 4. Kiểm tra thả chuột (wasReleasedThisFrame thay cho GetMouseButtonUp)
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            StopDrawing();
        }
    }

    void StartDrawing(Vector2 screenPos)
    {
        if (isDrawed)
            return;
        points.Clear();
        lineRenderer.positionCount = 0;
        isDrawing = true;
        AddPoint(GetWorldPosition(screenPos));
    }

    void ContinuingDrawing(Vector2 screenPos)
    {
        if (isDrawed)
            return;
        Vector3 currentPos = GetWorldPosition(screenPos);
        if (points.Count == 0 || Vector3.Distance(currentPos, points.Last()) > minDistance)
        {
            AddPoint(currentPos);
        }
    }
    // Dang ki event nay de nhan diem ve
    public Action<float> onStopDrawing;
    async void StopDrawing()
    {
        if (isDrawed)
            return;
        Vector3 oldScale = transform.localScale;
        isDrawing = false;
        lastScore = CalculateCircleScore();
        Debug.Log($"Độ chính xác: {lastScore:F2}%");
        if (lastScore > 10f)
            isDrawed = true;
        
        transform.DOScale(oldScale * 2f, 0.5f).SetEase(Ease.OutQuad);
        await Task.Delay(500);
        transform.DOScale(oldScale , 0.5f).SetEase(Ease.InQuad);
        onStopDrawing?.Invoke(lastScore);
        gm.HideAllPlayer();
    }
    
    void AddPoint(Vector3 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
    }

    float CalculateCircleScore()
    {
        if (points.Count < 20 || centerPoint == null) return 0f;

        List<float> distances = points.Select(p => Vector3.Distance(p, centerPoint.position)).ToList();
        float avgR = distances.Average();
        
        // Tính độ lệch chuẩn
        float sumSq = distances.Sum(d => Mathf.Pow(d - avgR, 2));
        float stdDev = Mathf.Sqrt(sumSq / distances.Count);
        
        // Độ chính xác dựa trên độ biến thiên của bán kính
        float precision = Mathf.Max(0, 1f - (stdDev / avgR * 2f));

        // Kiểm tra độ phủ góc (Sử dụng Atan2)
        float totalAngle = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 dir1 = (Vector2)(points[i] - centerPoint.position);
            Vector2 dir2 = (Vector2)(points[i+1] - centerPoint.position);
            totalAngle += Vector2.SignedAngle(dir1, dir2);
        }

        // Nếu vẽ đủ 1 vòng, trị tuyệt đối totalAngle sẽ gần 360 độ
        float coverage = Mathf.Clamp01(Mathf.Abs(totalAngle) / 300f);

        return precision * coverage * 100f;
    }

    Vector3 GetWorldPosition(Vector2 screenPos)
    {
        // Chuyển tọa độ màn hình sang tọa độ thế giới (Z là khoảng cách từ cam tới vật thể)
        float zDistance = Mathf.Abs(camera_for_drawing.transform.position.z);
        return camera_for_drawing.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
    }
}