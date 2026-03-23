using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AScroller;

namespace AScroller.Demo
{
    public class SelectedElement : MonoBehaviour
    {
        [SerializeField] private Image selectedIcon;
        [SerializeField] private AdvancedScroller advancedScroller;
        void Start()
        {
            ShowSelectedElement();
        }

        public void ShowSelectedElement()
        {
            selectedIcon.sprite = advancedScroller.SelectedElement().transform.GetChild(0).GetComponent<Image>().sprite;
        }

        public void OnBeginOnDrag()
        {
            Debug.Log("Begining of drag");
        }

        public void OnEndOfDrag()
        {
            Debug.Log("End of drag");
        }
    }
}

