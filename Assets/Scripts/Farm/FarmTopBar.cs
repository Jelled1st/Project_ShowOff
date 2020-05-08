using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmTopBar : MonoBehaviour
{
    [SerializeField] private List<SeedsScriptableObject> _seeds;
    [SerializeField] private GameObject _topBarPanel;

    private List<Image> _icons;

    // Start is called before the first frame update
    void Start()
    {
        ScaleTopBar();
        for(int i = 0; i < _seeds.Count; ++i)
        {
            AddIconToTopBar(LoadImageFromPath(_seeds[i].GetFile()));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ScaleTopBar()
    {
        RectTransform topBar = _topBarPanel.GetComponent<RectTransform>();
        topBar.sizeDelta = new Vector2(Screen.width, topBar.sizeDelta.y);
    }

    private GameObject LoadImageFromPath(string file)
    {
        GameObject imageObject = new GameObject(file, typeof(RectTransform));
        Image img = imageObject.AddComponent<Image>();
        Texture2D tex = Resources.Load(file) as Texture2D;
        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        if (img.sprite == null) Debug.Log("Something went wrong");
        Debug.Log(Resources.Load(file));
        return imageObject;
    }

    private void AddIconToTopBar(GameObject imageObject)
    {
        RectTransform topBar = _topBarPanel.GetComponent<RectTransform>();

        imageObject.GetComponent<RectTransform>().SetParent(topBar);
        Image img = imageObject.GetComponent<Image>();
        if (_icons == null) _icons = new List<Image>();
        _icons.Add(img);
        imageObject.GetComponent<RectTransform>().localPosition = new Vector2(0, -50);
    }
}
