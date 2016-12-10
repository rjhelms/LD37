using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public TextAsset NamesMale;
    public TextAsset NamesFemale;
    public TextAsset NamesSurnames;

    public Text NameText;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            NameText.text = GenerateName();
        }
    }

    string GenerateName()
    {
        string first_name;
        string last_name;
        string[] first_name_list;
        string[] last_name_list = NamesSurnames.text.Split(
            new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        bool male = false;
        if (Random.value < 0.5)
        {
            male = true;
        }
        if (male)
        {
            Debug.Log("male");
            first_name_list = NamesMale.text.Split(new[] { "\r\n", "\r", "\n" },
                                             System.StringSplitOptions.None);
        }
        else
        {
            Debug.Log("female");
            first_name_list = NamesFemale.text.Split(
                new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        }
        first_name = first_name_list[Random.Range(0, first_name_list.Length)];
        last_name = last_name_list[Random.Range(0, last_name_list.Length)];

        return string.Format("{0}\r\n{1}", first_name, last_name);
    }
}
