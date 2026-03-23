using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AScroller;

namespace AScroller.Demo
{
    public class Slots : MonoBehaviour
    {
        public List<AdvancedScroller> AScrollers;


        public void Spin()
        {
            float delay = 0;
            foreach (var scroll in AScrollers)
            {
                StartCoroutine(Delay(delay, () => scroll.DOScroll(UnityEngine.Random.Range(2.5f, 3.5f), -3.2f, ease.Linear)));
                delay += 0.3f;
            }
        }

        public IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}
