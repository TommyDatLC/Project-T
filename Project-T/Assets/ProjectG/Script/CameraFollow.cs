using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    public SpriteRenderer mapBoundsSprite;

    private Camera cam;
    private float camHeight;
    private float camWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        mapBoundsSprite = GameManager.instance.game_map;
    } // Kéo Sprite cái nền vào đây

    void LateUpdate()
    {
        if (target == null || mapBoundsSprite == null) return;

        Bounds b = mapBoundsSprite.bounds;
        float camHeight = GetComponent<Camera>().orthographicSize;
        float camWidth = camHeight * GetComponent<Camera>().aspect;

        // Tự động tính giới hạn từ Sprite
        float minX = b.min.x + camWidth;
        float maxX = b.max.x - camWidth;
        float minY = b.min.y + camHeight;
        float maxY = b.max.y - camHeight;

        float posX = Mathf.Clamp(target.position.x, minX, maxX);
        float posY = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(posX, posY, -10);
    }
}