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

    private Entity held_entity = null;

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
            int movement_x = 0;
            int movement_y = 0;
            int movement_rotation = 0;
            if (Input.GetKeyDown(KeyCode.W))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        movement_y -= 1;
                        break;
                    case 1:
                        movement_x -= 1;
                        break;
                    case 2:
                        movement_y += 1;
                        break;
                    case 3:
                        movement_x += 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        movement_y += 1;
                        break;
                    case 1:
                        movement_x += 1;
                        break;
                    case 2:
                        movement_y -= 1;
                        break;
                    case 3:
                        movement_x -= 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        movement_x += 1;
                        break;
                    case 1:
                        movement_y += 1;
                        break;
                    case 2:
                        movement_x -= 1;
                        break;
                    case 3:
                        movement_y -= 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                switch (Player.Rotation)
                {
                    case 0:
                        movement_x -= 1;
                        break;
                    case 1:
                        movement_y -= 1;
                        break;
                    case 2:
                        movement_x += 1;
                        break;
                    case 3:
                        movement_y += 1;
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                movement_rotation -= 1;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                movement_rotation += 1;
            }

            new_player_rotation += movement_rotation;

            if (new_player_rotation < 0)
            {
                new_player_rotation += 4;
            }
            if (new_player_rotation > 3)
            {
                new_player_rotation -= 4;
            }

            new_player_x += movement_x;
            new_player_y += movement_y;

            if (CanMove(new_player_x, new_player_y, new_player_rotation, Player))
            {
                bool valid_held_entity_move = true;
                if (held_entity)
                {
                    valid_held_entity_move = false;
                    int new_held_entity_x = held_entity.WorldX;
                    int new_held_entity_y = held_entity.WorldY;
                    int new_held_entity_rotation = held_entity.Rotation;
                    if (movement_x != 0 || movement_y != 0)
                    {
                        new_held_entity_x = held_entity.WorldX + movement_x;
                        new_held_entity_y = held_entity.WorldY + movement_y;
                        new_held_entity_rotation = held_entity.Rotation;
                    }
                    if (movement_rotation != 0)
                    {
                        Vector3 pivot_vector = PivotEntity(
                            Player, held_entity, movement_rotation);
                        new_held_entity_x = (int)pivot_vector.x;
                        new_held_entity_y = (int)pivot_vector.y;
                        new_held_entity_rotation = (int)pivot_vector.z;       
                    }
                    valid_held_entity_move = CanMove(new_held_entity_x, new_held_entity_y, new_held_entity_rotation, held_entity);

                    if (valid_held_entity_move)
                    {
                        held_entity.WorldX = new_held_entity_x;
                        held_entity.WorldY = new_held_entity_y;
                        held_entity.Rotation = new_held_entity_rotation;
                    }
                }
                if (valid_held_entity_move)
                {
                    Player.WorldX = new_player_x;
                    Player.WorldY = new_player_y;
                    Player.Rotation = new_player_rotation;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (held_entity)    // drop a held entity
                {
                    held_entity.Lifted = false;
                    Debug.Log(string.Format("Dropped {0}", held_entity.Name));
                    held_entity = null;
                } else
                {
                    switch (Player.Rotation)
                    {
                        case 0:
                            held_entity = GetEntityAtLocation(
                                Player.WorldX, Player.WorldY - 1);
                            break;
                        case 1:
                            held_entity = GetEntityAtLocation(
                                Player.WorldX -1, Player.WorldY);
                            break;
                        case 2:
                            held_entity = GetEntityAtLocation(
                                Player.WorldX, Player.WorldY + 1);
                            break;
                        case 3:
                            held_entity = GetEntityAtLocation(
                                Player.WorldX + 1, Player.WorldY);
                            break;
                    }
                    if (held_entity)
                    {
                        Debug.Log(string.Format("Lifted {0}", held_entity.Name));
                        held_entity.Lifted = true;
                    } else
                    {
                        Debug.Log("Nothing to lift.");
                    }
                }
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
            first_name_list = NamesMale.text.Split(new[] { "\r\n", "\r", "\n" },
                                             System.StringSplitOptions.None);
            relation_list = RelationsMale.text.Split(
                new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        }
        else
        {
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

    bool CanMove(int x, int y, int rotation, Entity moving_entity)
    {
        if (x < 0 | y < 0 | x > Constants.MAX_X | y > Constants.MAX_Y)
        {
            Debug.Log("movement blocked by world bounds: " + moving_entity.Name);
            return false;
        }
        foreach(Entity entity in WorldEntities)
        {
            if (moving_entity != entity & entity.OccupiesTile(x, y))
            {
                Debug.Log(string.Format(
                    "movement blocked by {0}: {1}",
                    entity.Name, moving_entity.Name));
                return false;
            }
        }
        return true;
    }

    Entity GetEntityAtLocation(int x, int y)
    {
        foreach(Entity entity in WorldEntities)
        {
            if (entity.OccupiesTile(x, y))
            {
                return entity;
            }
        }
        return null;
    }

    Vector3 PivotEntity(Entity pivot_entity, Entity moving_entity, int rotation)
    {
        int new_moving_rotation = moving_entity.Rotation + rotation;
        int new_pivot_rotation = pivot_entity.Rotation + rotation;
        int new_x = pivot_entity.WorldX;
        int new_y = pivot_entity.WorldY;
        if (new_moving_rotation < 0)
        {
            new_moving_rotation += 4;
        }
        if (new_moving_rotation > 3)
        {
            new_moving_rotation -= 4;
        }
        if (new_pivot_rotation < 0)
        {
            new_pivot_rotation += 4;
        }
        if (new_pivot_rotation > 3)
        {
            new_pivot_rotation -= 4;
        }
        switch (new_pivot_rotation)
        {
            case 0:
                new_y -= 1;
                break;
            case 1:
                new_x -= 1;
                break;
            case 2:
                new_y += 1;
                break;
            case 3:
                new_x += 1;
                break;
        }
        return new Vector3(new_x, new_y, new_moving_rotation);
    }
}
