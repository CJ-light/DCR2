using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameOptimationManager : MonoBehaviour
{
    public int nFrame = 60; // division de trabajo en 60 bloques, un pez hara labores como detectar muros, cada 60 frames
    [DisplayWithoutEdit] public int currentFrame;
    [DisplayWithoutEdit] public int fishCount;

    private void Awake()
    {
        currentFrame = 0;
        fishCount = -1;
    }

    private void Update()
    {
        currentFrame = (currentFrame + 1) % nFrame;
    }

    public int RegisterFish()
    {
        fishCount++;
        return fishCount % nFrame;
    }
}
