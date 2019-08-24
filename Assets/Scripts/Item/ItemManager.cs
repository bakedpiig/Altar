﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField]
    private ItemInfoUI itemInfoUI;
    private Dictionary<string, Item> itemDictionary;
    private Sprite droppedItem;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        itemDictionary = new Dictionary<string, Item>();
    }

    private void Start()
    {
        LoadItemJson();
        LoadItemSprite();
    }

    public void DropItem(string itemName, Vector3 position)
    {
        droppedItem = itemDictionary[itemName].Sprite;
        Instantiate(droppedItem, position, Quaternion.Euler(0, 0, 0)).name = itemDictionary[itemName].ItemName;
    }

    public Item GetItem(string itemName) => itemDictionary[itemName];

    public void OpenItemInfo(string itemName, Transform transform)
    {
        itemInfoUI.gameObject.SetActive(true);
        itemInfoUI.OpenItemInfoUI(itemDictionary[itemName]);
        itemInfoUI.transform.position = transform.position;
    }

    public void CloseItemInfo()
    {
        itemInfoUI.gameObject.SetActive(false);
    }

    private void LoadItemSprite()
    {
        List<Sprite> foodSprite = new List<Sprite>(Resources.LoadAll<Sprite>("Food"));
        foodSprite.ForEach((sprite) =>
        {
            if (itemDictionary.ContainsKey(sprite.name))
            {
                itemDictionary[sprite.name].Sprite = sprite;
            }
        });
    }

    private void LoadItemJson()
    {
        string json = File.ReadAllText($"{Application.dataPath}/Data/{nameof(Food)}.json");

        ItemList<Food> foodList = JsonUtility.FromJson<ItemList<Food>>(json);
        foodList.items.ForEach((food) => { itemDictionary.Add(food.ItemName, food); });

        json = File.ReadAllText($"{Application.dataPath}/Data/{nameof(Sacrifice)}.json");

        ItemList<Sacrifice> sacrificeList = JsonUtility.FromJson<ItemList<Sacrifice>>(json);
        sacrificeList.items.ForEach((sacrifice) => { itemDictionary.Add(sacrifice.ItemName, sacrifice); });
    }

    private void SaveItemJson()
    {
        List<Food> foods = new List<Food>();

        foods.Add(new Food("Bread", 3));
        foods.Add(new Food("Apple", 1));
        foods.Add(new Food("Steak", 5));

        ItemList<Food> foodList = new ItemList<Food>(foods);
        File.WriteAllText($"{Application.dataPath}/Data/{nameof(Food)}.json", JsonUtility.ToJson(foodList));

        List<Sacrifice> sacrifices = new List<Sacrifice>();

        sacrifices.Add(new Sacrifice("RottenApple", 1, 1));
        sacrifices.Add(new Sacrifice("Larva", 2, 2));
        sacrifices.Add(new Sacrifice("Boar", 4, 3));

        ItemList<Sacrifice> sacrificeList = new ItemList<Sacrifice>(sacrifices);
        File.WriteAllText($"{Application.dataPath}/Data/{nameof(Sacrifice)}.json", JsonUtility.ToJson(sacrificeList));
    }

    private class ItemList<T> where T : Item
    {
        public List<T> items;

        public ItemList(List<T> ts)
        {
            items = ts;
        }
    }
}