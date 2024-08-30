using UnityEngine;

[CreateAssetMenu(fileName = "Item Recipe", menuName = "Scriptable Objects/Item Recipe")]
public class CraftingRecipeScriptable : ScriptableObject
{
    public string recipeName;
    public RecipeType recipeType;
    public ItemTypeAndCount[] input;
    public ItemTypeAndCount[] output;
}

public enum RecipeType
{
    other,
    tools,
    food,
    furniture,
    weapons,
    ornaments,
    armour
};

[System.Serializable]
public class ItemTypeAndCount
{
    public ItemScriptable item;
    public int count;

    public ItemTypeAndCount(ItemScriptable itemType, int itemCount)
    {
        item = itemType;
        count = itemCount;
    }
}