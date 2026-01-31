using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoEnableTransition
{
    // Hàm khởi tạo này sẽ được gọi ngay khi Unity load hoặc compile code
    static AutoEnableTransition()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // Kiểm tra nếu người dùng vừa nhấn nút Play (trước khi cảnh bắt đầu chạy)
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EnableTransitionObject();
        }
    }

    private static void EnableTransitionObject()
    {
        // Tìm đối tượng có tên chính xác như trong ảnh của bạn
        GameObject transitionObj = GameObject.Find("UI_Transition");

        if (transitionObj != null)
        {
            if (!transitionObj.activeSelf)
            {
                transitionObj.SetActive(true);
                Debug.Log("<color=cyan>Editor:</color> Đã tự động kích hoạt <b>UI_Transition</b>.");
            }
        }
        else
        {
            // Nếu không tìm thấy (có thể do nằm trong Prefab chưa unpack hoặc sai tên)
            // Ta thử tìm cả các đối tượng đang bị ẩn
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in allObjects)
            {
                if (go.name == "UI_Transition" && !EditorUtility.IsPersistent(go))
                {
                    go.SetActive(true);
                    Debug.Log("<color=cyan>Editor:</color> Đã tìm thấy và kích hoạt <b>UI_Transition</b> ẩn.");
                    return;
                }
            }
        }
    }
}