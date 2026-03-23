using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AScroller;

namespace AScroller.Demo
{
    public class SelectElement : MonoBehaviour
    {
        [SerializeField] AdvancedScroller AScroller;
        [SerializeField] SelectedElement SelectedElementScript;
        [SerializeField] RectTransform Element;


        public void ScrollToElement()
        {
            AScroller.ScrollToElement(Element, 0.2f, SelectedElementScript.ShowSelectedElement);
        }
    }
}
