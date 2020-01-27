using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelSniping : MonoBehaviour
{
    public List<string> jewels;
    public Dropdown[] dropdowns;

    public GameObject AdvanceOne;
    public GameObject AdvanceTwo;
    public GameObject SaveListButton;

    string[] jewelSet;
    public List<JewelSet> pattern = new List<JewelSet>();
    List<GameObject> setRects = new List<GameObject>();

    public GameObject setTemplate;
    RectTransform templateRect;

    public RectTransform contentArea;
    Vector2 offset;
    int setIndex = 0;
    bool loaded = false;

	void Start ()
    {
        for (int i = 0; i < dropdowns.Length; i++)
        {
            dropdowns[i].AddOptions(jewels);
        }

        templateRect = contentArea.GetComponent<RectTransform>();
        offset = new Vector2(templateRect.rect.width, templateRect.rect.height);
        contentArea.sizeDelta = new Vector2(0, 0);

        if (PlayerPrefs.HasKey("sets"))
        {
            LoadList();
            loaded = true;
        }
    }

    IEnumerator GetJewelData()
    {
        using (WWW www = new WWW("https://mhworld.kiranico.com/decoration"))
        {
            yield return www;//wait for page to load

            string page = www.text.ToString();//turn page into readable text
            string[] substring = new String[] { "<tr>" };//what to seperate weapons by
            string[] jewelClass = page.Split(substring, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 2; i < jewelClass.Length; i++)
            {
                string[] lines = jewelClass[i].Split('\n');
                string[] jewelName = lines[2].Split('>');
                jewelName = jewelName[1].Split('<');
                jewels.Add(jewelName[0]);
            }
        }
    }

    public void AddSetToPattern()
    {
        jewelSet = new string[3] { jewels[dropdowns[0].value], jewels[dropdowns[1].value], jewels[dropdowns[2].value]};
        
        GameObject set = Instantiate(setTemplate, contentArea);
        set.name = setIndex.ToString();

        JewelSet js = set.GetComponent<JewelSet>();
        pattern.Add(js);
        js.Create(jewelSet, setIndex, this);

        RectTransform rt = set.GetComponent<RectTransform>();
        rt.sizeDelta = offset;

        contentArea.sizeDelta = new Vector2(offset.x, offset.y * pattern.Count);
        
        setIndex++;
        if (pattern.Count > 0)
        {
            AdvanceOne.SetActive(true);
            SaveListButton.SetActive(true);

            if (pattern.Count > 1)
            {
                AdvanceTwo.SetActive(true);
            }
        }
    }

    public void AddSetToPattern(List<string> setJewels)
    {
        jewelSet = new string[3] { setJewels[0], setJewels[1], setJewels[2] };

        GameObject set = Instantiate(setTemplate, contentArea);
        set.name = setIndex.ToString();

        JewelSet js = set.GetComponent<JewelSet>();
        pattern.Add(js);
        js.Create(jewelSet, setIndex, this);

        RectTransform rt = set.GetComponent<RectTransform>();
        rt.sizeDelta = offset;

        contentArea.sizeDelta = new Vector2(offset.x, offset.y * pattern.Count);

        setIndex++;
        if (pattern.Count > 0)
        {
            AdvanceOne.SetActive(true);
            SaveListButton.SetActive(true);

            if (pattern.Count > 1)
            {
                AdvanceTwo.SetActive(true);
            }
        }
    }

    public void Jump(int r)
    {
        for (int i = 0; i <= r; i++)
        {
            Destroy(pattern[0].gameObject);
            pattern.RemoveAt(0);
        }

        if (pattern.Count <= 2)
        {
            AdvanceTwo.SetActive(false);

            if (pattern.Count == 0)
            {
                AdvanceOne.SetActive(false);
                SaveListButton.SetActive(false);
            }
        }
        
        contentArea.sizeDelta = new Vector2(offset.x, offset.y * pattern.Count);
    }

    public void SaveList()
    {
        PlayerPrefs.SetInt("sets", pattern.Count);

        for (int i = 0; i < pattern.Count; i++)
        {
            JewelSet js = pattern[i];
            
            for (int j = 0; j < 3; j++)
            {
                PlayerPrefs.SetString("set" + i + j, js.text[j].text);
            }
        }
    }

    void LoadList()
    {
        int numSets = PlayerPrefs.GetInt("sets");

        List<string> set;
        for (int i = 0; i < numSets; i++)
        {
            set = new List<string>();

            for (int j = 0; j < 3; j++)
            {
                set.Add(PlayerPrefs.GetString("set" + i + j));
            }

            AddSetToPattern(set);
        }
    }
}
