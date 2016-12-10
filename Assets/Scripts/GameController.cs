using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Player.Rotation -= 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Player.Rotation += 1;
        }

        if (Player.WorldY < 0)
        {
            Player.WorldY = 0;
        }
        if (Player.WorldY > Constants.MAX_Y)
        {
            Player.WorldY = Constants.MAX_Y;
        }
        if (Player.WorldX < 0)
        {
            Player.WorldX = 0;
        }
        if (Player.WorldX > Constants.MAX_X)
        {
            Player.WorldX = Constants.MAX_X;
        }
        if (Player.Rotation < 0)
        {
            Player.Rotation += 4;
        }
        if (Player.Rotation > 3)
        {
            Player.Rotation -= 4;
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
