using UnityEngine;

[ExecuteInEditMode]
public class RenderReplacementShaderToTexture : MonoBehaviour
{
    [SerializeField]
    Shader replacementShader;

    [SerializeField]
    RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

    [SerializeField]
    FilterMode filterMode = FilterMode.Point;

    [SerializeField]
    int renderTextureDepth = 24;

    [SerializeField]
    CameraClearFlags cameraClearFlags = CameraClearFlags.Color;

    [SerializeField]
    Color background = Color.black;

    [SerializeField]
    string targetTexture = "_RenderTexture";

    public LayerMask outlineLayer;

    private RenderTexture renderTexture;
    private new Camera camera;

    private void Start()
    {
        Init();
    }
    
    private void Init()
    {
        foreach (Transform t in transform)
        {
            DestroyImmediate(t.gameObject);
        }

        Camera thisCamera = GetComponent<Camera>();
        Camera outlineCam = null;

        if (thisCamera)
        {
            for (int i = 0; i < thisCamera.transform.childCount; i++)
            {
                var cam = thisCamera.transform.GetChild(i).GetComponent<Camera>();

                if (cam)
                {
                    if (cam.GetComponent<OutlineCameraTag>())
                    {
                        if (outlineCam && cam.GetInstanceID() != outlineCam.GetInstanceID())
                        {
                            DestroyImmediate(cam.gameObject);
                            print("Here");
                            i--;
                        }
                        else
                        {
                            outlineCam = cam;
                        }
                    }
                }
            }
        }

        // Create a render texture matching the main camera's current dimensions.
        renderTexture = new RenderTexture(thisCamera.pixelWidth, thisCamera.pixelHeight, renderTextureDepth, renderTextureFormat);
        renderTexture.filterMode = filterMode;
        // Surface the render texture as a global variable, available to all shaders.
        Shader.SetGlobalTexture(targetTexture, renderTexture);

        if (outlineCam == null)
        {
            GameObject copy = new GameObject("Camera" + targetTexture);
            outlineCam = copy.AddComponent<Camera>();
            outlineCam.gameObject.AddComponent<OutlineCameraTag>();
        }
        // Setup a copy of the camera to render the scene using the normals shader.

        outlineCam.CopyFrom(thisCamera);
        outlineCam.transform.SetParent(transform);
        outlineCam.targetTexture = renderTexture;
        outlineCam.SetReplacementShader(replacementShader, "RenderType");
        outlineCam.depth = thisCamera.depth - 1;
        outlineCam.clearFlags = cameraClearFlags;
        outlineCam.backgroundColor = background;
    }
}
