using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class mainmenu : MonoBehaviour
{
    public GameObject camara;
    public GameObject cameraVR;
    public GameObject load;
    public GameObject MainM;
    public GameObject img;
    public bool VR;
    void Start()
    {
        
        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);
        load.SetActive(false);
        MainM.SetActive(true);
        img.SetActive(true);
        if (VR == true)
        {
            cameraVR.SetActive(true);
        }
        else
        {
            camara.SetActive(true);
        }


    }
    public void StartGame()
    {
        Cursor.visible = false;
    }

}
