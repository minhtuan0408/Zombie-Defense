// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AScroller.Utils;
using System;
using AScroller.Scroller;

namespace AScroller.LoadingAnim
{
    public class LoadingAnim : MonoBehaviour
    {
        private AdvancedScroller AdvancedScrollerScript;
        private Scroll ScrollScript;

        private void Awake()
        {
            AdvancedScrollerScript = transform.parent.GetComponent<AdvancedScroller>();
            ScrollScript = transform.GetComponent<Scroll>();

            if (AdvancedScrollerScript.LoadingAnimation != loadAnim.none && AdvancedScrollerScript.LoadingAnimPlayOnStart)
                SetScroll();
            else if (!AdvancedScrollerScript.LoadingAnimPlayOnStart)
                ScrollScript.ScrollState = scrollState.idle;

        }

        private void Start()
        {
            if (AdvancedScrollerScript.LoadingAnimPlayOnStart && AdvancedScrollerScript.LoadingAnimDelay == 0)
                LoadScroller();
            else if (AdvancedScrollerScript.LoadingAnimPlayOnStart && AdvancedScrollerScript.LoadingAnimDelay > 0)
                StartCoroutine(Utils.Utils.Delay(AdvancedScrollerScript.LoadingAnimDelay, () => LoadScroller()));
        }


        public void SetScroll()
        {
            foreach (var element in AdvancedScrollerScript.ElementsCanvasGroup)
                element.alpha = 0;

            if (AdvancedScrollerScript.LoadingAnimation == loadAnim.scale)
                foreach (var element in AdvancedScrollerScript.Elements)
                    element.transform.localScale = new Vector3(AdvancedScrollerScript.LoadingAnimScale.x, AdvancedScrollerScript.LoadingAnimScale.y, 1);

            if (AdvancedScrollerScript.LoadingAnimation == loadAnim.move)
                foreach (var element in AdvancedScrollerScript.Elements)
                    element.anchoredPosition += new Vector2(AdvancedScrollerScript.LoadingAnimMove.x, AdvancedScrollerScript.LoadingAnimMove.y);

            if (AdvancedScrollerScript.ShowSelector)
            {
                if (!ScrollScript.Selector.GetComponent<CanvasGroup>())
                    ScrollScript.Selector.gameObject.AddComponent<CanvasGroup>();

                CanvasGroup cg = ScrollScript.Selector.GetComponent<CanvasGroup>();
                cg.alpha = 0;
            }
        }

        public void LoadScroller()
        {
            ScrollScript.ScrollState = scrollState.loading;
            switch (AdvancedScrollerScript.LoadingAnimation)
            {
                case loadAnim.none:
                    break;
                case loadAnim.fade:
                    FadingLoad();
                    break;
                case loadAnim.continuousFade:
                    ContinuousFadingLoad();
                    break;
                case loadAnim.scale:
                    ScaleLoad();
                    break;
                case loadAnim.move:
                    MoveLoad();
                    break;
                default:
                    break;
            }
        }

        private void LoadSelector(Action callback = null)
        {
            if (AdvancedScrollerScript.ShowSelector)
                StartCoroutine(ScrollScript.Selector.GetComponent<CanvasGroup>().DOFade(1, 0.2f, () => callback?.Invoke()));
            else
                callback?.Invoke();
        }

