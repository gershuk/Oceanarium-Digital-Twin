using UnityEngine;

public class ItemScript : MonoBehaviour, IItemInfo
{
    public string info = "test";

    public string InfoLable => info;
}