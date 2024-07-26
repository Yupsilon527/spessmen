using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerMob : CreatureMob
{
    #region Digging
    [Header("Digging")]
    public float ThrowStrength = .33f;
    public float DigRange = 3;
    public float DigRadius = 1;
    public float DigCooldown = 1;
    public float DigCooldownUpgrades = .1f;
    public float MoveSpeedMultiplier = 1f;
    public int DigStrengthUpgrades = 5;

    protected int DiggingUpgrades = 0;
    public void UpgradeDigging()
    {
        DiggingUpgrades++;
    }
    public float GetDigTime()
    {
        return DigCooldown + DigCooldownUpgrades * DiggingUpgrades;
    }
    public int GetDigDamage()
    {
        return DiggingUpgrades >= DigStrengthUpgrades ? 2 : 1;
    }
    #endregion

    #region Components
    [Header("Components")]

    public CapsuleCollider2D collider;

    public PlayerDigging digging;
    public PlayerInput input;
    public PlayerFarmingComponent farmer;
    public DoorAccesser accesser;
    public PlayerItemHolding hauler;
    public InventoryComponent backpack;
    public BuilderComponent builder;
    public PlayerMenu menu;
    public ResourceController resources;
   protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<CapsuleCollider2D>();

        input = GetComponent<PlayerInput>();
        digging = GetComponent<PlayerDigging>();
        accesser = GetComponentInParent<DoorAccesser>();
        backpack = GetComponent<InventoryComponent>();
        hauler = GetComponent<PlayerItemHolding>();
        builder = GetComponent<BuilderComponent>();
        menu = GetComponent<PlayerMenu>();
        farmer = GetComponentInParent<PlayerFarmingComponent>();
        resources = GetComponent<ResourceController>();

    }
    #endregion


    protected override void Start()
    {
        base.Start();
        InterfaceController.main.TieToPlayer(this);
        if (SidewaysCamera.active!=null)
            SidewaysCamera.active.FollowMob(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!suspended)
            HandleMovement();
    }
    void HandleMovement()
    {
        if (CanMove)
            movement.Move(input.moveInput.x);
        movement.HandleFall();
    }

    #region Enter Exit Building
    public CompartimentComponent indoor;
    public override bool IsInside()
    {
        return indoor!=null;
    }
    public virtual void OnEnterBuilding()
    {
        gameObject.SetActive(false);
    }
    public void ExitBuilding()
    {
        indoor.UnloadMob(this);
    }
    public virtual void OnExitBuilding()
    {
        gameObject.SetActive(true);
    }
    #endregion
    public override void HandleShockwave(Vector2 center, Vector2 dir, float force_delta, float force, float damage)
    {
        base.HandleShockwave(center, dir, force_delta, force, damage);
        health.Health.ChargeValue(force_delta * damage);
    }
}
