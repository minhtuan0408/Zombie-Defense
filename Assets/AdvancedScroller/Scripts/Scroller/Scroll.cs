// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using AScroller.Utils;
using System;


namespace AScroller.Scroller
{
    public class Scroll : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [HideInInspector] public RectTransform Container;
        public AdvancedScroller AdvancedScrollerScript;
        private RectTransform AdvacendScrollerRectT;
        private Camera Cam;

        private bool _OnBeginDrag;
        private bool _OnEndDrag = true;

        private Vector2 lastMousePosition;
        private float scrollingSpeedMouseInput = 0.15f;
        private bool draging;
        private float scrollInput = 0;
        private float LastValidInput;
        private bool dragingByInheria;
        [HideInInspector] public bool dragingByAnim;
        private bool dragingByScrolling;
        private bool fadingState;

        private scrollState state;
        public scrollState ScrollState
        {
            get { return state; }
            set { state = value; }
        }

        public Image Selector;

        /// <summary>
        ///  
        /// </summary>
        private Vector2 startDragPosition;
        private Vector2 lastScrollPos;

        private void Awake()
        {
            SetReferences();
        }

        private void SetReferences()
        {
            Cam = Camera.main;
            AdvancedScrollerScript = GetComponentInParent<AdvancedScroller>();
            AdvacendScrollerRectT = AdvancedScrollerScript.GetComponent<RectTransform>();
            Container = GetComponent<RectTransform>();
            SetLastScrollPos();
        }


