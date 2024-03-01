using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractableObject, IHitable
{
    private DungeonMaster _GM;
    public SoundManager SM { get; set; }
    private GameObject _player;
    [SerializeField] private bool _opened;
    [SerializeField] private bool _inRadius;
    [SerializeField] private bool _opening;

    void Start()
    {
        _GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();

        SM = GetComponent<SoundManager>();
    }

    void Update()
    {
        //GetComponent<Animation>()["Open"].speed = 1 * -1;
    }

    public void Interact()
    {
        
        //if (_opened) GetComponent<Animator>().SetTrigger("Interacted");

        if (_opened) GetComponent<Animator>().SetFloat("Speed", 1);
        else GetComponent<Animator>().SetFloat("Speed", -1);
        
        _opened = !_opened;
    }

    public void GetHit(float amount, DamageType type, Transform part)
    {
        SM.PlaySound("GetHit");

        //GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity * 10f, ForceMode.Impulse); Need to finish it later
    }

    public void Open(AnimationEvent animationEvent)
    {

        
        Sound sound = Array.Find(SM.sounds, sound => sound.name == "Creak");

        if (sound.source.isPlaying) sound.source.Stop();
        
        SM.sounds[2].source.Stop();

        if (!_opened)
        {
            SM.PlaySound("Open");

            SM.PlaySound("Creak");
        }
        else
        {
            SM.PlaySound("Close");

            
        }

        
    }

    public void Close(AnimationEvent animationEvent)
    {

        Sound sound = Array.Find(SM.sounds, sound => sound.name == "Creak");

        if (sound.source.isPlaying) sound.source.Stop();
        
        SM.sounds[2].source.Stop();

        if (!_opened)
        {
            SM.PlaySound("Close");

            
        }
        else SM.PlaySound("Creak");
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.transform.root.gameObject.tag == "Player") _inRadius = true;
    }

    void OnTriggerStay(Collider trigger)
    {
        if (trigger.transform.root.gameObject.tag == "Player") _inRadius = true;
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.transform.root.gameObject.tag == "Player") _inRadius = false;
    }
}
