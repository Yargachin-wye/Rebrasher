using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestColor : MonoBehaviour
{
    [Header("Frames")]

    [SerializeField] private string _FileFMaskArm1 = "mask_arm_1";
    [SerializeField] private string _FileFMaskLeg1 = "mask_leg_1";
    [SerializeField] private string _FileFMaskHead = "mask_head";
    [SerializeField] private string _FileFMaskBody = "mask_body";
    [SerializeField] private string _FileFMaskLeg2 = "mask_leg_2";
    [SerializeField] private string _FileFMaskArm2 = "mask_arm_1";

    [SerializeField] private string _nameFileFrames = "Frames";

    [Header("Palette")]

    [SerializeField] private string _FilePMaskArm1 = "PaletteMask";
    [SerializeField] private string _FilePMaskLeg1 = "PaletteMask";
    [SerializeField] private string _FilePMaskHead = "PaletteMask";
    [SerializeField] private string _FilePMaskBody = "PaletteMask";
    [SerializeField] private string _FilePMaskLeg2 = "PaletteMask";
    [SerializeField] private string _FilePMaskArm2 = "PaletteMask";

    [SerializeField] private string _FilePArm1 = "Palette";
    [SerializeField] private string _FilePLeg1 = "Palette";
    [SerializeField] private string _FilePHead = "Palette";
    [SerializeField] private string _FilePBody = "Palette";
    [SerializeField] private string _FilePLeg2 = "Palette";
    [SerializeField] private string _FilePArm2 = "Palette";

    [Header("Compute Shader")]

    [SerializeField] private ComputeShader replaceColorShader;
    private string _kernelName = "ReplaceColors";

    private List<Color> _colorsK = new List<Color>();
    private List<Color> _colorsV = new List<Color>();
    int kernel;
    bool endRebrash = false;
    public void Rebrash()
    {
        kernel = replaceColorShader.FindKernel(_kernelName);

        Texture2D assetFrames = Resources.Load<Texture2D>(_nameFileFrames);
        ComputeBuffer texture = new ComputeBuffer(assetFrames.width * assetFrames.height, sizeof(float) * 4);
        Vector4[] data = new Vector4[assetFrames.width * assetFrames.height];
        texture.SetData(data);
        replaceColorShader.SetBuffer(kernel, "frames", texture);

        Dispatch(_FilePMaskArm2, _FilePArm2, _FileFMaskArm2);
        Dispatch(_FilePMaskLeg2, _FilePLeg2, _FileFMaskLeg2);
        Dispatch(_FilePMaskBody, _FilePBody, _FileFMaskBody);
        Dispatch(_FilePMaskHead, _FilePHead, _FileFMaskHead);
        Dispatch(_FilePMaskLeg1, _FilePLeg1, _FileFMaskLeg1);
        Dispatch(_FilePMaskArm1, _FilePArm1, _FileFMaskArm1);

        texture.GetData(data);

        Color[] colors = new Color[assetFrames.width * assetFrames.height];
        for (int i = 0; i < data.Length; i++)
        {
            colors[i] = new Color(data[i].x, data[i].y, data[i].z, data[i].w);
        }
        assetFrames.SetPixels(colors);
        assetFrames.Apply();

        endRebrash = true;
        texture.Dispose();
    }

    public void RebrashCPU()
    {
        Texture2D assetFrames = Resources.Load<Texture2D>(_nameFileFrames);

        Texture2D asset1 = Resources.Load<Texture2D>(_FileFMaskArm1);
        Dictionary<Color, Color>  colors1 = GetColorsD(_FilePMaskArm1, _FilePArm1);
        Texture2D asset2 = Resources.Load<Texture2D>(_FileFMaskLeg1);
        Dictionary<Color, Color> colors2 = GetColorsD(_FilePMaskLeg1, _FilePLeg1);
        Texture2D asset3 = Resources.Load<Texture2D>(_FileFMaskHead);
        Dictionary<Color, Color> colors3 = GetColorsD(_FilePMaskHead, _FilePHead);
        Texture2D asset4 = Resources.Load<Texture2D>(_FileFMaskBody);
        Dictionary<Color, Color> colors4 = GetColorsD(_FilePMaskBody, _FilePBody);
        Texture2D asset5 = Resources.Load<Texture2D>(_FileFMaskLeg2);
        Dictionary<Color, Color> colors5 = GetColorsD(_FilePMaskLeg2, _FilePLeg2);
        Texture2D asset6 = Resources.Load<Texture2D>(_FileFMaskArm2);
        Dictionary<Color, Color> colors6 = GetColorsD(_FilePMaskArm2, _FilePArm2);

        for (int x = 0; x <= assetFrames.width; x++)
        {
            for(int y = 0; y <= assetFrames.height; y++)
            {
                if (asset1.GetPixel(x, y).a != 0 && colors1.ContainsKey(asset1.GetPixel(x, y)) )
                {
                    assetFrames.SetPixel(x, y, colors1[asset1.GetPixel(x, y)]);
                    continue;
                }
                if (asset2.GetPixel(x, y).a != 0 && colors2.ContainsKey(asset2.GetPixel(x, y)))
                {
                    assetFrames.SetPixel(x, y, colors2[asset2.GetPixel(x, y)]);
                    continue;
                }
                if (asset3.GetPixel(x, y).a != 0 && colors3.ContainsKey(asset3.GetPixel(x, y)))
                {
                    assetFrames.SetPixel(x, y, colors3[asset3.GetPixel(x, y)]);
                    continue;
                }
                if (asset4.GetPixel(x, y).a != 0 && colors4.ContainsKey(asset4.GetPixel(x, y)))
                {
                    assetFrames.SetPixel(x, y, colors4[asset4.GetPixel(x, y)]);
                    continue;
                }
                if (asset5.GetPixel(x, y).a != 0 && colors5.ContainsKey(asset5.GetPixel(x, y)))
                {
                    assetFrames.SetPixel(x, y, colors5[asset5.GetPixel(x, y)]);
                    continue;
                }
                if (asset6.GetPixel(x, y).a != 0 && colors6.ContainsKey(asset6.GetPixel(x, y)))
                {
                    assetFrames.SetPixel(x, y, colors6[asset6.GetPixel(x, y)]);
                    continue;
                }
                assetFrames.SetPixel(x, y, Color.clear);
            }
        }
        assetFrames.Apply();
    }
    Dictionary<Color, Color> GetColorsD(string nameFilePaletteMask, string nameFilePalette)
    {
        endRebrash = false;
        Texture2D paletteMask = Resources.Load<Texture2D>(nameFilePaletteMask);
        Texture2D palette = Resources.Load<Texture2D>(nameFilePalette);

        Dictionary<Color, Color> colors = new Dictionary<Color, Color>();
        for (int y = 0; y < palette.height; y++)
        {
            for (int x = 0; x < palette.width; x++)
            {
                Color maskColor = paletteMask.GetPixel(x, y);
                Color frameColor = palette.GetPixel(x, y);

                if (maskColor.a < 1)
                    continue;

                if (!colors.ContainsKey(maskColor))
                {
                    colors.Add(maskColor, frameColor);
                }
            }
        }
        return colors;
    }
    IEnumerator GetColors(string nameFilePaletteMask, string nameFilePalette)
    {
        endRebrash = false;
        _colorsK = new List<Color>();
        _colorsV = new List<Color>();
        Texture2D paletteMask = Resources.Load<Texture2D>(nameFilePaletteMask);
        Texture2D palette = Resources.Load<Texture2D>(nameFilePalette);

        Dictionary<Color, Color> colors = new Dictionary<Color, Color>();
        for (int y = 0; y < palette.height; y++)
        {
            for (int x = 0; x < palette.width; x++)
            {
                Color maskColor = paletteMask.GetPixel(x, y);
                Color frameColor = palette.GetPixel(x, y);

                if (maskColor.a < 1)
                    continue;

                if (!colors.ContainsKey(maskColor))
                {
                    colors.Add(maskColor, frameColor);
                    _colorsK.Add(maskColor);
                    _colorsV.Add(frameColor);
                }
            }
        }

        int bufSize = _colorsK.Count;
        ComputeBuffer K = new ComputeBuffer(_colorsK.Count, sizeof(float) * 4);
        ComputeBuffer V = new ComputeBuffer(_colorsK.Count, sizeof(float) * 4);

        K.SetData(_colorsK);
        V.SetData(_colorsV);
        replaceColorShader.SetBuffer(kernel, "Kp", K);
        replaceColorShader.SetBuffer(kernel, "Vp", V);
        replaceColorShader.SetInt("iter", _colorsK.Count);
        yield return new WaitUntil(() => endRebrash);
        Debug.Log(_colorsK.Count);

        K.Release();
        V.Release();
    }
    void Dispatch(string maskPalete, string palete, string FrMask)
    {
        StartCoroutine(GetColors(maskPalete, palete));
        Texture2D asset = Resources.Load<Texture2D>(FrMask);
        replaceColorShader.SetTexture(kernel, "mask", asset);

        replaceColorShader.Dispatch(kernel, 1, 1, 1);//________________
    }
}
