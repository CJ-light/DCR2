using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHideOptions : MonoBehaviour
{

    [SerializeField] KeyCode toggleOptionsKey;
    public GameObject opciones;
    public GameObject elMenuInicial;
    public Button quit;
    public Camera camara;
    public string scrpt;
    public List<GameObject> advHide;

    void Start()
    {
        Button btn = quit.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleOptionsKey) && !elMenuInicial.activeSelf)
        {
            opciones.SetActive(!opciones.activeSelf);

            if (opciones.activeSelf)
            {
                ShowMouseCursor();
            }
            else
            {
                HideMouseCursor();
                for (int i = 0; i < advHide.Count; i++)
                {
                    advHide[i].SetActive(false);
                }
            }
        }
    }

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        (camara.GetComponent(scrpt) as MonoBehaviour).enabled = false;
    }

    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        (camara.GetComponent(scrpt) as MonoBehaviour).enabled = true;
    }

    void TaskOnClick()
    {
        opciones.SetActive(false);
        HideMouseCursor();
    }
}
