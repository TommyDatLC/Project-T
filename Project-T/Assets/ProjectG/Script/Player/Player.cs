using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float move_speed = 10.0f;
    [SerializeField] private float return_threshold = 1.5f;

    [Header("Visuals")]
    [SerializeField] private Sprite default_sprite;
    [SerializeField] private Sprite carrying_sprite;

    // State Logic
    private bool is_active = false;
    private bool is_carrying = false;
    private bool is_locked = false;

    // Internal References
    private SpriteRenderer sprite_renderer;
    private GameManager game_manager;
    private Transform target_slot;

    private void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void SetupPlayer(GameManager manager, Transform slot)
    {
        game_manager = manager;
        target_slot = slot;
        sprite_renderer.sprite = default_sprite;
    }

    public void SetActiveState(bool state)
    {
        is_active = state;
    }

    public void MovePlayer(Vector2 move_value)
    {
        if (!is_active || is_locked) return;

        // Applying your specific inverted-X movement logic
        Vector3 movement = new Vector3(-move_value.x, move_value.y, 0);
        transform.position += movement * move_speed * Time.deltaTime;

        // Check if player returned to spawn (center) while carrying an item
        if (is_carrying && Vector2.Distance(transform.position, Vector2.zero) < return_threshold)
        {
            lock_to_face();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!is_active || is_locked || is_carrying) return;

        // Check if the collided object has the Item script
        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            PickUpItem(item);
        }
    }

    private void PickUpItem(Item item)
    {
        is_carrying = true;
        sprite_renderer.sprite = carrying_sprite;
        
        // Notify Manager to toggle indicator for this player
        game_manager.OnItemPickedUp();
        
        Destroy(item.gameObject);
    }

    private void lock_to_face()
    {
        is_locked = true;
        is_active = false;
        
        // Snap to the specific slot (Eye/Nose/Mouth)
        transform.position = target_slot.position;
        
        // Notify Manager that turn is finished
        game_manager.OnItemPickedUp();
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        // Placeholder for any specific interaction logic if needed later
    }
}