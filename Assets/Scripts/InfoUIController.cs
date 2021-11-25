using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUIController: MonoBehaviour
{
    public GameObject infoUI;
    public TextMeshProUGUI[] textos;
    public TextAsset info;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            infoUI.SetActive(true);
            RellenarUI();
        }
    }

    private void RellenarUI()
    {
        string[] infoText = info.text.Split('\n');
        for(int i = 0; i < infoText.Length; i++)
        {
            textos[i].text = infoText[i];
            if(i == 2)
            {
                Image parent = textos[i].gameObject.GetComponentInParent<Image>();
                if(infoText[i].Trim() == "No")
                {
                    parent.color = Color.red;
                }
                else
                {
                    parent.color = Color.green;
                }
            }
        }
    }

}
