using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class copShad : MonoBehaviour {

    public ComputeShader copyShader;
    public Camera cam;
    public int width, height;

	// Use this for initialization
	void Start () {

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        renderTexture.antiAliasing = 2;
        renderTexture.Create();
        RenderTexture renderTextureCopy = new RenderTexture(width, height, 0);

        renderTextureCopy.enableRandomWrite = true;
        renderTextureCopy.antiAliasing = 1;
        renderTextureCopy.Create();
        Texture2D tex = new Texture2D(width, height);
        var copyKernelIdx = copyShader.FindKernel("Copy");

        cam.targetTexture = renderTexture;
        cam.Render();

        copyShader.SetTexture(copyKernelIdx, "dest", renderTextureCopy);
        copyShader.SetTexture(copyKernelIdx, "source", renderTexture);
        copyShader.SetInt("width", width);
        copyShader.SetInt("height", height);
        int threadsX = 32, threadsY = 32;
        copyShader.Dispatch(copyKernelIdx,
            (width + threadsX - 1) / threadsX,
            (height + threadsY - 1) / threadsY, 1);

        RenderTexture.active = renderTextureCopy;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Sreenshot.png", bytes);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
