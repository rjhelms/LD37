using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public int WorldX;
    public int WorldY;
    public int Rotation = 0;

    public string Name;
    public string ShortName;

    public bool Spawned = false;
    public bool Carried = false;
    public int CarriedX;
    public int CarriedY;
    public int CarriedW;
    public int CarriedH;
    public int w = 1;
    public int h = 1;
    public Sprite[] EntitySprites;

    private SpriteRenderer sprite_renderer;
    private GameController controller;
    private int spawn_y_offset;

	// Use this for initialization
	void Start () {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        controller = FindObjectOfType<GameController>();
        if (w <= h)
        {
            Rotation = 0;
            spawn_y_offset = h;
        }
        else
        {
            Rotation = 3;
            spawn_y_offset = w;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Spawned)
        {
            Vector3 target_position = CalculateTargetPosition();

            if (Carried)
            {
                if (controller.Player.WorldX < WorldX)
                {
                    target_position += new Vector3(
                        -(Constants.GRID_SIZE / 4),
                        Constants.LIFTED_OFFSET,
                        0.5f);
                }
                else if (controller.Player.WorldX > WorldX)
                {
                    target_position += new Vector3(
                        (Constants.GRID_SIZE / 4),
                        Constants.LIFTED_OFFSET,
                        0.5f);
                }
                else if (controller.Player.WorldY < WorldY)
                {
                    target_position += new Vector3(
                        0, Constants.LIFTED_OFFSET - Constants.GRID_SIZE / 4,
                        0);
                }
                else if (controller.Player.WorldY > WorldY)
                {
                    transform.position += new Vector3(
                        0, Constants.LIFTED_OFFSET + Constants.GRID_SIZE / 8,
                        0);
                }
            }
            target_position = Vector3.Lerp(
                transform.position, 
                target_position, 
                0.33f);
            transform.position = new Vector3(Mathf.Round(target_position.x), Mathf.Round(target_position.y), target_position.z);
            sprite_renderer.sprite = EntitySprites[Rotation];
        }
	}

    public bool OccupiesTile(int x, int y, bool check_spawn=true)
    {
        if (check_spawn & !Spawned) return false;
        int min_x = WorldX;
        int min_y = WorldY;
        int max_x;
        int max_y;
        if (Rotation == 0 | Rotation == 2)
        {
            max_x = WorldX + w;
            max_y = WorldY + h;
        }
        else
        {
            max_x = WorldX + h;
            max_y = WorldY + w;
        }
        if (Rotation == 1)
        {
            min_y -= w - 1;
            max_y -= w - 1;
        }
        if (Rotation == 2)
        {
            min_x -= w - 1;
            max_x -= w - 1;
            min_y -= h - 1;
            max_y -= h - 1;
        }
        if (Rotation == 3)
        {
            min_x -= h - 1;
            max_x -= h - 1;
        }
        if (x >= min_x & x < max_x & y >= min_y & y < max_y)
        {
            return true;
        }
        return false;
    }

    public void Spawn()
    {
        Spawned = true;
        if (w <= h)
        {
            Rotation = 0;
            spawn_y_offset = h;
        } else
        {
            Rotation = 3;
            spawn_y_offset = w;
        }
        transform.position = new Vector3(
            WorldX * Constants.GRID_SIZE,
            (WorldY - spawn_y_offset) * Constants.GRID_SIZE,
            -spawn_y_offset);
    }

    public void SetCarryPoint(int x, int y)
    {
        CarriedX = x;
        CarriedY = y;
        if (Rotation == 1 | Rotation == 3)
        {
            CarriedW = CarriedX - WorldX;
            CarriedH = CarriedY - WorldY;
        } else
        {
            CarriedW = CarriedY - WorldY;
            CarriedH = CarriedX - WorldX;
        }
        if (Rotation > 1)
        {
            CarriedW = -CarriedW;
            CarriedH = -CarriedH;
        }
    }

    public Vector2 CarriedOffset()
    {
        return new Vector2(CarriedX - WorldX, CarriedY - WorldY);
    }

    private Vector3 CalculateTargetPosition()
    {
        int target_world_x = WorldX;
        int target_world_y = WorldY;

        if (Rotation == 1)
        {
            target_world_y -= w - 1;
        }
        if (Rotation == 2)
        {
            target_world_x -= w - 1;
            target_world_y -= h - 1;
        }
        if (Rotation == 3)
        {
            target_world_x -= h-1;

        }
        return new Vector3(
            target_world_x * Constants.GRID_SIZE,
            target_world_y * Constants.GRID_SIZE,
            target_world_y);
    }
}
