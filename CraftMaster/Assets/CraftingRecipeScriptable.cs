using System.Collections;
using System.Collections.Generic;
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
    other = 0,
    tools = 1,
    food = 2,
    furniture = 3,
    weapons = 4,
    ornaments = 5,
    armour = 6
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