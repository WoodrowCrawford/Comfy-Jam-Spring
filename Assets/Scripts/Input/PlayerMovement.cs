using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    SpriteRenderer spriteRenderer;

    [SerializeField] private Animator playerAnimator;

    [SerializeField] private bool canMove = true;

    [SerializeField] float moveSpeed = 10f;

    void OnEnable()
    {
        PromptMessageBehavior.OnPromptMessageClicked += DisableMovement;
        PromptMessageBehavior.OnPromptMessageClosed += EnableMovement;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard += DisableMovement;
        DialogueUIBehavior.OnDialogueBoxOpen += DisableMovement;
        DialogueUIBehavior.OnDialogueBoxClose += EnableMovement;

        ItemPickup.OnItemPickupAnimation += SetPickUpAnimation;
    }

    void OnDisable()
    {
        PromptMessageBehavior.OnPromptMessageClicked -= DisableMovement;
        PromptMessageBehavior.OnPromptMessageClosed -= EnableMovement;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard -= DisableMovement;
        
        DialogueUIBehavior.OnDialogueBoxOpen -= DisableMovement;
        DialogueUIBehavior.OnDialogueBoxClose -= EnableMovement;

        ItemPickup.OnItemPickupAnimation -= SetPickUpAnimation;
    }



    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            playerAnimator.SetFloat("MoveX", moveInput.x);
            playerAnimator.SetBool("IsMoving", moveInput != Vector2.zero);

          
            if (moveInput.x < 0f)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveInput.x > 0f)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            myRigidBody.linearVelocity = Vector2.zero;
            playerAnimator.SetBool("IsMoving", false);
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

    public IEnumerator PlayPickUpAnimation()
    {
        playerAnimator.SetTrigger("Pickup");
        Debug.Log("Playing pickup animation");
        yield return new WaitForSeconds(0.5f);

        playerAnimator.ResetTrigger("Pickup");

        yield break;
    }

    public void SetPickUpAnimation()
    {
        StartCoroutine(PlayPickUpAnimation());
    }
}
