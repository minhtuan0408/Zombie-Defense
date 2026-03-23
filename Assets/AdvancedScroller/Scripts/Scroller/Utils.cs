// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AScroller;

namespace AScroller.Utils
{
    public static class Utils
    {
        private static Animations AnimationsScriptable;
        private static bool DebugAllowed = false;
        //private static Camera Cam = Camera.main;
        //private static RectTransform CamRect = Cam.GetComponent<RectTransform>();

        /*
        public static Vector2 ConvertCanvasToWorld(Vector2 point)
        {
             * Vector3 worldPoint;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(CamRect, point, Cam, out worldPoint);

            return worldPoint;
        }
        */
        public static void Initialize()
        {
            AnimationsScriptable = Resources.Load<Animations>("Animations/Animations");
            if (AnimationsScriptable == null)
            {
                Debug.LogError("[AScroll] Failed to load Animations");
            }
        }

        public static IEnumerator DOAnchoredPos(this RectTransform rectTransform, Vector2 targetPos, float duration, float delay = 0, ease Ease = ease.Linear, Action OnComplete = null)
        {
            yield return new WaitForSeconds(delay);

            AnimationCurve customCurve = GetEase(Ease);

            duration = Mathf.Abs(duration);
            float time = 0;
            Vector2 startPosition = rectTransform.anchoredPosition;

            while (time < duration)
            {
                float normalizedTime = Mathf.Clamp01(time / duration);
                float moveFactor = customCurve.Evaluate(normalizedTime);
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPos, moveFactor);
                time += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = targetPos;
            OnComplete?.Invoke();
        }
        public static IEnumerator DOMove(this Transform transform, Vector3 targetPosition, float duration, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            duration = Mathf.Abs(duration);
            Vector3 initialPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.LerpUnclamped(initialPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the final position is exactly the target position
            transform.position = targetPosition;
        }
        public static IEnumerator DOMoveX(this Transform transform, float targetXposition, float duration, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            duration = Mathf.Abs(duration);
            float initialXposition = transform.position.x;
            float elapsedTime = 0f;
            float xPos = 0;

            while (elapsedTime < duration)
            {
                xPos = Mathf.LerpUnclamped(initialXposition, targetXposition, elapsedTime / duration);
                transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the final position is exactly the target position
            transform.position = new Vector3(targetXposition, transform.position.y, transform.position.z);
        }
        public static IEnumerator DOMoveXspecial(this Transform transform, float targetXposition, float duration, Action onComplete = null, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            float initialPosition = transform.position.x;
            float elapsedTime = 0f;
            float distanceCovered = 0f; // Track the distance covered so far
            float totalDistance = targetXposition - transform.position.x;
            //Debug.Log(totalDistance);
            duration = Mathf.Abs(duration);

            while (elapsedTime < duration && Mathf.Abs(distanceCovered) < Mathf.Abs(totalDistance))
            {
                float distanceThisFrame = (totalDistance / duration) * Time.deltaTime;
                distanceCovered += distanceThisFrame;

                transform.position = new Vector3(transform.position.x + distanceThisFrame, transform.position.y, transform.position.z);
                elapsedTime += Time.deltaTime;

                yield return null; // Wait for the next frame
            }

            onComplete?.Invoke();
        }
        public static IEnumerator DOMoveYspecial(this Transform transform, float targetYposition, float duration, Action onComplete = null, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            float initialPosition = transform.position.y;
            float elapsedTime = 0f;
            float distanceCovered = 0f; // Track the distance covered so far
            float totalDistance = targetYposition - transform.position.y;
            //Debug.Log(totalDistance);
            duration = Mathf.Abs(duration);

            while (elapsedTime < duration && Mathf.Abs(distanceCovered) < Mathf.Abs(totalDistance))
            {
                float distanceThisFrame = (totalDistance / duration) * Time.deltaTime;
                distanceCovered += distanceThisFrame;

                transform.position = new Vector3(transform.position.x, transform.position.y + distanceThisFrame, transform.position.z);
                elapsedTime += Time.deltaTime;

                yield return null; // Wait for the next frame
            }

            onComplete?.Invoke();
        }
        public static IEnumerator DOScale(this Transform transform, Vector3 targetScale, float duration, Action onComplete = null, float delay = 0, ease Ease = ease.Linear)
        {
            yield return new WaitForSeconds(delay);

            AnimationCurve customCurve = GetEase(Ease);
            duration = Mathf.Abs(duration);
            float time = 0;
            Vector3 startPosition = transform.localScale;
            while (time < duration)
            {
                float normalizedTime = Mathf.Clamp01(time / duration);
                float scaleFactor = customCurve.Evaluate(normalizedTime);
                transform.localScale = Vector3.LerpUnclamped(startPosition, targetScale, scaleFactor);
                time += Time.deltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
            onComplete?.Invoke();
        }
        public static IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
        public static IEnumerator DOFade(this CanvasGroup canvasGroup, float alpha, float duration, Action onComplete = null, float delay = 0, ease Ease = ease.Linear)
        {
            yield return new WaitForSeconds(delay);

            AnimationCurve customCurve = GetEase(Ease);
            duration = Mathf.Abs(duration);
            float initialAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            float newAlpha = 0;

            while (elapsedTime < duration)
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                float fadeFactor = customCurve.Evaluate(normalizedTime);
                newAlpha = Mathf.LerpUnclamped(initialAlpha, alpha, fadeFactor);
                canvasGroup.alpha = newAlpha;
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the final position is exactly the target position
            canvasGroup.alpha = alpha;
            onComplete?.Invoke();
        }

        public static Tween OnComplete(this Tween tween, Action callback)
        {
            return tween.onComplete(callback);
        }
        public static Tween SetDelay(this Tween tween, float delay)
        {
            return tween.setDelay(delay);
        }

        public static bool IsNumberInRange(float number, float minExclusive, float maxExclusive)
        {
            return number > minExclusive && number < maxExclusive;
        }

        public static float RoundTo2Decimal(this float f)
        {
            return Mathf.Round(f * 100f) / 100f;
        }

        public static AnimationCurve GetEase(ease Ease)
        {
            if (!AnimationsScriptable)
                Initialize();

            AnimationCurve curve = null;

            switch (Ease)
            {
                case ease.Linear:
                    curve = AnimationsScriptable.Linear;
                    break;
                case ease.InSine:
                    curve = AnimationsScriptable.InSine;
                    break;
                case ease.OutSine:
                    curve = AnimationsScriptable.OutSine;
                    break;
                case ease.InOutSine:
                    curve = AnimationsScriptable.InOutSine;
                    break;
                case ease.InQuad:
                    curve = AnimationsScriptable.InQuad;
                    break;
                case ease.OutQuad:
                    curve = AnimationsScriptable.OutQuad;
                    break;
                case ease.InOutQuad:
                    curve = AnimationsScriptable.InOutQuad;
                    break;
                case ease.InCubic:
                    curve = AnimationsScriptable.InCubic;
                    break;
                case ease.OutCubic:
                    curve = AnimationsScriptable.OutCubic;
                    break;
                case ease.InOutCubic:
                    curve = AnimationsScriptable.InOutCubic;
                    break;
                case ease.InQuart:
                    curve = AnimationsScriptable.InQuart;
                    break;
                case ease.OutQuart:
                    curve = AnimationsScriptable.OutQuart;
                    break;
                case ease.InOutQuart:
                    curve = AnimationsScriptable.InOutQuart;
                    break;
                case ease.InQuint:
                    curve = AnimationsScriptable.InQuint;
                    break;
                case ease.OutQuint:
                    curve = AnimationsScriptable.OutQuint;
                    break;
                case ease.InOutQuint:
                    curve = AnimationsScriptable.InOutQuint;
                    break;
                case ease.InExpo:
                    curve = AnimationsScriptable.InExpo;
                    break;
                case ease.OutExpo:
                    curve = AnimationsScriptable.OutExpo;
                    break;
                case ease.InOutExpo:
                    curve = AnimationsScriptable.InOutExpo;
                    break;
                case ease.InCirc:
                    curve = AnimationsScriptable.InCirc;
                    break;
                case ease.OutCirc:
                    curve = AnimationsScriptable.OutCirc;
                    break;
                case ease.InOutCirc:
                    curve = AnimationsScriptable.InOutCirc;
                    break;
                case ease.InBack:
                    curve = AnimationsScriptable.InBack;
                    break;
                case ease.OutBack:
                    curve = AnimationsScriptable.OutBack;
                    break;
                case ease.InOutBack:
                    curve = AnimationsScriptable.InOutBack;
                    break;
                case ease.InElastic:
                    curve = AnimationsScriptable.InElastic;
                    break;
                case ease.OutElastic:
                    curve = AnimationsScriptable.OutElastic;
                    break;
                case ease.InOutElastic:
                    curve = AnimationsScriptable.InOutElastic;
                    break;
                case ease.InBounce:
                    curve = AnimationsScriptable.InBounce;
                    break;
                case ease.OutBounce:
                    curve = AnimationsScriptable.OutBounce;
                    break;
                case ease.InOutBounce:
                    curve = AnimationsScriptable.InOutBounce;
                    break;
                default:
                    curve = AnimationsScriptable.Linear;
                    break;
            }

            return curve;
        }

        public static int GetActiveChildrenCount(Transform parent)
        {
            int count = 0;

            foreach (Transform child in parent)
                if (child.gameObject.activeSelf)
                    count++;

            return count;
        }

        public static void DebugLog(string message)
        {
            if (DebugAllowed)
                Debug.LogError(string.Format("[AScroller] {0}", message));
        }
    }
}

