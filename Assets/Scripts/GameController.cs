﻿using System.Collections;
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
    public Text CarryingText;

    public Color NameTextNormal;
    public Color NameTextWarning;
    public Color NameTextUrgent;
    public Entity Player;

    public List<Entity> WorldEntities;

    private Entity held_entity = null;

    public GameObject[] MalePrefabs;
    public GameObject[] FemalePrefabs;
    public GameObject[] PossessionPrefabs;

    public Vector3 PrefabInstantiatePoint;
    public bool RelativeMovement = false;
    public float TimeBetweenSpawns = 1f;
    public float TimeBetweenSpawnsInitial = 5f;
    public float TimeBetweenSpawnsMinimum = 0.1f;
    public float TimeAccelerationFactor = 1.1f;
    private Entity next_entity;
    private int possessions_to_spawn;
    private string possession_prefix;
    private float time_to_next_spawn;

    // Use this for initialization
    void Start()
    {
        WorldEntities.Add(Player);
        next_entity = CreateImmigrant();
        possessions_to_spawn = Random.Range(0, 3);
        NameText.text = next_entity.Name;
        time_to_next_spawn = Time.time + TimeBetweenSpawnsInitial;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawn();

        if (Input.anyKeyDown)
        {
            int new_player_x = Player.WorldX;
            int new_player_y = Player.WorldY;
            int new_player_rotation = Player.Rotation;
            int movement_x, movement_y, movement_rotation;
            ProcessMovementInput(out movement_x, out movement_y, out movement_rotation);

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
                TryMove(movement_x, movement_y, movement_rotation);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryLift();
            }
        }
    }

    private void TryLift()
    {
        if (held_entity)    // drop a held entity
        {
            held_entity.Lifted = false;
            Debug.Log(string.Format("Dropped {0}", held_entity.Name));
            held_entity = null;
            CarryingText.text = "Nothing";
        }
        else
        {
            switch (Player.Rotation)
            {
                case 0:
                    held_entity = GetEntityAtLocation(
                        Player.WorldX, Player.WorldY - 1);
                    break;
                case 1:
                    held_entity = GetEntityAtLocation(
                        Player.WorldX - 1, Player.WorldY);
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
                CarryingText.text = held_entity.Name;
            }
            else
            {
                Debug.Log("Nothing to lift.");
            }
        }
    }

    private void ProcessMovementInput(out int movement_x, out int movement_y, out int movement_rotation)
    {
        movement_x = 0;
        movement_y = 0;
        movement_rotation = 0;
        if (RelativeMovement)
        {
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
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                movement_y += 1;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                movement_y -= 1;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                movement_x -= 1;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                movement_x += 1;
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
    }

    private void TryMove(int movement_x, int movement_y, int movement_rotation)
    {
        int new_player_x = Player.WorldX + movement_x;
        int new_player_y = Player.WorldY + movement_y;
        int new_player_rotation = Player.Rotation + movement_rotation;
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

    private void UpdateSpawn()
    {
        float time_remaining = time_to_next_spawn - Time.time;
        if (time_remaining <= 1)
        {
            NameText.color = NameTextUrgent;
        }
        else if (time_remaining <= 2)
        {
            NameText.color = NameTextWarning;
        }
        else
        {
            NameText.color = NameTextNormal;
        }
        if (time_remaining <= 0)
        {
            NameText.color = NameTextNormal;
            Debug.Log("Spawning entity");
            if (CanMove(
                next_entity.WorldX, next_entity.WorldY, next_entity.Rotation,
                next_entity))
            {
                next_entity.Spawn();
                WorldEntities.Add(next_entity);
                if (possessions_to_spawn == 0)
                {
                    next_entity = CreateImmigrant();
                    possessions_to_spawn = Random.Range(0, 3);
                }
                else
                {
                    next_entity = CreatePossession();
                    possessions_to_spawn--;
                }
                NameText.text = next_entity.Name;
            }
            else
            {
                Debug.Log("Game over!");
            }
            time_to_next_spawn = Time.time + TimeBetweenSpawns;
            TimeBetweenSpawns = TimeBetweenSpawns / TimeAccelerationFactor;
            if (TimeBetweenSpawns < TimeBetweenSpawnsMinimum)
            {
                TimeBetweenSpawns = TimeBetweenSpawnsMinimum;
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
                bool can_block = false;
                if (moving_entity == Player & !entity.Lifted)
                {
                    can_block = true;
                } else if (moving_entity.Lifted & entity != Player)
                {
                    can_block = true;
                } else if (!moving_entity.Lifted & moving_entity != Player)
                {
                    can_block = true;
                }
                if (can_block)
                {
                    Debug.Log(string.Format(
                        "movement blocked by {0}: {1}",
                        entity.Name, moving_entity.Name));
                    return false;
                }
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

    Entity CreateImmigrant()
    {
        bool male = false;
        if (Random.value < 0.5)
        {
            male = true;
        }
        GameObject new_immigrant;
        if (male)
        {
            new_immigrant = Instantiate(
                MalePrefabs[Random.Range(0, MalePrefabs.Length)], 
                PrefabInstantiatePoint, Quaternion.identity);
        } else
        {
            new_immigrant = Instantiate(
                FemalePrefabs[Random.Range(0, FemalePrefabs.Length)],
                PrefabInstantiatePoint, Quaternion.identity);
        }
        Entity new_entity = new_immigrant.GetComponent<Entity>();
        new_entity.Name = GenerateName(male);
        string[] name_split = new_entity.Name.Split(
            new[] { " " }, System.StringSplitOptions.None);
        new_entity.ShortName = 
            char.ToUpper(name_split[2][0]) + name_split[2].Substring(1) + " " 
            + name_split[0];
        new_entity.Rotation = Random.Range(0, 3);
        new_immigrant.name = new_entity.Name;
        possession_prefix = new_entity.ShortName;
        return new_entity;
    }

    Entity CreatePossession()
    {
        GameObject new_possession;
        new_possession = Instantiate(
            PossessionPrefabs[Random.Range(0, PossessionPrefabs.Length)],
            PrefabInstantiatePoint, Quaternion.identity);
        Entity new_entity = new_possession.GetComponent<Entity>();
        new_entity.Name = string.Format(
            "{0}'s {1}", possession_prefix, new_entity.ShortName);
        new_possession.name = new_entity.Name;
        return new_entity;
    }
}
