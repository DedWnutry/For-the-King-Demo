using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class Slot : MonoBehaviour, IDropHandler
{
    public bool occupied;
    public ItemSlot slotType;

    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;

        if (this.occupied == false)
        {
            switch (transform.name)
            {
                case "Head Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Head)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 0);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Torso Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Torso)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 2);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Arms Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Arm)
                        {

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 3);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Legs Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Leg)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 7);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Foot Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Leg)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 3);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Weapon Right Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Weapon)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 11);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

                case "Weapon Left Slot":
                    {
                        if (otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().stats.slotType == ItemSlot.Weapon)
                        {
                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SetInEquipSlot(otherItemTransform, 12);

                            otherItemTransform.transform.position = transform.position;

                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        else
                        {
                            otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                            if (otherItemTransform.GetComponent<Icon>().rotated == true) otherItemTransform.GetComponent<Icon>().RotateImage();

                            SwapBack(otherItemTransform);
                        }
                    }
                    break;

               case "Inventory":
                    {
                        otherItemTransform.GetComponent<Icon>().slotDetected = true;

                        SwapBack(otherItemTransform);
                    }
                    break;

                default:
                    {
                        if (transform.parent.GetComponent<Inventory>().CheckForFreeSpace(transform.name, eventData.pointerDrag.transform.GetComponent<Icon>().iconWidth, eventData.pointerDrag.transform.GetComponent<Icon>().iconHeight))
                        {
                            SetInEquipSlot(otherItemTransform, -1);

                            otherItemTransform.transform.position = transform.parent.GetComponent<Inventory>().StoreInBag(transform.name, otherItemTransform.GetComponent<Icon>().iconWidth, otherItemTransform.GetComponent<Icon>().iconHeight);

                        }
                        else
                        {
                            if (otherItemTransform.transform.parent.parent == transform.parent)
                            {
                                SwapBack(otherItemTransform);

                                transform.parent.GetComponent<Inventory>().StoreInBag(otherItemTransform.parent.transform.name, otherItemTransform.GetComponent<Icon>().iconWidth, otherItemTransform.GetComponent<Icon>().iconHeight);

                                otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().Store(-1);
                            }
                            else if (GetComponent<Slot>().occupied == true)
                            {
                                
                                SwapBack(otherItemTransform);
                            }
                            else 
                            {
                                otherItemTransform.GetComponent<Image>().enabled = false;

                            }
                        }
                    }
                    break;
            }
        }      
    }

    public void SetInEquipSlot(Transform otherItemTransform, int slotNum)
    {
        otherItemTransform.GetComponent<Icon>().slotDetected = true;

        otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().Store(slotNum);

        otherItemTransform.SetParent(transform);

        //otherItemTransform.SetParent(otherItemTransform.GetComponent<Icon>().sourceItem.GetComponent<Item>().player.GetComponent<Character>().inventory);

        otherItemTransform.GetComponent<Image>().raycastTarget = true;  
    }

    public void SwapBack(Transform otherItemTransform)
    {
        if (otherItemTransform.parent.GetComponent<Slot>() != null)
        {
            if (otherItemTransform.parent.GetComponent<Slot>().slotType == ItemSlot.Other) otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            else otherItemTransform.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            otherItemTransform.transform.position = otherItemTransform.transform.parent.transform.position;
        }
        else
        {
            otherItemTransform.transform.position = Vector3.zero;

            otherItemTransform.GetComponent<Icon>().itemIcon.enabled = false;
        }
            


        otherItemTransform.GetComponent<Icon>().slotDetected = true;

        
    }
}
