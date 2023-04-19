using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorReplacement : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private Vector2 _framesSize;
    [SerializeField] private string _nameFileFramesMask = "FramesMask";
    [SerializeField] private string _nameFileFrames = "Frames";

    [Header("Mask")]
    [SerializeField] private Vector2 _maskSize;
    [SerializeField] private string _nameFilePaletteMask = "PaletteMask";
    [SerializeField] private string _nameFilePalette = "Palette";

    [Header("Compute Shader")]
    [SerializeField] private ComputeShader replaceColorShader;
    [SerializeField] private string kernelName = "ReplaceColors";

    private Dictionary<Color, Color> colorMap = new Dictionary<Color, Color>();


    void Awake()
    {
        // Load textures
        Texture2D paletteMask = Resources.Load<Texture2D>(_nameFilePaletteMask);
        Texture2D palette = Resources.Load<Texture2D>(_nameFilePalette);
        Texture2D framesMask = Resources.Load<Texture2D>(_nameFileFramesMask);
        Texture2D frames = Resources.Load<Texture2D>(_nameFileFrames);

        // Create dictionary of colors
        // Populate color map
        for (int y = 0; y < paletteMask.height; y++)
        {
            for (int x = 0; x < paletteMask.width; x++)
            {
                Color maskColor = paletteMask.GetPixel(x, y);
                Color paletteColor = palette.GetPixel(x, y);

                if (maskColor.a < 1)
                    continue;

                if (!colorMap.ContainsKey(maskColor))
                {
                    colorMap.Add(maskColor, paletteColor);
                }
            }
        }

        // Execute compute shader
        int kernel = replaceColorShader.FindKernel(kernelName);
        ComputeBuffer colorMapBuffer = new ComputeBuffer(colorMap.Count, sizeof(float) * 4);
        ComputeBuffer argsBuffer = new ComputeBuffer(1, sizeof(int) * 3, ComputeBufferType.IndirectArguments);

        // Fill color map buffer
        Color[] colorMapArray = new Color[colorMap.Count];
        int i = 0;
        foreach (var kvp in colorMap)
        {
            colorMapArray[i] = new Color(kvp.Key.r, kvp.Key.g, kvp.Key.b, kvp.Value.r);
            i++;
        }
        colorMapBuffer.SetData(colorMapArray);

        // Set compute shader parameters
        replaceColorShader.SetBuffer(kernel, "colorMap", colorMapBuffer);
        replaceColorShader.SetTexture(kernel, "frameTexture", frames);
        replaceColorShader.SetTexture(kernel, "frameMaskTexture", framesMask);

        // Set indirect args buffer (draw a single triangle)
        uint[] args = new uint[3] { 3, 1, 0 };
        argsBuffer.SetData(args);

        // Dispatch compute shader
        replaceColorShader.DispatchIndirect(kernel, argsBuffer, 0);

        // Cleanup buffers
        colorMapBuffer.Release();
        argsBuffer.Release();
    }
}
