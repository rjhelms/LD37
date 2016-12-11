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
    public int w = 1;
    public int h = 1;

    public Sprite[] EntitySprites;

    private SpriteRenderer sprite_renderer;
    private GameController controller;
	// Use this for initialization
	void Start () {
        sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
        controller = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Spawned)
        {
            Vector3 target_position = new Vector3(
                WorldX * Constants.GRID_SIZE, 
                WorldY * Constants.GRID_SIZE, 
                WorldY);

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

    public bool OccupiesTile(int x, int y)
    {
        if (!Spawned) return false;
        int max_x;
        int max_y;
        if (Rotation == 0 | Rotation == 2)
        {
            max_x = WorldX + w;
            max_y = WorldY + h;
        } else {
            max_x = WorldX + h;
            max_y = WorldY + w;
        }
        if (x >= WorldX & x < max_x & y >= WorldY & y < max_y)
        {
            return true;
        }
        return false;
    }

    public void Spawn()
    {
        Spawned = true;
        int offset;
        if (w <= h)
        {
            Rotation = 0;
            offset = h;
        } else
        {
            Rotation = 1;
            offset = w;
        }
        transform.position = new Vector3(
            WorldX * Constants.GRID_SIZE,
            (WorldY - offset) * Constants.GRID_SIZE,
            -offset);
    }
}
