using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AScroller;

namespace AScroller.Demo
{
    public class ScrollwithButtons : MonoBehaviour
    {
        [SerializeField] AdvancedScroller AScroller;
        [SerializeField] SelectedElement SelectedElementScript;
        private bool scrolling = false;
        public void ScrollToNext()
        {
            if (scrolling)
                return;

            scrolling = true;
            AScroller.ScrollToNextElement(0.2f, () =>
            {
                SelectedElementScript.ShowSelectedElement();
                scrolling = false;
            });
        }

        public void ScrollToPrevious()
        {
            if (scrolling)
                return;

            scrolling = true;
            AScroller.ScrollToPreviousElement(0.2f, () =>
            {
                SelectedElementScript.ShowSelectedElement();
                scrolling = false;
            });
        }
    }
}
