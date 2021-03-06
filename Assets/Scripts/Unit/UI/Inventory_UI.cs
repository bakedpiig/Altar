﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    public GameObject player;
    public GameObject inventory;
    private float inventoryWidth;

    public GameObject selectedImage;
    private bool selectedCreate;
    private GameObject selected;

    public void Start()
    {
        inventoryWidth = inventory.GetComponent<RectTransform>().rect.width;

        float canvas = inventory.transform.parent.GetComponent<RectTransform>().localScale.x;
        inventoryWidth *= canvas;

        selectedCreate = false;

        SetSelectedSprite(0);
    }

    public void InventoryUI(BaseEventData _data)
    {
        var data = _data as PointerEventData;
        Vector3 position = data.position;

        int index = 0;
        for(;index<5;index++)
        {
            if (inventoryWidth / 5 * index + inventory.transform.position.x <= position.x &&
                position.x < inventoryWidth / 5 * (index + 1) + inventory.transform.position.x)
                break;
        }

        SetSelectedSprite(index);

        player.SendMessage("SelectItem", index);
    }

    public void SetSelectedSprite(int index)
    {
        if(!selectedCreate)
        {
            selected = Instantiate(selectedImage, new Vector3(inventoryWidth / 5 * index + inventory.transform.position.x + inventoryWidth / 10, inventoryWidth / 10), new Quaternion(0, 0, 0, 0), inventory.transform.parent);
            selectedCreate = true;
        }
        else
        {
            selected.transform.position = new Vector3(inventoryWidth / 5 * index + inventory.transform.position.x + inventoryWidth / 10, inventoryWidth / 10);
        }
    }
}