        private void Update()
        {
            #region Input
            if (AdvancedScrollerScript.Scrollable)
            {
                //scrollInput = Input.GetAxis("Horizontal");
                //scrollInput = (int)scrollInput;

                /*if (Input.GetMouseButtonDown(0))
                {
                    StartDragging();
                }
                else if (Input.GetMouseButton(0) && draging)
                {
                    HandleDragging();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    EndDragging();
                }*/
            }

            ApplyIneria();
            ApplyScaling();
            ApplySnaping();
            ApplyFading();

            ApplyDeflection();
            #endregion

            #region Events 
            if ((scrollInput != 0 && draging) || dragingByInheria)
            {
                _OnEndDrag = false;
                if (!_OnBeginDrag)
                {
                    _OnBeginDrag = true;
                    AdvancedScrollerScript.OnBeginOfDrag?.Invoke();
                }
                AdvancedScrollerScript.OnScrollChange?.Invoke();
            }
            else if ((scrollInput == 0 && !draging))
            {
                _OnBeginDrag = false;
                if (!_OnEndDrag)
                {
                    _OnEndDrag = true;
                    AdvancedScrollerScript.OnEndOfDrag?.Invoke();
                }
            }
            #endregion


            if (AdvancedScrollerScript.MovementType == movementType.Endless)
            {
                if (AdvancedScrollerScript.Type == type.Horizontal)
                {
                    foreach (var element in AdvancedScrollerScript.Elements)
                    {
                        element.anchoredPosition += Vector2.right * scrollInput * (AdvancedScrollerScript.ScrollSensitivity);
                    }

                    float HalfContainerWidth = Container.rect.width / 2;
                    foreach (var element in AdvancedScrollerScript.Elements)
                    {
                        if (element.anchoredPosition.x < -(HalfContainerWidth + (element.rect.width / 2)))
                        {
                            RepositionElement(true);
                        }
                        else if (element.anchoredPosition.x > (HalfContainerWidth + (element.rect.width / 2)))
                        {
                            RepositionElement(false);
                        }
                    }
                }
                else if (AdvancedScrollerScript.Type == type.Vertical)
                {
                    foreach (var element in AdvancedScrollerScript.Elements)
                    {
                        element.anchoredPosition += Vector2.up * scrollInput * (AdvancedScrollerScript.ScrollSensitivity);
                    }

                    float HalfContainerHeight = Container.rect.height / 2;
                    foreach (var element in AdvancedScrollerScript.Elements)
                    {
                        if (element.anchoredPosition.y < -(HalfContainerHeight + (element.rect.height / 2)))
                        {
                            //Debug.Log(-(HalfContainerHeight + (element.rect.height / 2)));
                            RepositionElement(true);
                        }
                        else if (element.anchoredPosition.y > (HalfContainerHeight + (element.rect.height / 2)))
                        {
                            RepositionElement(false);
                        }
                    }
                }
            }
            else if (AdvancedScrollerScript.MovementType == movementType.Clamped)
            {
                if (AdvancedScrollerScript.Type == type.Horizontal)
                {
                    Container.anchoredPosition += Vector2.right * scrollInput * (AdvancedScrollerScript.ScrollSensitivity);

                    float HalfWidthParent = AdvacendScrollerRectT.rect.width / 2;
                    float HalfWidthContainer = Container.rect.width / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    if (Container.anchoredPosition.x > (Mathf.Abs(pos)))
                    {
                        Container.anchoredPosition = new Vector2(pos, 0);
                    }
                    else if (Container.anchoredPosition.x < -(Mathf.Abs(pos)))
                    {
                        Container.anchoredPosition = new Vector2(-pos, 0);
                    }
                }
                else if (AdvancedScrollerScript.Type == type.Vertical)
                {
                    Container.anchoredPosition += Vector2.up * scrollInput * (AdvancedScrollerScript.ScrollSensitivity);

                    float HalfHeightParent = AdvacendScrollerRectT.rect.height / 2;
                    float HalfHeightContainer = Container.rect.height / 2;
                    float pos = HalfHeightContainer - HalfHeightParent;

                    if (Container.anchoredPosition.y > (Mathf.Abs(pos)))
                    {
                        Container.anchoredPosition = new Vector2(0, pos);
                    }
                    else if (Container.anchoredPosition.y < -(Mathf.Abs(pos)))
                    {
                        Container.anchoredPosition = new Vector2(0, -pos);
                    }
                }
            }
            else if (AdvancedScrollerScript.MovementType == movementType.Elastic)
            {
                if (AdvancedScrollerScript.Type == type.Horizontal)
                {
                    Container.anchoredPosition += Vector2.right * scrollInput * AdvancedScrollerScript.ScrollSensitivity;

                    float HalfWidthParent = AdvacendScrollerRectT.rect.width / 2;
                    float HalfWidthContainer = Container.rect.width / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    // toto sa spusti az ked skonci scrollovanie 
                    if (scrollInput == 0 && !draging && (Mathf.Abs(LastValidInput) <= 0 || !AdvancedScrollerScript.Inertia))
                    {
                        if (Container.anchoredPosition.x > (Mathf.Abs(pos)) && !Mathf.Approximately(Container.anchoredPosition.x, Mathf.Abs(pos)))
                        {
                            // tu pride pomala animacia na miesto 
                            StartCoroutine(Container.DOAnchoredPos(new Vector2(pos, Container.anchoredPosition.y), (0.3f * AdvancedScrollerScript.Elasticity)));
                        }
                        else if (Container.anchoredPosition.x < -(Mathf.Abs(pos)) && !Mathf.Approximately(Container.anchoredPosition.x, -Mathf.Abs(pos)))
                        {
                            StartCoroutine(Container.DOAnchoredPos(new Vector2(-pos, Container.anchoredPosition.y), (0.3f * AdvancedScrollerScript.Elasticity)));
                        }
                    }
                }
                else if (AdvancedScrollerScript.Type == type.Vertical)
                {
                    Container.anchoredPosition += Vector2.up * scrollInput * AdvancedScrollerScript.ScrollSensitivity;

                    float HalfHeightParent = AdvacendScrollerRectT.rect.height / 2;
                    float HalfHeightContainer = Container.rect.height / 2;
                    float pos = HalfHeightContainer - HalfHeightParent;

                    // toto sa spusti az ked skonci scrollovanie 
                    if (scrollInput == 0 && !draging && (Mathf.Abs(LastValidInput) <= 0 || !AdvancedScrollerScript.Inertia))
                    {
                        if (Container.anchoredPosition.y > (Mathf.Abs(pos)) && !Mathf.Approximately(Container.anchoredPosition.y, Mathf.Abs(pos)))
                        {
                            // tu pride pomala animacia na miesto 
                            StartCoroutine(Container.DOAnchoredPos(new Vector2(Container.anchoredPosition.x, pos), (0.3f * AdvancedScrollerScript.Elasticity)));
                        }
                        else if (Container.anchoredPosition.y < -(Mathf.Abs(pos)) && !Mathf.Approximately(Container.anchoredPosition.y, -Mathf.Abs(pos)))
                        {
                            StartCoroutine(Container.DOAnchoredPos(new Vector2(Container.anchoredPosition.x, -pos), (0.3f * AdvancedScrollerScript.Elasticity)));
                        }
                    }
                }
            }

            scrollInput = 0;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!AdvancedScrollerScript.Scrollable)
                return;

