using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
   [SerializeField] PlayerMovement player;
   private InputSystem_Actions inputHandler;

   void Start()
   {
       inputHandler = new InputSystem_Actions();
       inputHandler.Enable();
       inputHandler.Player.Enable();
       inputHandler.Player.Plant.Enable();
       inputHandler.Player.Plant.performed += OnInputPlanted;
   }

   private void OnInputPlanted(InputAction.CallbackContext ctx)
   {
       player.Plant();
       player.harvest();
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
