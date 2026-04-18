using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WardrobeBehavior : MonoBehaviour
{

    public GameObject hatsParent;
    public GameObject hatButtonPrefab;
    private readonly List<Button> spawnedHatButtons = new();

    public static bool playerPickedHatForTheDay = false;


    [Header("Buttons")]
    [SerializeField] private Button goAsIsButton;

    [Header("Hat Sprites")]
    [SerializeField] private Sprite redHatSprite;
    [SerializeField] private Sprite blueHatSprite;



    void Awake()
    {
        RewardsBehavior.OnHatRewardSelected += CreateHatSlot;
    }



    void OnEnable()
    {
        goAsIsButton.onClick.AddListener(() => playerPickedHatForTheDay = true);
    }


    void OnDisable()
    {
        Debug.Log("WardrobeBehavior disabled, unsubscribing from events.");
        goAsIsButton.onClick.RemoveAllListeners();

        foreach (Button spawnedHatButton in spawnedHatButtons)
        {
            if (spawnedHatButton == null)
            {
                continue;
            }

            spawnedHatButton.onClick.RemoveAllListeners();
        }

        spawnedHatButtons.Clear();
    }

    void OnDestroy()
    {
        RewardsBehavior.OnHatRewardSelected -= CreateHatSlot;
    }



    //a function to handle when a hat button is clicked, which will equip the hat the player selected
    private void HandleHatButtonClicked(Sprite hatSprite)
    {
        //if they clicked the red hat button, equip the red hat, if they clicked the blue hat button, equip the blue hat
        if (hatSprite == redHatSprite)
        {
            Debug.Log("Red hat button clicked, equipping red hat.");
            playerPickedHatForTheDay = true;
        }
        else if (hatSprite == blueHatSprite)
        {
            Debug.Log("Blue hat button clicked, equipping blue hat.");
            playerPickedHatForTheDay = true;
        }
    }


    //a function to create a new hat slot in the wardrobe for a given hat sprite, and add a listener to the button to equip the hat when clicked
    public void CreateHatSlot(Sprite hatSprite)
    {
        //create a new hat button in the wardrobe for the given hat sprite
        GameObject spawnedButtonObject = Instantiate(hatButtonPrefab, hatsParent.transform);
        Button spawnedHatButton = spawnedButtonObject.GetComponent<Button>();
        spawnedButtonObject.GetComponent<Image>().preserveAspect = true;

        //set the button's image to be the hat sprite
        spawnedButtonObject.transform.GetChild(0).GetComponent<Image>().sprite = hatSprite;

        //add a listener to the button to equip the hat when clicked
        spawnedHatButton.onClick.AddListener(() => HandleHatButtonClicked(hatSprite));
        spawnedHatButtons.Add(spawnedHatButton);
    }
    


}
