using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class convertImg : MonoBehaviour
{
    Texture2D tex;
    [SerializeField]GameObject targetImg;
    private Color[] tempcodedColors = new Color[20];
    public string[] pixelMap;
    [Header("Dimensions")]
    public int width;
    public int height;
    public Color[] colorCodes;

    private void getImageData()
    {
        tempcodedColors = new Color[20];

        RawImage img = targetImg.GetComponent<RawImage>();
        tex = img.texture as Texture2D;
        Color[] pix = tex.GetPixels();

        pixelMap = new string[tex.height];

        width = tex.width;
        height = tex.height; 

        convertToNumbers(pix, tex);
    }
    
    private void convertToNumbers(Color[] colors, Texture2D tex){
        int y = 0;
        for (int x = 0; x < tex.height; x++)
        {
            for (int u = 0; u < tex.width; u++)
            {
                if(colors[y] != new Color(colors[y].r,colors[y].b,colors[y].g,0f)){
                    if(checkColor(colors[y])){
                        for (int i = 0; i < tempcodedColors.Length; i++)
                        {
                            if(tempcodedColors[i] == colors[y]){
                                pixelMap[x] += i + ",";
                            }
                        }
                    }
                }else{
                    pixelMap[x] += 0 + ",";
                }
                y++;
            }
        }

        var codeColorLength = 0;
        for (int u = 1; u < tempcodedColors.Length; u++)
        {
            if(tempcodedColors[u] != new Color(0f,0f,0f,0f)){
                codeColorLength++;
            }
        }
        colorCodes = new Color[codeColorLength];
        for (int f = 0; f < colorCodes.Length; f++)
        {
            colorCodes[f] = tempcodedColors[f];
        }
    }

    private bool checkColor(Color c){
        bool colorFound = false;
        for (int i = 0; i < tempcodedColors.Length; i++)
        {
            if(tempcodedColors[i] == c){
                colorFound = true;
            }
        }

        if(!colorFound){
            for (int i = 1; i < 20; i++)
            {
                if(tempcodedColors[i] == new Color(0f,0f,0f,0f)){
                    tempcodedColors[i] = c;
                    i += 20;
                }
            }
        }
        return true;
    }

    private void deleteData(){   
        tempcodedColors = new Color[20];
        pixelMap = new string[0];
        width = 0;
        height = 0;
        colorCodes = new Color[0];
    }

}
