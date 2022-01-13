using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameCreator.Rendering
{
    public class ScreenFader : MonoBehaviour
    {
        public ForwardRendererData rendererData;

        private Material material;
        private Coroutine fadeCoroutine;
        private const string propertyAlpha = "_Alpha";

        private void Awake()
        {
            ScreenFadeFeature feature =
                rendererData.rendererFeatures.Find(f => f is ScreenFadeFeature) as ScreenFadeFeature;

            if(feature != null)
            {
                material = new Material(feature.settings.material);
                feature.settings.runtimeMaterial = material;
            }
            else
            {
                Debug.LogWarning("[ScreenFader]: Feature not assigned to ForwardRendererData!");
            }
        }


        public void FadeIn(float duration, System.Action callback = null) =>
            Fade(1, 0, duration, callback);

        public void FadeOut(float duration, System.Action callback = null) =>
            Fade(0, 1, duration, callback);

        public void Fade(float fromValue, float toValue, float duration, System.Action callback = null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeCoroutine(fromValue, toValue, duration, callback));
        }

        private IEnumerator FadeCoroutine(float fromValue, float toValue, float duration, System.Action callback = null)
        {
            if(duration <= 0)
            {
                material.SetFloat(propertyAlpha, toValue);
                fadeCoroutine = null;
                callback?.Invoke();
                yield break;
            }

            float remaining = duration;
            float t;

            do
            {
                t = 1f - (remaining / duration);
                float value = Mathf.Lerp(fromValue, toValue, t);
                material.SetFloat(propertyAlpha, value);

                remaining = Mathf.Max(0, remaining - Mathf.Min(Time.maximumDeltaTime, Time.unscaledDeltaTime));
                yield return null;                     
            }
            while (t < 1);

            fadeCoroutine = null;
            callback?.Invoke();
        }
    }
}