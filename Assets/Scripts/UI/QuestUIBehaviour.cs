using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUIBehavior : MonoBehaviour
{
    [SerializeField] GameObject questItemTemplate;
    [SerializeField] Transform listContainer;
    [SerializeField] GameObject questPanel;

    private void OnEnable()
    {
        QuestManager.OnQuestUpdated += RefreshUI;
        DayCycleManager.OnNewDay += RefreshUI;

        RefreshUI();
    }

    private void OnDisable()
    {
        QuestManager.OnQuestUpdated -= RefreshUI;
    }

    public void RefreshUI()
    {
        if (QuestManager.Instance == null) return;

        foreach (Transform child in listContainer) Destroy(child.gameObject);

        foreach (var entry in QuestManager.Instance.questProgress)
        {
            GameObject newLine = Instantiate(questItemTemplate, listContainer);
            TMP_Text text = newLine.GetComponent<TMP_Text>();

            int current = entry.Value.x;
            int goal = entry.Value.y;

            string prefix = (current >= goal) ? "● " : "○ ";
            text.text = $"{prefix} {entry.Key.ItemName} ({current}/{goal})";

            text.color = (current >= goal) ? Color.green : Color.white;
        }
    }

    public void ToggleQuestPanel(bool show)
    {
        if (questPanel != null)
        {
            questPanel.SetActive(show);
            Debug.Log("Quest Panel set to: " + show);
        }
        else
        {
            Debug.LogWarning("Quest Panel reference is missing on QuestUIBehavior!");
        }
    }

    public void ToggleQuestPanel()
    {
        if (questPanel != null)
        {
            bool currentState = questPanel.activeSelf;
            questPanel.SetActive(!currentState);
            Debug.Log("Quest Panel toggled to: " + !currentState);

            if(questPanel.activeSelf)
            {
                SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.listOpenClip2, questPanel.transform, false, 0f, 0f);
            }
            else
            {
                SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.listCloseClip, questPanel.transform, false, 0f, 0f);
            }
        }
    }
}
