using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour, IInteractableObject
{
    public DungeonMaster GM;
    public SoundManager SM;
    private RaycastHit _hit;
    private GameObject _bodyMesh;
    protected List<Transform> myBones = new List<Transform>();
    private Transform _myRootBone;
    protected Mesh _myMesh;
    protected Material[] _myMaterial;
    private Transform _rootParent;
    protected Rigidbody _RB;
    private Collider _coll;
    public GameObject card;
    [SerializeField] protected bool _equipped;
    [SerializeField] protected bool _stored;
    public bool selected;
    public int currDurrability;  
    public List<Transform> hittedTargets = new List<Transform>();
    public ItemStats stats;

    void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();

        SM = gameObject.AddComponent<SoundManager>();

        SM.sounds = new Sound[stats.sounds.Length];

        for (int i = 0; i < stats.sounds.Length; i++)
        {
            SM.sounds[i] = stats.sounds[i];
        }
    }

    public virtual void Start()
    {
        //bodyMesh = _player.transform.Find("Body").gameObject;

        _RB = GetComponent<Rigidbody>();

        _coll = GetComponentInChildren<Collider>();

        _RB.mass = stats.weight;

        currDurrability = stats.durability;
    }

    void Update()
    {
        if (!_equipped)
        {
            if (!selected && !card.GetComponent<Icon>().dragged)
            {
                card.GetComponent<Icon>().DisableItemIcon();

                card.GetComponent<Icon>().HideInfo();
            }
            else
            {
                card.GetComponent<Icon>().EnableItemIcon();

                if (_stored || GM.paused)
                {
                    if (!card.GetComponent<Icon>().dragged) card.transform.position = Input.mousePosition;
                }
                else card.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            }
        }

        selected = false;
    }

    public virtual void ConvertToSkinnedMesh()
    {
        Destroy(GetComponent<MeshFilter>());

        Destroy(GetComponent<MeshRenderer>());

        gameObject.AddComponent<SkinnedMeshRenderer>();

        GetComponent<SkinnedMeshRenderer>().sharedMesh = _myMesh;

        _rootParent = transform.root.transform;

        for (int i = 0; i < _rootParent.childCount; i++)
        {
            if (_rootParent.GetChild(i).gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            {
                _bodyMesh = _rootParent.GetChild(i).gameObject;

                break;
            }
        }

        myBones.Clear();

        GetComponent<SkinnedMeshRenderer>().rootBone = _myRootBone;

        foreach (Transform element in GetComponent<SkinnedMeshRenderer>().bones) myBones.Add(element);

        _myRootBone = GetComponent<SkinnedMeshRenderer>().rootBone;

        GetComponent<SkinnedMeshRenderer>().bones = _bodyMesh.GetComponent<SkinnedMeshRenderer>().bones;

        GetComponent<SkinnedMeshRenderer>().rootBone = _bodyMesh.GetComponent<SkinnedMeshRenderer>().rootBone;
    }

    public virtual void Equip(int slotNum)
    {
        _equipped = true;

        if (_stored == false)
        {
            GM.player.GetComponent<ICharStats>().ChangeEquipload(stats.weight);

            _stored = true;
        }

        GM.player.GetComponent<Character>().parts[slotNum].equipment = GetComponent<Transform>();
    }

    public virtual void Unequip()
    {
        card.transform.SetParent(GM.HUD.transform);

        transform.parent.transform.SetParent(null);

        for (int i = 0; i < GM.player.GetComponent<Character>().parts.Count; i++)
        {
            if (GM.player.GetComponent<Character>().parts[i].equipment == transform) 
            {
                GM.player.GetComponent<Character>().parts[i].equipment = null;

                break;
            }
        }

        _stored = false;

        _equipped = false;

        _RB.isKinematic = false;

        _RB.useGravity = true;

        _RB.freezeRotation = false;

        _RB.isKinematic = false;
    }

    public virtual void Store(int slotType)
    {
        if (_equipped) Unequip();
    }

    public bool Place()
    {
        bool result = false;

        if (GM.paused && card.transform.parent.GetComponent<Slot>() != null)
        {
            RaycastHit hit;

            Ray ray = GM.mainCam.ScreenPointToRay(Input.mousePosition); //Выпускает луч из камеры в точку, в которой находится курсор на экране

            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                float placingDistance = Vector3.Distance(GM.player.transform.position, hit.point); //Вычисляю расстояние точку, куда можно поместить объект

                if (hit.transform != null && placingDistance <= 5f && placingDistance > 0.5f) //Только если расстояние до поверхности адекватное
                {
                    _stored = false;

                    GM.player.GetComponent<ICharStats>().ChangeEquipload(-stats.weight);

                    transform.parent.gameObject.SetActive(true);

                    _RB.isKinematic = false;

                    _RB.useGravity = true;

                    _RB.freezeRotation = false;

                    _coll.enabled = true;

                    gameObject.transform.position = hit.point; //Предмет появляется в месте, куда попал луч

                    if (_equipped == true)
                    {
                        _equipped = false;

                        Unequip();

                        GetComponent<MeshRenderer>().enabled = true;
                    }

                    Drop();

                    result = true;
                }
            }
            //else if (equipped == true) Drop();
        }

        return result;
    }

    void Drop()
    {
        gameObject.SetActive(true);

        _RB.isKinematic = false;

        card.transform.SetParent(GM.HUD.transform);
    }

    void Durability(int amount)
    {
        currDurrability += amount;

        SM.PlaySound("GetHit");

        if (currDurrability <= 0)
        {
            Debug.Log(transform.parent.name + " broke");

            SM.PlaySound("ItemBroke");

            Destroy(card);

            Destroy(transform.parent.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (GetComponent<Rigidbody>().velocity.magnitude >= 2f) Durability(-(int)(GetComponent<Rigidbody>().velocity.magnitude * GetComponent<Rigidbody>().mass));
    }

    public void Interact()
    {
        Take();
    }

    void Take()
    {
        Inventory inv = GM.player.GetComponent<Character>().inventory.GetComponentInChildren<Inventory>();

        bool placed = false;

        for (int i = 0; i < inv.invHeight; i++)
        {
            for (int j = 0; j < inv.invWidth; j++)
            {
                if (inv.CheckForFreeSpace(inv.invMatrix[i, j].transform.name, stats.iconWidth, stats.iconHeight))
                {

                    inv.invMatrix[i, j].GetComponent<Slot>().SetInEquipSlot(card.transform, -1);

                    card.transform.position = inv.StoreInBag(inv.invMatrix[i, j].name, stats.iconWidth, stats.iconHeight);

                    //_card.GetComponent<Image>().enabled = true;

                    placed = true;

                    break;
                }
            }
           
            if (placed) break;
        }
    }

    void OnMouseOver()
    {
        if (!_equipped && Vector3.Distance(GM.player.transform.position, gameObject.transform.position) <= stats.pickUpDistance && Vector3.Distance(GM.mainCam.transform.position, gameObject.transform.position) > 0.5f && GM.player.GetComponent<Character>().inventory.parent.gameObject.activeSelf == true) selected = true;
    }


    void OnMouseExit()
    {
        if (!_equipped) selected = false;
    }

    void OnGUI() //Привязывает объект к положению мыши
    {
        /*
        if (Input.GetMouseButton(0) && equipped == false)
        {      
            Vector2 actualScreenPosition = new Vector2(Event.current.mousePosition.x, Screen.height - (Event.current.mousePosition.y + 25));

            imgObject.transform.position = actualScreenPosition; 
        }
        */
    }  
}

public enum ItemSlot { Weapon, Arm, Leg, Torso, Head, Ring, Necklace, Other }