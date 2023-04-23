using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;

    // Start is called before the first frame update
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipControl.ShowTooltip(content);
    }

    // Update is called once per frame
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipControl.HideTooltip();
    }
}
