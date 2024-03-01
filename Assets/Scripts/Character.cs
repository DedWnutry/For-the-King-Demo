using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Animations.Rigging;

public class Character : MonoBehaviour, ICharStats, IHitable
{
    [Header("General:")]
    public string charName;
    protected DungeonMaster GM;
    protected CharacterController controller;
    public Animator animator;
    public SoundManager SM { get; set; }
    protected bool inCombat;
    public List<BodyPart> parts = new List<BodyPart>();
    
    public MultiRotationConstraint headConstraint;
    public Transform inventory;
    public Transform viewPoint;
    public Transform _holdPointRight;
    public Transform _holdPointLeft;
    public Transform _holdPointMiddle;
    public Transform _sheathRight;
    public Transform _sheathLeft;
    public Transform _sheathBack;
    public Transform _groundCheckRight;
    public Transform _groundCheckLeft;

    [Header("Actions:")]
    public bool falling;
    public bool grounded;
    protected bool swimming;
    protected bool diving;
    protected bool suffocate;
    protected Vector3 velocity;
    [SerializeField] protected float jumpHeight = 10f;
    protected float jumpDistance = 10f;
    [SerializeField] protected float groundCheckDistance = 0.5f;
    protected float startFallPosition;
    public int maxFallHeight;
    public int minFallHeight;
    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float sprintSpeed = 5f;
    public float crouchSpeed = 0.5f;
    public float strafeSpeed = 5f;
    public float swimSpeed = 0.5f;
    public float rotationSpeed = 5f;
    public int jumpStamina;
    public float sprintStamina;
    public int dodgeStamina;
    protected float dodgeDelay;
    public float dodgeDistance = 5f;
    [HideInInspector] public bool block;
    private float _throwPower;
    [SerializeField] private float _maxThrowPower = 30f;
    public float attackSpeed = 1f;
    public float attackDelay;
    private bool _draw;
    protected Collider[] hitColliders;

    [Header("Stats:")]
    public bool dead;
    [field: SerializeField] public int lvl { get; set; }
    [field: SerializeField] public int exp { get; set; }
    [field: SerializeField] public int nextLvlExp { get; set; }
    [field: SerializeField] public GameObject indicators { get; set; }
    [field: SerializeField] public int STR { get; set; }
    [field: SerializeField] public int DEX { get; set; }
    [field: SerializeField] public int INT { get; set; }
    [field: SerializeField] public float maxHP { get; set; }
    [field: SerializeField] public float maxSP { get; set; }
    [field: SerializeField] public float maxMP { get; set; }
    [field: SerializeField] public float curHP { get; set; }
    [field: SerializeField] public float curSP { get; set; }
    [field: SerializeField] public float curMP { get; set; }
    [field: SerializeField] public float healthRegen { get; set; }
    [field: SerializeField] public float staminaRegen { get; set; }
    [field: SerializeField] public float manaRegen { get; set; }
    public float healthRegenDelay;
    public float staminaRegenDelay;
    protected float staminaCurrDelay;
    public float manaRegenDelay;
    public float totalWeight;
    public float carryWeight;
    [field: SerializeField] public int loadStage { get; set; }
    [field: SerializeField] public float maxOxygen { get; set; }
    public float curOxygen { get; set; }

    void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();

        inventory = GM.inventoryMenu.transform;

        animator = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        SM = GetComponent<SoundManager>();

        curHP = maxHP;

        curSP = maxSP;

