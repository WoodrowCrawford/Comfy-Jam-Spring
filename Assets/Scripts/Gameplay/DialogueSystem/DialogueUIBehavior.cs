using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class DialogueUIBehavior : MonoBehaviour
{
    public static DialogueUIBehavior instance;
    private ResponseHandlerBehavior _responseHandler;
    private TypewritterEffectBehavior _typewritterEffect;

    public delegate void DialogueBoxEvent();
    public static event DialogueBoxEvent OnDialogueBoxOpen;
    public static event DialogueBoxEvent OnDialogueBoxClose;


    [Header("Dialogue Box Settings")]
    [Tooltip("The dialogue box game object that will be enabled and disabled when showing and closing the dialogue box")]
    [SerializeField] private GameObject _dialogueBox;

    [Tooltip("The background image of the dialogue box")]
    [SerializeField] private Image _dialogueBackground;

   

    [Header("Dialogue Text Settings")]
    [Tooltip("The text label that will show the dialogue text")]
    [SerializeField] private TMP_Text _textLabel;
    [SerializeField] private Image _speakerImageA;
    [SerializeField] private Image _speakerImageB;



    [Header("Response Box Settings")]
    [Tooltip("The background image of the response box")]
    [SerializeField] private Image _responseBoxBG;



    [Header("Response Text Settings")]
    [Tooltip("The text label that will show the response text")]
    [SerializeField] private TMP_Text _responseText;



    //A bool to check if the dialogue box is open
    public static bool IsOpen { get; private set; }



    public DialogueObjectBehavior testDialogueObject;

    private void Awake()
    {
       
        //Gets the components on awake
        _typewritterEffect = GetComponent<TypewritterEffectBehavior>();
        _responseHandler = GetComponent<ResponseHandlerBehavior>(); 

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }



    //A function that shows the dialogue box
    public void ShowDialogue(DialogueObjectBehavior dialogueObject)
    {
        if (dialogueObject == null)
        {
            CloseDialogueBox();
            return;
        }

        DialogueResponseEvents matchingResponseEvents = GetResponseEvents(dialogueObject);

        if (matchingResponseEvents != null)
        {
            AddResponseEvents(matchingResponseEvents.Events);
        }
        else
        {
            AddResponseEvents(null);
        }

        
        //Stops time and shows the Cursor while the dialogue is open
        Cursor.visible = true;
        Time.timeScale = 0.0f;

        //Sets is open to be true
        IsOpen = true;

        //Enables the dialogue box game object
        _dialogueBox.SetActive(true);

        //Invokes the OnDialogueBoxOpen event
        OnDialogueBoxOpen?.Invoke();

        //Starts the step through dialogue coroutine
        StartCoroutine(StepThroughDialogue(dialogueObject));

        
    }



    

    //A function that adds response events
    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        _responseHandler.AddResponseEvents(responseEvents);
    }

    private DialogueResponseEvents GetResponseEvents(DialogueObjectBehavior dialogueObject)
    {
        DialogueResponseEvents[] responseEventComponents = FindObjectsByType<DialogueResponseEvents>(FindObjectsInactive.Include);

        foreach (DialogueResponseEvents responseEventComponent in responseEventComponents)
        {
            if (responseEventComponent.DialogueObject == dialogueObject)
            {
                return responseEventComponent;
            }
        }

        return null;
    }

    private IEnumerator StepThroughDialogue(DialogueObjectBehavior dialogueObject)
    {
         

        //gets the length of the dialogue in dialogueObject
        for(int i =  0; i < dialogueObject.Dialogue.Length; i++)
        {
           
            DialogueLine dialogue = dialogueObject.Dialogue[i];

            // Build a runtime display string so we don't overwrite the ScriptableObject text.
            string displayText = dialogue.text;

            //Replaces the {playerName} tag with the player's name
            PlayerBehavior playerBehavior = FindAnyObjectByType<PlayerBehavior>();
            if (playerBehavior != null)
            {
                displayText = displayText.Replace("{playerName}", playerBehavior.PlayerName);
            }

            //replaces the {currentDay} tag with the current day
            string currentDayText = DayCycleManager.CurrentDay.ToString();
            displayText = displayText.Replace("{CurrentDay}", currentDayText);
            displayText = displayText.Replace("{currentDay}", currentDayText);

            //set the speaker images based on the dialogue line's speaker sprites
            if (dialogue.speakerSpriteA != null)
            {
                _speakerImageA.sprite = dialogue.speakerSpriteA;
                _speakerImageA.gameObject.SetActive(true);
            }
            else         
         {
                _speakerImageA.gameObject.SetActive(false);
            }

            if (dialogue.speakerSpriteB != null)
            {
                _speakerImageB.sprite = dialogue.speakerSpriteB;
                _speakerImageB.gameObject.SetActive(true);
            }
            else
            {
                _speakerImageB.gameObject.SetActive(false);
            }
        
            yield return _typewritterEffect.Run(displayText, _textLabel);

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
            {
                break;
            }
    

            //Waits until the given input has been pressed before continuing using the new input system
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame);

            
        }

        //if the dialogue has responses
        if (dialogueObject.HasResponses)
        {
            //Show the responses 
            _responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            //If it doesnt have any responses then close the dialogue box
            CloseDialogueBox();
        }

        
    }

    
    //A function that closes the dialogue box
    public void CloseDialogueBox()
    {
        
        Time.timeScale = 1.0f;

        //Sets is open to false
        IsOpen = false;

        

        //disables the dialogue box game object (hides it)
        _dialogueBox.SetActive(false);

        //Sets the text label's text to be empty
        _textLabel.text = string.Empty;

        //Invokes the OnDialogueBoxClose event
        OnDialogueBoxClose?.Invoke();
    } 
}
