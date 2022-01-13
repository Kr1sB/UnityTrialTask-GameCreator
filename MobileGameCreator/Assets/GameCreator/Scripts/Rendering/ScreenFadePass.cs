using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameCreator.Rendering
{
    public class ScreenFadePass : ScriptableRenderPass
    {
        private ScreenFadeFeature.Settings settings = null;

        public ScreenFadePass(ScreenFadeFeature.Settings newSettings)
        {
            settings = newSettings;
            renderPassEvent = newSettings.renderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer command = CommandBufferPool.Get(settings.identifier);

            RenderTargetIdentifier source = BuiltinRenderTextureType.CameraTarget;
            RenderTargetIdentifier destination = BuiltinRenderTextureType.CurrentActive;

            command.Blit(source, destination, settings.runtimeMaterial);
            context.ExecuteCommandBuffer(command);

            CommandBufferPool.Release(command);
        }
    }
}