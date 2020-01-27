using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunction : MonoBehaviour
{
    public Items items;

    #region Family Display Specific variables
    public GameObject weaponButtonPrefab;
    public RectTransform contentHolder;
    List<GameObject> buttons = new List<GameObject>();
    Vector2 contentRectStart = new Vector2();
    Vector2 startPos = new Vector2();
    float offset;
    float buttonSize;
    bool populatedBefore = false;
    WeaponFamily displayFamily;
    public Sprite missingImage;
    #endregion

    #region Weapon Tree Display
    char downArrow = '\u25BC';
    public Text treeText;
    #endregion

    #region Materials Display
    public Text matText;
    #endregion

    public void BuildFamilyTree(int familyIndex)
    {
        CheckContentRectSize();
        
        displayFamily = items.weapons[familyIndex];

        int WeaponListCount = displayFamily.weapons.Count;

        contentHolder.sizeDelta = new Vector2(offset * 9, (displayFamily.branches.Count-1) * offset);

        int treeCount = 1;

        for (int i = 0; i < WeaponListCount; i++)
        {
            Weapon w = displayFamily.weapons[i];
            if (i != 0)
            {
                if (treeCount < displayFamily.branches.Count)
                {
                    if (i == displayFamily.branches[treeCount])
                    {
                        treeCount++;
                        startPos.y -= offset;

                        if (w.tier == 0)
                        {
                            startPos.x = offset / 2;
                        }
                        else
                        {
                            startPos.x = displayFamily.weapons[w.previous].spot.x + offset;
                        }
                    }
                }
            }
            
            GameObject o = Instantiate(weaponButtonPrefab, contentHolder);
            o.name = w.weaponName;

            RectTransform rt = o.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(buttonSize, buttonSize);
            rt.localPosition = startPos;

            Button b = o.GetComponent<Button>();

            if (i > displayFamily.weaponImage.Length - 1)
            {
                b.image.sprite = missingImage;
            }
            else
            {
                b.image.sprite = displayFamily.weaponImage[i];
            }

            w.spot = startPos;
            b.onClick.AddListener(delegate { BuildWeaponTree(w, displayFamily); });

            Text t = o.transform.GetChild(0).GetComponent<Text>();
            t.text = w.rarity.ToString();
            t.color = items.rarityColors[w.rarity-1];
            buttons.Add(o);
            startPos.x += offset;
        }

        populatedBefore = true;
    }

    private void CheckContentRectSize()
    {
        if (!populatedBefore)
        {
            contentRectStart = contentHolder.sizeDelta;            
        }
        else
        {
            contentHolder.sizeDelta = contentRectStart;

            foreach (GameObject b in buttons)
            {
                Destroy(b);
            }
            buttons.Clear();

            treeText.text = "";
            matText.text = "";
        }

        offset = contentHolder.rect.width;
        startPos = new Vector2(offset / 2, -(offset / 2));
        buttonSize = offset - (offset * .22f);
    }

    public void BuildWeaponTree(Weapon w, WeaponFamily wf)
    {
        treeText.text = "";
        matText.text = "";
        List<Weapon> tree = new List<Weapon>();
        List<string> items = new List<string>();
        List<int> num = new List<int>();

        GetPrevious(w, wf, ref tree);
        CompileMats(w, wf, ref items, ref num);

        for (int i = tree.Count-1; i >= 0 ; i--)
        {
            //print(tree[i].weaponName);
            treeText.text += tree[i].weaponName + '\n';

            if (i != 0)
            {
                treeText.text += downArrow.ToString() + '\n';
            }
        }

        for (int i = items.Count-1; i >= 0; i--)
        {
            int rem = i % 2;

            matText.text += items[i] + " x " + num[i];

            if (rem == 0)
            {
                matText.text += "     ";
            }
            else
            {
                matText.text += '\n';
            }
                
        }
    }

    void CompileMats(Weapon w, WeaponFamily wf, ref List<string> s, ref List<int> n)
    {
        if (w.forge)
        {
            for (int i = 0; i < w.forgeItem.Count; i++)
            {
                if (s.Contains(w.forgeItem[i]))
                {
                    n[s.IndexOf(w.forgeItem[i])] += w.forgeNum[i];
                }
                else
                {
                    s.Add(w.forgeItem[i]);
                    n.Add(w.forgeNum[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < w.item.Count; i++)
            {
                if (s.Contains(w.item[i]))
                {
                    n[s.IndexOf(w.item[i])] += w.num[i];
                }
                else
                {
                    s.Add(w.item[i]);
                    n.Add(w.num[i]);
                }
            }
            CompileMats(wf.weapons[w.previous], wf, ref s, ref n);
        }

    }

    void GetPrevious(Weapon w, WeaponFamily wf, ref List<Weapon> tree)
    {
        tree.Add(w);

        if (w.previous >= 0)
        {
            GetPrevious(wf.weapons[w.previous], wf, ref tree);
        }
    }
}
