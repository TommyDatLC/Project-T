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
            var res = EndGameData.Result;

            // 1. Setup Background Screenshot
            if (EndGameData.Screenshot != null)
            {
                root.style.backgroundImage = new StyleBackground(EndGameData.Screenshot);
            }

            // 2. Add Dimmer Overlay
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

            yield return new WaitForSeconds(2f);

            // 3. Bird Dramatic Popup (Injection Point for Emojis)
            VisualElement bird = birdDramaticUXML.Instantiate();
            root.Add(bird);
            
            Label birdLabel = bird.Q<Label>(); 
            VisualElement square = bird.Q("BirdSquare");

            if (birdLabel != null) birdLabel.text = res.emoji;
            if (square != null)
            {
                square.style.borderTopColor = res.themeColor;
                square.style.borderBottomColor = res.themeColor;
                square.style.borderLeftColor = res.themeColor;
                square.style.borderRightColor = res.themeColor;
            }

            AnimateIn(square);
            yield return new WaitForSeconds(3f);
            root.Remove(bird);

            // 4. Reaction Text
            VisualElement reaction = birdReactionUXML.Instantiate();
            root.Add(reaction);
            
            Label reactionLabel = reaction.Q<Label>("ReactionText");
            if (reactionLabel != null)
            {
                reactionLabel.style.color = res.themeColor;
                reactionLabel.text = GetReactionText(res.type);
            }
            
            yield return new WaitForSeconds(4f);
            root.Remove(reaction);

            // 5. Final Stats
            VisualElement stats = endStatsUXML.Instantiate();
            root.Add(stats);
            
            stats.Q<Label>("EndingTitle").text = res.endingTitle;
            stats.Q<Label>("EndingTitle").style.color = res.themeColor;
            stats.Q<Label>("ScoreValue").text = $"{EndGameData.FinalScore:F1}%";
            stats.Q<Label>("TimeValue").text = $"{EndGameData.FinalTime:F1}s";

            // Button Listeners
            stats.Q<Button>("RestartBtn").clicked += () => SceneManager.LoadSceneAsync("Scenes/GamePlay");
            stats.Q<Button>("MenuBtn").clicked += () => SceneManager.LoadSceneAsync("MainMenu");
        }

        private string GetReactionText(EndgameEvaluator.EndingType type)
        {
            return type switch
            {
                EndgameEvaluator.EndingType.SecretFamily => "The flock recognizes its own. You are home.",
                EndgameEvaluator.EndingType.Frightening  => "The predator fled in absolute terror!",
                EndgameEvaluator.EndingType.Questioning  => "The bird is... deeply confused by your existence.",
                EndgameEvaluator.EndingType.Nonchalant   => "It left, but it wasn't impressed.",
                EndgameEvaluator.EndingType.Failure      => "You've become a very artistic snack.",
                _                                       => "The encounter ends."
            };
        }

        private void AnimateIn(VisualElement el)
        {
            if (el == null) return;
            el.style.opacity = 0;
            el.transform.scale = Vector3.one * 0.5f;
            
            DOTween.To(() => 0f, x => el.style.opacity = x, 1f, 0.5f);
            DOTween.To(() => el.transform.scale, x => el.transform.scale = x, Vector3.one, 0.8f).SetEase(Ease.OutBack);
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