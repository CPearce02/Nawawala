using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject objectToSpawn; // The GameObject you want to spawn
    public float spawnPadding = 0.5f; // Padding from screen edges for spawning
    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;

    private void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        spawnAreaMin = new Vector2(-screenWidth / 2 + spawnPadding - 5, -screenHeight / 2 + spawnPadding);
        spawnAreaMax = new Vector2(screenWidth / 2 - spawnPadding - 5, screenHeight / 2 - spawnPadding);

        foreach (Transform child in transform)
        {
            if(child.TryGetComponent<TutorialNodeBehaviour>(out TutorialNodeBehaviour tutorialNodeBehaviour))
            {
                tutorialNodeBehaviour.SetUp(this);
            }
        }
    }


    Texture2D tex;

    public void StartTutorial(Texture2D targetTex)
    {
        tex = targetTex;
        Color[] pix = tex.GetPixels();

        ConvertImgToObjects(pix, tex);
    }

    private void ConvertImgToObjects(Color[] colors, Texture2D tex){
        int y = 0;
        for (int x = 0; x < tex.height; x++)
        {
            for (int u = 0; u < tex.width; u++)
            {
                if(colors[y] != new Color(colors[y].r,colors[y].b,colors[y].g,0f))
                {
                    SpawnObject();
                }
                y++;
            }
        }

        // var codeColorLength = 0;
        // for (int u = 1; u < tempcodedColors.Length; u++)
        // {
        //     if(tempcodedColors[u] != new Color(0f,0f,0f,0f)){
        //         codeColorLength++;
        //     }
        // }
        // colorCodes = new Color[codeColorLength];
        // for (int f = 0; f < colorCodes.Length; f++)
        // {
        //     colorCodes[f] = tempcodedColors[f];
        // }
    }

    private void SpawnObject()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreaMin.x + PlayerManager.PlayerTrans.position.x, spawnAreaMax.x+ PlayerManager.PlayerTrans.position.x),
            Random.Range(spawnAreaMin.y+ PlayerManager.PlayerTrans.position.y, spawnAreaMax.y+ PlayerManager.PlayerTrans.position.y),
            0
        );

        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
    }
}