        private void FadingLoad()
        {
            // toto ked sa skonci musi prist nastavenie ze  ScrollScript.ScrollState = scrollState.idle; len teraz by sa mi to volalo podla toho kolko mam elementov a to je zbytocne treba to nejak prerobit aby sa len raz 
            float duration = AdvancedScrollerScript.LoadingAnimDuration;
            StartCoroutine(Utils.Utils.Delay(duration + 0.1f, delegate
            {
                LoadSelector();
                ScrollScript.ScrollState = scrollState.idle;
            }));
            foreach (var element in AdvancedScrollerScript.ElementsCanvasGroup)
            {
                StartCoroutine(element.DOFade(ScrollScript.GetAlphaAccordingDistance(element.transform), duration, Ease: AdvancedScrollerScript.LoadingAnimEase));
            }
        }
        private void ContinuousFadingLoad()
        {
            Tween newTween;
            if (!gameObject.GetComponent<Tween>())
                newTween = gameObject.AddComponent<Tween>(); //new Tween;
            else
            {
                newTween = gameObject.GetComponent<Tween>();
                newTween.ClearSequence();
            }

            newTween.OnComplete(() =>
            {
                LoadSelector();
                ScrollScript.ScrollState = scrollState.idle;
                //Debug.Log("Hotovo");
            });
            /*newTween.Append(delegate { Debug.Log("1"); });
            newTween.Append(delegate { Debug.Log("2"); });
            newTween.Append(delegate { Debug.Log("3"); });
            newTween.Append(delegate { Debug.Log("4"); });
            newTween.Append(delegate { Debug.Log("5"); });*/
            foreach (var element in AdvancedScrollerScript.ElementsCanvasGroup)
            {
                newTween.Append(() => StartCoroutine(element.DOFade(ScrollScript.GetAlphaAccordingDistance(element.transform), AdvancedScrollerScript.LoadingAnimDuration, Ease: AdvancedScrollerScript.LoadingAnimEase)));
            }

            newTween.Play();
        }
        private void ScaleLoad()
        {
            Tween newTween;
            if (!gameObject.GetComponent<Tween>())
                newTween = gameObject.AddComponent<Tween>(); //new Tween;
            else
            {
                newTween = gameObject.GetComponent<Tween>();
                newTween.ClearSequence();
            }

            newTween.OnComplete(() =>
            {
                LoadSelector();
                ScrollScript.ScrollState = scrollState.idle;
            });

            /*foreach (var element in AdvancedScrollerScript.ElementsCanvasGroup)
            {
                newTween.Append(() => StartCoroutine(element.DOFade(ScrollScript.GetAlphaAccordingDistance(element.transform), 0.5f)));
            }*/
            foreach (var element in AdvancedScrollerScript.Elements)
            {
                Vector2 scale = ScrollScript.GetScaleAccordingDistance(element.transform);
                Vector3 v = new Vector3(scale.x, scale.y, 1);
                newTween.Append(() =>
                {
                    StartCoroutine(element.GetComponent<CanvasGroup>().DOFade(ScrollScript.GetAlphaAccordingDistance(element.transform), AdvancedScrollerScript.LoadingAnimDuration));
                    StartCoroutine(element.DOScale(v, AdvancedScrollerScript.LoadingAnimDuration, Ease: AdvancedScrollerScript.LoadingAnimEase));
                });
            }


            newTween.Play();
        }
        private void MoveLoad()
        {
            Tween newTween;
            if (!gameObject.GetComponent<Tween>())
                newTween = gameObject.AddComponent<Tween>(); //new Tween;
            else
            {
                newTween = gameObject.GetComponent<Tween>();
                newTween.ClearSequence();
            }

            newTween.OnComplete(() =>
            {
                LoadSelector();
                ScrollScript.ScrollState = scrollState.idle;
            });

            foreach (var element in AdvancedScrollerScript.Elements)
            {
                float dis = ScrollScript.GetDeflectPosAccordingDistance(element);
                Vector2 pos = Vector2.zero;
                if (AdvancedScrollerScript.Type == type.Horizontal)
                    pos = new Vector3(element.anchoredPosition.x + (-1 * AdvancedScrollerScript.LoadingAnimMove.x), dis, 1);
                else if (AdvancedScrollerScript.Type == type.Vertical)
                    pos = new Vector3(dis, element.anchoredPosition.y + (-1 * AdvancedScrollerScript.LoadingAnimMove.y), 1);

                newTween.Append(() =>
                {
                    StartCoroutine(element.GetComponent<CanvasGroup>().DOFade(ScrollScript.GetAlphaAccordingDistance(element.transform), AdvancedScrollerScript.LoadingAnimDuration));

                    StartCoroutine(element.DOAnchoredPos(pos, AdvancedScrollerScript.LoadingAnimDuration, Ease: AdvancedScrollerScript.LoadingAnimEase));
                });
            }


            newTween.Play();
        }
    }
}
