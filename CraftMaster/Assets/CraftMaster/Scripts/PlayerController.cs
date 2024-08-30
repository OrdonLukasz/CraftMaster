using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("PlayerController")] 
    [SerializeField] public Transform Camera;
    [Range(0.1f, 5)] public float croughSpeed = 1.0f;
    [SerializeField, Range(1, 10)] private float walkingSpeed = 3.0f;
    [SerializeField, Range(2, 20)] private float runingSpeed = 4.0f;
    [SerializeField, Range(0, 20)] private float jumpSpeed = 6.0f;
    [SerializeField, Range(0.5f, 10)] private float lookSpeed = 2.0f;
    [SerializeField, Range(10, 120)] private float lookXLimit = 80.0f;

    [Space(20)] [Header("Advance")] 
    [SerializeField] private float runningFOV = 65.0f;
    [SerializeField] private float speedToFOV = 4.0f;
    [SerializeField] private float croughHeight = 1.0f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float timeToRunning = 2.0f;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canRun = true;

    [Space(20)] [Header("HandsHide")] 
    [SerializeField] private bool canHideDistanceWall = true;
    [SerializeField, Range(0.1f, 5)] private float hideDistance = 1.5f;
    [SerializeField] private int layerMaskInt = 1;

    [Space(20)] [Header("Input")]
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool moving;
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float walkingValue;
    [SerializeField] private KeyCode croughKey = KeyCode.LeftControl;
    
    private bool isCrough = false;
    private float baseCroughHeight;
    private float rotationX = 0;
    private float baseFOV;
    private Camera camera;
    private float lookVertical;
    private float lookHorizontal;
    private float runningValue;
    private bool wallDistance;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        baseCroughHeight = characterController.height;
        baseFOV = camera.fieldOfView;
        runningValue = runingSpeed;
        walkingValue = walkingSpeed;
    }

    private void Update()
    {
        HandleMovement();
        CheckRotation();
    }

    private void HandleMovement()
    {
        RaycastHit CroughCheck;
        RaycastHit ObjectCheck;

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        isRunning = !isCrough ? canRun ? Input.GetKey(KeyCode.LeftShift) : false : false;
        vertical = canMove ? (isRunning ? runningValue : walkingValue) * Input.GetAxis("Vertical") : 0;
        horizontal = canMove ? (isRunning ? runningValue : walkingValue) * Input.GetAxis("Horizontal") : 0;
        if (isRunning) runningValue = Mathf.Lerp(runningValue, runingSpeed, timeToRunning * Time.deltaTime);
        else runningValue = walkingValue;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * vertical) + (right * horizontal);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        moving = horizontal < 0 || vertical < 0 || horizontal > 0 || vertical > 0 ? true : false;

        if (Input.GetKey(croughKey))
        {
            isCrough = true;
            float Height = Mathf.Lerp(characterController.height, croughHeight, 5 * Time.deltaTime);
            characterController.height = Height;
            walkingValue = Mathf.Lerp(walkingValue, croughSpeed, 6 * Time.deltaTime);
        }
        else if (!Physics.Raycast(GetComponentInChildren<Camera>().transform.position,
            transform.TransformDirection(Vector3.up), out CroughCheck, 0.8f, 1))
        {
            if (characterController.height != baseCroughHeight)
            {
                isCrough = false;
                float Height = Mathf.Lerp(characterController.height, baseCroughHeight, 6 * Time.deltaTime);
                characterController.height = Height;
                walkingValue = Mathf.Lerp(walkingValue, walkingSpeed, 4 * Time.deltaTime);
            }
        }

        if (wallDistance != Physics.Raycast(GetComponentInChildren<Camera>().transform.position,
                transform.TransformDirection(Vector3.forward), out ObjectCheck, hideDistance, layerMaskInt) &&
            canHideDistanceWall)
        {
            wallDistance = Physics.Raycast(GetComponentInChildren<Camera>().transform.position,
                transform.TransformDirection(Vector3.forward), out ObjectCheck, hideDistance, layerMaskInt);
        }
    }

    private void CheckRotation()
    {
        if (Cursor.lockState == CursorLockMode.Locked && canMove)
        {
            lookVertical = -Input.GetAxis("Mouse Y");
            lookHorizontal = Input.GetAxis("Mouse X");

            rotationX += lookVertical * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookHorizontal * lookSpeed, 0);

            if (isRunning && moving)
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, runningFOV, speedToFOV * Time.deltaTime);
            else camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, baseFOV, speedToFOV * Time.deltaTime);
        }
    }
}