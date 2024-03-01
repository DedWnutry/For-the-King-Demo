using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public int invWidth;
    public int invHeight;
    public Transform[,] invMatrix;
    public GameObject slotPrefab;
    private Color32 defaultColor;

    void Awake()
    {

        SetupInventory();
    }

    public void SetupInventory()
    {
        

        invMatrix = new Transform[invHeight, invWidth];

        for (int i = 0; i < transform.childCount; i++)
        {
            int k = GetNum(transform.GetChild(i).name);

            int x = k / (invWidth);

            int y = k % (invWidth);

            invMatrix[x, y] = transform.GetChild(i);
        }
    }

    void Start()
    {
        defaultColor = GetComponent<Image>().color;


        //GetComponent<GridLayoutGroup>().enabled = false;
    }

    public bool CheckForFreeSpace(string slotName, int vertical, int horizontal)
    {
        bool result = false;

        int k = GetNum(slotName);

        int x = k / (invWidth);

        int y = k % (invWidth);

        if ((x + horizontal ) <= invHeight && (y + vertical) <= invWidth)
        {
            for (int i = x; i < (x + horizontal); i++)
            {
                for (int j = y; j < (y + vertical); j++)
                {
                    if (invMatrix[i, j].GetComponent<Slot>().occupied == false) result = true;
                    else
                    {
                        result = false;

                        break;
                    }

                }
                
                if (result == false) break;
            }
        }
        else result = false;

        return result;
    }

    public Vector3 StoreInBag(string slotName, int vertical, int horizontal)
    {
        int k = GetNum(slotName);

        int x = k / (invWidth);

        int y = k % (invWidth);

        Vector3 result = new Vector3(0, 0, 0);

        for (int i = x; i < (x + horizontal); i++)
        {
            for (int j = y; j < (y + vertical); j++)
            {
                invMatrix[i, j].GetComponent<Slot>().occupied = true;

                //invMatrix[i, j].GetComponent<Image>().raycastTarget = false;

                //invMatrix[i, j].GetComponent<Image>().color = new Color32(255, 0, 0, 255);

                //invMatrix[i, j].GetComponent<Image>().raycastTarget = false;

                ///totalHorizontal += invMatrix[i, j].transform.position.x;

                //totalVertical += invMatrix[i, j].transform.position.y;

                //result += invMatrix[i, j].transform.position;

                //result = (invMatrix[x, y].transform.position + invMatrix[i, j].transform.position) / i;
            }
        }
        /*
        float totalHorizontal = invMatrix[x, y].transform.position.x;

        float totalVertical = invMatrix[x, y].transform.position.y;

        totalHorizontal = invMatrix[x, y].transform.position.x + horizontal * 50;

        totalVertical = invMatrix[x, y].transform.position.y + vertical + 50;

        var centerX = totalHorizontal / (horizontal * vertical);

        var centerY = totalVertical / (horizontal * vertical);

        Vector2 result = new Vector3(centerX, centerY);
        */
        return result = invMatrix[x, y].transform.position;
    }

    public void EnableSlots(string slotName, int vertical, int horizontal)
    {
        int k = GetNum(slotName);

        int x = k / (invWidth);

        int y = k % (invWidth);

        for (int i = x; i < (x + horizontal); i++)
        {
            for (int j = y; j < (y + vertical); j++)
            {
                invMatrix[i, j].GetComponent<Slot>().occupied = false;
                
                invMatrix[i, j].GetComponent<Image>().raycastTarget = true;

                //invMatrix[i, j].GetComponent<Image>().color = defaultColor;
            }
        }
    }

    IEnumerator InitSlots()
    {
        /*
        for (int i = 0; i < invWidth * invHeight; i++)
        {
            var clone = Instantiate(slotPrefab, transform);

            clone.name = "slot (" + i + ")";  
        }
        */

        yield return null;
    }

    public void GetIndex()
    {
        string name = EventSystem.current.currentSelectedGameObject.transform.name;

        int k = GetNum(name);

        int x = k / (invWidth);

        int y = k % (invWidth);

        //matrix[x, y] = float.Parse(EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().text);

        Debug.Log($"{name} {x} {y}");
    }

    private int GetNum(string name)
    {
        Regex regex = new Regex("\\((\\d+)\\)");

        Match match = regex.Match(name);

        if (!match.Success) throw new Exception("Unrecognized object name");

        Group group = match.Groups[1];

        string num = group.Value;

        return Convert.ToInt32(num);
    }
}
