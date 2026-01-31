using System;
using System.Threading.Tasks;
using ProjectG.Script.UI.InGame.Overlay;
using Script;
using Script.EndGame;
using Script.Item;
using Unity.Cinemachine;
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
   public Spawner spawner_obj;
   // Dang ki event nay de biet thoi gian con lai cua nguoi choi de render
   public Action<float> onTimeCounting;
   public CircleDrawer circle_drawer;
   public CinemachineCamera virtual_camera;
   public OverlayController overlay_controller;
   
   public bool fulfilledFamilyCondition = false;
   private bool isEnding = false;
   
   
   void Start()
   {
       
       circle_drawer.gameObject.SetActive(false);
       slowUpdate();
       instance = this;
       inputHandler = new InputSystem_Actions();
       inputHandler.Enable();
       inputHandler.Player.Enable();
       inputHandler.Player.Interact.Enable();
       circle_drawer.GameManagerInit(this);
       for (int i = 0; i < player.Length; i++)
       {
           Debug.Log($"Player {i}th");
           player[i].setLock(true);
           int ith = i;
           player[i].InitGM(this);
           player[i].on_player_locked += () =>
           {
               if (ith == player.Length - 1)
               {
                   Finalize();
                   return;
               }
               else
               {
                   SetCurrentPlayer(ith + 1);
               }
           };
       }
           SetCurrentPlayer(0);
       
       //inputHandler.Player.Interact.performed += player.Interact;
   }

   async void SetCurrentPlayer(int i)
   {
       overlay_controller.Show($"Player {i + 1} phase",2);
       await Task.Delay(2000);
       currentPlayer = i;
       Debug.Log($"Player {currentPlayer} is unlocked");
       OnChangePlayer?.Invoke();
       GetCurrentPlayer().setLock(false);
       virtual_camera.Target.TrackingTarget = GetCurrentPlayer().transform;
       Debug.Log("Spawning");
       spawner_obj.Spawn(Item.maxItem - Item.itemCount);
   }

   async void slowUpdate()
   {
       for (;;)
       {
           Player p = GetCurrentPlayer();
           float time = p.getCurrentTime();
               onTimeCounting?.Invoke(time);
               if (time > 120)
               {
                    p.StopTime();
                    p.isLoss = true;
               }
           await Task.Delay(100);
//           Debug.Log(time);
       }
   }

   private float sumTime;
   async void Finalize()
   {
       overlay_controller.Show($"Draw phase",2);
       await Task.Delay(2000);
       for (int i = 0; i < player.Length; i++)
       {
           sumTime += player[i].getSumTime();
       }
       circle_drawer.onStopDrawing -= HandleEndGame;
       circle_drawer.onStopDrawing += HandleEndGame;
       
       circle_drawer.gameObject.SetActive(true);
       virtual_camera.Target.TrackingTarget = null;
   }
   private void OnDestroy()
   {
       // Clean up events
       if (circle_drawer != null)
       {
           circle_drawer.onStopDrawing -= HandleEndGame;
       }

       // Clean up Input System
       if (inputHandler != null)
       {
           inputHandler.Player.Disable();
           inputHandler.Dispose();
       }

       instance = null;
   }

   public void HandleEndGame(float score)
   {
       if (isEnding) return;
       isEnding = true;

       // 1. Freeze and Cleanup
       circle_drawer.onStopDrawing -= HandleEndGame;
       circle_drawer.enabled = false;
       circle_drawer.gameObject.SetActive(false); 
       inputHandler?.Disable();
       this.enabled = false;

    
       EndGameData.FinalScore = score;
       EndGameData.FinalTime = sumTime;
       // Perform the evaluation here
       EndGameData.Result = EndgameEvaluator.EvaluateGame(score, sumTime, 120f, fulfilledFamilyCondition);

       // 3. Capture and Go
       // Make sure "EndGame" matches the name in your Build Settings exactly
       GetComponent<ScreenCaptureManager>().CaptureAndTransition("EndGame");
   }
   public void HideAllPlayer()
   {
       for (int i = 0; i < player.Length; i++)
       {
           player[i].HideAndDisable();
       }
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
