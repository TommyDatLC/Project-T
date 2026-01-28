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
       inputHandler.Player.GodOffer.Enable();
       inputHandler.Player.RejectOffer.Enable();
       inputHandler.Player.AcceptOffer.Enable();
       inputHandler.Player.Plant.performed += OnInputPlanted;
       inputHandler.Player.GodOffer.performed += OnGodOfferOnperformed;
       inputHandler.Player.AcceptOffer.performed += AcceptOfferOnperformed ;
       inputHandler.Player.RejectOffer.performed += RejectOfferOnperformed;
   }

   private void RejectOfferOnperformed(InputAction.CallbackContext obj)
   {
       player.AccptOffer =  false;
   }

   void AcceptOfferOnperformed(InputAction.CallbackContext context)
   {
       player.AccptOffer =  true;
   }
   void OnGodOfferOnperformed(InputAction.CallbackContext context)
   {
       player.Offer();
   }
   private void OnInputPlanted(InputAction.CallbackContext ctx)
   {
       player.Plant();
       player.Harvest();
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
