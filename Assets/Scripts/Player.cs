using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{

    [SerializeField] private CharacterClass _class;
    public Color fogColor;
    public float fogDensity;
    public bool fogEnabled;
    public FogMode fogMode;

    public override void Start()
    {
        controller = GetComponent<CharacterController>();

        indicators.GetComponent<ActorUI>().strTxt.text = STR.ToString();

        indicators.GetComponent<ActorUI>().dexTxt.text = DEX.ToString();

        indicators.GetComponent<ActorUI>().intTxt.text = INT.ToString();

        maxHP = STR * 10;

        maxSP = DEX * 10;

        maxMP = INT * 10;

        curHP = maxHP;

        curSP = maxSP;

        curMP = maxMP;

        indicators.GetComponent<ActorUI>().nameTxt.text = transform.name;

        indicators.GetComponent<ActorUI>().classTxt.text = _class.ToString();

        indicators.GetComponent<ActorUI>().lvlTxt.text = lvl.ToString();

        indicators.GetComponent<ActorUI>().experienceBar.fillAmount = (float)exp / (float)nextLvlExp;

        indicators.GetComponent<ActorUI>().expTxt.text = exp.ToString() + " / " + nextLvlExp.ToString();

        indicators.GetComponent<ActorUI>().healthBar.maxValue = maxHP;

        indicators.GetComponent<ActorUI>().healthBar.value = curHP;

        indicators.GetComponent<ActorUI>().healthBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 100, maxHP); //Moves the slider to the left relative to the screen border

        indicators.GetComponent<ActorUI>().curHealthTxt.text = curHP + " / " + maxHP;

        indicators.GetComponent<ActorUI>().staminaBar.maxValue = maxSP;

        indicators.GetComponent<ActorUI>().staminaBar.value = curSP;

        indicators.GetComponent<ActorUI>().staminaBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 100, maxSP);

        indicators.GetComponent<ActorUI>().curStaminaTxt.text = curSP + " / " + maxSP;

        indicators.GetComponent<ActorUI>().manaBar.maxValue = maxMP;

        indicators.GetComponent<ActorUI>().manaBar.value = curMP;

        indicators.GetComponent<ActorUI>().manaBar.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 100, maxMP);

        indicators.GetComponent<ActorUI>().curManaTxt.text = curMP + " / " + maxMP;

        curOxygen = maxOxygen;

        indicators.GetComponent<ActorUI>().oxygenBar.maxValue = maxOxygen;

        indicators.GetComponent<ActorUI>().oxygenBar.value = maxOxygen;

        ChangeEquipload(0);

        base.Start();
    }

    public override void Update()
    {
        if (!GM.paused)
        {
            int attackType = 0;

            float staminaUsage = 0;

            if (Input.GetMouseButton(1))
            {
                if (Input.GetAxis("Mouse X") > 0) attackType = 1;

                if (Input.GetAxis("Mouse X") < 0) attackType = 2;

                if (Input.GetAxis("Mouse Y") < 0) attackType = 3;

                if (Input.GetAxis("Mouse Y") > 0) attackType = 4;
                
                animator.SetInteger("Speed", 0);
            }

            if (curSP >= staminaUsage && Time.time >= attackDelay && Input.GetMouseButtonUp(1)) //Provides a delay between hits
            {
                if (Input.GetAxis("Mouse X") > 0) attackType = 1;

                if (Input.GetAxis("Mouse X") < 0) attackType = 2;

                if (Input.GetAxis("Mouse Y") < 0) attackType = 3;

                if (Input.GetAxis("Mouse Y") > 0) attackType = 4;

                animator.SetInteger("Speed", 1);

                attackDelay = Time.time + 1f / attackSpeed;

                if (parts.Find(x => x.type == "Weapon Right").equipment != null)
                {
                    staminaUsage = parts.Find(x => x.type == "Weapon Right").equipment.GetComponentInChildren<Item>().stats.weight * 10;

                    ChangeStamina(-staminaUsage);
                }
                else
                {
                    staminaUsage = 10;

                    ChangeStamina(-staminaUsage);
                }
            }

            if (Input.GetAxis("Mouse Y") == 0 && Input.GetAxis("Mouse X") == 0 && Input.GetMouseButton(1) && block == false)
            {
                animator.SetBool("BlockRight", true);

                block = true;
            }

            Attack(attackType);

            if (block && Input.GetMouseButtonUp(1))
            {
                animator.SetBool("BlockRight", false);

                block = false;

                DisableHitbox();
            }

            if (Input.GetKeyUp(GM.drawWeaponKey)) DrawWeapon();
            
            
        }

        base.Update();
    }

    public override int ChangeEquipload(float value)
    {
        //carryWeight = strength * 10;

        totalWeight += value;

        if ((totalWeight / carryWeight) < 0.33)
        {
            loadStage = 1;

            indicators.GetComponent<ActorUI>().equipLoadTxt.text = totalWeight + " / " + carryWeight + " (light)";

            indicators.GetComponent<ActorUI>().equipLoadSlider.fillRect.GetComponent<Image>().color = indicators.GetComponent<ActorUI>().loadGrad.Evaluate(0f);
        }

        if ((totalWeight / carryWeight) >= 0.33)
        {
            loadStage = 2;

            indicators.GetComponent<ActorUI>().equipLoadTxt.text = totalWeight + " / " + carryWeight + " (medium)";

            indicators.GetComponent<ActorUI>().equipLoadSlider.fillRect.GetComponent<Image>().color = indicators.GetComponent<ActorUI>().loadGrad.Evaluate(0.33f);
        }

        if ((totalWeight / carryWeight) >= 0.66)
        {
            loadStage = 3;

            indicators.GetComponent<ActorUI>().equipLoadTxt.text = totalWeight + " / " + carryWeight + " (heavy)";

            indicators.GetComponent<ActorUI>().equipLoadSlider.fillRect.GetComponent<Image>().color = indicators.GetComponent<ActorUI>().loadGrad.Evaluate(0.66f);
        }

        if ((totalWeight / carryWeight) >= 1)
        {
            loadStage = 4;

            indicators.GetComponent<ActorUI>().equipLoadTxt.text = totalWeight + " / " + carryWeight + " (overencumbered)";

            indicators.GetComponent<ActorUI>().equipLoadSlider.fillRect.GetComponent<Image>().color = indicators.GetComponent<ActorUI>().loadGrad.Evaluate(1f);
        }

        indicators.GetComponent<ActorUI>().equipLoadSlider.maxValue = carryWeight;

        indicators.GetComponent<ActorUI>().equipLoadSlider.value = totalWeight;

        return loadStage;
    }

    public void ChangeExperience(int expGain)
    {
        exp += expGain;

        indicators.GetComponent<ActorUI>().experienceBar.fillAmount = (float)exp / (float)nextLvlExp;

        if (exp >= nextLvlExp) LevelUp();
    }

    public void LevelUp()
    {
        exp = 0;

        nextLvlExp += nextLvlExp * lvl;

        lvl++;
    }
    public override void Movement()
    {
        if (grounded & !GM.paused)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (Input.GetKey(GM.sprintKey) && Input.GetKey(GM.moveForward) && curSP >= sprintStamina)
                {       
                    controller.Move(transform.forward * Input.GetAxis("Vertical") * (sprintSpeed / loadStage) * Time.deltaTime + transform.right * Input.GetAxis("Horizontal") * (sprintSpeed / loadStage) * Time.deltaTime);

                    animator.SetInteger("Movement", 4);

                    ChangeStamina(-sprintSpeed * loadStage * Time.deltaTime);
                }
                else if (Input.GetKey(GM.walkKey))
                {
                    controller.Move(transform.forward * Input.GetAxis("Vertical") * (walkSpeed / loadStage) * Time.deltaTime + transform.right * Input.GetAxis("Horizontal") * (walkSpeed / loadStage) * Time.deltaTime);

                    animator.SetInteger("Movement", 2);
                }
                else if (Input.GetKey(GM.sneakKey))
                {
                    controller.Move(transform.forward * Input.GetAxis("Vertical") * (crouchSpeed / loadStage) * Time.deltaTime + transform.right * Input.GetAxis("Horizontal") * (crouchSpeed / loadStage) * Time.deltaTime);

                    animator.SetInteger("Movement", 1);
                }
                else
                {
                    controller.Move(transform.forward * Input.GetAxis("Vertical") * (runSpeed / loadStage) * Time.deltaTime + transform.right * Input.GetAxis("Horizontal") * (runSpeed / loadStage) * Time.deltaTime);

                    animator.SetInteger("Movement", 3);
                }

                var camRotation = GM.mainCam.transform.rotation;

                camRotation.x = 0;

                camRotation.z = 0;

                transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, rotationSpeed * Time.deltaTime);

                //transform.Rotate(0, GM.mainCam.transform.rotation.eulerAngles.y * Time.deltaTime, 0, Space.World);

                //transform.Rotate(transform.rotation.x, Quaternion.Slerp(transform.rotation, GM.mainCam.transform.rotation, rotationSpeed * Time.deltaTime), transform.rotation.z, Space.Self);
            }
            else animator.SetInteger("Movement", 0);
        }
    }


    public override void Attack(int attackType)
    {
        base.Attack(attackType);
    }

    public override void Footsteps(AnimationEvent animationEvent)
    {
        if (grounded)
        {
            Collider[] surface;

            surface = Physics.OverlapSphere((_groundCheckRight.transform.position +_groundCheckLeft.transform.position) / 2, groundCheckDistance, GM.environmentMask);

            if (surface != null)
            {
                switch (surface[0].tag)
                {
                    case "Stone":
                        {
                            GM.SM.PlaySound("FootstepsStone");
                        }
                        break;

                    case "Wood":
                        {
                            GM.SM.PlaySound("FootstepsWood");
                        }
                        break;
                }
            }

            Noise((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, 50 * animationEvent.intParameter * loadStage);
        }

        base.Footsteps(animationEvent);
    }

    public override void JumpsAndStrafes()
    {
        controller.Move(velocity * Time.deltaTime);

        if (grounded & Input.GetKeyDown(GM.jumpKey) & curSP >= jumpStamina + (jumpStamina * (loadStage / 4)))
        {
            if (curSP >= dodgeStamina + (dodgeStamina * (loadStage / 4)) & Time.time >= dodgeDelay)
            {
                int strafeType = 0;

                if (Input.GetKey(GM.moveLeft))
                {
                    animator.SetTrigger("Dodge");

                    animator.SetTrigger("Left");

                    StartCoroutine(Strafe(-1));

                    Noise((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, 100 * loadStage);
                }

                else if (Input.GetKey(GM.moveRight))
                {
                    animator.SetTrigger("Dodge");

                    animator.SetTrigger("Right");

                    StartCoroutine(Strafe(1));

                    Noise((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, 100 * loadStage);
                }

                else if (Input.GetKey(GM.moveBackward))
                {
                    animator.SetTrigger("Dodge");

                    animator.SetTrigger("Back");

                    StartCoroutine(Strafe(0));

                    Noise((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, 100 * loadStage);
                }

                else if (Input.GetKey(GM.moveForward))
                {
                    velocity = transform.forward * jumpDistance + new Vector3(0, jumpHeight / 2, 0);

                    if (animator.GetInteger("Movement") == 4) velocity = transform.forward * jumpDistance + new Vector3(0, jumpHeight / 2, 0);

                    animator.SetTrigger("Jump");

                    animator.SetTrigger("Ahead");

                    dodgeDelay = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;

                    ChangeStamina(-jumpStamina * loadStage);
                }

                else
                {
                    animator.SetTrigger("Jump");

                    animator.SetTrigger("Up");

                    dodgeDelay = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;

                    velocity.y = Mathf.Sqrt(jumpHeight * 1f * GM.gravity);

                    ChangeStamina(-jumpStamina * loadStage);
                }
            }
        }
    }

    IEnumerator Strafe(int strafeType)
    {
        dodgeDelay = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;

        ChangeStamina(-dodgeStamina * loadStage);

        float startTime = Time.time;

        while (Time.time < startTime + animator.GetCurrentAnimatorStateInfo(0).length)
        {
            switch (strafeType)
            {
                case -1:
                    controller.Move((transform.right * -dodgeDistance) * 10 * Time.deltaTime);
                    break;

                case 0:
                    controller.Move((transform.forward * -dodgeDistance) * 10 * Time.deltaTime);
                    break;

                case 1:
                    controller.Move((transform.right * dodgeDistance) * 10 * Time.deltaTime);
                    break;
            }

            yield return null;
        }
    }

    public void FallDamage()
    {
        float fallDistance = startFallPosition - transform.position.y;

        float healthDamage = 0;

        //PlaySound("Land");

        Noise((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, -velocity.y * loadStage * 2);

        if (fallDistance > minFallHeight)
        {
            healthDamage = (int)Mathf.Clamp(maxHP * (fallDistance / maxFallHeight) - DEX, 0, maxHP);

            //dexGain += (int)healthDamage;

            if (fallDistance >= maxFallHeight) healthDamage = maxHP;

            ChangeHealth((int)(healthDamage));

            Debug.Log(transform.name + "  fell from " + fallDistance + " units and took " + healthDamage + " damage");
        }

        falling = false;

        velocity = Vector3.zero;

        Debug.Log("I landed");
    }

    public void Noise(Vector3 pos, float volume)
    {
        hitColliders = Physics.OverlapSphere(pos, volume / 10, GM.actorsMask);

        foreach (Collider coll in hitColliders) if (coll.transform.root.GetComponent<Character>() != null && coll.transform.root.GetComponent<Character>().dead == false && coll.transform == coll.transform.root.GetComponent<Character>().viewPoint) //Need to change later to make sound wave contact with head body part of actor
        {
            coll.transform.root.GetComponent<NPC>().Alarm(volume / Vector3.Distance(coll.transform.position, pos));
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 4)
        {
            if (diving && !swimming)
            {
                swimming = true;

                diving = false;

                StopCoroutine(Suffocate());

                if (curOxygen < maxOxygen) curOxygen = maxOxygen;

                suffocate = false;

                indicators.GetComponent<ActorUI>().oxygenBar.gameObject.SetActive(false);

                fogEnabled = false;

                RenderSettings.fog = fogEnabled;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        /*
        if (col.gameObject.layer == 4)
        {
            if (parts[5].transform.position.y <= col.transform.position.y) _swimming = true;
            if (parts[5].transform.position.y > col.transform.position.y) _swimming = false;

            if (_swimming && parts[0].transform.position.y < col.transform.position.y) _diving = true;
            if (_swimming && parts[0].transform.position.y >= col.transform.position.y) _diving = false;
        }
        */
        if (col.gameObject.layer == 4)
        {
            if (diving)
            {
                fogEnabled = true;

                RenderSettings.fogColor = fogColor;

                RenderSettings.fogDensity = fogDensity;

                RenderSettings.fog = fogEnabled;

                RenderSettings.fogMode = fogMode;
            }

            if (viewPoint.transform.position.y <= col.transform.position.y) swimming = true;
            else swimming = false;
        }
        //if (animator.GetInteger("Movement") == 5) agent.speed = swimSpeed;
    }

    public override void Death()
    {
        GM.deathScreen.gameObject.SetActive(true);

        GM.deathScreen.GetComponent<Animation>().Play("DeathScreen");

        GM.Invoke("GameOver", GM.deathScreen.GetComponent<Animation>().clip.length);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(_groundCheckRight.transform.position, groundCheckDistance);

        Gizmos.DrawWireSphere(_groundCheckLeft.transform.position, groundCheckDistance);

        Gizmos.color = Color.yellow;

        float MS = 0;

        switch (animator.GetInteger("Movement"))
        {
            case 4:
                {
                    MS = sprintSpeed;
                }
                break;


            case 3:
                {
                    MS = runSpeed;
                }
                break;

            case 2:
                {
                    MS = walkSpeed;
                }
                break;

            case 1:
                {
                    MS = crouchSpeed;
                }
                break;

        }
        Gizmos.DrawWireSphere((_groundCheckRight.transform.position + _groundCheckLeft.transform.position) / 2, MS * loadStage);
    }
}
