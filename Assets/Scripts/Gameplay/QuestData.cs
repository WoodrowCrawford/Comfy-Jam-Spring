using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/QuestData")]
public class QuestData : ScriptableObject
{
    public List<ItemData> possibleItems;
    public int itemsToRequest = 3;
}
