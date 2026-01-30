using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
   [SerializeField] public Player player;
   private InputSystem_Actions inputHandler;
   public static GameManager instance; 
   void Start()
   {
       instance = this;
       inputHandler = new InputSystem_Actions();
       inputHandler.Enable();
       inputHandler.Player.Enable();
       inputHandler.Player.Interact.Enable();
       inputHandler.Player.Interact.performed += player.Interact;
   }

   private void OnDestroy()
   {
       instance = null;
   }


   // private void OnEnable()
    // {
    //     inputHandler.Enable();
    // }
    //
    // private void OnDisable()
    // {
    //     inputHandler.Disable();
    // }
    
    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = inputHandler.Player.Move.ReadValue<Vector2>();
        player.Move(moveValue);
    }
}
