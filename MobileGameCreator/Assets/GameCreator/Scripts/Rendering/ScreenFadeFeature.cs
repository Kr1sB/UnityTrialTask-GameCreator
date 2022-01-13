using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameCreator.Rendering
{
    public class ScreenFadeFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public bool isEnabled = true;
            public string identifier = "Screen Fade";

            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            public Material material = null;

            [System.NonSerialized] public Material runtimeMaterial = null;

            public bool isValid =>
                (runtimeMaterial != null) && isEnabled;
        }

        public Settings settings = null;
        private ScreenFadePass renderPass = null;

        public override void Create()
        {
            renderPass = new ScreenFadePass(settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.isValid)
                renderer.EnqueuePass(renderPass);
        }
    }
}