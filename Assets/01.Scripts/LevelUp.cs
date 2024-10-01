using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        GameManager.Instance.Stop();
        rect.localScale = Vector3.one;
    }

    public void Hide()
    {
        GameManager.Instance.Resume();
        rect.localScale = Vector3.zero;
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

}
