using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler  //Добавляем чтобы работал OnPointerEnter и OnPointerExit
{
    public GameObject sourceItem;
    private ItemStats _stats;
    private Transform _statsPanel;
    public bool selected;
    public bool dragged;
    public bool rotated;
    private CanvasGroup canvasGroup;
    public bool slotDetected;
    public int iconHeight;
    public int iconWidth;
    public Image itemIcon;
    public Text itemTitle;
    public Text slashDmgTxt;
    public Text bluntDmgTxt;
    public Text thrustDmgTxt;
    public Image slashDmgIcon;
    public Image bluntDmgIcon;
    public Image thrustDmgIcon;
    public Sprite spriteSlash;
    public Sprite spriteBlunt;
    public Sprite spriteThrust;
    public Text strScaleTxt;
    public Text dexScaleTxt;
    public Text intScaleTxt;
    public Text slashResTxt;
    public Text bluntResTxt;
    public Text thrustResTxt;
    public Text fireResTxt;
    public Text coldResTxt;
    public Text poisonResTxt;
    public Text lightResTxt;
    public Text weightTxt;
    public Text priceTxt;
    public Text durabilityTxt;
    public Slider durabilitySlider;
    public Text descriptionTxt;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        _stats = sourceItem.GetComponent<Item>().stats;

        iconWidth = _stats.iconWidth;

        iconHeight = _stats.iconHeight;

        _statsPanel = transform.GetChild(0);

        ItemInfoCardSetup();
    }

    void ItemInfoCardSetup()
    {
        transform.name = _stats.itemTitle + " Stats";

        transform.SetParent(sourceItem.GetComponent<Item>().GM.HUD.transform);

        itemIcon.sprite = _stats.sprite;

        itemIcon.rectTransform.sizeDelta = new Vector2(iconWidth * 50, iconHeight * 50);

        itemTitle.text = _stats.itemTitle;

        weightTxt.text = _stats.weight.ToString();

        priceTxt.text = _stats.price.ToString();

        durabilityTxt.text = sourceItem.GetComponent<Item>().currDurrability.ToString() + " / " + _stats.durability.ToString();

        durabilitySlider.maxValue = _stats.durability;

        durabilitySlider.value = sourceItem.GetComponent<Item>().currDurrability;

        descriptionTxt.text = _stats.description;

        if (sourceItem.GetComponent<Weapon>() != null)
        {
            slashDmgTxt.text = _stats.swingDmg.ToString();

            bluntDmgTxt.text = _stats.aboveDmg.ToString();

            thrustDmgTxt.text = _stats.stabDmg.ToString();

            strScaleTxt.text = _stats.strScale.ToString() + "/" + _stats.strReq.ToString();

            dexScaleTxt.text = _stats.dexScale.ToString() + "/" + _stats.dexReq.ToString();

            intScaleTxt.text = _stats.intScale.ToString() + "/" + _stats.intReq.ToString();

            slashDmgTxt.text = _stats.swingDmg[0].value.ToString();

            bluntDmgTxt.text = _stats.aboveDmg[0].value.ToString();

            thrustDmgTxt.text = _stats.stabDmg[0].value.ToString();

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            if (_stats.swingDmg[0].dmgType == (DamageType)0) slashDmgIcon.sprite = spriteSlash;

                            if (_stats.swingDmg[0].dmgType == (DamageType)1) slashDmgIcon.sprite = spriteBlunt;

                            if (_stats.swingDmg[0].dmgType == (DamageType)2) slashDmgIcon.sprite = spriteThrust;
                        }
                        break;

                    case 1:
                        {
                            if (_stats.aboveDmg[0].dmgType == (DamageType)0) bluntDmgIcon.sprite = spriteSlash;

                            if (_stats.aboveDmg[0].dmgType == (DamageType)1) bluntDmgIcon.sprite = spriteBlunt;

                            if (_stats.aboveDmg[0].dmgType == (DamageType)2) bluntDmgIcon.sprite = spriteThrust;
                        }
                        break;

                    case 2:
                        {
                            if (_stats.stabDmg[0].dmgType == (DamageType)0) thrustDmgIcon.sprite = spriteSlash;

                            if (_stats.stabDmg[0].dmgType == (DamageType)1) thrustDmgIcon.sprite = spriteBlunt;

                            if (_stats.stabDmg[0].dmgType == (DamageType)2) thrustDmgIcon.sprite = spriteThrust;
                        }
                        break;
                }
            }
        }
        else if (sourceItem.GetComponent<Cloth>() != null)
        {
            slashResTxt.text = _stats.slashRes.ToString();

            bluntResTxt.text = _stats.bluntRes.ToString();

            thrustResTxt.text = _stats.thrustRes.ToString();

            fireResTxt.text = _stats.fireRes.ToString();

            coldResTxt.text = _stats.coldRes.ToString();

            poisonResTxt.text = _stats.poisonRes.ToString();

            lightResTxt.text = _stats.lightRes.ToString();

            itemTitle.text = _stats.itemTitle;

            weightTxt.text = _stats.weight.ToString();

            priceTxt.text = _stats.price.ToString();
        }

        _statsPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            if (dragged) RotateImage();
            else if (selected) ShowInfo();
        }
        else if (Input.GetMouseButtonUp(1)) HideInfo();   
    }
       
    public void OnDrop(PointerEventData eventData)
    {
        Transform contactTransform = eventData.pointerDrag.transform;

        if (contactTransform.GetComponent<Icon>() != null)
        {
            //contactTransform.GetComponent<Icon>().OnBeginDrag(eventData);

            if (contactTransform.GetComponent<Icon>().rotated == true) contactTransform.GetComponent<Icon>().RotateImage();

            contactTransform.parent.GetComponent<Slot>().SwapBack(contactTransform);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        slotDetected = false;

        itemIcon.raycastTarget = true;

        itemIcon.raycastTarget = true;

        canvasGroup.blocksRaycasts = false; 

        if (transform.parent.GetComponent<Slot>() != null)
        {
            if (transform.parent.GetComponent<Slot>().slotType == ItemSlot.Other) transform.parent.parent.GetComponentInChildren<Inventory>().EnableSlots(transform.parent.name, iconWidth, iconHeight);
        }
        GetComponent<RectTransform>().pivot = new Vector2(0f, 1.0f);

        dragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

        itemIcon.enabled = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!slotDetected && transform.parent.GetComponent<Slot>() != null)
        {
            if (sourceItem.GetComponent<Item>().Place() == true) itemIcon.enabled = false;
            else transform.parent.GetComponent<Slot>().SwapBack(transform);
        }

        canvasGroup.blocksRaycasts = true;

        dragged = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
    }

    public void EnableItemIcon()
    {
        itemIcon.enabled = true;
  
    }

    public void DisableItemIcon()
    {
        itemIcon.enabled = false;
    }

    public void ShowInfo()
    {
        _statsPanel.gameObject.SetActive(true);
    }

    public void HideInfo()
    {
        _statsPanel.gameObject.SetActive(false);
    }

    public void RotateImage()
    {
        if (!rotated)
        {
            rotated = !rotated;

            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 90));

            GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);

            GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);

            int iconWidthOld = iconWidth;

            int iconHeightOld = iconHeight;

            iconWidth = iconHeightOld;

            iconHeight = iconWidthOld;
        }
        else
        {
            rotated = !rotated;

            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);

            int iconWidthOld = iconWidth;

            int iconHeightOld = iconHeight;

            iconWidth = iconHeightOld;

            iconHeight = iconWidthOld;
        }
    }
}