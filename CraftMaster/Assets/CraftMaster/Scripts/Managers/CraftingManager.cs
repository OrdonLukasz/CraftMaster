using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }
    public RecipeType selectedRecipeType;

    [SerializeField] private CraftingRecipeScriptable[] recipes;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private Transform recipeParent;

    private List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        selectedRecipeType = RecipeType.other;
        UpdateRecipeUI();
    }

    private void UpdateRecipeUI()
    {
        foreach (Transform child in recipeParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i].recipeType == selectedRecipeType)
            {
                GameObject newRecipe = Instantiate(recipePrefab, recipeParent);
                newRecipe.name = recipes[i].name;
            }
        }

        for (int i = 0; i < recipeParent.childCount; i++)
        {
            ItemRecipe recipeScript = recipeParent.GetChild(i).GetComponent<ItemRecipe>();
            CraftingRecipeScriptable recipeSO = null;

            foreach (CraftingRecipeScriptable r in recipes)
            {
                if (r.recipeName == recipeParent.GetChild(i).name)
                {
                    recipeSO = r;
                    break;
                }
            }

            recipeScript.UpdateRecipeUI(recipeSO);
        }
    }

    public bool CanCraftRecipe(CraftingRecipeScriptable recipeScriptable)
    {
        items = InventoryManager.Instance.GetAllItems();

        int foundItems = 0;

        foreach (ItemTypeAndCount neededItemAndCount in recipeScriptable.input)
        {
            foreach (ItemTypeAndCount foundItemAndCount in items)
            {
                if (foundItemAndCount.item == neededItemAndCount.item &&
                    foundItemAndCount.count >= neededItemAndCount.count)
                {
                    foundItems++;
                    break;
                }
            }
        }

        return foundItems == recipeScriptable.input.Length;
    }

    public void SelectRecipeType(string type)
    {
        switch (type)
        {
            case "other":
                selectedRecipeType = RecipeType.other;
                break;
            case "weapons":
                selectedRecipeType = RecipeType.weapons;
                break;
            case "tools":
                selectedRecipeType = RecipeType.tools;
                break;
        }

        UpdateRecipeUI();
    }
}