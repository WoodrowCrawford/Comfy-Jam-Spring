using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidBody;

    [SerializeField] private bool canMove = true;

    [SerializeField] float moveSpeed = 10f;

    void OnEnable()
    {
        PromptMessageBehavior.OnPromptMessageClicked += DisableMovement;
        PromptMessageBehavior.OnPromptMessageClosed += EnableMovement;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard += DisableMovement;
        DialogueUIBehavior.OnDialogueBoxOpen += DisableMovement;
        DialogueUIBehavior.OnDialogueBoxClose += EnableMovement;
    }

    void OnDisable()
    {
        PromptMessageBehavior.OnPromptMessageClicked -= DisableMovement;
        PromptMessageBehavior.OnPromptMessageClosed -= EnableMovement;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard -= DisableMovement;
        
        DialogueUIBehavior.OnDialogueBoxOpen -= DisableMovement;
        DialogueUIBehavior.OnDialogueBoxClose -= EnableMovement;
    }



    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void Move()
    {
        if (canMove)
        {
            myRigidBody.linearVelocity = moveInput * moveSpeed;
        }
        else
        {
            myRigidBody.linearVelocity = Vector2.zero;
        }
    }

    void DisableMovement()
    {
        canMove = false;
    }

    void EnableMovement()
    {
        canMove = true;
    }
}
