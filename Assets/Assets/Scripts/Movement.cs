using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour
{
    Camera mainCam;
    UnitCore unitCore;
    CharacterController player;
    Animator anim;
    CameraControl cam;

    [Header("Input")]
    public float horizontal;
    public float vertical;

    [Header("Values")]
    public LayerMask floorLayer;
    [SyncVar]
    public bool staminaCheck;
    public bool isMounted = false;
    public float mountSpeed = 12.5f;
    public float acceleratedSpeed;

    public TextMeshProUGUI debug;

    void Start()
    {
        //Get Components
        mainCam = Camera.main;
        cam = mainCam.GetComponent<CameraControl>();
        unitCore = GetComponent<UnitCore>();
        player = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        debug = GameObject.Find("DEBUG!").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!unitCore.isPlayerReviving)
        {
            Animate();
        }
    }

    void LateUpdate()
    {
        if (!UI_Chat.i.active && !unitCore.isPlayerReviving)
        {
            Move();

            if (!unitCore.lockRotation && !UI_State.isInUI)
            {
                FaceMouseDirection();
            }
        }
    }

    void Move()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            unitCore.CmdAlterMoving(true);
        }
        else
        {
            unitCore.CmdAlterMoving(false);
        }

        float speed = unitCore.speedMove;
        if (Input.GetKey(Binds.i.GetBind("Sprint").key) && !isMounted && (horizontal != 0 || vertical != 0))
        {
            if (staminaCheck)
            {
                unitCore.CmdChangeResource(Source.Type.Natural, Defs.ResourceTypes.Stamina, unitCore.sprintDrain * Time.deltaTime, true);
                acceleratedSpeed = Mathf.Clamp(acceleratedSpeed + (unitCore.acceleration * Time.deltaTime), 0, unitCore.speedSprint - unitCore.speedMove);
                speed = unitCore.speedMove + acceleratedSpeed;
                unitCore.CmdAlterSprinting(true);
            }
            else
            {
                acceleratedSpeed = 0;
                unitCore.CmdAlterSprinting(false);
            }
        }
        else
        {
            acceleratedSpeed = 0;
            unitCore.CmdAlterSprinting(false);
        }

        Vector3 veloctiy = Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical), 1);
        if (!isMounted)
        {
            veloctiy *= speed;
        }
        else
        {
            veloctiy *= mountSpeed;
        }
        player.SimpleMove(veloctiy);

        debug.text = "Speed: " + speed;
    }

    public void FaceMouseDirection()
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, floorLayer))
        {
            transform.LookAt(new Vector3(hit.point.x, 200.5f, hit.point.z));
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        */

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, floorLayer))
        {
            transform.LookAt(new Vector3(hit.point.x, 200.5f, hit.point.z));
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

    }

    void Animate()
    {
        anim.SetFloat("Horizontal", horizontal);
        anim.SetFloat("Vertical", vertical);
    }
}