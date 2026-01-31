using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Script.EndGame
{
    public class EndGameController : MonoBehaviour
    {
        [Header("UI Setup")]
        public UIDocument uiDoc;

        public VisualTreeAsset darkenUXML;
        public VisualTreeAsset birdDramaticUXML;
        public VisualTreeAsset birdReactionUXML;
        public VisualTreeAsset endStatsUXML;

        private VisualElement root;

        void Start()
        {
            root = uiDoc.rootVisualElement;
            root.Clear();
            StartCoroutine(ResultSequence());
        }

        private IEnumerator ResultSequence()
        {
            // 1. Setup the Background (The Screenshot)
            if (EndGameData.Screenshot != null)
            {
                root.style.backgroundImage = new StyleBackground(EndGameData.Screenshot);
            }

            // 2. Add Persistent Darken Overlay
            // This stays in the root while other popups are added/removed on top of it
            VisualElement darken = darkenUXML.Instantiate();
            darken.style.position = Position.Absolute;
            darken.style.width = Length.Percent(100);
            darken.style.height = Length.Percent(100);
            root.Add(darken);

            VisualElement dimmer = darken.Q("Dimmer");
            if (dimmer != null)
            {
                dimmer.style.opacity = 0;
                DOTween.To(() => 0f, x => dimmer.style.opacity = x, 0.8f, 1.5f);
            }

            yield return new WaitForSeconds(3f);

            // 3. Bird Dramatic Popup (Show -> Wait -> Hide)
            VisualElement bird = birdDramaticUXML.Instantiate();
            root.Add(bird);
            AnimateIn(bird.Q("BirdSquare"));
            yield return new WaitForSeconds(4f);
            root.Remove(bird);

            // 4. Reaction Text (Show -> Wait -> Hide)
            VisualElement reaction = birdReactionUXML.Instantiate();
            root.Add(reaction);
            var res = EndGameData.Result;
            
            Label reactionLabel = reaction.Q<Label>("ReactionText");
            if (reactionLabel != null)
                reactionLabel.text = res.type == EndgameEvaluator.EndingType.SecretFamily ? "Family..." : "Acceptable.";
            
            yield return new WaitForSeconds(5f);
            root.Remove(reaction);

            // 5. Final Stats
            VisualElement stats = endStatsUXML.Instantiate();
            root.Add(stats);
            
            stats.Q<Label>("EndingTitle").text = res.endingTitle;
            stats.Q<Label>("EndingTitle").style.color = res.themeColor;
            stats.Q<Label>("ScoreValue").text = $"{EndGameData.FinalScore:F1}%";
            stats.Q<Label>("TimeValue").text = $"{EndGameData.FinalTime:F1}s";

            // Button Listeners - Ensure these scene names match your Build Settings
            stats.Q<Button>("RestartBtn").clicked += () => SceneManager.LoadScene("MainGameScene");
            stats.Q<Button>("MenuBtn").clicked += () => SceneManager.LoadScene("MainMenu");
        }

        private void AnimateIn(VisualElement el)
        {
            if (el == null) return;
            el.style.opacity = 0;
            DOTween.To(() => 0f, x => el.style.opacity = x, 1f, 1f);
        }
        
        private void OnDestroy()
        {
            if (EndGameData.Screenshot != null)
            {
                Destroy(EndGameData.Screenshot);
                EndGameData.Screenshot = null;
            }
        }
    }
}