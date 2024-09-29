using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSum : MonoBehaviour
{
    public float phyDmgDealt, magicDmgDealt, trueDmgDealt, sanityDmgDealt;
    public float healDone, shieldDone, manaHealDone, staminaHealDone, sanityHealDone;
    public float phyDmgTaken, magicDmgTaken, trueDmgTaken, sanityDmgTaken;
    public float phyDmgMitigated, magicDmgMitigated;

    [SerializeField] private Text nameText;
    [SerializeField] private Image charcIcon;

    [SerializeField] private Text phyDmgDealtTxt;
    [SerializeField] private Text magicDmgDealtTxt;
    [SerializeField] private Text trueDmgDealtTxt;
    [SerializeField] private Text sanityDmgDealtTxt;
    [SerializeField] private Text totalDmgDealtTxt;

    [SerializeField] private Text phyDmgTakenTxt;
    [SerializeField] private Text magicDmgTakenTxt;
    [SerializeField] private Text trueDmgTakenTxt;
    [SerializeField] private Text sanityDmgTakenTxt;
    [SerializeField] private Text totalDmgTakenTxt;

    [SerializeField] private Text phyDmgMitigatedTxt;
    [SerializeField] private Text magicDmgMitigatedTxt;
    [SerializeField] private Text totalDmgMitigatedTxt;

    [SerializeField] private Text healDoneTxt;
    [SerializeField] private Text healManaDoneTxt;
    [SerializeField] private Text healStaminaDoneTxt;
    [SerializeField] private Text healSanityDoneTxt;
    [SerializeField] private Text shieldDoneTxt;

    public void UpdateValues(string name, Sprite icon)
    {
        //nameText.text = name;
        charcIcon.sprite = icon;

        phyDmgDealtTxt.text = phyDmgDealt.ToString();
        magicDmgDealtTxt.text = magicDmgDealt.ToString();
        trueDmgDealtTxt.text = trueDmgDealt.ToString();
        sanityDmgDealtTxt.text = sanityDmgDealt.ToString();
        totalDmgDealtTxt.text = (phyDmgDealt + magicDmgDealt + trueDmgDealt).ToString();

        phyDmgTakenTxt.text = phyDmgTaken.ToString();
        magicDmgTakenTxt.text = magicDmgTaken.ToString();
        trueDmgTakenTxt.text = trueDmgTaken.ToString();
        sanityDmgTakenTxt.text = sanityDmgTaken.ToString();
        totalDmgTakenTxt.text = (phyDmgTaken + magicDmgTaken + trueDmgTaken).ToString();

        phyDmgMitigatedTxt.text = phyDmgMitigated.ToString();
        magicDmgMitigatedTxt.text = magicDmgMitigated.ToString();
        totalDmgMitigatedTxt.text = (phyDmgMitigated + magicDmgMitigated).ToString();

        healDoneTxt.text = healDone.ToString();
        healManaDoneTxt.text = manaHealDone.ToString();
        healStaminaDoneTxt.text = staminaHealDone.ToString();
        healSanityDoneTxt.text = sanityHealDone.ToString();
        shieldDoneTxt.text = shieldDone.ToString();
    }
}
