// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace AScroller
{
    public class Tween : MonoBehaviour
    {
        private IEnumerator currentCoroutine;

        private List<Action> Sequence = new List<Action>();
        private Action onCompleteAction = null;
        private float delay;


        public void Insert(int position, Action toDo)
        {
            Sequence.Insert(position, toDo);
        }

        public void Append(Action toDo)
        {
            Sequence.Add(toDo);
        }

        public void ClearSequence()
        {
            Sequence.Clear();
        }

        public Tween onComplete(Action onComplete)
        {
            onCompleteAction = onComplete;
            return this;
        }
        public Tween setDelay(float delay)
        {
            this.delay = delay;
            return this;
        }


        /* public void Sequence(Action onComplete = null)
        {
            StartCoroutine(RunSequence(onComplete));
        }*/

        private IEnumerator RunSequence()
        {
            yield return new WaitForSeconds(delay);

            foreach (var action in Sequence)
            {
                yield return StartCoroutine(this.OneActionCoroutine(action));
            }


            onCompleteAction?.Invoke();
        }


        private IEnumerator OneActionCoroutine(Action action)
        {
            action?.Invoke();
            yield return new WaitForSeconds(0.1f);
        }


        private IEnumerator doCoroutine(Action toDo)
        {
            toDo?.Invoke();
            yield return null;
        }

        public void Play()
        {
            //Debug.Log("toto ide");
            StartCoroutine(RunSequence());
        }
    }
}


