using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture bufferArray;
    int bufferWidth, bufferHeight;
    int handleCAInit, handleCSMain, handleCAStep;

    int bufferId;
    [Range(0,255)]
    public int ruleId=60;
    void Start()
    {
        bufferWidth = Screen.width;
        bufferHeight = Screen.height;

        for (int i = 0; i < 2; i++)
        {
            bufferArray = new RenderTexture(bufferWidth, bufferHeight, 2, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
            bufferArray.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
            bufferArray.enableRandomWrite = true;
            bufferArray.volumeDepth = 2;
            bufferArray.Create();
            Debug.Log($"bufferArray.dimension {bufferArray.dimension}");
            Debug.Log($"bufferArray.depth {bufferArray.depth}");
            Debug.Log($"bufferArray.volumeDepth {bufferArray.volumeDepth}");
        }
        handleCAInit = shader.FindKernel("CAInit");
        handleCAStep = shader.FindKernel("CAStep");

        // Set variables for shaders.
        shader.SetInt("initActivated", (int)bufferWidth / 2);
        shader.SetInt("bufferHeight", bufferHeight);
        shader.SetInt("bufferWidth", bufferWidth);
        shader.SetInt("ruleId", ruleId);
        shader.SetTexture(handleCAInit, "bufferArray", bufferArray);
        shader.SetTexture(handleCAStep, "bufferArray", bufferArray);

        // Run the init shader.
        shader.Dispatch(handleCAInit, bufferWidth, 1, 2);

        bufferId = 0;
    }

    // Update is called once per frame
    void Update()
    {
        shader.SetInt("bufferId", bufferId);
        shader.Dispatch(handleCAStep, bufferWidth, bufferHeight, 1);
        bufferId = 1 - bufferId;
    }

    // Attach this script to the main camera,
    // and then we can override the OnRenderImage function.
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        Graphics.Blit(bufferArray, dest, bufferId, 0);
    }


}
