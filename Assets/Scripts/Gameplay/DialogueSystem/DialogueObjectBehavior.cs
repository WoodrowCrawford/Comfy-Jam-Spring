using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string text;

    public Sprite speakerSpriteA;
    public Sprite speakerSpriteB;
}



[CreateAssetMenu(menuName ="Dialogue/DialogueObject")]
public class DialogueObjectBehavior : ScriptableObject
{
    [SerializeField] private DialogueLine[] _dialogue;
    [SerializeField] private ResponseBehavior[] _responses;

    public DialogueLine[] Dialogue { get { return _dialogue; } }
    public bool HasResponses { get { return Responses != null && Responses.Length > 0; } } 

    public ResponseBehavior[] Responses { get {  return _responses; } }

}
