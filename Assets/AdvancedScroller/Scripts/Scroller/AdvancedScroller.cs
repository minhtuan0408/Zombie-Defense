// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AScroller.Scroller;
using AScroller.Utils;

namespace AScroller
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AdvancedScroller : MonoBehaviour
    {
        public bool Scrollable = true;
        public type Type;
        public movementType MovementType;
        public float Elasticity = 0.2f;
        public List<RectTransform> Elements;
        public float ScrollSensitivity = 20.0f;
        public padding Padding;
        public float Spacing;
        public bool Mask = true;
        public padding MaskPadding;
        public Vector2Int MaskSoftness;
        public sizeMode ElementsSizeMode;
        public Vector2 ElementsSize = new Vector2(100, 100);
        public alignment Alignment;
        public GameObject StartFromElement;
        public bool ReverseOrder;
        public bool Inertia;
        [Range(0f, 1f)]
        public float DecelerationRate = 0.98f;
        public bool Scaling;
        public float ScalingPosition = 0.5f;
        public Vector2 ScalingFactor = new Vector2(0.8f, 0.8f);
        public float ScalingDistance = 3.5f;
        public ease ScalingAnimEase;
        public AnimationCurve ScalingAnimEaseCustom = AnimationCurve.Linear(0, 0, 1, 1);
        public bool Snapping;
        public float SnappingPosition = 0.5f;
        public float SnappingSpeed = 20;
        public bool Fading;
        public float FadingPosition = 0.5f;
        public float FadingDistance = 0.5f;
        public ease FadingAnimEase;
        public AnimationCurve FadingAnimEaseCustom = AnimationCurve.Linear(0, 0, 1, 1);
        public float FadingMaxAlpha = 0;
        public float FadingMinAlpha = 1;
        public bool Deflection;
        public float DeflectPosition = 0.5f;
        public float DeflectDistance = 0.5f;
        public ease DeflectAnimEase;
        public AnimationCurve DeflectAnimEaseCustom = AnimationCurve.Linear(0, 0, 1, 1);
        public float MaxDeflection = 100f;
        public float PositionOfSelection = 0.5f;
        public bool ShowSelector;
        public Sprite SelectorSprite;
        public Vector2 SelectorSize;
        public Vector2 SelectorOffset;
        public scrollAnim ScrollingAnimation;
        public Vector3 ScrollingAnimScale = new Vector3(0.8f, 0.8f, 0.8f);
        public loadAnim LoadingAnimation;
        public bool LoadingAnimPlayOnStart;
        public float LoadingAnimDuration = 0.5f;
        public float LoadingAnimDelay = 0f;
        public Vector2 LoadingAnimScale;
        public Vector2 LoadingAnimMove;
        public ease LoadingAnimEase;

        [Space(40)]
        public UnityEvent OnScrollChange;
        public UnityEvent OnBeginOfDrag;
        public UnityEvent OnEndOfDrag;

        [HideInInspector] public ContentFitter ContentFitterScript;
        private BoxCollider2D boxCollider;
        [HideInInspector] public RectTransform AdvancedScrollerRectT;
        [HideInInspector] public Scroll ScrollScript;
        [HideInInspector] public List<CanvasGroup> ElementsCanvasGroup = new List<CanvasGroup>();
        private LoadingAnim.LoadingAnim LoadingAnimScript;
        [HideInInspector] public easeType EaseTypeScaling;
        [HideInInspector] public easeType EaseTypeFading;
        [HideInInspector] public easeType EaseTypeDeflect;

        public Vector2 SizeDelta
        {
            get
            {
                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();
                return AdvancedScrollerRectT.sizeDelta;
            }
            set
            {
                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();
                AdvancedScrollerRectT.sizeDelta = value;
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                if (Type != type.Horizontal)
                {
                    Debug.LogError("[AScroll] Cant get verticalNormalizedPosition from Horizoznal scroller, try horizontalNormalizedPosition instead.");
                    return 0;
                }

                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();

                return GetHorizontalNormalizetPosFromPos(ScrollScript.Container.anchoredPosition.x).RoundTo2Decimal();
            }
            set
            {
                if (Type != type.Horizontal)
                {
                    Debug.LogError("[AScroll] Cant set verticalNormalizedPosition from Horizoznal scroller, try horizontalNormalizedPosition instead.");
                    return;
                }

                if (MovementType == movementType.Endless)
                {
                    Debug.LogError("[AScroll] Cant set NormalizedPosition with Endless movementType, try ScrollToElement instead");
                    return;
                }

                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();

                float HalfWidthParent = AdvancedScrollerRectT.rect.width / 2;
                float HalfWidthContainer = ScrollScript.Container.rect.width / 2;
                float pos = HalfWidthContainer - HalfWidthParent;

                float dis = Mathf.Abs(pos) * 2;
                float normalizedPos = Mathf.Clamp01(1 - value);
                float containerPos = -Mathf.Abs(pos) + (dis * normalizedPos);
                ScrollScript.Container.anchoredPosition = new Vector2(containerPos, ScrollScript.Container.anchoredPosition.y);
                //Debug.Log(containerPos);
                //StartCoroutine(ScrollScript.Container.DOAnchoredPos(new Vector2(containerPos, ScrollScript.Container.anchoredPosition.y), 0.5f));
            }
        }
        private float GetHorizontalNormalizetPosFromPos(float xPos)
        {
            float HalfWidthParent = AdvancedScrollerRectT.rect.width / 2;
            float HalfWidthContainer = ScrollScript.Container.rect.width / 2;
            float pos = HalfWidthContainer - HalfWidthParent;

            float dis = Mathf.Abs(pos) * 2;
            //Debug.Log("pos" + pos);
            float containerPos = Mathf.Abs(pos) - xPos;
            float normalizedPos = containerPos / dis;
            return normalizedPos;
        }
        public float verticalNormalizedPosition
        {
            get
            {
                if (Type != type.Vertical)
                {
                    Debug.LogError("[AScroll] Cant get horizontalNormalizedPosition from Vertical scroller, try verticalNormalizedPosition instead.");
                    return 0;
                }

                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();

                return GetVerticalNormalizetPosFromPos(ScrollScript.Container.anchoredPosition.y).RoundTo2Decimal();
            }
            set
            {
                if (Type != type.Vertical)
                {
                    Debug.LogError("[AScroll] Cant get horizontalNormalizedPosition from Vertical scroller, try verticalNormalizedPosition instead.");
                    return;
                }
                if (MovementType == movementType.Endless)
                {
                    Debug.LogError("[AScroll] Cant set NormalizedPosition with Endless movementType, try ScrollToElement instead");
                    return;
                }

                if (AdvancedScrollerRectT == null)
                    AdvancedScrollerRectT = GetComponent<RectTransform>();

                float HalfWidthParent = AdvancedScrollerRectT.rect.height / 2;
                float HalfWidthContainer = ScrollScript.Container.rect.height / 2;
                float pos = HalfWidthContainer - HalfWidthParent;

                float dis = Mathf.Abs(pos) * 2;
                float normalizedPos = Mathf.Clamp01(1 - value);
                float containerPos = -Mathf.Abs(pos) + (dis * normalizedPos);
                ScrollScript.Container.anchoredPosition = new Vector2(ScrollScript.Container.anchoredPosition.x, containerPos);
            }
        }
        private float GetVerticalNormalizetPosFromPos(float yPos)
        {
            float HalfWidthParent = AdvancedScrollerRectT.rect.height / 2;
            float HalfWidthContainer = ScrollScript.Container.rect.height / 2;
            float pos = HalfWidthContainer - HalfWidthParent;

            float dis = Mathf.Abs(pos) * 2;
            //Debug.Log("pos" + pos);
            float containerPos = Mathf.Abs(pos) - yPos;
            float normalizedPos = containerPos / dis;

            return normalizedPos;
        }

        public void ScrollToElement(RectTransform Element, float duration = 0f, Action OnComplete = null)
        {
            if (Type == type.Horizontal)
            {
                if (MovementType == movementType.Clamped || MovementType == movementType.Elastic)
                {
                    float targetNormalizedPos = GetHorizontalNormalizetPosFromPos(-1 * (Element.anchoredPosition.x - DisSelectionPosFromCenter()));
                    //horizontalNormalizedPosition = normalizedPos;

                    float HalfWidthParent = AdvancedScrollerRectT.rect.width / 2;
                    float HalfWidthContainer = ScrollScript.Container.rect.width / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    float dis = Mathf.Abs(pos) * 2;
                    float normalizedPos = Mathf.Clamp01(1 - targetNormalizedPos);
                    float containerPos = -Mathf.Abs(pos) + (dis * normalizedPos);
                    if (duration == 0)
                    {
                        ScrollScript.Container.anchoredPosition = new Vector2(containerPos, ScrollScript.Container.anchoredPosition.y);
                        OnComplete?.Invoke();
                    }
                    else
                        StartCoroutine(ScrollScript.Container.DOAnchoredPos(new Vector2(containerPos, ScrollScript.Container.anchoredPosition.y), duration, OnComplete: OnComplete));
                }
                else if (MovementType == movementType.Endless)
                {
                    float disSelectionElement = ScrollScript.GetPosFromSlider(PositionOfSelection).x - Element.transform.position.x;

                    ScrollScript.dragingByAnim = true;
                    Action onComplete = delegate
                    {
                        ScrollScript.dragingByAnim = false;
                        OnComplete?.Invoke();
                        //onComplete?.Invoke();
                    };
                    foreach (var element in Elements)
                    {
                        Vector3 pos = element.transform.position + new Vector3(disSelectionElement, 0, 0);
                        if (duration == 0)
                            element.transform.position = pos;
                        else
                            StartCoroutine(element.transform.DOMoveXspecial(pos.x, duration, onComplete));
                    }
                }
            }
            else if (Type == type.Vertical)
            {
                if (MovementType == movementType.Clamped || MovementType == movementType.Elastic)
                {
                    float targetNormalizedPos = GetVerticalNormalizetPosFromPos(-1 * (Element.anchoredPosition.y - DisSelectionPosFromCenter()));
                    //verticalNormalizedPosition = normalizedPos;

                    float HalfWidthParent = AdvancedScrollerRectT.rect.height / 2;
                    float HalfWidthContainer = ScrollScript.Container.rect.height / 2;
                    float pos = HalfWidthContainer - HalfWidthParent;

                    float dis = Mathf.Abs(pos) * 2;
                    float normalizedPos = Mathf.Clamp01(1 - targetNormalizedPos);
                    float containerPos = -Mathf.Abs(pos) + (dis * normalizedPos);
                    if (duration == 0)
                    {
                        ScrollScript.Container.anchoredPosition = new Vector2(ScrollScript.Container.anchoredPosition.x, containerPos);
                        OnComplete?.Invoke();
                    }
                    else
                        StartCoroutine(ScrollScript.Container.DOAnchoredPos(new Vector2(ScrollScript.Container.anchoredPosition.x, containerPos), duration, OnComplete: OnComplete));
                }
                else if (MovementType == movementType.Endless)
                {
                    float disSelectionElement = ScrollScript.GetPosFromSlider(PositionOfSelection).y - Element.transform.position.y;

                    ScrollScript.dragingByAnim = true;
                    Action onComplete = delegate
                    {
                        ScrollScript.dragingByAnim = false;
                        OnComplete?.Invoke();
                        //onComplete?.Invoke();
                    };
                    foreach (var element in Elements)
                    {
                        Vector3 pos = element.transform.position + new Vector3(0, disSelectionElement, 0);
                        if (duration == 0)
                            element.transform.position = pos;
                        else
                            StartCoroutine(element.transform.DOMoveYspecial(pos.y, duration, onComplete));
                    }
                }
            }
        }
        private float DisSelectionPosFromCenter()
        {
            float dis = 0;
            if (Type == type.Horizontal)
            {
                dis = ScrollScript.GetAnchoredPosFromSlider(PositionOfSelection).x - (AdvancedScrollerRectT.rect.width / 2);
            }
            else if (Type == type.Vertical)
            {
                dis = ScrollScript.GetAnchoredPosFromSlider(PositionOfSelection).y - (AdvancedScrollerRectT.rect.height / 2);
            }

            return dis;
        }

        public GameObject SelectedElement()
        {
            return ScrollScript.GetNearestElementToPosition(ScrollScript.GetPosFromSlider(PositionOfSelection)).gameObject;
        }

        public void ScrollToNextElement(float duration = 0, Action OnComplete = null)
        {
            RectTransform element = SelectedElement().GetComponent<RectTransform>();

            int index = Elements.IndexOf(element);

            index++;
            if (index >= Elements.Count) index = 0;
            RectTransform nextElement = Elements[index];
            ScrollToElement(nextElement, duration, OnComplete);
        }
        public void ScrollToPreviousElement(float duration = 0, Action OnComplete = null)
        {
            RectTransform element = SelectedElement().GetComponent<RectTransform>();

            int index = Elements.IndexOf(element);

            index--;
            if (index < 0) index = Elements.Count - 1;
            RectTransform previousElement = Elements[index];
            ScrollToElement(previousElement, duration, OnComplete);
        }

        public void DOScroll(float duration = 2, float speed = 1, ease Ease = ease.Linear, Action callback = null)
        {
            StartCoroutine(ScrollScript.Scrolling(duration, speed, Ease, callback));
        }


        private void Awake()
        {
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
            SetElementsList();

            AdvancedScrollerRectT = GetComponent<RectTransform>();
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(AdvancedScrollerRectT.rect.width, AdvancedScrollerRectT.rect.height);
            //ScrollScript = GetComponentInChildren<Scroll>(true);

            LoadingAnimScript = GetComponentInChildren<LoadingAnim.LoadingAnim>(true);

        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            /*if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
            }*/

            /*        if (!ContentFitterScript)
                        ContentFitterScript = transform.GetChild(0).GetComponent<ContentFitter>();
                    ContentFitterScript.UpdateParentSize();

                    SetMask();*/
#endif
        }

        public void SetMask()
        {
            if (!GetComponent<RectMask2D>())
                gameObject.AddComponent<RectMask2D>();

            RectMask2D mask = GetComponent<RectMask2D>();
            mask.enabled = Mask;
            if (Mask)
            {
                mask.padding = new Vector4(MaskPadding.Left, MaskPadding.Bottom, MaskPadding.Right, MaskPadding.Top);
                mask.softness = MaskSoftness;
            }
        }
        public void SetElementsList()
        {
            Elements = new List<RectTransform>();
            for (int i = 0; i < ScrollScript.transform.childCount; i++)
            {
                RectTransform child = ScrollScript.transform.GetChild(i).GetComponent<RectTransform>();
                if (child.gameObject.activeSelf)
                    Elements.Add(child);
            }
        }
        public void SetSelector()
        {
            ScrollScript.Selector.gameObject.SetActive(ShowSelector);

            if (!ShowSelector)
                return;

            if (!ScrollScript.Selector)
            {
                Debug.LogError("[AScroll] Selector image is missing, set AScroll > Elements Container > Sroll script > Selector");
                return;
            }

            ScrollScript.Selector.sprite = SelectorSprite;
            ScrollScript.Selector.transform.position = ScrollScript.GetPosFromSlider(PositionOfSelection);
            RectTransform rt = ScrollScript.Selector.GetComponent<RectTransform>();
            rt.sizeDelta = SelectorSize;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + SelectorOffset.x, rt.anchoredPosition.y + SelectorOffset.y);
        }
        public void LoadScroller(float delay)
        {
            if (LoadingAnimation == loadAnim.none)
            {
                Debug.LogError("[AScroll] Loading Animation is set to None. Make sure to set up loading animation first");
            }

            //LoadingAnimEase = Ease;
            ScrollScript.ScrollState = scrollState.loading;
            LoadingAnimScript.SetScroll();
            StartCoroutine(Utils.Utils.Delay(delay, () => LoadingAnimScript.LoadScroller()));
        }

    }

    [Serializable]
    public struct padding
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
    }
    public enum type
    {
        Horizontal,
        Vertical
    }
    public enum movementType
    {
        Endless,
        Clamped,
        Elastic
    }
    public enum sizeMode
    {
        Fixed,
        Free,
    }
    public enum alignment
    {
        Center,
        Left,
        Right,
        Top,
        Bottom
    }
    public enum scrollAnim
    {
        none,
        scalling,
    }
    public enum loadAnim
    {
        none,
        fade,
        continuousFade,
        scale,
        move,
        //moveAndFade,

    }

    public enum ease
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad, OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InElastic,
        OutElastic,
        InOutElastic,
        InBounce,
        OutBounce,
        InOutBounce,
    }

    public enum easeType
    {
        Default,
        Custom,
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(AdvancedScroller))]
    public class AdvancedScrollerEditor : Editor
    {
        new SerializedObject serializedObject;

        SerializedProperty m_Elements;
        SerializedProperty m_Padding;
        SerializedProperty m_OnScrollChange;
        SerializedProperty m_OnBeginOfDrag;
        SerializedProperty m_OnEndOfDrag;
        SerializedProperty m_MaskPadding;
        SerializedProperty m_SelectorSprite;
        SerializedProperty m_StartFromElement;

        GenericMenu ScalingDropMenu;
        GenericMenu FadingDropMenu;
        GenericMenu DeflectDropMenu;

        private GUIStyle headerStyle;
        private GUIStyle OffsetStyle;
        private GUIStyle OffsetSecondRowStyle;
        private GUIStyle bgColorStyle;

        void OnEnable()
        {
            serializedObject = new SerializedObject(target);
            // Create a custom GUIStyle for section headers
            /*headerStyle = new GUIStyle(EditorStyles.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 14;
            headerStyle.normal.textColor = Color.blue;*/

            // Create a custom GUIStyle for bold labels
            //OffsetStyle = new GUIStyle(EditorStyles.label);
            //OffsetStyle.padding = new RectOffset(20, 0, 0, 0);

            m_Elements = serializedObject.FindProperty("Elements");
            m_Padding = serializedObject.FindProperty("Padding");
            m_OnScrollChange = serializedObject.FindProperty("OnScrollChange");
            m_OnBeginOfDrag = serializedObject.FindProperty("OnBeginOfDrag");
            m_OnEndOfDrag = serializedObject.FindProperty("OnEndOfDrag");
            m_MaskPadding = serializedObject.FindProperty("MaskPadding");
            m_SelectorSprite = serializedObject.FindProperty("SelectorSprite");
            m_StartFromElement = serializedObject.FindProperty("StartFromElement");

        }
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            OffsetStyle = new GUIStyle(EditorStyles.label);
            OffsetStyle.padding = new RectOffset(20, 0, 0, 0);

            OffsetSecondRowStyle = new GUIStyle(EditorStyles.label);
            OffsetSecondRowStyle.padding = new RectOffset(40, 0, 0, 0);

            headerStyle = new GUIStyle(EditorStyles.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 14;
            headerStyle.normal.textColor = new Color32(207, 231, 255, 255);

            bgColorStyle = new GUIStyle(EditorStyles.label);
            bgColorStyle.normal.background = MakeTex(2, 2, new Color32(0, 0, 0, 30)); //new Color32(49, 50, 51, 255)

            AdvancedScroller scroller = (AdvancedScroller)target;

            if (scroller.Elements.Count != Utils.Utils.GetActiveChildrenCount(scroller.ScrollScript.transform))
                scroller.SetElementsList();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("General", headerStyle);
            //EditorGUILayout.PropertyField(m_Elements, new GUIContent("Elements"));
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Scrollable = EditorGUILayout.Toggle("Scrollable", scroller.Scrollable);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Type = (type)EditorGUILayout.EnumPopup("Type", scroller.Type);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.MovementType = (movementType)EditorGUILayout.EnumPopup("Movement Type", scroller.MovementType);
            switch (scroller.MovementType)
            {
                case movementType.Endless:
                    scroller.Alignment = alignment.Center;
                    if (!scroller.ScrollScript)
                        scroller.ScrollScript = scroller.GetComponentInChildren<Scroll>(true);
                    scroller.ScrollScript.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    break;

                case movementType.Clamped:
                    // Draw other fields related to Clamped scroller
                    break;

                case movementType.Elastic:
                    //scroller.showElasticity = EditorGUILayout.Toggle("Show Elasticity", scroller.showElasticity);
                    EditorGUILayout.BeginVertical(OffsetStyle);
                    scroller.Elasticity = EditorGUILayout.FloatField("Elasticity", scroller.Elasticity);
                    EditorGUILayout.EndVertical();

                    break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.ScrollSensitivity = EditorGUILayout.FloatField("Scroll Sensitivity", scroller.ScrollSensitivity);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.LoadingAnimation = (loadAnim)EditorGUILayout.EnumPopup("Loading Animation", scroller.LoadingAnimation);
            if (scroller.LoadingAnimation != loadAnim.none)
            {
                EditorGUILayout.BeginVertical(OffsetStyle);
                scroller.LoadingAnimPlayOnStart = EditorGUILayout.Toggle("Play on Start", scroller.LoadingAnimPlayOnStart);
                EditorGUILayout.EndVertical();
                scroller.ElementsCanvasGroup.Clear();
                foreach (var element in scroller.Elements)
                {
                    if (!element.GetComponent<CanvasGroup>())
                        element.gameObject.AddComponent<CanvasGroup>();

                    scroller.ElementsCanvasGroup.Add(element.GetComponent<CanvasGroup>());
                }
            }
            EditorGUILayout.BeginVertical(OffsetSecondRowStyle);
            if (scroller.LoadingAnimPlayOnStart)
            {
                scroller.LoadingAnimDelay = EditorGUILayout.FloatField("Delay", scroller.LoadingAnimDelay);
                if (scroller.LoadingAnimDelay < 0) scroller.LoadingAnimDelay = 0;
            }
            EditorGUILayout.EndVertical();
            if (scroller.LoadingAnimation != loadAnim.none)
            {
                EditorGUILayout.BeginVertical(OffsetStyle);
                scroller.LoadingAnimEase = (ease)EditorGUILayout.EnumPopup("Ease", scroller.LoadingAnimEase);
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(OffsetStyle);
                scroller.LoadingAnimDuration = EditorGUILayout.FloatField("Duration", scroller.LoadingAnimDuration);
                EditorGUILayout.EndVertical();
            }
            switch (scroller.LoadingAnimation)
            {
                case loadAnim.none:
                    scroller.LoadingAnimPlayOnStart = false;
                    break;

                case loadAnim.fade:
                    break;
                case loadAnim.scale:
                    EditorGUILayout.BeginVertical(OffsetStyle);
                    scroller.LoadingAnimScale = EditorGUILayout.Vector2Field("Scale", scroller.LoadingAnimScale);
                    EditorGUILayout.EndVertical();
                    break;
                case loadAnim.move:
                    EditorGUILayout.BeginVertical(OffsetStyle);
                    scroller.LoadingAnimMove = EditorGUILayout.Vector2Field("Move", scroller.LoadingAnimMove);
                    EditorGUILayout.EndVertical();
                    break;
            }
            EditorGUILayout.EndVertical();

            /*EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.ScrollingAnimation = (scrollAnim)EditorGUILayout.EnumPopup("Scrolling Animation", scroller.ScrollingAnimation);
            switch (scroller.ScrollingAnimation)
            {
                case scrollAnim.none:
                    break;

                case scrollAnim.scalling:
                    EditorGUILayout.BeginVertical(OffsetStyle);
                    scroller.ScrollingAnimScale = EditorGUILayout.Vector3Field("Scale", scroller.ScrollingAnimScale);
                    EditorGUILayout.EndVertical();
                    break;


            }
            EditorGUILayout.EndVertical();*/
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Elements", headerStyle);
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.ElementsSizeMode = (sizeMode)EditorGUILayout.EnumPopup("Element Size Mode", scroller.ElementsSizeMode);
            switch (scroller.ElementsSizeMode)
            {
                case sizeMode.Fixed:
                    EditorGUILayout.BeginHorizontal(OffsetStyle);
                    scroller.ElementsSize = EditorGUILayout.Vector2Field("Elements Size", scroller.ElementsSize);
                    EditorGUILayout.EndHorizontal();
                    break;
                case sizeMode.Free:
                    break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            switch (scroller.MovementType)
            {
                case movementType.Endless:
                    EditorGUILayout.PropertyField(m_StartFromElement, new GUIContent("Start From"));
                    break;
                case movementType.Clamped:
                    scroller.Alignment = (alignment)EditorGUILayout.EnumPopup("Alignment", scroller.Alignment);
                    break;
                case movementType.Elastic:
                    scroller.Alignment = (alignment)EditorGUILayout.EnumPopup("Alignment", scroller.Alignment);
                    break;
            }
            #region warning message for alignment
            if (scroller.Type == type.Horizontal)
            {
                if (scroller.Alignment == alignment.Top || scroller.Alignment == alignment.Bottom)
                    EditorGUILayout.HelpBox("Top and Bottom Alignment works only for Vertical Scroller", MessageType.Warning);
            }
            else if (scroller.Type == type.Vertical)
            {
                if (scroller.Alignment == alignment.Left || scroller.Alignment == alignment.Right)
                    EditorGUILayout.HelpBox("Left and Right Alignment works only for Horizontal Scroller", MessageType.Warning);
            }
            #endregion
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            EditorGUILayout.PropertyField(m_Padding, new GUIContent("Padding"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Spacing = EditorGUILayout.FloatField("Spacing", scroller.Spacing);
            EditorGUILayout.EndVertical();
            //scroller.ReverseOrder = EditorGUILayout.Toggle("Reverse Order", scroller.ReverseOrder);
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Others", headerStyle);
            #region selection pos
            EditorGUILayout.BeginHorizontal(bgColorStyle);
            EditorGUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("Position Of Selection", "Determines the position of the element that is selected"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            float newSelectionPosition = EditorGUILayout.FloatField("", scroller.PositionOfSelection, GUILayout.MaxWidth(50));
            newSelectionPosition = Mathf.Clamp01(newSelectionPosition);
            if (newSelectionPosition != scroller.PositionOfSelection)
                scroller.PositionOfSelection = newSelectionPosition;
            scroller.PositionOfSelection = GUILayout.HorizontalSlider((float)Mathf.Round(scroller.PositionOfSelection * 100f) / 100f, 0f, 1f, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Inertia = EditorGUILayout.Toggle("Inertia", scroller.Inertia);
            if (scroller.Inertia)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.DecelerationRate = EditorGUILayout.FloatField("Deceleration Rate", scroller.DecelerationRate);
                scroller.DecelerationRate = Mathf.Clamp(scroller.DecelerationRate, 0f, 1f);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Scaling = EditorGUILayout.Toggle("Scaling", scroller.Scaling);
            if (scroller.Scaling)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Scaling Position", "Specifies the scaling position of the element"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                float newScalingPosition = EditorGUILayout.FloatField("", scroller.ScalingPosition, GUILayout.MaxWidth(50));
                newScalingPosition = Mathf.Clamp01(newScalingPosition);
                if (newScalingPosition != scroller.ScalingPosition)
                    scroller.ScalingPosition = newScalingPosition;
                scroller.ScalingPosition = GUILayout.HorizontalSlider((float)Mathf.Round(scroller.ScalingPosition * 100f) / 100f, 0f, 1f, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                #region menu 
                ScalingDropMenu = new GenericMenu();
                ScalingDropMenu.AddItem(new GUIContent("Defaults Ease"), false, () => { scroller.EaseTypeScaling = easeType.Default; });
                ScalingDropMenu.AddItem(new GUIContent("Custom Ease"), false, () => { scroller.EaseTypeScaling = easeType.Custom; });
                #endregion

                if (scroller.EaseTypeScaling == easeType.Default)
                    scroller.ScalingAnimEase = (ease)EditorGUILayout.EnumPopup("Ease", scroller.ScalingAnimEase);
                else
                    scroller.ScalingAnimEaseCustom = EditorGUILayout.CurveField("Ease", scroller.ScalingAnimEaseCustom);

                if (EditorGUILayout.DropdownButton(new GUIContent(""), FocusType.Passive, GUILayout.Width(20)))
                    ScalingDropMenu.ShowAsContext();

                EditorGUILayout.EndHorizontal();
                scroller.ScalingFactor = EditorGUILayout.Vector2Field(new GUIContent("Scaling Factor", "Element will be scaled from 1 to this value"), scroller.ScalingFactor, GUILayout.ExpandWidth(true));
                scroller.ScalingDistance = EditorGUILayout.FloatField(
                    new GUIContent("Distance", "Element starts to scale according to the distance from Scaling Position"),
                    scroller.ScalingDistance, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Snapping = EditorGUILayout.Toggle(new GUIContent("Snapping", "Snap elements to snaping position"), scroller.Snapping);
            if (scroller.Snapping)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Snapping Position", "Specifies the snap position of the elements"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                float newSnappingPosition = EditorGUILayout.FloatField("", scroller.SnappingPosition, GUILayout.MaxWidth(50));
                newSnappingPosition = Mathf.Clamp01(newSnappingPosition);
                if (newSnappingPosition != scroller.SnappingPosition)
                    scroller.SnappingPosition = newSnappingPosition;
                scroller.SnappingPosition = GUILayout.HorizontalSlider((float)Mathf.Round(scroller.SnappingPosition * 100f) / 100f, 0f, 1f, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.SnappingSpeed = EditorGUILayout.FloatField(new GUIContent("Speed", "Speed of snap animation"), scroller.SnappingSpeed, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Fading = EditorGUILayout.Toggle("Fading", scroller.Fading);
            if (scroller.Fading)
            {
                scroller.ElementsCanvasGroup.Clear();
                foreach (var element in scroller.Elements)
                {
                    if (!element.GetComponent<CanvasGroup>())
                        element.gameObject.AddComponent<CanvasGroup>();

                    scroller.ElementsCanvasGroup.Add(element.GetComponent<CanvasGroup>());
                }
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Fading Position", "Specifies position for fading"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                float newFadingPosition = EditorGUILayout.FloatField("", scroller.FadingPosition, GUILayout.MaxWidth(50));
                newFadingPosition = Mathf.Clamp01(newFadingPosition);
                if (newFadingPosition != scroller.FadingPosition)
                    scroller.FadingPosition = newFadingPosition;
                scroller.FadingPosition = GUILayout.HorizontalSlider((float)Mathf.Round(scroller.FadingPosition * 100f) / 100f, 0f, 1f, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(OffsetStyle);
                #region menu 
                FadingDropMenu = new GenericMenu();
                FadingDropMenu.AddItem(new GUIContent("Defaults Ease"), false, () => { scroller.EaseTypeFading = easeType.Default; });
                FadingDropMenu.AddItem(new GUIContent("Custom Ease"), false, () => { scroller.EaseTypeFading = easeType.Custom; });
                #endregion

                if (scroller.EaseTypeFading == easeType.Default)
                    scroller.FadingAnimEase = (ease)EditorGUILayout.EnumPopup("Ease", scroller.FadingAnimEase);
                else
                    scroller.FadingAnimEaseCustom = EditorGUILayout.CurveField("Ease", scroller.FadingAnimEaseCustom);

                if (EditorGUILayout.DropdownButton(new GUIContent(""), FocusType.Passive, GUILayout.Width(20)))
                    FadingDropMenu.ShowAsContext();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.FadingDistance = EditorGUILayout.FloatField(new GUIContent("Fading Distance", "Fading is applied according distance from fading position"), scroller.FadingDistance, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.FadingMinAlpha = EditorGUILayout.FloatField(new GUIContent("Min Alpha", "Alpha of the most distant elements"), scroller.FadingMinAlpha, GUILayout.ExpandWidth(true));
                scroller.FadingMinAlpha = Mathf.Clamp01(scroller.FadingMinAlpha);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.FadingMaxAlpha = EditorGUILayout.FloatField(new GUIContent("Max Alpha", "Alpha of nearest elements from fading position"), scroller.FadingMaxAlpha, GUILayout.ExpandWidth(true));
                scroller.FadingMaxAlpha = Mathf.Clamp01(scroller.FadingMaxAlpha);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Deflection = EditorGUILayout.Toggle("Deflection", scroller.Deflection);
            if (scroller.Deflection)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.BeginVertical();
                GUILayout.Label(new GUIContent("Deflect Position", "Determines the deflection position of elements"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                float newDeflectPosition = EditorGUILayout.FloatField("", scroller.DeflectPosition, GUILayout.MaxWidth(50));
                newDeflectPosition = Mathf.Clamp01(newDeflectPosition);
                if (newDeflectPosition != scroller.DeflectPosition)
                    scroller.DeflectPosition = newDeflectPosition;
                scroller.DeflectPosition = GUILayout.HorizontalSlider((float)Mathf.Round(scroller.DeflectPosition * 100f) / 100f, 0f, 1f, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(OffsetStyle);
                #region menu 
                DeflectDropMenu = new GenericMenu();
                DeflectDropMenu.AddItem(new GUIContent("Defaults Ease"), false, () => { scroller.EaseTypeDeflect = easeType.Default; });
                DeflectDropMenu.AddItem(new GUIContent("Custom Ease"), false, () => { scroller.EaseTypeDeflect = easeType.Custom; });
                #endregion

                if (scroller.EaseTypeDeflect == easeType.Default)
                    scroller.DeflectAnimEase = (ease)EditorGUILayout.EnumPopup("Ease", scroller.DeflectAnimEase);
                else
                    scroller.DeflectAnimEaseCustom = EditorGUILayout.CurveField("Ease", scroller.DeflectAnimEaseCustom);

                if (EditorGUILayout.DropdownButton(new GUIContent(""), FocusType.Passive, GUILayout.Width(20)))
                    DeflectDropMenu.ShowAsContext();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.MaxDeflection = EditorGUILayout.FloatField("Max Deflection", scroller.MaxDeflection, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.DeflectDistance = EditorGUILayout.FloatField(new GUIContent("Deflect Distance", "Distance from the deflecting position from which the elements start to deflect"), scroller.DeflectDistance, GUILayout.ExpandWidth(true));
                if (scroller.DeflectDistance <= 0)
                    scroller.DeflectDistance = 0.01f;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.Mask = EditorGUILayout.Toggle("Mask", scroller.Mask);
            if (scroller.Mask)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.PropertyField(m_MaskPadding, new GUIContent("Padding"));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.MaskSoftness = EditorGUILayout.Vector2IntField("Softness", scroller.MaskSoftness);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(bgColorStyle);
            scroller.ShowSelector = EditorGUILayout.Toggle("Show Selector", scroller.ShowSelector);
            if (scroller.ShowSelector)
            {
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.SelectorSize = EditorGUILayout.Vector2Field("Size", scroller.SelectorSize, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                scroller.SelectorOffset = EditorGUILayout.Vector2Field("Offset", scroller.SelectorOffset, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(OffsetStyle);
                EditorGUILayout.PropertyField(m_SelectorSprite, new GUIContent("Selector Sprite"));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(m_OnScrollChange, new GUIContent("OnScrollChange"));
            EditorGUILayout.PropertyField(m_OnBeginOfDrag, new GUIContent("OnBeginOfDrag"));
            EditorGUILayout.PropertyField(m_OnEndOfDrag, new GUIContent("OnEndOfDrag"));

            if (!scroller.ContentFitterScript)
                scroller.ContentFitterScript = scroller.GetComponentInChildren<ContentFitter>(true);
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                scroller.ContentFitterScript.SetStartingPosAccordingAlignment();
            }

            scroller.ContentFitterScript.UpdateParentSize();
            scroller.SetMask();
            scroller.SetSelector();


            // Apply the modified properties
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                //serializedObject.Update();
                EditorUtility.SetDirty(target);
            }
        }
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
#endif
}

