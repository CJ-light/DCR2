using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusSubmenu : MonoBehaviour
{
    public Button yourButton;
    public List<Button> otherButtons;
    public GameObject to_show;
    public List<GameObject> to_hide;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if (to_show.activeInHierarchy == false)
        {
            to_show.SetActive(true);

            for (int i = 0; i < to_hide.Count; i++)
            {
                to_hide[i].SetActive(false);
            }
            
            yourButton.GetComponent<Image>().color = Color.black;
            yourButton.transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.white;
            for (int i = 0; i < otherButtons.Count; i++)
            {
                otherButtons[i].GetComponent<Image>().color = Color.white;
                otherButtons[i].transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.black;
            }
        }
    }
}
