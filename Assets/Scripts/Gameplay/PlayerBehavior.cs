using UnityEngine;

/// <summary>
/// Will contain all the behaviors for the player such as name, health and other stats.
/// </summary>
public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private string playerName;

    //public getter for the player name
    public string PlayerName => playerName;

    void OnEnable()
    {
        PromptMessageBehavior.OnNameCreated += SetPlayerName;
    }

    void OnDisable()
    {
        PromptMessageBehavior.OnNameCreated -= SetPlayerName;
    }


    public void SetPlayerName(string name)
    {
        playerName = name;
    }   


    public void Test()
    {
        Debug.Log("Dialoigue UI is working");
    }
}
