using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRecipe : MonoBehaviour
{
    public CraftingRecipeScriptable recipeScriptable;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject plusSignPrefab;
    [SerializeField] private GameObject equalSignPrefab;
    [SerializeField] private Gradient fadeGradientCanCraft;
    [SerializeField] private Gradient fadeGradientCantCraft;

    private bool canCraftRecipe;

    public void OnPointerEnter()
    {
        canCraftRecipe = CraftingManager.Instance.CanCraftRecipe(recipeScriptable);
        StartCoroutine(FadeIn());
    }

    public void OnPointerExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
        
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
    
    public void UpdateRecipeUI(CraftingRecipeScriptable craftingRecipeScriptable)
    {
        recipeScriptable = craftingRecipeScriptable;

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipeScriptable.input.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, transform);
            newItem.transform.GetChild(0).GetComponent<Image>().sprite = recipeScriptable.input[i].item.icon;
            newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipeScriptable.input[i].count.ToString();

            if (i < recipeScriptable.input.Length - 1)
            {
                Instantiate(plusSignPrefab, transform);
            }
        }

        Instantiate(equalSignPrefab, transform);

        for (int i = 0; i < recipeScriptable.output.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, transform);
            newItem.transform.GetChild(0).GetComponent<Image>().sprite = recipeScriptable.output[i].item.icon;
            newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipeScriptable.output[i].count.ToString();

            if (i < recipeScriptable.output.Length - 1)
            {
                Instantiate(plusSignPrefab, transform);
            }
        }
    }
    
    private IEnumerator FadeIn()
    {
        float gradientTime = 0;

        while (gradientTime < 1)
        {
            foreach (Transform child in transform)
            {
                Image[] imageArray = child.GetComponentsInChildren<Image>();

                List<Image> images = new List<Image>(imageArray);

                foreach (Image i in images)
                {
                    if (i.color != Color.black)
                    {
                        if (canCraftRecipe)
                        {
                            i.color = fadeGradientCanCraft.Evaluate(gradientTime);
                        }
                        else
                        {
                            i.color = fadeGradientCantCraft.Evaluate(gradientTime);
                        }
                    }
                }
            }

            gradientTime += Time.deltaTime * 3;

            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float gradientTime = 1;

        while (gradientTime > 0)
        {
            foreach (Transform child in transform)
            {
                Image[] imageArray = child.GetComponentsInChildren<Image>();

                List<Image> images = new List<Image>(imageArray);

                foreach (Image i in images)
                {
                    if (i.color != Color.black)
                    {
                        if (canCraftRecipe)
                        {
                            i.color = fadeGradientCanCraft.Evaluate(gradientTime);
                        }
                        else
                        {
                            i.color = fadeGradientCantCraft.Evaluate(gradientTime);
                        }
                    }
                }
            }

            gradientTime -= Time.deltaTime * 3;

            yield return null;
        }
    }
}
