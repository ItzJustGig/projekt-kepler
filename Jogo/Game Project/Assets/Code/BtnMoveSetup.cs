using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnMoveSetup : MonoBehaviour
{
    ActionBox actionBox;

    int i = -1;

    void Start()
    {
        actionBox = this.transform.parent.parent.parent.parent.parent.parent.gameObject.GetComponent<ActionBox>();

        SetId(int.Parse(this.transform.Find("Id").gameObject.GetComponent<Text>().text));

        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate { actionBox.OnMoveBtn(i); });
    }

    void SetId(int id)
    {
        i = id;
    }

    public void UpdateToolTip(string text)
    {
        this.GetComponent<TooltipButton>().text = text;
    }

    public void UpdateToolTip(string text, string sec)
    {
        this.GetComponent<TooltipButton>().text = text;
        this.GetComponent<TooltipButton>().textSec = sec;
    }

    public int GetId()
    {
        return i;
    }
}
