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
        bool isMoving = canMove && moveInput != Vector2.zero;

        if (canMove)
        {
            myRigidBody.linearVelocity = moveInput * moveSpeed;
            playerAnimator.SetFloat("MoveX", moveInput.x);
            playerAnimator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                PlayMoveSound();
            }
            else
            {
                StopMoveSound();
            }

          
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
            StopMoveSound();
           
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

    public void PlayMoveSound()
    {
        //first check to see if this clip is already playing, and if it is, we don't want to play it again
        if (SoundManager.instance.IsSoundFXClipPlaying(SoundManager.instance.walkGrassClip))
        {
            return;
        }

        else
        {
            SoundManager.instance.PlaySoundFXClipAtSetVolume(SoundManager.instance.soundFXObject, SoundManager.instance.walkGrassClip, transform, true, 0f, 0f, 0.4f);
        }
    }

    void StopMoveSound()
    {
        if (SoundManager.instance.IsSoundFXClipPlaying(SoundManager.instance.walkGrassClip))
        {
            SoundManager.instance.StopSoundFXClip(SoundManager.instance.walkGrassClip);
        }
    }

    
}
