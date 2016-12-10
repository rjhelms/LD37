﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public int WorldX;
    public int WorldY;
    public int Rotation = 0;

    public string Name;

    public bool Lifted = false;

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
        transform.position = new Vector3(
            WorldX * Constants.GRID_SIZE, WorldY * Constants.GRID_SIZE, WorldY);
        if (Lifted)
        {
            if (controller.Player.WorldX < WorldX)
            {
                transform.position = new Vector3(
                    transform.position.x - (Constants.GRID_SIZE / 4),
                    transform.position.y + Constants.LIFTED_OFFSET,
                    transform.position.z);
            }
            else if (controller.Player.WorldX > WorldX)
            {
                transform.position = new Vector3(
                    transform.position.x + (Constants.GRID_SIZE / 4),
                    transform.position.y + Constants.LIFTED_OFFSET,
                    transform.position.z);
            }
            else if (controller.Player.WorldY < WorldY)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + Constants.LIFTED_OFFSET - Constants.GRID_SIZE / 4,
                    transform.position.z);
            } else if (controller.Player.WorldY > WorldY)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + Constants.LIFTED_OFFSET + Constants.GRID_SIZE / 4,
                    transform.position.z);
            }
        }
        sprite_renderer.sprite = EntitySprites[Rotation];
	}

    public bool OccupiesTile(int x, int y)
    {
        if (Lifted) return false;
        if (x == WorldX & y == WorldY)
        {
            return true;
        }
        return false;
    }
}
