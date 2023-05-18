using UnityEngine;
using System.Collections.Generic;

public class Player
{

    public int ID = 0;
    public int selection = 0;

    public Rigidbody2D rigidbody;
    public CapsuleCollider2D collider;

    public PlayerDigging digging;
    public PlayerMob movement;
    public PlayerFarmingComponent farmer;
    public DoorAccesser accesser;
    public PlayerItemHolding hauler;
    public InventoryComponent backpack;
    public BuilderComponent builder;
    public PlayerMenu menu;
    public PlayerStats stats;
    public HealthController health;
    public ResourceController resources;

    public Player(GameObject parent, int id)
    {
        ID = id;
        collider = parent.GetComponent<CapsuleCollider2D>();
        rigidbody = parent.GetComponent<Rigidbody2D>();

        movement = parent.GetComponent<PlayerMob>();
        movement.parent = this;
        digging = parent.GetComponent<PlayerDigging>();
        digging.parent = this;
        accesser = parent.GetComponentInParent<DoorAccesser>();
        accesser.parent = this;
        backpack = parent.GetComponent<InventoryComponent>();
        hauler = parent.GetComponent<PlayerItemHolding>();
        hauler.parent = this;
        builder = parent.GetComponent<BuilderComponent>();
        builder.parent = this;
        menu = parent.GetComponent<PlayerMenu>();
        menu.parent = this;
        farmer = parent.GetComponentInParent<PlayerFarmingComponent>();
        farmer.parent = this;
        health = parent.GetComponent<HealthController>();
        resources = parent.GetComponent<ResourceController>();
        stats = parent.GetComponent<PlayerStats>();

        InterfaceController.main.TieToPlayer(this);
    }

    public Vector2 moveInput;
    public Vector2 fireInput;
    public Vector2 miscInput;
    public void HandleControls()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        /*fireInput.x = Input.GetAxis(command + " Horizontal Fire");
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