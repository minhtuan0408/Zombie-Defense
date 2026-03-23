// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AScroller;

namespace AScroller.Demo
{
    public class DemoTesting : MonoBehaviour
    {
        [SerializeField] private AdvancedScroller AScrollerHorizontal;
        [SerializeField] private AdvancedScroller AScrollerVertical;

        [Header("Set Normalized pos")]
        [SerializeField] private float HorizontalNormalizedPos;
        [SerializeField] private float VerticalNormalizedPos;

        [Header("For Scroll to Element")]
        [SerializeField] private RectTransform HorizontalElement;
        [SerializeField] private RectTransform VerticalElement;

        public void GetSelectedElement()
        {
            GameObject element = AScrollerHorizontal.SelectedElement();
            Debug.Log(element.name, element);
        }

        public void GetHorizontalNormalizedPos()
        {
            Debug.Log(AScrollerHorizontal.horizontalNormalizedPosition);
        }
        public void SetHorizontalNormalizedPos()
        {
            AScrollerHorizontal.horizontalNormalizedPosition = HorizontalNormalizedPos;
        }
        public void GetVerticalNormalizedPos()
        {
            Debug.Log(AScrollerVertical.verticalNormalizedPosition);
        }
        public void SetVerticalNormalizedPos()
        {
            AScrollerVertical.verticalNormalizedPosition = VerticalNormalizedPos;
        }

        public void ScrollToElement()
        {
            AScrollerHorizontal.ScrollToElement(HorizontalElement, 0.2f);

            AScrollerVertical.ScrollToElement(VerticalElement, 0.2f);
        }

        public void ScollToNext()
        {
            AScrollerHorizontal.ScrollToNextElement(0.2f);
        }
        public void ScollToPrevious()
        {
            AScrollerHorizontal.ScrollToPreviousElement(0.2f);
        }

        public void LoadScroll()
        {
            AScrollerVertical.LoadScroller(1f);
        }
        public void OnBegin()
        {
            Debug.Log("On Begin of Drag");
        }

        public void OnChange()
        {
            Debug.Log("On Scroller Change");
        }

        public void OnEnd()
        {
            Debug.Log("On End of Drag");
        }
    }
}
