using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using Script.EndGame;

namespace Script
{
    public class ScreenCaptureManager : MonoBehaviour
    {
        [Header("UI Toolkit Setup")]
        [SerializeField] private UIDocument mainHUD; // Drag your In-Game HUD here

        /// <summary>
        /// Call this from GameManager.HandleEndGame
        /// </summary>
        public void CaptureAndTransition(string nextSceneName)
        {
            StartCoroutine(CaptureRoutine(nextSceneName));
        }

        private IEnumerator CaptureRoutine(string nextSceneName)
        {
            // 1. Hide ALL UI Toolkit elements so they don't block the drawing in the shot
            if (mainHUD != null)
            {
                mainHUD.rootVisualElement.style.display = DisplayStyle.None;
            }

            // 2. Wait for the GPU to finish rendering the frame WITHOUT the UI
            yield return new WaitForEndOfFrame();

            // 3. Create a Texture matching the screen resolution
            Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // 4. Read the pixels from the backbuffer
            // This captures everything the camera sees (the world, the drawing, etc.)
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            // 5. Store the texture in our static carrier
            // If there was an old screenshot, we destroy it to save memory
            if (EndGameData.Screenshot != null) Destroy(EndGameData.Screenshot);
            EndGameData.Screenshot = screenTexture;

            // 6. Transition to the result scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}