using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Plugins.UnityGenshinPostProcessing.Scripts
{
    public class PostProcessPass : ScriptableRenderPass
    {
        private static readonly int BloomColor = Shader.PropertyToID("_BloomColor");
        private static readonly int BloomThreshold = Shader.PropertyToID("_BloomThreshold");
        private static readonly int BloomIntensity = Shader.PropertyToID("_BloomIntensity");
        private static readonly int BloomWeights = Shader.PropertyToID("_BloomWeights");
        private static readonly int BlurRadius = Shader.PropertyToID("_BlurRadius");
        private static readonly int BloomTextureA = Shader.PropertyToID("_BloomTextureA");
        private static readonly int BloomTextureB = Shader.PropertyToID("_BloomTextureB");
        private static readonly int BloomTextureC = Shader.PropertyToID("_BloomTextureC");
        private static readonly int BloomTextureD = Shader.PropertyToID("_BloomTextureD");
        private readonly Material material;

        private readonly RTHandle[] bloomBufferA;

        private readonly RTHandle[] bloomBufferB;

        private bool useBloom;

        private readonly int iterations;

        private float downSampleScale;

        public PostProcessPass(RenderPassEvent renderPassEvent, Shader shader, int iterations)
        {
            material = CoreUtils.CreateEngineMaterial(shader);
            this.iterations = iterations;
            bloomBufferA = new RTHandle[this.iterations];
            bloomBufferB = new RTHandle[this.iterations];

            profilingSampler = new ProfilingSampler(nameof(PostProcessPass));
            this.renderPassEvent = renderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraType = renderingData.cameraData.cameraType;
            if (cameraType is CameraType.Preview or CameraType.Reflection)
            {
                return;
            }

            var stack = VolumeManager.instance.stack;
            var bloomVolume = stack.GetComponent<BloomVolume>();
            // var colorGradingVolume = stack.GetComponent<ColorGradingVolume>();

            if (!bloomVolume.IsActive())
            {
                return;
            }

            useBloom = bloomVolume.IsActive();
            downSampleScale = bloomVolume.downSampleScale.value;
            SetupBuffer(ref renderingData);
            SetupMaterial(bloomVolume, ref renderingData);

            var command = CommandBufferPool.Get();
            Render(command, ref renderingData);
            context.ExecuteCommandBuffer(command);
            CommandBufferPool.Release(command);
        }

        private void SetupBuffer(ref RenderingData renderingData)
        {
            var cameraDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            var descriptor = new RenderTextureDescriptor(cameraDescriptor.width, cameraDescriptor.height, cameraDescriptor.graphicsFormat, 0, 0);

            if (useBloom)
            {
                descriptor.width = (int)Math.Round(descriptor.width * downSampleScale);
                descriptor.height = (int)Math.Round(descriptor.height * downSampleScale);
                for (int i = 0; i < iterations; i++)
                {
                    RenderingUtils.ReAllocateIfNeeded(ref bloomBufferA[i], descriptor, FilterMode.Bilinear, name: $"_BloomBufferA_{i}");
                    RenderingUtils.ReAllocateIfNeeded(ref bloomBufferB[i], descriptor, FilterMode.Bilinear, name: $"_BloomBufferB_{i}");
                    descriptor.width = Math.Max(descriptor.width / 2, 1);
                    descriptor.height = Math.Max(descriptor.height / 2, 1);
                }
            }
        }

        private void SetupMaterial(BloomVolume bloomVolume, ref RenderingData renderingData)
        {
            var screenFactor = renderingData.cameraData.cameraTargetDescriptor.height / 1080f;

            CoreUtils.SetKeyword(material, "_BLOOM_COLOR", bloomVolume.mode.value == BloomMode.Color);
            CoreUtils.SetKeyword(material, "_BLOOM_BRIGHTNESS", bloomVolume.mode.value == BloomMode.Brightness);

            if (!useBloom) return;
            material.SetFloat(BloomThreshold, bloomVolume.threshold.value);
            material.SetFloat(BloomIntensity, bloomVolume.intensity.value);
            material.SetVector(BloomWeights, bloomVolume.weights.value);
            material.SetColor(BloomColor, bloomVolume.color.value);
            material.SetFloat(BlurRadius, bloomVolume.blurRadius.value * downSampleScale * screenFactor);
        }

        private void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            using (new ProfilingScope(commandBuffer, profilingSampler))
            {
                if (useBloom)
                {
                    RTHandle[] bufferA = bloomBufferA, bufferB = bloomBufferB;

                    Blit(commandBuffer, source, bufferA[0], Pass.BloomPrefilter);

                    Blit(commandBuffer, bufferA[0], bufferB[0], Pass.BloomHorizontalBlur1X);
                    Blit(commandBuffer, bufferB[0], bufferA[0], Pass.BloomVerticalBlur1X);

                    for (int i = 1; i < iterations; i++)
                    {
                        Blit(commandBuffer, bufferA[i - 1], bufferB[i], Pass.BloomHorizontalBlur2X);
                        Blit(commandBuffer, bufferB[i], bufferA[i], Pass.BloomVerticalBlur1X);
                    }

                    commandBuffer.SetGlobalTexture(BloomTextureA, bufferA[0]);
                    commandBuffer.SetGlobalTexture(BloomTextureB, bufferA[1]);
                    commandBuffer.SetGlobalTexture(BloomTextureC, bufferA[2]);
                    commandBuffer.SetGlobalTexture(BloomTextureD, bufferA[3]);

                    Blit(commandBuffer, bufferB[0], bufferB[0], Pass.BloomUpsample);

                    commandBuffer.SetGlobalTexture(BloomTextureA, bufferB[0]);
                }

                base.Blit(commandBuffer, ref renderingData, material, (int)Pass.ColorGrading);
            }
        }

        private void Blit(CommandBuffer commandBuffer, RTHandle source, RTHandle destination, Pass pass)
        {
            const RenderBufferLoadAction load = RenderBufferLoadAction.DontCare;
            const RenderBufferStoreAction save = RenderBufferStoreAction.Store;
            Blitter.BlitCameraTexture(commandBuffer, source, destination, load, save, material, (int)pass);
        }

        public void Dispose()
        {
            for (int i = 0; i < iterations; i++)
            {
                bloomBufferA[i]?.Release();
                bloomBufferB[i]?.Release();
            }
            CoreUtils.Destroy(material);
        }

        private enum Pass
        {
            Blit,
            BloomPrefilter,
            BloomHorizontalBlur1X,
            BloomHorizontalBlur2X,
            BloomVerticalBlur1X,
            BloomVerticalBlur2X,
            BloomUpsample,
            ColorGrading
        }
    }
}
