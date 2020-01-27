using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelSet : MonoBehaviour
{
    public int index;
    public Text[] text;
    public Image image;
    public Button button;
    JewelSniping snipe;

    public Color even = Color.gray;
    public Color odd = Color.black;
    public Color pressedColor = Color.cyan;
    Color curColor;
    public bool pressed = false;

    public void Create(string[] jewels, int c, JewelSniping js)
    {
        index = c;
        snipe = js;

        for (int i = 0; i < text.Length; i++)
        {
            text[i].text = jewels[i];
        }

        if (c % 2 == 0)
        {
            image.color = even;
        }
        else
        {
            image.color = odd;
        }

        curColor = image.color;
    }

    public void UpdateIndex(int i)
    {
        index = i;

        if (i % 2 == 0)
        {
            image.color = even;
        }
        else
        {
            image.color = odd;
        }

        curColor = image.color;
    }

    public void MarkSet()
    {
        if (pressed)
        {
            
            image.color = curColor;
        }
        else
        {
            image.color = pressedColor;
        }
        pressed = !pressed;
    }
}
