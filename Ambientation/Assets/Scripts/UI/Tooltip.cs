using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Text contentField;
    public LayoutElement layoutelement;

    public int characterWrapLimit;

    public void SetText(string content)
    {
        contentField.text = content;
        int contentLength = contentField.text.Length;

        layoutelement.enabled = (contentLength > characterWrapLimit) ? true : false;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Application.isEditor)
        {
            int contentLength = contentField.text.Length;

            layoutelement.enabled = (contentLength > characterWrapLimit) ? true : false;
        }

        Vector2 position = Input.mousePosition + new Vector3(0, 50, 0);

        transform.position = position;
    }
}
