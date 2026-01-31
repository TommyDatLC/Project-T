using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Script;
using Script.Item;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float interaction_radius = 1.0f;
    public Action<List<(string, Action<Player>)>> onMeetInteraction;
    [Header("Sprite")]
    SpriteRenderer spriteRenderer;
    Animator ani;

    public Action on_player_locked;
    [SerializeField] private Camera cam;
    Rigidbody2D rb;
    private Sprite pickingItem_sprite;
    bool is_locked = false;
    private float start_runtime_counting, final_time;
    bool is_Counter_Started = false;
    [SerializeField] ReturnPoint return_point;
    public bool isLoss; // Người chơi đã thua hay chưa
    public void setLock(bool locked)
    {
        is_locked = locked;

        // Kiểm tra an toàn trước khi Invoke
        if (locked)
            on_player_locked?.Invoke();

        // Kiểm tra rb và cam có tồn tại không trước khi dùng
        if (cam != null) cam.gameObject.SetActive(!locked);

        if (rb == null) rb = GetComponent<Rigidbody2D>(); // Tự tìm lại nếu chưa có
        if (rb != null) rb.simulated = !locked;
    }

    public void StopTime()
    {
        final_time = Time.time - start_runtime_counting;
        setLock(true);
        Debug.Log($"final time is: {final_time}");
        is_Counter_Started = false;

    }

    void StartTime()
    {
        start_runtime_counting = Time.time;
        is_Counter_Started = true;

    }
    [Header("Item")]
    Item pickingItem = null;
    [SerializeField] private Transform viTriDo;
    public void SetItem(Item item)
    {
        if (pickingItem == null)
            pickingItem = item;
        item.transform.SetParent(viTriDo, false);
        item.transform.localPosition = Vector3.zero;
        StartTime();
    }

    public bool isHoldingItem()
    {
        return pickingItem != null;
    }
    InteractableObject CheckNearbyFieldInteractable()
    {
        Vector2 detectionCenter = transform.position;
        // Returns an array of ALL colliders within the circle
        Collider2D hitColliders = Physics2D.OverlapCircle(detectionCenter, interaction_radius, interactable_layer_mask);
        if (hitColliders == null) return null;
        InteractableObject interactableObjectObj = hitColliders.GetComponent<InteractableObject>();
        Debug.Log("check interaction");
        return interactableObjectObj;
    }
    public void Move(Vector2 move_value)
    {
        if (is_locked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        // 1. Tính toán hướng dựa trên logic của bạn (giữ nguyên cái -move_value.x của bạn)
        float moveX = -move_value.x;// A : MoveX = 1 D: MoveX = -1 else 0
        float moveY = move_value.y;// W : MoveY = 1 S: MoveY = -1 else 0

        // 2. Thực hiện di chuyển bằng Rigidbody2D thay vì transform
        // Chúng ta giữ nguyên vận tốc hiện tại nhưng thay đổi hướng X và Y
        rb.linearVelocity = new Vector2(moveX * speed, moveY * speed);

        // 3. Xử lý lật mặt (Flip)
        if (moveX > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private InteractableObject _oldInteractableObject;
    void meetInteractable()
    {

        InteractableObject i = CheckNearbyFieldInteractable();
        if (i == _oldInteractableObject)
            return;
        Debug.Log(onMeetInteraction == null);
        onMeetInteraction?.Invoke(i == null ? null : i.GetList());
        Debug.Log("meet interaction");
        if (i != null)
            i.OnInteractionChange = () =>
            {
                onMeetInteraction?.Invoke(i == null ? null : i.GetList());
            };
        _oldInteractableObject = i;
    }
    public void Interact(InputAction.CallbackContext ctx)
    {

    }
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        return_point.Init(this);
    }
    [SerializeField] LayerMask interactable_layer_mask;


    // Update is called once per frame


    public float getCurrentTime()
    {
        if (!is_Counter_Started)
            return -1;
        return Time.time - start_runtime_counting;
    }
    void FixedUpdate()
    {

        if (is_locked)
            return;
        meetInteractable();
        UpdateAnimator();
    }

    public void RotateItem()
    {
        pickingItem.Rotate();
    }

    [SerializeField] private float fadeDuration;

    public void HideAndDisable()
    {
        // Chạy Coroutine để xử lý việc mờ dần
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        Color startColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Tính toán độ Alpha (từ 1 giảm về 0)
            float newAlpha = Mathf.Lerp(startColor.a, 0, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            yield return null; // Đợi đến frame tiếp theo
        }

        // Đảm bảo alpha bằng 0 tuyệt đối
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0);

        // Sau khi mờ hẳn thì disable GameObject hoặc Component

        // Hoặc spriteRenderer.enabled = false;
    }
    void UpdateAnimator()
    {
        bool isHorizontal = Math.Abs(rb.linearVelocity.x) > 0.1f;
        bool isPlayerUp = rb.linearVelocity.y > 0;
        bool isPlayerDown = rb.linearVelocity.y < 0;

        ani.SetBool("IsPlayerHorizontal", isHorizontal);
        ani.SetBool("IsPlayerUp", isPlayerUp);
        ani.SetBool("IsPlayerDown", isPlayerDown);
        ani.SetBool("IsNhatDo", isHoldingItem());
    }
}