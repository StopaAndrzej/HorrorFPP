using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class KidManager : InteractableObjectBase
{
    [SerializeField]  private List<Text> txts;

    //save value set by user in editor
    private float defaultAlphaValue;


    private void Start()
    {
        defaultAlphaValue = txts[0].color.a;
    }

    public override void InteractMulti()
    {
        foreach(Text element in txts)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, 1f); ;
        }
    }

    public override void DeInteractMulti()
    {
        foreach (Text element in txts)
        {
            element.color = new Color(element.color.r, element.color.g, element.color.b, defaultAlphaValue);
        }
    }

}
