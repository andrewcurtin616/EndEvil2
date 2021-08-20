using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;//added
using UnityEngine.UI;//added

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    /*void Start()
	{
		GetComponent<Renderer>().material.color = Color.white;
	}

	void OnMouseEnter()
	{
		GetComponent<Renderer>().material.color = Color.red;
	}

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }*/


    string tempString;

    private void Start()
    {
        tempString = GetComponentInChildren<Text>().text;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().text = "= " + tempString + " =";
        GameObject.FindObjectOfType<TitleScreenController>().HoverNoise();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().text = tempString;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInChildren<Text>().text = tempString;
    }
}
