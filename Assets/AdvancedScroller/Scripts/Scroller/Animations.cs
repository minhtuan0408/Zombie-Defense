// Advanced Scroller made by Simon Podracky. 
// 7.9.2023

using UnityEngine;

namespace AScroller
{
    [CreateAssetMenu(fileName = "Animations", menuName = "Scriptables/AScroller/Animations")]
    public class Animations : ScriptableObject
    {
        public AnimationCurve Linear;
        public AnimationCurve InSine;
        public AnimationCurve OutSine;
        public AnimationCurve InOutSine;
        public AnimationCurve InQuad;
        public AnimationCurve OutQuad;
        public AnimationCurve InOutQuad;
        public AnimationCurve InCubic;
        public AnimationCurve OutCubic;
        public AnimationCurve InOutCubic;
        public AnimationCurve InQuart;
        public AnimationCurve OutQuart;
        public AnimationCurve InOutQuart;
        public AnimationCurve InQuint;
        public AnimationCurve OutQuint;
        public AnimationCurve InOutQuint;
        public AnimationCurve InExpo;
        public AnimationCurve OutExpo;
        public AnimationCurve InOutExpo;
        public AnimationCurve InCirc;
        public AnimationCurve OutCirc;
        public AnimationCurve InOutCirc;
        public AnimationCurve InBack;
        public AnimationCurve OutBack;
        public AnimationCurve InOutBack;
        public AnimationCurve InElastic;
        public AnimationCurve OutElastic;
        public AnimationCurve InOutElastic;
        public AnimationCurve InBounce;
        public AnimationCurve OutBounce;
        public AnimationCurve InOutBounce;

        [ContextMenu("Set Curves")]
        public void SetCurves()
        {
            Linear = AnimationCurve.Linear(0, 0, 1, 1);

            InSine = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InSine.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1, 1.5f, 1.5f)
            };

            OutSine = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutSine.keys = new Keyframe[]
            {
            new Keyframe(0, 0, 1.5f, 1.5f),
            new Keyframe(1, 1)
            };

            InOutSine = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutSine.keys = new Keyframe[]
            {
            new Keyframe(0, 0, 1.5f, 1.5f),
            new Keyframe(1, 1,1.5f, 1.5f)
            };

            InQuad = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InQuad.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1, 2, 2)
            };

            OutQuad = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutQuad.keys = new Keyframe[]
            {
            new Keyframe(0, 0,2,2),
            new Keyframe(1, 1)
            };

            InOutQuad = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutQuad.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.5f, 2, 2),
            new Keyframe(1, 1)
            };

            InCubic = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InCubic.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1, 3, 3)
            };

            OutCubic = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutCubic.keys = new Keyframe[]
            {
            new Keyframe(0, 0,3,3),
            new Keyframe(1, 1)
            };

            InOutCubic = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutCubic.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.5f, 3, 3),
            new Keyframe(1, 1)
            };

            InQuart = AnimationCurve.Linear(0, 0, 1, 1);
            InQuart.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1,3.5f,4)
            };

            OutQuart = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutQuart.keys = new Keyframe[]
            {
            new Keyframe(0, 0,4f,3.5f),
            new Keyframe(1, 1)
            };

            InOutQuart = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutQuart.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.5f,3.5f, 3.5f),
            new Keyframe(1, 1)
            };

            InQuint = AnimationCurve.Linear(0, 0, 1, 1);
            InQuint.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1,4.5f,5f)
            };

            OutQuint = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutQuint.keys = new Keyframe[]
            {
            new Keyframe(0, 0,5,4.5f),
            new Keyframe(1, 1)
            };

            InOutQuint = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutQuint.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.5f,4.5f, 4.5f),
            new Keyframe(1, 1)
            };

            InExpo = AnimationCurve.Linear(0, 0, 1, 1);
            InExpo.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(1, 1,5.5f,6f)
            };

            OutExpo = AnimationCurve.EaseInOut(0, 0, 1, 1);
            OutExpo.keys = new Keyframe[]
            {
            new Keyframe(0, 0,6,5.5f),
            new Keyframe(1, 1)
            };

            InOutExpo = AnimationCurve.EaseInOut(0, 0, 1, 1);
            InOutExpo.keys = new Keyframe[]
            {
            new Keyframe(0, 0),
            new Keyframe(0.5f, 0.5f,5.5f, 5.5f),
            new Keyframe(1, 1)
            };
        }
    }
}
