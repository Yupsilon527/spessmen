using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public int ID = 0;
    public int selection = 0;

    public Rigidbody2D rigidbody;
    public CapsuleCollider2D collider;

    public PlayerDigging digging;
    public PlayerMovement movement;
    public PlayerCarryItem hauler;

    public Player(GameObject parent, int id)
    {
        ID = id;
        collider = parent.GetComponent<CapsuleCollider2D>();
        rigidbody = parent.GetComponent<Rigidbody2D>();

        movement = parent.GetComponent<PlayerMovement>();
        movement.parent = this;
        digging = parent.GetComponent<PlayerDigging>();
        digging.parent = this;
        hauler = parent.GetComponent<PlayerCarryItem>();
        hauler.parent = this;
    }

    public Vector2 moveInput;
    public Vector2 fireInput;
    public Vector2 miscInput;
    public void HandleControls()
    {
        string command = "Player " + ID;

        /*moveInput.x = Input.GetAxis(command + " Horizontal");
        moveInput.y = Input.GetAxis(command + " Vertical");

        fireInput.x = Input.GetAxis(command + " Horizontal Fire");
        fireInput.y = Input.GetAxis(command + " Vertical Fire");

        miscInput.x = Input.GetAxis(command + " Change Weapon");*/
    }
    public void CleanControls()
    {
        moveInput = Vector2.zero;
        fireInput = Vector2.zero;
        miscInput = Vector2.zero;
    }
}