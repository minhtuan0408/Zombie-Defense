// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AScroller
{
    public class AdvancedScrollerSpawn : Editor
    {
        [MenuItem("GameObject/UI/Advanced Scroller")]
        private static void CreateAdvancedScroller()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }

            GameObject advancedScrollerPrefab = Resources.Load<GameObject>("AdvancedScroller"); // Replace with your prefab path
            if (advancedScrollerPrefab != null)
            {
                GameObject advancedScroller = Instantiate(advancedScrollerPrefab, canvas.transform);
                advancedScroller.name = "AdvancedScroller";
            }
            else
            {
                Debug.LogError("[AScroll] AdvancedScroller prefab not found. Make sure to assign the correct path.");
            }
        }
    }
}
