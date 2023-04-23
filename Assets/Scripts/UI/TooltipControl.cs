using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipControl : MonoBehaviour
{
    private static TooltipControl current;

    public Tooltip tooltip;
    public void Awake()
    {
        current = this;
    }

    public static void ShowTooltip(string content)
    {
        current.tooltip.SetText(content);
        current.tooltip.gameObject.SetActive(true);


    }

    public static void HideTooltip()
    {
        current.tooltip.gameObject.SetActive(false);
        
    }
}
