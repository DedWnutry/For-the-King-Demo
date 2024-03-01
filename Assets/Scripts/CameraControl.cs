using UnityEngine;
using UnityEngine.UI;

public class CameraControl: MonoBehaviour
{
    protected DungeonMaster GM;
    [SerializeField] private float _interactionDistance = 10f;
    protected float XRot;
    protected float YRot;
    [SerializeField] protected float MaxAngle = 60f;
    private float _distanceToObject;

    void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();
    }

    public virtual void Update()
    {
        RaycastHit hit;

        Ray ray;

        if (GM.paused == true)
        {
            ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                _distanceToObject = Vector3.Distance(hit.transform.position, transform.position);

                if (hit.transform.tag == "Items" && _distanceToObject < _interactionDistance) GM.SetCursor(true);
                else if (hit.transform.tag != "Items" || _distanceToObject > _interactionDistance) GM.SetCursor(false);
            }
        }
        else
        {
            //Cursor.visible = false;

            ray = new Ray(transform.position, transform.forward * _interactionDistance);

            //Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.red);

            if (Physics.Raycast(ray, out hit))
            {
                _distanceToObject = Vector3.Distance(hit.transform.position, transform.position);

                if (_distanceToObject < _interactionDistance & hit.transform.GetComponent<IInteractableObject>() != null & hit.transform.gameObject.layer == 3 || hit.transform.gameObject.layer == 6)
                {
                    if (hit.transform.GetComponent<Item>() != null)
                    {
                        hit.transform.GetComponent<Item>().selected = true;

                        hit.transform.GetComponent<Item>().card.GetComponent<Icon>().ShowInfo();
                    }

                    if (Input.GetKeyDown(GM.interactKey)) hit.transform.GetComponent<IInteractableObject>().Interact();

                }
            }

            XRot -= Input.GetAxis("Mouse Y") * GM.mouseSensitivity * Time.deltaTime;

            XRot = Mathf.Clamp(XRot, -MaxAngle, MaxAngle);

            YRot -= Input.GetAxis("Mouse X") * GM.mouseSensitivity * Time.deltaTime;

            YRot = Mathf.Clamp(YRot, -MaxAngle, MaxAngle);

            transform.localRotation = Quaternion.Euler(XRot, -YRot, 0f);

            //transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse Y") * sensa * Time.deltaTime);
        }
    }

    public void SwapToInventoryCam(bool state)
    {
    
        if (state)
        {
            GetComponent<Animation>()["SwitchToInventoryCamAnim"].speed = 1;

            //GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

            //GetComponent<Camera>().cullingMask = 6;
        }
        else
        {
            GetComponent<Animation>()["SwitchToInventoryCamAnim"].speed = -1;
        }
        
        GetComponent<Animation>().Play();

    }
}
