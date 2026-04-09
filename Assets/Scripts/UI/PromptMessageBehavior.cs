using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PromptMessageBehavior : MonoBehaviour
{

    public delegate void PromptMessageEventHandler();
    public delegate void NameCreatedEventHandler(string name);

    public static event PromptMessageEventHandler OnPromptMessageClicked;
    public static event PromptMessageEventHandler OnPromptMessageClosed;
    public static event PromptMessageEventHandler OnPromptMessageSubmitted;
    public static event NameCreatedEventHandler OnNameCreated;


    
    [SerializeField] private GameObject nameInputField;


    void OnEnable()
    {
        nameInputField.GetComponent<TMP_InputField>().onSelect.AddListener((cxt) => OnPromptMessageClicked?.Invoke());
        nameInputField.GetComponent<TMP_InputField>().onEndEdit.AddListener((cxt) => OnPromptMessageClosed?.Invoke());
        nameInputField.GetComponent<TMP_InputField>().onSubmit.AddListener((ctx) => SubmitPromptMessage());
        
    }

    void OnDisable()
    {
        nameInputField.GetComponent<TMP_InputField>().onSelect.RemoveAllListeners();
        nameInputField.GetComponent<TMP_InputField>().onEndEdit.RemoveAllListeners();
        nameInputField.GetComponent<TMP_InputField>().onSubmit.RemoveAllListeners();
    }



    [Header("Prompts Data")]
    [SerializeField] private GameObject promptMessageUI;
    [SerializeField] private string pickNamePrompt = "What is your name?";


    void Start()
    {
        //set the prompt message to 
        SetPromptMessage(pickNamePrompt);
    }


    public void SetPromptMessage(string message)
    {
        if (promptMessageUI == null)
        {
            return;
        }

        TextMeshProUGUI promptText = promptMessageUI.GetComponentInChildren<TextMeshProUGUI>();

        if (promptText != null)
        {
            promptText.text = message;
        }
    }


    public void SubmitPromptMessage()
    {
        if (promptMessageUI.GetComponentInChildren<TextMeshProUGUI>().text == pickNamePrompt && !string.IsNullOrEmpty(nameInputField.GetComponent<TMP_InputField>().text))
        {
            OnPromptMessageSubmitted?.Invoke();
            Debug.Log("Player name submitted: " + nameInputField.GetComponent<TMP_InputField>().text + "! Welcome, " + nameInputField.GetComponent<TMP_InputField>().text + "!");

            //fire an event here that will tell the player behavior to set the player name to the name that was submitted in the input field
            OnNameCreated?.Invoke(nameInputField.GetComponent<TMP_InputField>().text);
        }
    }
}