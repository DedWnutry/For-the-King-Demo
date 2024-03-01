using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IInteractableObject
{
    private DungeonMaster _GM;
    [SerializeField] private bool _inRadius;
    [SerializeField] private bool _holding;
    [SerializeField] private Vector3 _holdOffset;

    void Start()
    {
        _GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();
    }

    public void Interact()
    {
        Hold();
    }

    void Hold()
    {
        if (!_holding && _inRadius)
        {
            _holding = true;

            transform.SetParent(_GM.player.GetComponent<Character>()._holdPointMiddle);

            _GM.player.GetComponent<ICharStats>().ChangeEquipload(GetComponent<Rigidbody>().mass);

            transform.position = (_GM.player.GetComponent<Character>()._holdPointMiddle.position);

            transform.rotation = _GM.player.GetComponent<Character>()._holdPointMiddle.rotation;

            _GM.player.GetComponent<Character>().animator.SetTrigger("Throw");

            _GM.player.GetComponent<Character>().animator.SetFloat("Speed", 0);

            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 6)
        {
            if (collision.collider.transform.root.gameObject != transform.root.gameObject && !_holding)
            {
                collision.collider.transform.root.GetComponent<Character>().ChangeHealth((int)(GetComponent<Rigidbody>().mass + GetComponent<Rigidbody>().velocity.magnitude));

                Debug.Log(transform.root.name + " hits the " + collision.collider.transform.root.name + ", speed: " + GetComponent<Rigidbody>().velocity.magnitude);
            }
        }
    }
}
