// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AScroller.Scroller;
using System.Linq;

namespace AScroller
{
    [ExecuteInEditMode]
    public class ContentFitter : MonoBehaviour
    {
        private AdvancedScroller AdvancedScrollerScript;
        private Scroll ScrollScript;
        private RectTransform rectTransform;

        private Vector2 lastElementSize;
        private float lastSpacing;
        private bool lastElementOrder;
        private alignment lastAlignment;
        private sizeMode lastSizeMode;
        private type lastType;
        private padding lastPadding;
        private GameObject lastStartFromElement;
        private float lastSelectionPos;
        private movementType lastMovementType;
        private int lastElementsCount;
        private Dictionary<RectTransform, Vector2> ElementsRects = new Dictionary<RectTransform, Vector2>();
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            rectTransform = GetComponent<RectTransform>();
            AdvancedScrollerScript = transform.parent.GetComponent<AdvancedScroller>();
            ScrollScript = GetComponent<Scroll>();
        }

        private void Update()
        {
            if (CheckIfAnythingChanged())
                UpdateParentSize();
        }


        public void UpdateParentSize()
        {
            if (!rectTransform)
                rectTransform = GetComponent<RectTransform>();

            if (!AdvancedScrollerScript)
                AdvancedScrollerScript = transform.parent.GetComponent<AdvancedScroller>();
            if (!ScrollScript)
                ScrollScript = GetComponent<Scroll>();

            if (!rectTransform || !AdvancedScrollerScript || !ScrollScript) return;

            if (!CheckIfAnythingChanged())
                return;


            SetSizeOfElements();

            lastType = AdvancedScrollerScript.Type;
            lastSelectionPos = AdvancedScrollerScript.PositionOfSelection;
            lastMovementType = AdvancedScrollerScript.MovementType;
            if (lastElementsCount != Utils.Utils.GetActiveChildrenCount(transform))
            {
                lastElementsCount = Utils.Utils.GetActiveChildrenCount(transform);
                AdvancedScrollerScript.SetElementsList();
            }

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                float totalWidth = 0f;
                float maxHeight = 0f;

                for (int i = 0; i < transform.childCount; i++)
                {
                    RectTransform childRectTransform = transform.GetChild(i).GetComponent<RectTransform>();

                    if (childRectTransform != null && childRectTransform.gameObject.activeSelf)
                    {
                        totalWidth += childRectTransform.rect.width;

                        if (i < transform.childCount - 1)
                        {
                            totalWidth += AdvancedScrollerScript.Spacing;
                        }

                        maxHeight = Mathf.Max(maxHeight, childRectTransform.rect.height);
                    }
                }
                rectTransform.sizeDelta = new Vector2(totalWidth + AdvancedScrollerScript.Padding.Left + AdvancedScrollerScript.Padding.Right, maxHeight + AdvancedScrollerScript.Padding.Top + AdvancedScrollerScript.Padding.Bottom);
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                float totalHeight = 0f;
                float maxWidth = 0f;

                for (int i = 0; i < transform.childCount; i++)
                {
                    RectTransform childRectTransform = transform.GetChild(i).GetComponent<RectTransform>();

                    if (childRectTransform != null && childRectTransform.gameObject.activeSelf)
                    {
                        totalHeight += childRectTransform.rect.height;

                        if (i < transform.childCount - 1)
                        {
                            totalHeight += AdvancedScrollerScript.Spacing;
                        }

                        maxWidth = Mathf.Max(maxWidth, childRectTransform.rect.width);
                    }
                }
                rectTransform.sizeDelta = new Vector2(maxWidth + AdvancedScrollerScript.Padding.Left + AdvancedScrollerScript.Padding.Right, totalHeight + AdvancedScrollerScript.Padding.Top + AdvancedScrollerScript.Padding.Bottom);
            }

            PositionChildrenWithSpacing();
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                SetStartingPosAccordingAlignment();
            }
#else
        SetStartingPosAccordingAlignment();
