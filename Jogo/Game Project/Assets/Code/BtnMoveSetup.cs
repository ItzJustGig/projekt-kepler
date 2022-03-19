using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnMoveSetup : MonoBehaviour
{
    BattleSystem battleSystem;


    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();

        int i = int.Parse(this.transform.Find("Id").gameObject.GetComponent<Text>().text);

        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate { battleSystem.OnMoveBtn(i); });
    }
}
