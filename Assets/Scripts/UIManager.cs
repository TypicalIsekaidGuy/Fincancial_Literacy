using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject[] Panels;
    public GameObject[] buttons;
    public Sprite[] image;
    public Sprite[] pressedImage;
    public GameObject background;

    public void SwitchPanel(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if(index+1==i)
                buttons[i].GetComponent<Image>().sprite = pressedImage[i];
            else
                buttons[i].GetComponent<Image>().sprite = image[i];
        }
        background.SetActive(true);
        if (index == -1)
        {
            background.SetActive(false);
            foreach (var pan in Panels)
                pan.SetActive(false);
        }
        else
        {
            for (int i = 0; i < Panels.Length; i++)
            {
                if (i == index)
                    Panels[i].SetActive(true);
                else
                    Panels[i].SetActive(false);
            }
        }
    }
}