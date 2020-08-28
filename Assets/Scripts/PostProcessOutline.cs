using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.BeforeStack, "Post Process Outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    public IntParameter scale = new IntParameter { value = 1 };
    public FloatParameter depthThreshold = new FloatParameter { value = 0.2f };

    [Range(0, 1)]
    public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };
    [Range(0, 1)]
    public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
    public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7f };

    public ColorParameter color = new ColorParameter { value = Color.white };
    public FloatParameter outlineIntensity = new FloatParameter { value = 1 };
}

public sealed class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    private Camera gameCamera;
    private Camera sceneCamera;

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Outline Post Process"));

        sheet.properties.SetFloat("_Scale", settings.scale);

        var clipToViewMat = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;

        sheet.properties.SetMatrix("_ClipToViewMatrix", clipToViewMat);

        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);
        sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
        sheet.properties.SetColor("_Color", settings.color);
        sheet.properties.SetFloat("_Intensity", settings.outlineIntensity);

        if (!context.isSceneView)
        {
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
        else
        {
            context.command.Blit(context.source, context.destination);
        }
    }
}