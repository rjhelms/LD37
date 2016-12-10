using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public const int GRID_SIZE = 48;
    public const int MAX_X = 8;
    public const int MAX_Y = 8;

    public TextAsset NamesMale;
    public TextAsset NamesFemale;
    public TextAsset NamesSurnames;

    public TextAsset RelationsMale;
    public TextAsset RelationsFemale;

    public Text NameText;

    public Entity Player;

    // Use this for initialization
    void Start()
    {
        NameText.text = GenerateName();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            NameText.text = GenerateName();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Player.WorldY += 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Player.WorldY -= 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Player.WorldX -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Player.WorldX += 1;
        }
        if (Player.WorldY < 0)
        {
            Player.WorldY = 0;
        }
        if (Player.WorldY > MAX_Y)
        {
            Player.WorldY = MAX_Y;
        }
        if (Player.WorldX < 0)
        {
            Player.WorldX = 0;
        }
        if (Player.WorldX > MAX_X)
        {
            Player.WorldX = MAX_X;
        }
    }

    string GenerateName()
    {
        string first_name;
        string last_name;
        string relation;

        string[] first_name_list;
        string[] last_name_list = NamesSurnames.text.Split(
            new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        string[] relation_list;

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
            relation_list = RelationsMale.text.Split(
                new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        }
        else
        {
            Debug.Log("female");
            first_name_list = NamesFemale.text.Split(
                new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            relation_list = RelationsFemale.text.Split(
                new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        }
        first_name = first_name_list[Random.Range(0, first_name_list.Length)];
        last_name = last_name_list[Random.Range(0, last_name_list.Length)];
        relation = relation_list[Random.Range(0, relation_list.Length)];
        return string.Format("{0} {1}, {2}", first_name, last_name,
                             relation);
    }
}
