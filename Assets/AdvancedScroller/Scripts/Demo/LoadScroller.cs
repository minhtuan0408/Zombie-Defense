using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AScroller;

namespace AScroller.Demo
{
    public class LoadScroller : MonoBehaviour
    {
        [SerializeField] private AdvancedScroller AScroller;
        [SerializeField] private GameObject ClickBtn;
        [SerializeField] private GameObject TopBar;

        public void LoadOnBtnClick()
        {
            StartCoroutine(DOFade(ClickBtn.GetComponent<CanvasGroup>(), 0, 0.3f, () =>
            {
                ClickBtn.SetActive(false);

                TopBar.SetActive(true);
                StartCoroutine(DOFade(TopBar.GetComponent<CanvasGroup>(), 1, 0.5f));

                AScroller.gameObject.SetActive(true);
                AScroller.GetComponent<CanvasGroup>().alpha = 1;
                AScroller.LoadScroller(0f);
            }));

        }

        public void BackBtn()
        {
            StartCoroutine(DOFade(TopBar.GetComponent<CanvasGroup>(), 0, 0.2f, () =>
            {
                TopBar.SetActive(false);
            }));

            StartCoroutine(DOFade(AScroller.GetComponent<CanvasGroup>(), 0, 0.2f, () =>
            {
                AScroller.gameObject.SetActive(false);
                ClickBtn.SetActive(true);
                StartCoroutine(DOFade(ClickBtn.GetComponent<CanvasGroup>(), 1, 0.3f));
            }));


        }

        private IEnumerator DOFade(CanvasGroup canvasGroup, float alpha, float duration, Action onComplete = null)
        {
            duration = Mathf.Abs(duration);
            float initialAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            float newAlpha = 0;

            while (elapsedTime < duration)
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                newAlpha = Mathf.LerpUnclamped(initialAlpha, alpha, normalizedTime);
                canvasGroup.alpha = newAlpha;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = alpha;
            onComplete?.Invoke();
        }
    }
}
