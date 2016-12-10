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

    public Entity[] WorldEntities;

    // Use this for initialization
    void Start()
    {
        NameText.text = GenerateName();
        WorldEntities[0].Name = GenerateName(true);
        WorldEntities[0].gameObject.name = WorldEntities[0].Name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            NameText.text = GenerateName();
            int new_player_x = Player.WorldX;
            int new_player_y = Player.WorldY;
            int new_player_rotation = Player.Rotation;
            if (Input.GetKeyDown(KeyCode.W))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        new_player_y -= 1;
                        break;
                    case 1:
                        new_player_x -= 1;
                        break;
                    case 2:
                        new_player_y += 1;
                        break;
                    case 3:
                        new_player_x += 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        new_player_y += 1;
                        break;
                    case 1:
                        new_player_x += 1;
                        break;
                    case 2:
                        new_player_y -= 1;
                        break;
                    case 3:
                        new_player_x -= 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        new_player_x += 1;
                        break;
                    case 1:
                        new_player_y += 1;
                        break;
                    case 2:
                        new_player_x -= 1;
                        break;
                    case 3:
                        new_player_y -= 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        new_player_x -= 1;
                        break;
                    case 1:
                        new_player_y -= 1;
                        break;
                    case 2:
                        new_player_x += 1;
                        break;
                    case 3:
                        new_player_y += 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                new_player_rotation -= 1;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                new_player_rotation += 1;
            }

            if (new_player_rotation < 0)
            {
                new_player_rotation += 4;
            }
            if (new_player_rotation > 3)
            {
                new_player_rotation -= 4;
            }

            if (CanMove(new_player_x, new_player_y, new_player_rotation))
            {
                Player.WorldX = new_player_x;
                Player.WorldY = new_player_y;
                Player.Rotation = new_player_rotation;
            }
        }
    }

    string GenerateName(bool? male=null)
    {
        string first_name;
        string last_name;
        string relation;

        string[] first_name_list;
        string[] last_name_list = NamesSurnames.text.Split(
            new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        string[] relation_list;

        if (male == null)
        {
            if (Random.value < 0.5)
            {
                male = true;
            }
        }
        if (male == true)
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

    bool CanMove(int x, int y, int rotation)
    {
        if (x < 0 | y < 0 | x > Constants.MAX_X | y > Constants.MAX_Y)
        {
            Debug.Log("movement blocked by world bounds.");
            return false;
        }
        foreach(Entity entity in WorldEntities)
        {
            if (entity.OccupiesTile(x, y))
            {
                Debug.Log(string.Format("movement blocked by {0}", entity.Name));
                return false;
            }
        }
        return true;
    }
}
