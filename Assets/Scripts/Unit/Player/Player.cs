﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Unit
{
    public static Player Instance { get; private set; }
    public float maxDistance;
    public float invincibleTime;

    public GameObject gameManager;

    public Text interactionText;

    public Transform equippedItemTransform;
    private float handDistance;

    public GameObject inventoryUI;

    public bool isCreated = false;
    public bool isHungerZero = false;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        hunger = 100f;
        health = 10;
        isAttacked = false;

        interactionObj = null;

        inventory = new string[5];

        invenQuantity = new int[5];

        handDistance = Vector3.Distance(equippedItemTransform.transform.position, new Vector3(0, -0.05f));

        blinkColor = new Color[2] { new Color(0, 0, 0, 0), GetComponent<SpriteRenderer>().color };

        pinnedRecipes = new List<string>();

        equippedItem = equippedItemTransform.GetComponent<EquippedItem>();

        itemCells = inventoryUI.GetComponentsInChildren<ItemCell>();

        isCreated = true;

        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (Hunger > 0f)
            Hunger -= Time.deltaTime / 6;
        else if(!isHungerZero)
        {
            StartCoroutine(DecreaseHealth());
            isHungerZero = true;
        }

        Move(faceDirection);

        if (interactionObj != null)
        {
            if (!interactionObj.CompareTag("DroppedItem") && Vector3.SqrMagnitude(interactionObj.transform.position - transform.position) >= 0.25f)
            {
                Vector3 scale = transform.localScale;
                if (interactionObj.transform.position.x > transform.position.x)
                    scale.x = Mathf.Abs(scale.x);
                else
                    scale.x = -Mathf.Abs(scale.x);

                transform.localScale = scale;

                if (equippedItem.state != EquippedItem.State.Swing)
                {
                    if (interactionObj != null)
                    {
                        SetEquippedItemTransform(interactionObj.transform.position - transform.position);
                    }
                    else
                    {
                        SetEquippedItemTransform(faceDirection);
                    }
                }
            }
        }
        else
        {
            Vector3 scale = transform.localScale;
            if (faceDirection.x > 0)
                scale.x = Mathf.Abs(scale.x);
            else if (faceDirection.x < 0)
                scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;
            SetEquippedItemTransform(faceDirection);
        }

        interactionObj = null;

        var colliders = Physics2D.OverlapCircleAll(transform.position, maxDistance);

        float minDistance = maxDistance * maxDistance;
        foreach (var collider in colliders) {
            if (!collider.CompareTag("Player"))
            {
                if(collider.CompareTag("Portal"))
                {
                    if (Vector3.SqrMagnitude(collider.transform.position - transform.position) <= 1.0f)
                    {
                        interactionObj = collider.gameObject;
                        break;
                    }
                    else continue;
                }
                float tempDistance = Vector3.SqrMagnitude(collider.transform.position - transform.position);

                if(tempDistance<=1.0f && collider.CompareTag("DroppedItem"))
                {
                    droppedItem = collider.gameObject;
                }
                else if (tempDistance <= minDistance)
                {
                    minDistance = tempDistance;
                    interactionObj = collider.gameObject;
                }
            }
        }

        gameManager.GetComponent<MapManager>().CheckPositionInTilemap(gameObject);

        if(interactionObj!=null && interactionObj.CompareTag("Altar"))
        {
            interactionText.text = "제단";
        }
        else if(interactionObj != null && interactionObj.CompareTag("Portal"))
        {
            interactionText.text = "이동";
        }
        else if(inventory[invenIdx]==null)
        {
            interactionText.text = "상호작용";
        }
        else if(Item.itemDictionary[inventory[invenIdx]] is Food || Item.itemDictionary[inventory[invenIdx]] is Sacrifice)
        {
            interactionText.text = "먹기";
        }
        else if(Item.itemDictionary[inventory[invenIdx]] is MeleeWeapon || Item.itemDictionary[inventory[invenIdx]] is RangedWeapon)
        {
            interactionText.text = "공격";
        }

        if (faceDirection != Vector3.zero && !isMoving)
            animator.SetBool("isMoving", isMoving = true);
        else if(faceDirection == Vector3.zero && isMoving)
            animator.SetBool("isMoving", isMoving = false);
    }

    public void FirstSetting()
    {
        inventory[0] = "Knife";
        invenQuantity[0] = 1;

        SelectItem(0);
        itemCells[0].GetComponent<Image>().color = Color.white;
        itemCells[0].SetItemCell(inventory[0], invenQuantity[0]);
    }
}
