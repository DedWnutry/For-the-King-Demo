using UnityEngine.UI;
using UnityEngine;

public class Cloth : Item
{
    public override void Start()
    {
        card = Instantiate(stats.cardPrefab);

        card.GetComponent<Icon>().sourceItem = gameObject;

        base.Start();

        ConvertToRegularMesh();
    }

    void ConvertToRegularMesh()
    {
        if (_equipped == false)
        {
            _RB.isKinematic = false;

            _RB.useGravity = true;

            _RB.freezeRotation = false;

            _myMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;

            _myMaterial = GetComponent<SkinnedMeshRenderer>().sharedMaterials;

            //Destroy(transform.GetComponent<SkinnedMeshRenderer>());

            transform.GetComponent<SkinnedMeshRenderer>().enabled = false;

            gameObject.AddComponent<MeshFilter>();

            GetComponent<MeshFilter>().sharedMesh = _myMesh;

            gameObject.AddComponent<MeshRenderer>();

            GetComponent<MeshRenderer>().sharedMaterials = _myMaterial;
            /*
            for (int i = 0; i < myBones.Count; i++)
            {
                GetComponent<SkinnedMeshRenderer>().bones[i] = myBones[i];
            }

            GetComponent<SkinnedMeshRenderer>().rootBone = myRootBone;*/

            myBones.Clear();

        }
    }

    public override void Equip(int slotNum)
    {

       

        _RB.useGravity = false;

        transform.parent.SetParent(GM.player.transform);

        transform.parent.gameObject.SetActive(true);

        ConvertToSkinnedMesh();

        GetComponent<MeshRenderer>().enabled = false;

        card.transform.position = card.transform.parent.position;

        GetComponent<SkinnedMeshRenderer>().enabled = true;

        //GetComponent<CapsuleCollider>().center = gameObject.GetComponent<SkinnedMeshRenderer>().bounds.center;

        _RB.isKinematic = true;

        //transform.position = GetComponent<CapsuleCollider>().bounds.center;

        //rbody.useGravity = false;

        //rbody.constraints = RigidbodyConstraints.FreezePosition;

        //rbody.freezeRotation = true;

        //rbody.constraints = RigidbodyConstraints.FreezeRotation;

        //GetComponent<Collider>().enabled = false;

        //GetComponent<CapsuleCollider>().center = gameObject.transform.position;

        //GetComponent<CapsuleCollider>().transform.rotation = gameObject.GetComponent<SkinnedMeshRenderer>().transform.rotation;

        //transform.position = GetComponent<SkinnedMeshRenderer>().transform.position;

        //GetComponent<CapsuleCollider>().center = gameObject.transform.position;

        //GetComponent<CapsuleCollider>().center = new Vector3(0, 0, 0);

        base.Equip(slotNum);
    }

    public override void Unequip()
    {
        ConvertToRegularMesh();

        base.Unequip();
    }

    public override void Store(int slotType)
    {
        if (slotType == -1)
        {
            if (_stored == false)
            {
                GM.player.GetComponent<ICharStats>().ChangeEquipload(stats.weight);

                _stored = true;

                transform.parent.gameObject.SetActive(false);
            }
        }
        else Equip(slotType);

        transform.parent.transform.SetParent(GM.player.transform);  
    }

    int CalculateResistance(int part, Collider coll, DamageType type)
    {
        int result = 0;

        //coll.GetComponent<Collider>().transform.root.GetComponent<Character>().HitBodyPart(part);

        switch (type)
        {
            case DamageType.Slash:
                {
                    if (coll.transform.root.GetComponent<Character>() != null && coll.transform.root.GetComponent<Character>().parts[part].equipment != null)
                    {
                        result = coll.transform.root.GetComponent<Character>().parts[part].equipment.GetComponentInChildren<Item>().stats.slashRes;
                    }

                    else result = 0;
                }
                break;

            case DamageType.Thrust:
                {
                    if (coll.transform.root.GetComponent<Character>() != null && coll.transform.root.GetComponent<Character>().parts[part].equipment != null)
                    {
                        result = coll.transform.root.GetComponent<Character>().parts[part].equipment.GetComponentInChildren<Item>().stats.thrustRes;
                    }

                    else result = 0;
                }
                break;

            case DamageType.Blunt:
                {
                    if (coll.transform.root.GetComponent<Character>() != null && coll.transform.root.GetComponent<Character>().parts[part].equipment != null)
                    {
                        result = coll.transform.root.GetComponent<Character>().parts[part].equipment.GetComponentInChildren<Item>().stats.bluntRes;
                    }

                    else result = 0;
                }
                break;
        }
        return result;
    }
}