#endif
        }

        private void PositionChildrenWithSpacing()
        {
            if (ScrollScript.ScrollState == scrollState.loading && Application.isPlaying)
                return;

            lastStartFromElement = AdvancedScrollerScript.StartFromElement;

            int reverseMultiplaier = 1;
            List<RectTransform> ListOfElements = new List<RectTransform>();
            ListOfElements.AddRange(AdvancedScrollerScript.Elements);

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                if (AdvancedScrollerScript.ReverseOrder)
                    reverseMultiplaier = 1;
                else
                    reverseMultiplaier = -1;

                float currentX = (reverseMultiplaier * rectTransform.rect.width) * 0.5f + AdvancedScrollerScript.Padding.Left;
                if (AdvancedScrollerScript.MovementType == movementType.Endless && AdvancedScrollerScript.StartFromElement/*&& !EditorApplication.isPlayingOrWillChangePlaymode*/)
                    if (AdvancedScrollerScript.StartFromElement.activeSelf)
                    {
                        List<RectTransform> temp = new List<RectTransform>();
                        temp.AddRange(ReorderElementList(ListOfElements, AdvancedScrollerScript.StartFromElement));
                        temp.RemoveAll(g => !g.gameObject.activeSelf);
                        ListOfElements.Clear();
                        ListOfElements.AddRange(temp);
                        currentX = (AdvancedScrollerScript.AdvancedScrollerRectT.rect.width) * (AdvancedScrollerScript.PositionOfSelection - 0.5f) + AdvancedScrollerScript.Padding.Left;
                        currentX -= (AdvancedScrollerScript.StartFromElement.GetComponent<RectTransform>().rect.width / 2);
                    }

                for (int i = 0; i < ListOfElements.Count; i++)
                {
                    RectTransform childRectTransform = ListOfElements[i];//transform.GetChild(i).GetComponent<RectTransform>();

                    if (childRectTransform != null && childRectTransform.gameObject.activeSelf)
                    {
                        float childWidth = childRectTransform.rect.width;

                        Vector2 newPosition = new Vector2(currentX + ((-1 * reverseMultiplaier) * childWidth) * 0.5f, 0f);
                        #region usporiadanie elementov pre endless scrolle
                        if (AdvancedScrollerScript.MovementType == movementType.Endless /*&& !EditorApplication.isPlayingOrWillChangePlaymode*/)
                        {
                            float HalfContainerWidth = rectTransform.rect.width / 2;

                            if (newPosition.x > (HalfContainerWidth + (childWidth / 2)))
                                newPosition = new Vector2(newPosition.x - rectTransform.rect.width - AdvancedScrollerScript.Spacing, newPosition.y);
                            else if (newPosition.x < -(HalfContainerWidth + (childWidth / 2)))
                                newPosition = new Vector2(newPosition.x + rectTransform.rect.width + AdvancedScrollerScript.Spacing, newPosition.y);
                        }
                        #endregion
                        childRectTransform.anchoredPosition = newPosition;

                        currentX += ((-1 * reverseMultiplaier) * (childWidth + AdvancedScrollerScript.Spacing));
                    }
                }

            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                if (AdvancedScrollerScript.ReverseOrder)
                    reverseMultiplaier = -1;
                else
                    reverseMultiplaier = 1;

                float currentY = (reverseMultiplaier * rectTransform.rect.height) * 0.5f + (-1 * reverseMultiplaier * AdvancedScrollerScript.Padding.Top);//(reverseMultiplaier * rectTransform.rect.height) * 0.5f + AdvancedScrollerScript.Padding.Top; //(reverseMultiplaier * rectTransform.rect.height) * 0.5f + (-1 * reverseMultiplaier * AdvancedScrollerScript.Padding.Top); // prec -
                if (AdvancedScrollerScript.MovementType == movementType.Endless && AdvancedScrollerScript.StartFromElement/*&& !EditorApplication.isPlayingOrWillChangePlaymode*/)
                    if (AdvancedScrollerScript.StartFromElement.activeSelf)
                    {
                        List<RectTransform> temp = new List<RectTransform>();
                        temp.AddRange(ReorderElementList(ListOfElements, AdvancedScrollerScript.StartFromElement));
                        ListOfElements.Clear();
                        ListOfElements.AddRange(temp);
                        currentY = (AdvancedScrollerScript.AdvancedScrollerRectT.rect.height) * (AdvancedScrollerScript.PositionOfSelection - 0.5f) + AdvancedScrollerScript.Padding.Top;
                        currentY += (AdvancedScrollerScript.StartFromElement.GetComponent<RectTransform>().rect.height / 2);
                    }

                for (int i = 0; i < ListOfElements.Count; i++)
                {
                    RectTransform childRectTransform = ListOfElements[i];

                    if (childRectTransform != null)
                    {
                        float childHeight = childRectTransform.rect.height;

                        Vector2 newPosition = new Vector2(0f, currentY + ((-1 * reverseMultiplaier) * childHeight) * 0.5f); // tu dat -
                        #region usporiadanie elementov pre endless scrolle
                        if (AdvancedScrollerScript.MovementType == movementType.Endless /*&& !EditorApplication.isPlayingOrWillChangePlaymode*/)
                        {
                            float HalfContainerHeight = rectTransform.rect.height / 2;

                            if (newPosition.y > (HalfContainerHeight + (childHeight / 2)))
                                newPosition = new Vector2(newPosition.x, newPosition.y - rectTransform.rect.height - AdvancedScrollerScript.Spacing);
                            else if (newPosition.y < -(HalfContainerHeight + (childHeight / 2)))
                                newPosition = new Vector2(newPosition.x, newPosition.y + rectTransform.rect.height + AdvancedScrollerScript.Spacing);
                        }
                        #endregion
                        childRectTransform.anchoredPosition = newPosition;

                        currentY += ((-1 * reverseMultiplaier) * (childHeight + AdvancedScrollerScript.Spacing)); // tu dat -=
                    }
                }
            }

            lastSpacing = AdvancedScrollerScript.Spacing;
            lastElementOrder = AdvancedScrollerScript.ReverseOrder;
            lastPadding = AdvancedScrollerScript.Padding;
        }
        private void SetSizeOfElements()
        {
            lastSizeMode = AdvancedScrollerScript.ElementsSizeMode;
            lastElementSize = AdvancedScrollerScript.ElementsSize;

            if (AdvancedScrollerScript.ElementsSizeMode == sizeMode.Free)
                return;

            foreach (var element in AdvancedScrollerScript.Elements)
            {
                element.GetComponent<RectTransform>().sizeDelta = AdvancedScrollerScript.ElementsSize;
            }
        }
        public void SetStartingPosAccordingAlignment()
        {
            if (!AdvancedScrollerScript || !rectTransform)
                Init();

            RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();
            if (AdvancedScrollerScript.Alignment == alignment.Center)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else if (AdvancedScrollerScript.Alignment == alignment.Left)
            {
                float newPos = (rectTransform.rect.width - parentRectTransform.rect.width) / 2;
                rectTransform.anchoredPosition = new Vector2(newPos, 0);
            }
            else if (AdvancedScrollerScript.Alignment == alignment.Right)
            {
                float newPos = -1 * ((rectTransform.rect.width - parentRectTransform.rect.width) / 2);
                rectTransform.anchoredPosition = new Vector2(newPos, 0);
            }
            else if (AdvancedScrollerScript.Alignment == alignment.Top)
            {
                float newPos = -1 * ((rectTransform.rect.height - parentRectTransform.rect.height) / 2);
                rectTransform.anchoredPosition = new Vector2(0, newPos);
            }
            else if (AdvancedScrollerScript.Alignment == alignment.Bottom)
            {
                float newPos = ((rectTransform.rect.height - parentRectTransform.rect.height) / 2);
                rectTransform.anchoredPosition = new Vector2(0, newPos);
            }

            lastAlignment = AdvancedScrollerScript.Alignment;
        }

        private bool CheckIfAnythingChanged()
        {
            if (lastElementSize != AdvancedScrollerScript.ElementsSize)
            {
                Utils.Utils.DebugLog("Element size changed");
                return true;
            }

            if (lastSpacing != AdvancedScrollerScript.Spacing)
            {
                Utils.Utils.DebugLog("spacing changed");
                return true;
            }

            if (lastElementOrder != AdvancedScrollerScript.ReverseOrder)
            {
                Utils.Utils.DebugLog("element order changed");
                return true;
            }

            if (lastAlignment != AdvancedScrollerScript.Alignment)
            {
                Utils.Utils.DebugLog("aligment changed");
                return true;
            }
            if (lastSizeMode != AdvancedScrollerScript.ElementsSizeMode)
            {
                Utils.Utils.DebugLog("size mode changed");
                return true;
            }
            if (lastType != AdvancedScrollerScript.Type)
            {
                Utils.Utils.DebugLog("type changed");
                return true;
            }

            if (lastPadding.Left != AdvancedScrollerScript.Padding.Left || lastPadding.Right != AdvancedScrollerScript.Padding.Right || lastPadding.Top != AdvancedScrollerScript.Padding.Top || lastPadding.Bottom != AdvancedScrollerScript.Padding.Bottom)
            {
                Utils.Utils.DebugLog("padding changed");
                return true;
            }
            if (lastStartFromElement != AdvancedScrollerScript.StartFromElement)
            {
                Utils.Utils.DebugLog("start from element changed");
                return true;
            }
            if (lastSelectionPos != AdvancedScrollerScript.PositionOfSelection)
            {
                Utils.Utils.DebugLog("position of selection changed");
                return true;
            }
            if (lastMovementType != AdvancedScrollerScript.MovementType)
            {
                Utils.Utils.DebugLog("movement type changed");
                return true;
            }

            if (AdvancedScrollerScript.ElementsSizeMode == sizeMode.Free)
                if (CheckIfElementsSizeChanged())
                {
                    Utils.Utils.DebugLog("element size changed");
                    return true;
                }

            if (lastElementsCount != Utils.Utils.GetActiveChildrenCount(transform))
            {
                Utils.Utils.DebugLog("Child Count Changed");
                return true;
            }

            return false;
        }

        private List<RectTransform> ReorderElementList(List<RectTransform> listToReorder, GameObject firstObject)
        {
            List<RectTransform> ResultList = new List<RectTransform>();

            if (!firstObject)
                return ResultList;

            if (!listToReorder.Contains(firstObject.transform as RectTransform))
                return ResultList;

            int currentIndex = listToReorder.IndexOf(firstObject.transform as RectTransform);

            for (int i = currentIndex; i < listToReorder.Count; i++)
                ResultList.Add(listToReorder[i]);

            for (int i = 0; i < currentIndex; i++)
                ResultList.Add(listToReorder[i]);


            return ResultList;
        }
        private bool CheckIfElementsSizeChanged()
        {
            bool changed = false;

            //Debug.Log(ElementsRects.Count);
            if (ElementsRects.Count <= 0)
                changed = true;

            foreach (var element in ElementsRects.Keys)
            {
                if (AdvancedScrollerScript.Elements.Contains(element))
                {
                    if (AdvancedScrollerScript.Elements[AdvancedScrollerScript.Elements.IndexOf(element)].sizeDelta != ElementsRects[element])
                    {
                        changed = true;
                        break;
                    }
                }
            }

            ElementsRects.Clear();
            foreach (var e in AdvancedScrollerScript.Elements)
            {
                ElementsRects.Add(e, e.sizeDelta);
            }

            //Debug.Log(changed);
            return changed;
        }


        private void DebugList(List<RectTransform> list)
        {
            Debug.Log("\n");
            foreach (var item in list)
            {
                Debug.Log(" | " + item.name);
            }
        }
    }
}