        curMP = maxMP;

    }

    public virtual void Update()
    {
        grounded = Physics.CheckSphere(_groundCheckRight.position, groundCheckDistance, GM.environmentMask) | Physics.CheckSphere(_groundCheckLeft.transform.position, groundCheckDistance, GM.environmentMask);

        velocity.y += -GM.gravity * Time.deltaTime; //Acceleration of gravity

        if (velocity.y < -3)
        {
            if (grounded)
            {
                if (falling)
                {
                    FallDamage();
                }
                else velocity.y = -1; //Resets free fall acceleration after hitting the ground/other objects
            }
            else
            {
                if (!falling)
                {
                    falling = true;

                    startFallPosition = transform.position.y;
                }
            }
        }

        //RegenHealth();

        //RegenStamina();

        //RegenMana();

        Movement();

        //Attack(0);

        Swimming();

        JumpsAndStrafes();

        Kick();

        Throw();
    }

    public virtual void ChangeHealth(float value)
    {

        indicators.GetComponent<ActorUI>().healthBar.maxValue = maxHP;

        indicators.GetComponent<ActorUI>().healthBar.value = curHP;

        if (curHP < maxHP && Time.time >= healthRegenDelay) RegenHealth();

        if (curHP <= 0)
        {
            curHP = 0;

            Death();
        }

        if (value != 0)
        {
            curHP += value;

            indicators.gameObject.SetActive(true);

            indicators.GetComponent<ActorUI>().curHealthTxt.GetComponent<Text>().text = curHP + " / " + maxHP;

            if (value < 0) healthRegenDelay = Time.time + 5f;
        }
    }

    public void RegenHealth()
    {
        if (curHP < maxHP && Time.time >= healthRegenDelay) ChangeHealth(healthRegen);
    }

    public void ChangeStamina(float value)
    {
        if (value != 0)
        {
            curSP += value;

            staminaCurrDelay = Time.time + staminaRegenDelay;

            if (value < 0) RegenStamina();

            //dexGain += (int)staminaUsage;
        }

        curSP = Mathf.Clamp(curSP, 0, maxSP);

        indicators.GetComponent<ActorUI>().staminaBar.value = curSP;

        indicators.GetComponent<ActorUI>().curStaminaTxt.text = Mathf.Round(curSP) + " / " + maxSP;
    }

    public void RegenStamina()
    {
        if (staminaCurrDelay <= Time.time && curSP < maxSP)
        {
            ChangeStamina(((staminaRegen * 10) / loadStage) * Time.deltaTime);

            StartCoroutine(StaminaCoroutine());
        }
        else
        {
            if (indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax.x <= indicators.GetComponent<ActorUI>().staminaBar.fillRect.anchorMax.x)
            {
                indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2(indicators.GetComponent<ActorUI>().staminaBar.fillRect.anchorMax.x, indicators.GetComponent<ActorUI>().staminaBar.fillRect.anchorMax.y);

            }

            StopCoroutine(StaminaCoroutine());
        }
    }

    IEnumerator StaminaCoroutine()
    {
        while (indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax.x > indicators.GetComponent<ActorUI>().staminaBar.fillRect.anchorMax.x)
        {
            indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2(indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax.x - .1f, indicators.GetComponent<ActorUI>().staminaBar.transform.GetChild(1).GetComponent<RectTransform>().anchorMax.y);


            yield return new WaitForSeconds(staminaRegenDelay);
        }
    }

    public void ChangeMana(float value)
    {
        if (value < 0) curMP += value;
        else curMP += value;

        indicators.GetComponent<ActorUI>().manaBar.value = curMP;

        indicators.GetComponent<ActorUI>().curManaTxt.text = curMP + " / " + maxMP;

        indicators.GetComponent<ActorUI>().manaBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 100, maxMP);

        if (curMP < 0) curMP = 0;
    }

    public void RegenMana()
    {
        if (curMP < maxMP && Time.time >= manaRegenDelay) ChangeMana(manaRegen);
    }

    public void ChangeExperience(int value)
    {

    }

    public void ChangePoise(float value)
    {

    }

    public virtual int ChangeEquipload(float value)
    {
        return 0;
    }

    public void ThrowPower(float throwPower, float maxThrowPower)
    {
        if (throwPower > 0) indicators.GetComponent<ActorUI>().throwBar.gameObject.SetActive(true);

        else indicators.GetComponent<ActorUI>().throwBar.gameObject.SetActive(false);

        indicators.GetComponent<ActorUI>().throwBar.maxValue = maxThrowPower;

        indicators.GetComponent<ActorUI>().throwBar.value = throwPower;
    }
    
    public void HitBodyPart(int part)
    {
        //if (curHP <= 0 && !_isPlayerChar) parts[part].GetComponent<BodyPart>().Dismember();
    }
    
    public virtual void Movement()
    {
        
    }

    public void Swimming()
    {
        //if (parts[5].transform.position.y <= _waterLine) _swimming = true;

        //if (parts[5].transform.position.y > _waterLine) _swimming = false;

        //if (_swimming && _cam.transform.position.y < _waterLine) _diving = true;

        if (swimming)
        {
            //if (_diving && _cam.transform.position.y >= _waterLine) _diving = false;

            if (falling && !grounded ) swimming = true;

            if (!grounded && !diving && Input.GetKeyDown(GM.jumpKey)) diving = true;

            falling = false;

            grounded = false;

            //velocity.y -= master.gravity * Time.deltaTime;

            velocity.y = 0;

            if (Input.GetKey(GM.moveForward))
            {
                animator.SetInteger("Movement", 6);

                controller.Move(transform.forward * Input.GetAxis("Vertical") * (swimSpeed / loadStage) * Time.deltaTime);
            }

            if (Input.GetKey(GM.moveBackward))
            {
                animator.SetInteger("Movement", 6);

                controller.Move(-transform.forward * -Input.GetAxis("Vertical") * (swimSpeed / loadStage) * Time.deltaTime);

            }
            else animator.SetInteger("Movement", 0);    
        }

        if (diving)
        {
            velocity.y = -1f;

            falling = false;

            grounded = false;

            indicators.GetComponent<ActorUI>().oxygenBar.gameObject.SetActive(true);

            curOxygen -= 10 * Time.deltaTime;

            indicators.GetComponent<ActorUI>().oxygenBar.value = curOxygen;

            if (Input.GetKey(GM.moveForward))
            {
                animator.SetInteger("Movement", 5);

                controller.Move(GM.mainCam.transform.forward * Input.GetAxis("Vertical") * (swimSpeed / loadStage) * Time.deltaTime);
            }

            if (Input.GetKey(GM.moveBackward))
            {
                animator.SetInteger("Movement", 5);

                controller.Move(-GM.mainCam.transform.forward * -Input.GetAxis("Vertical") * (swimSpeed / loadStage) * Time.deltaTime);

            }
            else animator.SetInteger("Movement", 0);

            if (!suffocate && curOxygen <= 0) StartCoroutine(Suffocate());
        }
    }

    public IEnumerator Suffocate()
    {
        suffocate = true;

        while (diving)
        {
            yield return new WaitForSeconds(1);
            
            ChangeHealth(maxHP / 60);
        }       
    }

    public virtual void JumpsAndStrafes()
    {

    }

    void FallDamage()
    {

    }

    public virtual void Attack(int attackType)
    {
        animator.SetTrigger("Attack");

        switch (attackType)
        {
            case 1:

                animator.SetInteger("AttackType", 1);

                break;

            case 2:

                animator.SetInteger("AttackType", 2);

                break;

            case 3:

                animator.SetInteger("AttackType", 3);

                break;

            case 4:

                animator.SetInteger("AttackType", 4);

                break;

            case 0:

                break;
        }

        animator.SetTrigger("Stop");
    }

    public void EnableHitbox(AnimationEvent animationEvent)
    {
        if (parts.Find(x => x.type == "Weapon Right").equipment != null)
        {
            foreach (Collider coll in parts.Find(x => x.type == "Weapon Right").equipment.transform.GetComponentsInChildren<Collider>()) coll.enabled = true;

            //animator.SetFloat("Speed", 0);

            //dmg = animationEvent.intParameter;

            //weapon.GetComponent<Item>().attackType = animationEvent.stringParameter;

            //weapon.GetComponent<Collider>().enabled = true;
        } 
    }

    public void DisableHitbox()
    {        
        if (parts.Find(x => x.type == "Weapon Right").equipment != null)
        {
            parts.Find(x => x.type == "Weapon Right").equipment.transform.GetComponentInChildren<Item>().hittedTargets.Clear();

            foreach (Collider coll in parts.Find(x => x.type == "Weapon Right").equipment.transform.GetComponentsInChildren<Collider>()) coll.enabled = false;

            //weapon.GetComponent<Item>().hittedTarget = null;

            //weapon.GetComponent<Collider>().enabled = false;

            //dmg = 0;
        }



        //animator.SetTrigger("Reset");

        //animator.SetFloat("Speed", 1);

        //animator.SetTrigger("Stop");
    }

    public virtual void Footsteps(AnimationEvent animationEvent)
    {
        SM.PlaySound("Footsteps");
    }

    public void Knockback()
    {
        DisableHitbox();

        animator.SetFloat("Speed", -1f);

    }


    public void Throw()
    {
        if (_holdPointMiddle.childCount > 0)
        {
            Transform projectile = _holdPointMiddle.GetChild(0);

            if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
            {
                _throwPower += 100f * Time.deltaTime;

                ThrowPower(_throwPower, _maxThrowPower);
            }

            float staminaUsage = (projectile.GetComponentInChildren<Rigidbody>().mass * _throwPower) / STR;

            if (curSP < staminaUsage)
            {
                _throwPower = 0;

                Debug.Log("Throwing power: " + _throwPower);

                projectile.GetComponentInChildren<Rigidbody>().isKinematic = false;

                ThrowPower(_throwPower, _maxThrowPower);

                projectile.transform.GetChild(0).transform.parent = null;

                //weaponR.GetChild(0).transform.parent = null; 
            }

            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0) || _throwPower >= _maxThrowPower)
            {
                Debug.Log("Throwing power: " + _throwPower);

                animator.SetFloat("Speed", 1);

                ChangeStamina(-staminaUsage);

                projectile.GetComponentInChildren<Rigidbody>().isKinematic = false;

                //weaponR.GetComponentInChildren<Rigidbody>().isKinematic = false;

                projectile.GetComponentInChildren<Rigidbody>().AddForce(GM.mainCam.transform.forward * _throwPower, ForceMode.Impulse);

                //weaponR.GetComponentInChildren<Rigidbody>().AddForce(cam.transform.forward * throwPower, ForceMode.Impulse);

                _throwPower = 0;

                ChangeEquipload(-projectile.GetComponent<Rigidbody>().mass);

                ThrowPower(_throwPower, _maxThrowPower);

                //projectile.GetComponent<Item>().hold = false;

                projectile.transform.parent = null;

                //weaponR.GetChild(0).transform.parent = null;   
            }
        }
    }

    public void Kick()
    {
        if (Input.GetKeyDown(GM.kickKey))
        {
            animator.Play("Kick");

            ChangeStamina(-10);
        }
    }

    public void DrawWeapon()
    {
        if (parts.Find(x => x.type == "Weapon Right").equipment != null)
        {
            parts.Find(x => x.type == "Weapon Right").equipment.transform.position = _holdPointRight.transform.position;

            parts.Find(x => x.type == "Weapon Right").equipment.transform.rotation = _holdPointRight.transform.rotation;

            parts.Find(x => x.type == "Weapon Right").equipment.GetComponent<Collider>().enabled = false;

            animator.SetTrigger("Draw");

            animator.SetInteger("Hand", 1);

            parts.Find(x => x.type == "Weapon Right").equipment.GetComponent<Rigidbody>().isKinematic = true;

            parts.Find(x => x.type == "Weapon Right").equipment.GetComponent<Rigidbody>().useGravity = false;
        }

        if (parts.Find(x => x.type == "Weapon Left").equipment != null)
        {

        }
    }

    void LaterUpdate()
    {
        //transform.LookAt(cam.position + cam.forward); //Billboard option, needs to set Canvas.Render Mode = World Space
    }

    void OnTriggerExit(Collider col)
    {
        //if (col.gameObject.layer == 4 && parts[0].transform.position.y < col.transform.position.y) _diving = true;
        //if (col.gameObject.layer == 4 && parts[0].transform.position.y >= col.transform.position.y) _diving = false;

        if (col.gameObject.layer == 4)
        {
            swimming = false;
        }
    }

    public virtual void GetHit(float amount, DamageType type, Transform part)
    {
        ChangeHealth(amount);

        SM.PlaySound("GetHit");
    }

    public virtual void Death()
    {

    }
}

public enum CharacterClass { Warrior, Rogue, Mage }

[System.Serializable]
public class BodyPart
{
    public string type;
    public List<Transform> transformsLinks = new List<Transform>();
    public Transform equipment;
}