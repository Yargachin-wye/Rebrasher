using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestColor : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private Vector2 _framesSize;
    [SerializeField] private string _nameFileFramesMask = "FramesMask";
    [SerializeField] private string _nameFileFrames = "Frames";

    [Header("Mask")]
    [SerializeField] private Vector2 _maskSize;
    [SerializeField] private string _nameFilePaletteMask = "PaletteMask";
    [SerializeField] private string _nameFilePalette = "Palette";


    void Start()
    {
        Texture2D assetPaletteMask = Resources.Load<Texture2D>(_nameFilePaletteMask);
        Texture2D assetPalette = Resources.Load<Texture2D>(_nameFilePalette);

        Dictionary<Color, Color> colors = new Dictionary<Color, Color>();

        for (int y = 0; y < _maskSize.y; y++)
        {
            for (int x = 0; x < _maskSize.x; x++)
            {
                Color maskColor = assetPaletteMask.GetPixel(x, y);
                Color frameColor = assetPalette.GetPixel(x, y);

                if (maskColor.a < 1)
                    continue;

                if (!colors.ContainsKey(maskColor))
                {
                    colors.Add(maskColor, frameColor);
                }
            }
        }
        Rebrash(colors);
    }
    private void RebrashColor(Color colorKey, Color colorVal)
    {
        Texture2D assetFramesMask = Resources.Load<Texture2D>(_nameFileFramesMask);
        Texture2D assetFrames = Resources.Load<Texture2D>(_nameFileFrames);

        for (int y = 0; y < _framesSize.y; y++)
        {
            for (int x = 0; x < _framesSize.x; x++)
            {
                Color color = assetFramesMask.GetPixel(x, y);
                if (color.a < 1)
                {
                    assetFrames.SetPixel(x, y, Color.clear);
                    continue;
                }

                if (assetFramesMask.GetPixel(x, y) == colorKey)
                {
                    assetFrames.SetPixel(x, y, colorVal);
                    //Debug.Log(x + "; " + y);
                }
            }
        }
        assetFrames.Apply();
    }
    private void Rebrash(Dictionary<Color, Color> colors)
    {
        foreach(var color in colors)
        {
            RebrashColor(color.Key, color.Value);
        }
    }
}
