using UnityEngine.UI;
using UnityEngine;

public class Weapon : Item
{
    public override void Start()
    {
        card = Instantiate(stats.cardPrefab);

        card.GetComponent<Icon>().sourceItem = gameObject;

        base.Start();
    }

    public override void Equip(int slotNum)
    {
        _RB.isKinematic = false;

        _RB.useGravity = true;

        //_RB.freezeRotation = true;

        //_RB.constraints = RigidbodyConstraints.FreezePosition;

        _RB.constraints = RigidbodyConstraints.FreezeAll;

        GetComponent<Collider>().enabled = false;

        transform.gameObject.SetActive(true);

        if (slotNum == 11)
        {
            transform.position = GM.player.GetComponent<Character>()._holdPointRight.position;

            transform.rotation = GM.player.GetComponent<Character>()._holdPointRight.rotation;

            transform.parent.SetParent(GM.player.GetComponent<Character>()._holdPointRight);
        }
        if (slotNum == 12)
        {
            transform.position = GM.player.GetComponent<Character>()._holdPointLeft.position;

            transform.rotation = GM.player.GetComponent<Character>()._holdPointLeft.rotation;

            transform.parent.SetParent(GM.player.GetComponent<Character>()._holdPointLeft);
        }
        /*
        transform.gameObject.AddComponent<MeshFilter>();

        transform.GetComponent<MeshFilter>().sharedMesh = _myMesh;

        transform.gameObject.AddComponent<MeshRenderer>();

        transform.GetComponent<MeshRenderer>().sharedMaterials = _myMaterial;
          
        transform.GetComponent<SkinnedMeshRenderer>().rootBone = _myRootBone;

        myBones.Clear();

        transform.GetComponent<SkinnedMeshRenderer>().enabled = false;

        //Destroy(GetComponent<SkinnedMeshRenderer>());*/

        base.Equip(slotNum);
    }

    public override void Store(int slotType)
    {
        if (slotType == -1)
        {
            if (_stored == false)
            {
                GM.player.GetComponent<ICharStats>().ChangeEquipload(stats.weight);

                _stored = true;   
            }

            transform.parent.SetParent(GM.player.transform);

            transform.gameObject.SetActive(false);
        }
        else Equip(slotType);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_equipped)
        {
            float dmg = 0;

            int attackType = transform.root.GetComponent<Character>().animator.GetInteger("AttackType");

            DamageType dmgType = stats.swingDmg[0].dmgType;

            Transform part = collision.collider.transform;

            switch (attackType)
            {
                case 1:
                    {
                        dmg = stats.swingDmg[0].value;

                        dmgType = stats.swingDmg[0].dmgType;
                    }
                    break;

                case 2:
                    {
                        dmg = stats.stabDmg[0].value;

                        dmgType = stats.stabDmg[0].dmgType;
                    }
                    break;

                case 3:
                    {
                        dmg = stats.aboveDmg[0].value;

                        dmgType = stats.aboveDmg[0].dmgType;
                    }
                    break;

                case 4:
                    {
                        dmg = stats.swingDmg[0].value;

                        dmgType = stats.swingDmg[0].dmgType;
                    }
                    break;
            }

            if (collision.collider.gameObject.layer == 6)
            {
                
                //Collider myCollider = collision.GetContact(0).thisCollider;

                if (collision.collider.transform.root.transform != transform.root.transform & collision.collider.transform.root.GetComponent<IHitable>() != null & !hittedTargets.Contains(collision.collider.transform.root.transform))
                {
                    hittedTargets.Add(collision.collider.transform.root.transform);

                    collision.collider.transform.root.transform.GetComponent<IHitable>().GetHit(-dmg, dmgType, part);


                    Debug.Log(collision.collider.transform.root.name + " got hit with " + transform.name + " taking " + dmg + " damage (" + dmgType + ") to " + part.name + " from " + transform.root.name);
                }
                
            }
            
            if (collision.collider.gameObject.layer == 3)
            {
                SM.PlaySound("Hit");

                GM.player.GetComponent<Character>().Knockback();

                GM.player.GetComponent<Player>().Noise(collision.contacts[0].point, 10 * _RB.mass);

                Debug.Log(transform.root.name + " hits " + collision.collider.transform.name);

                if (collision.collider.GetComponent<IHitable>() != null) collision.collider.GetComponent<IHitable>().GetHit(-dmg, dmgType, part);
            }   
        }
    }
}
