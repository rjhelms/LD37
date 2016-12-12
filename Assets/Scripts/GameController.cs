using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    STARTING,
    RUNNING,
    LOSING
}

public class GameController : MonoBehaviour
{
    #region Public fields
    public GameState State;
    public Camera MainCamera;
    public AudioSource SoundPlayer;

    public TextAsset NamesMale;
    public TextAsset NamesFemale;
    public TextAsset NamesSurnames;

    public TextAsset RelationsMale;
    public TextAsset RelationsFemale;

    public Text NameText;
    public Text CarryingText;
    public Text ScoreText;
    public GameObject GetReadyPanel;

    public Color NameTextNormal;
    public Color NameTextWarning;
    public Color NameTextUrgent;
    public Entity Player;

    public List<Entity> WorldEntities;

    private Entity held_entity = null;

    public GameObject[] MalePrefabs;
    public GameObject[] FemalePrefabs;
    public GameObject[] PossessionPrefabs;
    public GameObject SpawnWarningBoxPrefab;

    public Vector3 PrefabInstantiatePoint;
    public float TimeBetweenSpawns = 1f;
    public float TimeBetweenSpawnsInitial = 5f;
    public float TimeBetweenSpawnsMinimum = 0.1f;
    public float TimeAccelerationFactor = 1.1f;
    public float TimeGetReady = 1.0f;
    public int MaxEntities = 10;
    public int MaxEntitiesStep = 2;
    public AudioClip WarningSound;
    public AudioClip SpawnSound;
    public AudioClip LiftSound;
    public AudioClip DropSound;
    public AudioClip BlockedSound;
    public AudioClip MoveSound;
    public AudioClip GameOverSound;
    #endregion

