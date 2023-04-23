using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class globalVals : MonoBehaviour
{
    // Start is called before the first frame update
    public bool gameJustEnter = true;
    public static globalVals instance;
    public int frameRateTarget;
    private int targetFrameRate;

    int m_frameCounter = 0;
    private float m_timeCounter = 0.0f;
    public float m_lastFramerate = 0.0f;
    private float m_refreshTime = 0.5f;

    public GameObject player;
    void Start()
    {
    	QualitySettings.vSyncCount = 0;
    	Application.targetFrameRate = frameRateTarget;
        DontDestroyOnLoad(this);
        instance = this;
    }
    void Update(){
        

    	if(Application.targetFrameRate != frameRateTarget){
              Application.targetFrameRate = frameRateTarget;
        }

        if( m_timeCounter < m_refreshTime )
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0
            m_lastFramerate = (float)m_frameCounter/m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }
    
    
}