            if (ScrollState == scrollState.loading)
                return;
            //if (CheckMouseHit()) // toto sa nemusi pouzivat ak to je pointerEventData
            //{
            draging = true;
            ScrollAnimScaling(true);
            //}
            lastMousePosition = Input.mousePosition;

            // Store the starting drag position
            startDragPosition = eventData.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!AdvancedScrollerScript.Scrollable)
                return;

            if (ScrollState == scrollState.loading)
                return;

            Vector2 dragDelta = eventData.position - startDragPosition;

            // Apply the drag delta to the content's anchored position
            //Vector2 newAnchoredPosition = Container.anchoredPosition + dragDelta;

            // Update the content's anchored position
            //Container.anchoredPosition = new Vector2(newAnchoredPosition.x, Container.anchoredPosition.y);

            // Update the starting drag position for the next frame
            startDragPosition = eventData.position;

            //Debug.LogError(dragDelta);

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
                //lastMousePosition = Input.mousePosition;
                scrollInput = dragDelta.x * scrollingSpeedMouseInput;
                if (Mathf.Abs(scrollInput) > 0.5f) LastValidInput = scrollInput * 0.5f;
            }
            else
            {
                //Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
                //lastMousePosition = Input.mousePosition;
                scrollInput = dragDelta.y * scrollingSpeedMouseInput;
                if (Mathf.Abs(scrollInput) > 0.5f) LastValidInput = scrollInput * 0.5f;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!AdvancedScrollerScript.Scrollable)
                return;

            if (ScrollState == scrollState.loading)
                return;

            draging = false;

            ScrollAnimScaling(false);
        }

        private void RepositionElement(bool scrollToLeft)
        {
            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                List<RectTransform> children = new List<RectTransform>();
                for (int i = 0; i < Container.childCount; i++)
                    children.Add(Container.GetChild(i).GetComponent<RectTransform>());
                children.RemoveAll(g => !g.gameObject.activeSelf);

                RectTransform firstElement = children[0];
                RectTransform lastElement = children[children.Count - 1];

                //RectTransform firstElement = AdvancedScrollerScript.Elements[0];
                //RectTransform lastElement = AdvancedScrollerScript.Elements[AdvancedScrollerScript.Elements.Count - 1];

                //RectTransform firstElement = Container.GetChild(0).GetComponent<RectTransform>();
                //RectTransform lastElement = Container.GetChild(Container.childCount - 1).GetComponent<RectTransform>();

                if (scrollToLeft)
                {
                    //Debug.Log("      Left");
                    float elementWidth = firstElement.rect.width;
                    Vector2 newPosition = lastElement.anchoredPosition + new Vector2(lastElement.rect.width / 2, 0) + new Vector2(elementWidth / 2, 0) + new Vector2(AdvancedScrollerScript.Spacing, 0);
                    firstElement.anchoredPosition = newPosition;

                    firstElement.SetAsLastSibling();
                }
                else
                {
                    //Debug.Log("Right");
                    float elementWidth = lastElement.rect.width;
                    Vector2 newPosition = firstElement.anchoredPosition - new Vector2(firstElement.rect.width / 2, 0) - new Vector2(elementWidth / 2, 0) - new Vector2(AdvancedScrollerScript.Spacing, 0);
                    lastElement.anchoredPosition = newPosition;

                    lastElement.SetAsFirstSibling();
                }
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                List<RectTransform> children = new List<RectTransform>();
                for (int i = 0; i < Container.childCount; i++)
                    children.Add(Container.GetChild(i).GetComponent<RectTransform>());
                children.RemoveAll(g => !g.gameObject.activeSelf);

                RectTransform firstElement = children[0];
                RectTransform lastElement = children[children.Count - 1];
                //RectTransform firstElement = Container.GetChild(0).GetComponent<RectTransform>();
                //RectTransform lastElement = Container.GetChild(Container.childCount - 1).GetComponent<RectTransform>();

                if (scrollToLeft)
                {
                    //Debug.Log("      Down");
                    float elementHeight = lastElement.rect.height;
                    Vector2 newPosition = firstElement.anchoredPosition + new Vector2(0, firstElement.rect.height / 2) + new Vector2(0, elementHeight / 2) + new Vector2(0, AdvancedScrollerScript.Spacing);
                    lastElement.anchoredPosition = newPosition;

                    //firstElement.SetAsLastSibling();
                    lastElement.SetAsFirstSibling();
                }
                else
                {
                    //Debug.Log("UP");
                    float elementHeight = firstElement.rect.height;
                    Vector2 newPosition = lastElement.anchoredPosition - new Vector2(0, lastElement.rect.height / 2) - new Vector2(0, elementHeight / 2) - new Vector2(0, AdvancedScrollerScript.Spacing);
                    firstElement.anchoredPosition = newPosition;

                    //lastElement.SetAsFirstSibling();
                    firstElement.SetAsLastSibling();
                }
            }
        }

        private bool CheckMouseHit()
        {
            /*RaycastHit2D hit;
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

            hit = Physics2D.Raycast(ray.origin, new Vector2(0, 0));
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<AdvancedScroller>() == AdvancedScrollerScript)
                {
                    return true;
                }
            }
            return false;*/
            return true;
        }
        private bool ChangedPosWithoutDraging()
        {
            if (AdvancedScrollerScript.MovementType == movementType.Clamped || AdvancedScrollerScript.MovementType == movementType.Elastic)
            {
                if (lastScrollPos != Container.anchoredPosition)
                {
                    SetLastScrollPos();
                    return true;
                }
            }
            else if (AdvancedScrollerScript.MovementType == movementType.Endless)
            {
                if (AdvancedScrollerScript.Elements.Count > 0)
                    if (lastScrollPos != AdvancedScrollerScript.Elements[0].anchoredPosition)
                    {
                        SetLastScrollPos();
                        return true;
                    }
            }

            return false;
        }

        private void SetLastScrollPos()
        {
            if (AdvancedScrollerScript.MovementType == movementType.Clamped || AdvancedScrollerScript.MovementType == movementType.Elastic)
                lastScrollPos = Container.anchoredPosition;
            else if (AdvancedScrollerScript.MovementType == movementType.Endless)
                if (AdvancedScrollerScript.Elements.Count > 0)
                    lastScrollPos = AdvancedScrollerScript.Elements[0].anchoredPosition;
        }

        private void StartDragging()
        {
            if (CheckMouseHit())
            {
                draging = true;
                ScrollAnimScaling(true);
            }
            lastMousePosition = Input.mousePosition;

        }
        private void HandleDragging()
        {
            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;
                scrollInput = deltaMousePosition.x * scrollingSpeedMouseInput;
                //Debug.Log(scrollInput);
                if (Mathf.Abs(scrollInput) > 0) LastValidInput = scrollInput;
            }
            else
            {
                Vector2 deltaMousePosition = (Vector2)Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;
                scrollInput = deltaMousePosition.y * scrollingSpeedMouseInput;
                //Debug.Log(scrollInput);
                if (Mathf.Abs(scrollInput) > 0) LastValidInput = scrollInput;
            }
        }
        private void EndDragging()
        {
            draging = false;

            ScrollAnimScaling(false);
        }

        private void ApplyIneria()
        {
            LastValidInput *= Mathf.Pow(AdvancedScrollerScript.DecelerationRate, Time.unscaledDeltaTime);
            if (!draging && AdvancedScrollerScript.Inertia && Mathf.Abs(LastValidInput) > 0)
            {
                #region funkcne stare(nie velmi dobre) 
                /*
                dragingByInheria = true;
                 * LastValidInput *= AdvancedScrollerScript.DecelerationRate;
                if (Mathf.Abs(LastValidInput) <= 0.05f)
                {
                    dragingByInheria = false;
                    LastValidInput = 0;
                }
                scrollInput = LastValidInput;*/
                #endregion

                dragingByInheria = true;
                if (Mathf.Abs(LastValidInput) <= 0.15f)
                {
                    dragingByInheria = false;
                    LastValidInput = 0;
                }
                scrollInput = LastValidInput;
            }
            else if (!draging && !AdvancedScrollerScript.Inertia)
            {
                scrollInput = 0;
            }
        }

        private void ApplyScaling()
        {
            if (ScrollState == scrollState.loading) return;
            if (!AdvancedScrollerScript.Scaling)
            {
                foreach (Transform element in AdvancedScrollerScript.Elements)
                    element.localScale = Vector3.one;
                return;
            }

            foreach (Transform element in AdvancedScrollerScript.Elements)
                UpdateElementScale(element);
        }
        private void UpdateElementScale(Transform element)
        {
            /*float distance = 0;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.transform.position.x - GetPosFromSlider(AdvancedScrollerScript.ScalingPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.transform.position.y - GetPosFromSlider(AdvancedScrollerScript.ScalingPosition).y);

            float normalizedDistance = AdvancedScrollerScript.ScalingDistance / distance;
            float scale = Mathf.Lerp(1f, AdvancedScrollerScript.ScalingFactor, normalizedDistance);*/
            //Debug.Log(scale + " " + normalizedDistance + " distance:" + distance);
            Vector2 scale = GetScaleAccordingDistance(element);
            element.localScale = new Vector3(scale.x, scale.y, 1f);
        }
        public Vector2 GetScaleAccordingDistance(Transform element)
        {
            Vector2 scale = new Vector2(1, 1);
            if (!AdvancedScrollerScript.Scaling)
                return scale;

            float distance = 0;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.transform.position.x - GetPosFromSlider(AdvancedScrollerScript.ScalingPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.transform.position.y - GetPosFromSlider(AdvancedScrollerScript.ScalingPosition).y);

            AnimationCurve customCurve;
            if (AdvancedScrollerScript.EaseTypeScaling == easeType.Default)
                customCurve = Utils.Utils.GetEase(AdvancedScrollerScript.ScalingAnimEase);
            else
                customCurve = AdvancedScrollerScript.ScalingAnimEaseCustom;
            //old but works
            /*if (distance == 0) distance = 0.0001f;
            float normalizedDistance = Mathf.Clamp01(AdvancedScrollerScript.ScalingDistance / distance);
            float curveFactor = customCurve.Evaluate(normalizedDistance);*/
            float normalizedDistance = Mathf.Clamp01(distance / AdvancedScrollerScript.ScalingDistance);
            float curveFactor = customCurve.Evaluate(1 - normalizedDistance);

            //float normalizedDistance = AdvancedScrollerScript.ScalingDistance / distance;
            scale = new Vector2(Mathf.LerpUnclamped(1f, AdvancedScrollerScript.ScalingFactor.x, curveFactor), Mathf.LerpUnclamped(1f, AdvancedScrollerScript.ScalingFactor.y, curveFactor));

            return scale;
        }
        private Vector3 GetScalingPos()
        {
            Vector3 pos = AdvancedScrollerScript.transform.position;

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(AdvancedScrollerScript.SizeDelta.x * (AdvancedScrollerScript.ScalingPosition - 0.5f), pos.y, 0f));
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(pos.x, AdvancedScrollerScript.SizeDelta.y * (AdvancedScrollerScript.ScalingPosition - 0.5f), 0f));
            }
            return pos;
        }

        private void ApplySnaping()
        {
            // to sa musi na koniec tahania spustat asi
            if (ScrollState == scrollState.loading) return;

            if (!AdvancedScrollerScript.Snapping)
                return;

            if (draging || dragingByInheria || dragingByAnim || dragingByScrolling)
                return;

            Transform nearestElement = GetNearestElementToPosition(GetPosFromSlider(AdvancedScrollerScript.SnappingPosition));

            if (AdvancedScrollerScript.MovementType == movementType.Clamped || AdvancedScrollerScript.MovementType == movementType.Elastic)
            {
                float targetPosition = 0;

                // Move the container towards the target position
                if (AdvancedScrollerScript.Type == type.Horizontal)
                {
                    float HalfWidthParent = transform.parent.GetComponent<RectTransform>().rect.width / 2;
                    float HalfWidthContainer = Container.rect.width / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    targetPosition = Container.position.x - (nearestElement.position.x - GetPosFromSlider(AdvancedScrollerScript.SnappingPosition).x);
                    if (Utils.Utils.IsNumberInRange(Container.anchoredPosition.x, -Mathf.Abs(pos), Mathf.Abs(pos)))
                    {
                        Container.position = Vector3.LerpUnclamped(Container.position, new Vector3(targetPosition, Container.position.y, Container.position.z), AdvancedScrollerScript.SnappingSpeed * Time.deltaTime);
                    }
                }
                if (AdvancedScrollerScript.Type == type.Vertical)
                {
                    float HalfWidthParent = transform.parent.GetComponent<RectTransform>().rect.height / 2;
                    float HalfWidthContainer = Container.rect.height / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    targetPosition = Container.position.y - (nearestElement.position.y - GetPosFromSlider(AdvancedScrollerScript.SnappingPosition).y);
                    if (Utils.Utils.IsNumberInRange(Container.anchoredPosition.y, -Mathf.Abs(pos), Mathf.Abs(pos)))
                    {
                        Container.position = Vector3.LerpUnclamped(Container.position, new Vector3(Container.position.x, targetPosition, Container.position.z), AdvancedScrollerScript.SnappingSpeed * Time.deltaTime);
                    }
                }
            }
            else if (AdvancedScrollerScript.MovementType == movementType.Endless)
            {
                // opat beres podla najblizsieho elementu 
                // ale posuvat sa to musi element po elemente nie cely container 
                Vector3 posOfNearestElement = nearestElement.position;

                foreach (var element in AdvancedScrollerScript.Elements)
                {
                    float targetPosition = 0;
                    if (AdvancedScrollerScript.Type == type.Horizontal)
                        targetPosition = element.position.x - (posOfNearestElement.x - GetPosFromSlider(AdvancedScrollerScript.SnappingPosition).x);
                    else if (AdvancedScrollerScript.Type == type.Vertical)
                        targetPosition = element.position.y - (posOfNearestElement.y - GetPosFromSlider(AdvancedScrollerScript.SnappingPosition).y);

                    //Vector3 targetPosition = element.position - (posOfNearestElement - GetPosFromSlider(AdvancedScrollerScript.SnappingPosition));
                    //Debug.Log(element.position + " " + targetPosition);
                    //if (Mathf.Abs(element.position.x - targetPosition.x) > 0.4f)
                    if (AdvancedScrollerScript.Type == type.Horizontal)
                        element.position = Vector3.Lerp(element.position, new Vector3(targetPosition, element.position.y, element.position.z), AdvancedScrollerScript.SnappingSpeed * Time.deltaTime);
                    else if (AdvancedScrollerScript.Type == type.Vertical)
                        element.position = Vector3.Lerp(element.position, new Vector3(element.position.x, targetPosition, element.position.z), AdvancedScrollerScript.SnappingSpeed * Time.deltaTime);
                }
            }
        }
        public Transform GetNearestElementToPosition(Vector3 position)
        {
            Transform nearestElement = null;
            float minDistance = Mathf.Infinity;

            bool horizontall = false;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                horizontall = true;
            else if (AdvancedScrollerScript.Type == type.Vertical)
                horizontall = false;

            foreach (Transform element in AdvancedScrollerScript.Elements)
            {
                float distance = 0;
                if (horizontall)
                    distance = Mathf.Abs(element.position.x - position.x);
                else
                    distance = Mathf.Abs(element.position.y - position.y);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestElement = element;
                }
            }

            return nearestElement;
        }
        private Vector3 GetSnapingPos()
        {
            Vector3 pos = AdvancedScrollerScript.transform.position;

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(AdvancedScrollerScript.SizeDelta.x * (AdvancedScrollerScript.SnappingPosition - 0.5f), 0, 0f));
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(0, AdvancedScrollerScript.SizeDelta.y * (AdvancedScrollerScript.SnappingPosition - 0.5f), 0f));
            }


            return pos;
        }

        private void ApplyFading()
        {
            if (ScrollState == scrollState.loading) return;
            if (!AdvancedScrollerScript.Fading)
            {
                if (/*!Application.isPlaying &&*/ fadingState)
                {
                    fadingState = false;
                    foreach (CanvasGroup element in AdvancedScrollerScript.ElementsCanvasGroup)
                        element.alpha = 1;
                }
                return;
            }

            fadingState = true;
            foreach (CanvasGroup element in AdvancedScrollerScript.ElementsCanvasGroup)
                UpdateElementAlpha(element);
        }
        private void UpdateElementAlpha(CanvasGroup element)
        {
            /*float distance = 0;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.transform.position.x - GetPosFromSlider(AdvancedScrollerScript.FadingPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.transform.position.y - GetPosFromSlider(AdvancedScrollerScript.FadingPosition).y);

            float normalizedDistance = AdvancedScrollerScript.FadingDistance / distance;
            float alpha = Mathf.Lerp(AdvancedScrollerScript.MinAlpha, AdvancedScrollerScript.MaxAlpha, normalizedDistance);*/
            //Debug.Log(scale + " " + normalizedDistance + " distance:" + distance);
            element.alpha = GetAlphaAccordingDistance(element.transform);
        }
        public float GetAlphaAccordingDistance(Transform element)
        {
            float alpha = 1;
            if (!AdvancedScrollerScript.Fading)
                return alpha;

            float distance = 0;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.position.x - GetPosFromSlider(AdvancedScrollerScript.FadingPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.position.y - GetPosFromSlider(AdvancedScrollerScript.FadingPosition).y);

            AnimationCurve customCurve;
            if (AdvancedScrollerScript.EaseTypeFading == easeType.Default)
                customCurve = Utils.Utils.GetEase(AdvancedScrollerScript.FadingAnimEase);
            else
                customCurve = AdvancedScrollerScript.FadingAnimEaseCustom;

            float normalizedDistance = Mathf.Clamp01(distance / AdvancedScrollerScript.FadingDistance);
            //float normalizedDistance = AdvancedScrollerScript.FadingDistance / distance;
            float curveFactor = customCurve.Evaluate(1 - normalizedDistance);
            alpha = Mathf.Lerp(AdvancedScrollerScript.FadingMinAlpha, AdvancedScrollerScript.FadingMaxAlpha, curveFactor);
            return alpha;
        }
        private Vector3 GetFadingPos()
        {
            Vector3 pos = AdvancedScrollerScript.transform.position;

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(AdvancedScrollerScript.SizeDelta.x * (AdvancedScrollerScript.FadingPosition - 0.5f), pos.y, 0f));
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(pos.x, AdvancedScrollerScript.SizeDelta.y * (AdvancedScrollerScript.FadingPosition - 0.5f), 0f));
            }


            return pos;
        }

        private void ApplyDeflection()
        {
            if (ScrollState == scrollState.loading) return;
            if (!AdvancedScrollerScript.Deflection)
            {
                foreach (RectTransform element in AdvancedScrollerScript.Elements)
                {
                    if (AdvancedScrollerScript.Type == type.Horizontal)
                        element.anchoredPosition = new Vector2(element.anchoredPosition.x, 0f);
                    else if (AdvancedScrollerScript.Type == type.Vertical)
                        element.anchoredPosition = new Vector2(0f, element.anchoredPosition.y);
                }
                return;
            }

            foreach (RectTransform element in AdvancedScrollerScript.Elements)
                UpdateElementPos(element);
        }
        private void UpdateElementPos(RectTransform element)
        {
            /*float distance = 0;
            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.transform.position.x - GetPosFromSlider(AdvancedScrollerScript.DeflectPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.transform.position.y - GetPosFromSlider(AdvancedScrollerScript.DeflectPosition).y);
            float normalizedDistance = AdvancedScrollerScript.DeflectDistance / distance;
            float pos = Mathf.Lerp(0f, AdvancedScrollerScript.MaxDeflection, normalizedDistance);*/
            float pos = GetDeflectPosAccordingDistance(element);

            if (AdvancedScrollerScript.Type == type.Horizontal)
                element.anchoredPosition = new Vector3(element.anchoredPosition.x, pos, 0f);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                element.anchoredPosition = new Vector3(pos, element.anchoredPosition.y, 0f);
        }
        public float GetDeflectPosAccordingDistance(RectTransform element)
        {
            float distance = 0;
            if (!AdvancedScrollerScript.Deflection)
                return distance;

            if (AdvancedScrollerScript.Type == type.Horizontal)
                distance = Mathf.Abs(element.transform.position.x - GetPosFromSlider(AdvancedScrollerScript.DeflectPosition).x);
            else if (AdvancedScrollerScript.Type == type.Vertical)
                distance = Mathf.Abs(element.transform.position.y - GetPosFromSlider(AdvancedScrollerScript.DeflectPosition).y);

            AnimationCurve customCurve;
            if (AdvancedScrollerScript.EaseTypeDeflect == easeType.Default)
                customCurve = Utils.Utils.GetEase(AdvancedScrollerScript.DeflectAnimEase);
            else
                customCurve = AdvancedScrollerScript.DeflectAnimEaseCustom;

            float normalizedDistance = distance / AdvancedScrollerScript.DeflectDistance;
            float curveFactor = customCurve.Evaluate(1 - normalizedDistance);
            float pos = Mathf.Lerp(0f, AdvancedScrollerScript.MaxDeflection, curveFactor);

            return pos;
        }

        private Vector3 GetDeflectionPos()
        {
            Vector3 pos = AdvancedScrollerScript.ScrollScript.transform.position;


            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(AdvancedScrollerScript.SizeDelta.x * (AdvancedScrollerScript.DeflectPosition - 0.5f), 0, 0f));
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(0, AdvancedScrollerScript.SizeDelta.y * (AdvancedScrollerScript.DeflectPosition - 0.5f), 0f));
            }


            return pos;
        }

        public Vector3 GetPosFromSlider(float sliderPos)
        {
            if (!AdvancedScrollerScript.AdvancedScrollerRectT)
                AdvancedScrollerScript.AdvancedScrollerRectT = AdvancedScrollerScript.GetComponent<RectTransform>();

            Vector3 pos = transform.position;

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(AdvancedScrollerScript.SizeDelta.x * (sliderPos - AdvancedScrollerScript.AdvancedScrollerRectT.pivot.x), 0, 0f));
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = AdvancedScrollerScript.AdvancedScrollerRectT.TransformPoint(new Vector3(0, AdvancedScrollerScript.SizeDelta.y * (sliderPos - AdvancedScrollerScript.AdvancedScrollerRectT.pivot.y), 0f));
            }

            return pos;
        }

        public Vector2 GetAnchoredPosFromSlider(float sliderPos)
        {
            Vector2 pos = Vector2.zero;

            if (AdvancedScrollerScript.Type == type.Horizontal)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.x, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = new Vector2(AdvancedScrollerScript.SizeDelta.x * (sliderPos), 0f);
            }
            else if (AdvancedScrollerScript.Type == type.Vertical)
            {
                //pos = new Vector3(Mathf.Lerp(0, AdvancedScrollerScript.SizeDelta.y, AdvancedScrollerScript.ScalingPosition), pos.y, 0);
                pos = new Vector2(0f, AdvancedScrollerScript.SizeDelta.y * (sliderPos));
            }

            return pos;
        }

        private void ScrollAnimScaling(bool startDragging)
        {
            if (AdvancedScrollerScript.ScrollingAnimation != scrollAnim.scalling)
                return;

            if (startDragging)
            {
                StartCoroutine(transform.DOScale(AdvancedScrollerScript.ScrollingAnimScale, 0.1f));
            }
            else if (!startDragging)
            {
                StartCoroutine(transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
            }
        }

        public IEnumerator Scrolling(float duration, float speed, ease Ease, Action callback)
        {
            dragingByScrolling = true;

            AnimationCurve customCurve = Utils.Utils.GetEase(Ease);
            duration = Mathf.Abs(duration);
            float time = 0;
            float startSpeed = 0;

            while (time < duration)
            {
                float normalizedTime = Mathf.Clamp01(time / duration);
                float speedFactor = customCurve.Evaluate(normalizedTime);
                float curvedSpeed = Mathf.LerpUnclamped(startSpeed, speed, speedFactor);
                scrollInput = curvedSpeed;

                time += Time.fixedDeltaTime;
                yield return null;
            }


            callback?.Invoke();
            dragingByScrolling = false;
        }
    }

    public enum scrollState
    {
        loading,
        idle,
        scrolling
    }

}



