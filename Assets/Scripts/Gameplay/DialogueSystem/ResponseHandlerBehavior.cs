using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandlerBehavior : MonoBehaviour
{
    private DialogueUIBehavior _dialogueUI;
    private ResponseEvent[] responseEvents;

    [SerializeField] private RectTransform _responseBox;
    [SerializeField] private RectTransform _responseButtonTemplate;
    [SerializeField] private RectTransform _responseContainer;

   
    List<GameObject> tempResponseButtons = new List<GameObject>();


    private void Awake()
    {
        //gets the needed components on awake
        _dialogueUI = GetComponent<DialogueUIBehavior>();
    }

  
    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponses(ResponseBehavior[] responses)
    {
        for (int i = 0; i < responses.Length; i++) 
        {
            ResponseBehavior response = responses[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(_responseButtonTemplate.gameObject, _responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            tempResponseButtons.Add(responseButton);
        }

        _responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(ResponseBehavior response, int responseIndex)
    {
        _responseBox.gameObject.SetActive(false);

        foreach(GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex >= 0 && responseIndex < responseEvents.Length)
        {
            responseEvents[responseIndex]?.OnPickedResponse?.Invoke();
        }

        responseEvents = null;


        if(response.DialogueObject)
        {
            _dialogueUI.CloseDialogueBox();
            _dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            _dialogueUI.CloseDialogueBox();
        }
        
    }
}

