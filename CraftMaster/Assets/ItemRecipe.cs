using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecipe : MonoBehaviour
{
    public CraftingRecipeScriptable recipeScriptable;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject plusSignPrefab;
    [SerializeField] GameObject equalSignPrefab;

    [SerializeField] Gradient fadeGradientCanCraft;
    [SerializeField] Gradient fadeGradientCantCraft;

    private bool canCraftRecipe;

    public void OnPointerEnter()
    {
        canCraftRecipe = CraftingManager.Instance.CanCraftRecipe(recipeScriptable);
        
    }

    public void OnPointerExit()
    {
        StopAllCoroutines();
        
    }

    public void OnPointerClick()
    {
        if (CraftingManager.Instance.CanCraftRecipe(recipeScriptable))
        {
            InventoryManager.Instance.CraftItems(new List<ItemTypeAndCount>(recipeScriptable.output), new List<ItemTypeAndCount>(recipeScriptable.input));

            canCraftRecipe = CraftingManager.Instance.CanCraftRecipe(recipeScriptable);
            if (!canCraftRecipe)
            {
                
            }
        }
    }
}
