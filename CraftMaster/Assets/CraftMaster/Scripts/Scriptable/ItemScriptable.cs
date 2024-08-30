using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemScriptable : ScriptableObject
{
    public string name;
    public Sprite icon;
    public GameObject prefab;
    public int stackMax;
    public int craftSuccessPercentage;
}
