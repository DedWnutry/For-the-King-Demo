using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.Audio;
using System.Collections.Generic;

public class Container : MonoBehaviour, IInteractableObject, IHitable
{
    private DungeonMaster _GM;
    private bool _inRadius;
    private bool _opened;
    [SerializeField] private Transform _containerUIPrefab;
    [SerializeField] private int _containerHeight;
    [SerializeField] private int _containerWidth;
    public SoundManager SM { get; set; }
    [SerializeField] private float _maxDurability;
    [SerializeField] private float _currDurability;
    [SerializeField] private float _discardingTime = 10f;
    [SerializeField] private float _breakThreshold = 10f;

    void Start()
    {
        _GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();

        SM = GetComponent<SoundManager>();

        _containerUIPrefab = Instantiate(_containerUIPrefab, _GM.HUD.transform);

        _containerUIPrefab.name = transform.name + " Inventory";

        _containerUIPrefab.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(_containerWidth * 50, _containerHeight * 50);

        _containerUIPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(_containerUIPrefab.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, _containerUIPrefab.GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 40);

        _containerUIPrefab.GetComponent<Text>().text = transform.name;

        for (int i = 0; i < _containerWidth * _containerHeight; i++)
        {
            Instantiate(_containerUIPrefab.GetChild(0).GetComponent<Inventory>().slotPrefab, _containerUIPrefab.GetChild(0));

            _containerUIPrefab.GetChild(0).GetChild(i).name = "Slot (" + i + ")";
        }

        _containerUIPrefab.GetChild(0).GetComponent<Inventory>().invWidth = _containerWidth;

        _containerUIPrefab.GetChild(0).GetComponent<Inventory>().invHeight = _containerHeight;

        _containerUIPrefab.GetChild(0).GetComponent<Inventory>().SetupInventory();

        _currDurability = _maxDurability;
    }

    void Update()
    {
        
        if (_opened)
        {
            if (!_inRadius) OpenOrCloseContainer();

            _containerUIPrefab.transform.position = _GM.mainCam.WorldToScreenPoint(transform.position);
        }
    }

    public void GetHit(float amount, DamageType type, Transform part)
    {
        SM.PlaySound("GetHit");

        //GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity * 10f, ForceMode.Impulse);

        if (type != DamageType.Blunt) amount /= 2;

        ChangeDurability(amount);
    }

    void ChangeDurability(float value)
    {
        _currDurability += value;

        if (_currDurability <= 0) 
        {
            Break();
            foreach (Transform item in transform.parent.GetChild(2)) item.SetParent(null);
        }
    }

    void Break()
    {
        _currDurability = 0;

        transform.parent.GetChild(2).gameObject.SetActive(true);

        transform.parent.GetChild(2).transform.position = transform.position;

        SM.PlaySound("Broke");

        transform.parent.GetChild(1).gameObject.SetActive(true);

        transform.parent.GetChild(1).transform.position = transform.position;

        Invoke("DiscardParts", _discardingTime);

        foreach (Collider coll in transform.GetComponents<Collider>()) coll.enabled = false;

        GetComponent<Rigidbody>().isKinematic = true;

        GetComponent<Rigidbody>().useGravity = false;

        Debug.Log(transform.name + " broke");

        GetComponent<MeshRenderer>().enabled = false;
    }

    void DiscardParts()
    {
        Destroy(transform.parent.gameObject);
    }

    public void Interact()
    {
        if (_inRadius) OpenOrCloseContainer();
    }

    void OpenOrCloseContainer()
    {
        if (!_opened) _containerUIPrefab.gameObject.SetActive(true);
        else _containerUIPrefab.gameObject.SetActive(false);

        _opened = !_opened;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (GetComponent<Rigidbody>().velocity.magnitude >= _breakThreshold) GetHit(-GetComponent<Rigidbody>().velocity.magnitude * GetComponent<Rigidbody>().mass, DamageType.Blunt, null);
    }
    
    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.transform.root.gameObject == _GM.player) _inRadius = true;
    }

    void OnTriggerStay(Collider trigger)
    {
        if (trigger.transform.root.gameObject == _GM.player) _inRadius = true;
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.root.gameObject == _GM.player) _inRadius = false;
    }
}
