using System;
using System.Threading.Tasks;
using ProjectG.Script.UI.InGame.Overlay;
using Script;
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
       for (int i = 0; i < player.Length; i++)
       {
           Debug.Log($"Player {i+1}th");
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
                   SetCurrentPlayer(ith + 1);
               }
           };
       }
       SetCurrentPlayer(0);
       //inputHandler.Player.Interact.performed += player.Interact;
   }
   
   public void HandleEndGame(float score)
   {
       // 1. Guard against multiple calls
       if (isEnding) return;
       isEnding = true;

       // 2. Unsubscribe immediately
       circle_drawer.onStopDrawing -= HandleEndGame;

       // 3. HARD LOCK: Freeze everything
       Time.timeScale = 0; // Physical freeze
       circle_drawer.enabled = false;
       circle_drawer.gameObject.SetActive(false); 
    
       if (inputHandler != null) {
           inputHandler.Player.Disable();
           inputHandler.Disable();
       }
    
       this.enabled = false; // Stops Update() loops

       // 4. Safety Check for the Manager
       if (EndGameManager.instance == null) {
           Debug.LogError("EndGameManager is missing from the scene!");
           Time.timeScale = 1; // Unfreeze so you aren't stuck in a broken state
           return;
       }

       // 5. Sequence Handoff
       Player p = GetCurrentPlayer();
       float timeSpent = p != null ? p.getCurrentTime() : 0f;
       var result = EndgameEvaluator.EvaluateGame(score, timeSpent, 120f, fulfilledFamilyCondition);
 
       EndGameManager.instance.StartEndingSequence(result, timeSpent, score);
   }
   
   async void SetCurrentPlayer(int i)
   {
       overlay_controller.Show($"Player {i+1} phase",2);
       await Task.Delay(2000);
       currentPlayer = i;
       Debug.Log($"Player {currentPlayer} is unlocked");
       OnChangePlayer?.Invoke();
       GetCurrentPlayer().setLock(false);
       virtual_camera.Target.TrackingTarget = GetCurrentPlayer().transform;
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
   async void Finalize()
   {
       overlay_controller.Show($"Draw phase", 2);
       await Task.Delay(2000);
    
       // Unsubscribe first to prevent double-registration
       circle_drawer.onStopDrawing -= HandleEndGame; 
       circle_drawer.onStopDrawing += HandleEndGame;
    
       circle_drawer.gameObject.SetActive(true);
       virtual_camera.Target.TrackingTarget = null;
   }
   private void OnDisable()
   {
       if (inputHandler != null)
       {
           // Explicitly disable the action maps to prevent memory leaks
           inputHandler.Player.Disable();
           inputHandler.UI.Disable();
           inputHandler.Disable();
       }
   }

   private void OnDestroy()
   {
       // Good practice to dispose of the handler entirely
       if (inputHandler != null)
       {
           inputHandler.Dispose();
           circle_drawer.onStopDrawing -= HandleEndGame;
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
    
    public void TriggerEndgameSequence(float scoreOverride = -1)
    {
        // If time ran out or items collected before drawing, 
        // we might need a default score or use the current drawing score.
        float finalScore = scoreOverride >= 0 ? scoreOverride : circle_drawer.lastScore;
    
        float timeSpent = GetCurrentPlayer().getCurrentTime();
        var result = EndgameEvaluator.EvaluateGame(finalScore, timeSpent, 120f, fulfilledFamilyCondition);

        EndGameManager.instance.StartEndingSequence(result, timeSpent, finalScore);
    }
}
