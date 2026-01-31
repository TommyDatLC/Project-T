using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
   [SerializeField] public Player[] player;
   int currentPlayer = 0;
   public Action OnChangePlayer;
   private InputSystem_Actions inputHandler;
   public static GameManager instance; 
   public SpriteRenderer game_map;
   public SpriteRenderer spawn;
   public Action<float> onTimeCounting;
   void Start()
   {
       slowUpdate();
       instance = this;
       inputHandler = new InputSystem_Actions();
       inputHandler.Enable();
       inputHandler.Player.Enable();
       inputHandler.Player.Interact.Enable();
       for (int i = 0; i < player.Length; i++)
       {
           Debug.Log($"Player {i}th");
           player[i].setLock(true);
           int ith = i;
           player[i].on_player_locked += () =>
           {
               if (ith == player.Length - 1)
               {
                   Finalize();
                   return;
               }
               else
               {
                   currentPlayer = ith + 1;
                   Debug.Log($"Player {currentPlayer} is unlocked");
                   OnChangePlayer?.Invoke();
                   GetCurrentPlayer().setLock(false);
               }
           };
           GetCurrentPlayer().setLock(false);
           
       }
       
       //inputHandler.Player.Interact.performed += player.Interact;
   }

   async void slowUpdate()
   {
       for (;;)
       {
           float time = GetCurrentPlayer().getCurrentTime();
               onTimeCounting?.Invoke(time);
           await Task.Delay(100);
//           Debug.Log(time);
       }
   }
   void Finalize()
   {
       
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
    public Player GetCurrentPlayer()
    {
        return player[currentPlayer];
    }
    void Update()
    {
        Vector2 moveValue = inputHandler.Player.Move.ReadValue<Vector2>();
        GetCurrentPlayer().Move(moveValue);
    }
}
