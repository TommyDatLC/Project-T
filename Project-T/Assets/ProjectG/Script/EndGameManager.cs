using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Script
{
    public class EndGameManager : MonoBehaviour
    {
        public static EndGameManager instance;

        [Header("Main UI Container")]
        public UIDocument mainUIDoc;

        [Header("UXML Assets")]
        public VisualTreeAsset darkenUXML;
        public VisualTreeAsset birdDramaticUXML;
        public VisualTreeAsset birdReactionUXML;
        public VisualTreeAsset endStatsUXML;

        private VisualElement root;

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            root = mainUIDoc.rootVisualElement;
            root.style.display = DisplayStyle.None; 
        }

        public void StartEndingSequence(EndgameEvaluator.EndgameResult result, float time, float score)
        {
            StartCoroutine(EndingRoutine(result, time, score));
        }

        private IEnumerator EndingRoutine(EndgameEvaluator.EndgameResult result, float time, float score)
        {
            root.style.display = DisplayStyle.Flex;
            root.Clear();

            // --- STEP 1: DARKEN (Stay for the whole sequence) ---
            VisualElement darken = darkenUXML.Instantiate();
            darken.style.position = Position.Absolute;
            darken.style.width = Length.Percent(100);
            darken.style.height = Length.Percent(100);
            root.Add(darken);
            
            VisualElement dimmer = darken.Q("Dimmer");
            float alpha = 0;
            DOTween.To(() => alpha, x => dimmer.style.opacity = x, 0.8f, 1.5f).SetUpdate(true);
            
            yield return new WaitForSecondsRealtime(3f); // Realtime because game is frozen

            // --- STEP 2: BIRD POPUP (Appear -> Wait -> Disappear) ---
            VisualElement bird = birdDramaticUXML.Instantiate();
            root.Add(bird);
            AnimateIn(bird.Q("BirdSquare"));
            
            yield return new WaitForSecondsRealtime(4f);
            root.Remove(bird); // Remove bird before reaction appears

            // --- STEP 3: REACTION (Appear -> Wait -> Disappear) ---
            VisualElement reaction = birdReactionUXML.Instantiate();
            root.Add(reaction);
            reaction.Q<Label>("ReactionText").text = result.type == EndgameEvaluator.EndingType.SecretFamily ? "Family..." : "Acceptable.";
            
            yield return new WaitForSecondsRealtime(5f);
            root.Remove(reaction); // Remove reaction before stats appear

            // --- STEP 4: FINAL STATS ---
            VisualElement stats = endStatsUXML.Instantiate();
            root.Add(stats);
            
            stats.Q<Label>("EndingTitle").text = result.endingTitle;
            stats.Q<Label>("EndingTitle").style.color = result.themeColor;
            stats.Q<Label>("ScoreValue").text = $"{score:F1}%";

            // Buttons: Use SceneManager or a manual reset
            stats.Q<Button>("RestartBtn").clicked += () => {
                Time.timeScale = 1; // MUST reset time scale before loading
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            };
        }

        private void AnimateIn(VisualElement el)
        {
            if (el == null) return;
            el.style.opacity = 0;
            float val = 0;
            DOTween.To(() => val, x => el.style.opacity = x, 1f, 1f).SetUpdate(true);
        }
    }
}