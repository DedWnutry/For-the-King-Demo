using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public class ItemStats : ScriptableObject
{
    public string itemTitle;
    public GameObject cardPrefab;
    public float pickUpDistance = 50f;
    public Sprite sprite;
    public ItemSlot slotType;
    public Damage[] swingDmg;
    public Damage[] aboveDmg;
    public Damage[] stabDmg;
    public int slashRes;
    public int thrustRes;
    public int bluntRes;
    public int fireRes;
    public int coldRes;
    public int poisonRes;
    public int lightRes;
    public float strScale;
    public float dexScale;
    public float intScale;
    public int strReq;
    public int dexReq;
    public int intReq;
    public float weight;
    public int price;
    public int durability;
    public int iconWidth;
    public int iconHeight;
    public string description;
    public Sound[] sounds;
    
}
