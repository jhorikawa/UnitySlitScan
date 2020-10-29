using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SlitScanManager : MonoBehaviour
{

    public Material slitScanMat;
    public RawImage slitScanRawImage;
    public AspectRatioFitter aspectRatioFitter;

    private WebCamTexture webCamTexture;
    private int targetFrame = 30;
    public int refsize = 128;
    private int width, height;

    private Texture2DArray texArray;

    public int texNum = 50;
    private bool init = false;

    private Texture2D smallTex;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFrame;
        
        webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, refsize, refsize, targetFrame);
        webCamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (webCamTexture.width > 100)
        {
            if (init == false)
            {
                Init();
                init = true;
            }

            try
            {
                smallTex = TextureScaler.scaled(webCamTexture, width, height);

                for (int i = 0; i < texArray.depth - 1; i++)
                {
                    texArray.SetPixels32(texArray.GetPixels32(i + 1), i);
                }
                
                texArray.SetPixels32(smallTex.GetPixels32(), texArray.depth - 1);
                texArray.Apply();

                Destroy(smallTex);
            }
            catch (System.Exception e)
            {
                
            }
        }
    }

    private void Init()
    {
        slitScanRawImage.material = slitScanMat;
        aspectRatioFitter.aspectRatio = webCamTexture.width / (float) webCamTexture.height;

        float scale = refsize / (float) webCamTexture.width;
        width = Mathf.FloorToInt(webCamTexture.width * scale);
        height = Mathf.FloorToInt(webCamTexture.height * scale);
        texArray = new Texture2DArray(width, height, texNum, TextureFormat.RGB24, webCamTexture.mipmapCount, true);
        
        slitScanMat.SetTexture("_Textures", texArray);
        slitScanMat.SetInt("_TexturesNum", texNum);
    }

}