    #region Private fields
    private Entity next_entity;
    private int possessions_to_spawn;
    private string possession_prefix;
    private float time_to_next_spawn;
    private float get_ready_time;
    private int current_screen_w;
    private int current_screen_h;
    private int warning_sound_played;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        InitScreen();
        WorldEntities.Add(Player);
        get_ready_time = Time.time + TimeGetReady;
        State = GameState.STARTING;
    }

    // Update is called once per frame
    void Update()
    {
        if (current_screen_w != Screen.width | current_screen_h != Screen.height)
        {
            InitScreen();
        }
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
        if (State == GameState.STARTING)
        {
            if (Time.time >= get_ready_time)
            {
                GetReadyPanel.SetActive(false);
                next_entity = CreateImmigrant();
                Instantiate(
                    SpawnWarningBoxPrefab, new Vector3(
                        next_entity.WorldX * Constants.GRID_SIZE,
                        next_entity.WorldY * Constants.GRID_SIZE,
                        10), Quaternion.identity);
                possessions_to_spawn = Random.Range(0, 4);
                NameText.text = next_entity.Name;
                time_to_next_spawn = Time.time + TimeBetweenSpawnsInitial;
                warning_sound_played = 0;
                State = GameState.RUNNING;
            }
        } else if (State == GameState.RUNNING)
        { 
        UpdateSpawn();

            if (Input.anyKeyDown)
            {
                int new_player_x = Player.WorldX;
                int new_player_y = Player.WorldY;
                int new_player_rotation = Player.Rotation;
                int movement_x, movement_y, movement_rotation;
                ProcessMovementInput(
                    out movement_x, out movement_y, out movement_rotation);

                if (movement_x != 0 | movement_y != 0 | movement_rotation != 0)
                {
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

                    if (CanMove(new_player_x, new_player_y, new_player_rotation,
                        Player))
                    {
                        TryMove(movement_x, movement_y, movement_rotation);
                    }
                    else
                    {
                        SoundPlayer.PlayOneShot(BlockedSound);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TryLift();
                }
            }
        } else if (State == GameState.LOSING)
        {
            if (!SoundPlayer.isPlaying)
            {
                SceneManager.LoadScene("gameover");
            }
        }
    }
    #endregion

    void InitScreen()
    {
        current_screen_h = Screen.height;
        current_screen_w = Screen.width;
        float multiplier = current_screen_h / 480.0f;
        if (multiplier >= 1)
        {
            multiplier = Mathf.Floor(multiplier);
        }
        MainCamera.orthographicSize = (current_screen_h / multiplier) / 2;
        Debug.Log(current_screen_h);
        Debug.Log(multiplier);
    }

    #region Private Methods

    private void ProcessMovementInput(out int movement_x, out int movement_y,
                                      out int movement_rotation)
    {
        movement_x = 0;
        movement_y = 0;
        movement_rotation = 0;
        if (ScoreManager.Instance.RelativeMovement)
        {
            if (Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.Keypad8))
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
            if (Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.Keypad2))
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
            if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.Keypad4))
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
            if (Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.Keypad6))
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
            if (Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.Keypad8))
            {
                movement_y += 1;
            }
            if (Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.Keypad2))
            {
                movement_y -= 1;
            }
            if (Input.GetKeyDown(KeyCode.A) | Input.GetKeyDown(KeyCode.Keypad4))
            {
                movement_x -= 1;
            }
            if (Input.GetKeyDown(KeyCode.D) | Input.GetKeyDown(KeyCode.Keypad6))
            {
                movement_x += 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetKeyDown(KeyCode.Keypad7))
        {
            movement_rotation -= 1;
        }
        if (Input.GetKeyDown(KeyCode.E) | Input.GetKeyDown(KeyCode.Keypad9))
        {
            movement_rotation += 1;
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScoreManager.Instance.RelativeMovement = !ScoreManager.Instance.RelativeMovement;
        }
    }

    private void TryLift()
    {
        if (held_entity)    // drop a held entity
        {
            held_entity.Carried = false;
            SoundPlayer.PlayOneShot(DropSound);
            Debug.Log(string.Format("Dropped {0}", held_entity.Name));
            held_entity = null;
            CarryingText.text = "Nothing";
        }
        else
        {
            int held_x;
            int held_y;
            switch (Player.Rotation)
            {
                case 0:
                    held_x = Player.WorldX;
                    held_y = Player.WorldY - 1;
                    break;
                case 1:
                    held_x = Player.WorldX - 1;
                    held_y = Player.WorldY;
                    break;
                case 2:
                    held_x = Player.WorldX;
                    held_y = Player.WorldY + 1;
                    break;
                case 3:
                    held_x = Player.WorldX + 1;
                    held_y = Player.WorldY;
                    break;
                default:
                    held_x = 0;
                    held_y = 0;
                    break;
            }
            held_entity = GetEntityAtLocation(held_x, held_y);
            if (held_entity)
            {
                Debug.Log(string.Format("Lifted {0}", held_entity.Name));
                held_entity.Carried = true;
                held_entity.SetCarryPoint(held_x, held_y);
                CarryingText.text = held_entity.Name;
                SoundPlayer.PlayOneShot(LiftSound);
            }
            else
            {
                Debug.Log("Nothing to lift.");
            }
        }
    }

    private void TryMove(int movement_x, int movement_y, int movement_rotation)
    {
        int new_player_x = Player.WorldX + movement_x;
        int new_player_y = Player.WorldY + movement_y;
        int new_player_rotation = Player.Rotation + movement_rotation;
        if (new_player_rotation < 0)
        {
            new_player_rotation += 4;
        } else if (new_player_rotation > 3)
        {
            new_player_rotation -= 4;
        }
        bool valid_held_entity_move = true;
        if (held_entity)
        {
            valid_held_entity_move = false;
            int new_carried_position_x = held_entity.WorldX;
            int new_carried_position_y = held_entity.WorldY;
            int new_carried_rotation = held_entity.Rotation;
            int new_carried_carry_x = held_entity.CarriedX;
            int new_carried_carry_y = held_entity.CarriedY;
            if (movement_x != 0 || movement_y != 0)
            {
                new_carried_position_x = held_entity.WorldX + movement_x;
                new_carried_position_y = held_entity.WorldY + movement_y;
                new_carried_rotation = held_entity.Rotation;
                new_carried_carry_x = held_entity.CarriedX + movement_x;
                new_carried_carry_y = held_entity.CarriedY + movement_y;
            }
            if (movement_rotation != 0)
            {
                Vector3 new_position_vector;
                Vector2 new_carry_vector;
                PivotEntity(
                    Player, held_entity, movement_rotation,
                    out new_position_vector, out new_carry_vector);
                new_carried_position_x = (int)new_position_vector.x;
                new_carried_position_y = (int)new_position_vector.y;
                new_carried_rotation = (int)new_position_vector.z;
                new_carried_carry_x = (int)new_carry_vector.x;
                new_carried_carry_y = (int)new_carry_vector.y;
            }
            valid_held_entity_move = CanMove(
                new_carried_position_x, new_carried_position_y,
                new_carried_rotation, held_entity);

            if (valid_held_entity_move)
            {
                held_entity.WorldX = new_carried_position_x;
                held_entity.WorldY = new_carried_position_y;
                held_entity.Rotation = new_carried_rotation;
                held_entity.SetCarryPoint(
                    new_carried_carry_x, new_carried_carry_y);
            }
        }
        if (valid_held_entity_move)
        {
            Player.WorldX = new_player_x;
            Player.WorldY = new_player_y;
            Player.Rotation = new_player_rotation;
            SoundPlayer.PlayOneShot(MoveSound);
        } else
        {
            SoundPlayer.PlayOneShot(BlockedSound);
        }
    }

    private void UpdateSpawn()
    {
        float time_remaining = time_to_next_spawn - Time.time;
        if (time_remaining <= 1)
        {
            if (warning_sound_played < 2)
            {
                SoundPlayer.PlayOneShot(WarningSound);
                warning_sound_played++;
            }
            NameText.color = NameTextUrgent;
        }
        else if (time_remaining <= 2)
        {
            if (warning_sound_played < 1)
            {
                SoundPlayer.PlayOneShot(WarningSound);
                warning_sound_played++;
            }
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
                if (WorldEntities.Count > MaxEntities)
                {
                    DeleteRandomEntity();
                }
                WorldEntities.Add(next_entity);
                foreach (GameObject spawnbox
                    in GameObject.FindGameObjectsWithTag("WarningBox"))
                {
                    Destroy(spawnbox);
                }
                if (possessions_to_spawn == 0)
                {
                    ScoreManager.Instance.Score += 1000;
                    next_entity = CreateImmigrant();
                    possessions_to_spawn = Random.Range(0, 4);
                }
                else
                {
                    ScoreManager.Instance.Score += 100;
                    next_entity = CreatePossession();
                    possessions_to_spawn--;
                }
                NameText.text = next_entity.Name;
                for (int x = 0; x < Constants.MAX_X; x++)
                {
                    for (int y=0; y< Constants.MAX_Y; y++)
                    {
                        if (next_entity.OccupiesTile(x, y, false))
                        {
                            Instantiate(
                                SpawnWarningBoxPrefab, new Vector3(
                                    x * Constants.GRID_SIZE,
                                    y * Constants.GRID_SIZE,
                                    10), Quaternion.identity);
                        }
                    }
                }
                warning_sound_played = 0;
                SoundPlayer.PlayOneShot(SpawnSound);
            }
            else
            {
                State = GameState.LOSING;
                SoundPlayer.PlayOneShot(GameOverSound);
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

    private string GenerateName(bool? male=null)
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

    private bool CanMove(int check_x, int check_y, int rotation, Entity moving_entity)
    {
        // determine ranges of tiles occupied by moving_entity
        int check_x_min = check_x;
        int check_x_max = check_x;
        int check_y_min = check_y;
        int check_y_max = check_y;

        if (rotation == 0 | rotation == 2)
        {
            check_x_max += moving_entity.w;
            check_y_max += moving_entity.h;
        }
        else
        {
            check_x_max += moving_entity.h;
            check_y_max += moving_entity.w;
        }
        if (rotation == 1)
        {
            check_y_min -= moving_entity.w - 1;
            check_y_max -= moving_entity.w - 1;
        }
        if (rotation == 2)
        {
            check_x_min -= moving_entity.w -1;
            check_x_max -= moving_entity.w -1;
            check_y_min -= moving_entity.h - 1;
            check_y_max -= moving_entity.h - 1;
        }
        if (rotation == 3)
        {
            check_x_min -= moving_entity.h-1;
            check_x_max -= moving_entity.h-1;

        }
        // for each of these tiles...
        for (int x = check_x_min; x < check_x_max; x++)
        {
            for (int y = check_y_min; y < check_y_max; y++)
            { 
                // if it's out of bound, abort
                if (x < 0 | y < 0 | x > Constants.MAX_X | y > Constants.MAX_Y)
                {
                    Debug.Log("movement blocked by world bounds: " + moving_entity.Name);
                    return false;
                }

                // otherwise, start checking other entities
                foreach (Entity entity in WorldEntities)
                {
                    // check if it's an entity we can collide with
                    bool can_block = false;
                    if (moving_entity == entity)
                    {
                        // if we other entity is us, don't even bother checking
                        // farther
                        can_block = false;
                    } else if (moving_entity == Player & !entity.Carried)
                    {
                        // if we're the player and it's something we're not
                        // carrying
                        can_block = true;
                    }
                    else if (moving_entity.Carried & entity != Player)
                    {
                        // or if we're being carried, and it's not the player
                        can_block = true;
                    }
                    else if (!moving_entity.Carried & moving_entity != Player)
                    {
                        // or if we're NOT being carried and we're not the
                        // player
                        can_block = true;
                    }
                    if (can_block & entity.OccupiesTile(x, y))
                    {
                        // if it's a blockable pair, and the other entity
                        // occupies the current tile, we're blocked
                        Debug.Log(string.Format(
                            "movement blocked by {0}: {1}",
                            entity.Name, moving_entity.Name));
                        return false;
                    }
                }
            } // ... then check the next tile
        }
        return true; // nothing left to check - it's good!
    }

    private Entity GetEntityAtLocation(int x, int y)
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

    private void PivotEntity(Entity pivot_entity, Entity moving_entity, 
                                int rotation, out Vector3 new_entity_position,
                                out Vector2 new_carry_position)
    {
        // calculate rotation for the two items
        int new_moving_rotation = moving_entity.Rotation + rotation;
        int new_pivot_rotation = pivot_entity.Rotation + rotation;

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

        // figure out where we'll be holding it by
        int new_held_x = pivot_entity.WorldX;
        int new_held_y = pivot_entity.WorldY;

        switch (new_pivot_rotation)
        {
            case 0:
                new_held_y -= 1;
                break;
            case 1:
                new_held_x -= 1;
                break;
            case 2:
                new_held_y += 1;
                break;
            case 3:
                new_held_x += 1;
                break;
        }

        int new_x = 0;
        int new_y = 0;

        if (rotation < 0)
        {
            new_x = -(moving_entity.WorldY - pivot_entity.WorldY) + pivot_entity.WorldX;
            new_y = (moving_entity.WorldX - pivot_entity.WorldX) + pivot_entity.WorldY;
        }
        else
        {
            new_x = (moving_entity.WorldY - pivot_entity.WorldY) + pivot_entity.WorldX;
            new_y = -(moving_entity.WorldX - pivot_entity.WorldX) + pivot_entity.WorldY;
        }

        new_carry_position = new Vector2(new_held_x, new_held_y);
        new_entity_position = new Vector3(
            new_x, new_y, new_moving_rotation);
    }

    private Entity CreateImmigrant()
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

    private Entity CreatePossession()
    {
        GameObject new_possession;
        new_possession = Instantiate(
            PossessionPrefabs[Random.Range(0, PossessionPrefabs.Length)],
            PrefabInstantiatePoint, Quaternion.identity);
        Entity new_entity = new_possession.GetComponent<Entity>();
        new_possession.transform.position -= new Vector3(0, (new_entity.h - 1) * Constants.GRID_SIZE, 0);
        new_entity.Name = string.Format(
            "{0}'s {1}", possession_prefix, new_entity.ShortName);
        new_possession.name = new_entity.Name;
        return new_entity;
    }

    private void DeleteRandomEntity()
    {
        Entity randomEntity = Player;
        // can't delete player, or what they're carrying
        while (randomEntity == Player | randomEntity.Carried)
        {
            randomEntity = WorldEntities[Random.Range(0, WorldEntities.Count)];
            Debug.Log("Trying to delete " + randomEntity.Name);
        }
        WorldEntities.Remove(randomEntity);
        Destroy(randomEntity.gameObject);
        MaxEntities += MaxEntitiesStep;
    }
    #endregion
}
