using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTracker : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Transform player;
    [SerializeField] private Sprite readyBullet;
    [SerializeField] private Sprite usedBullet;
    [SerializeField] private Sprite readyShell;
    [SerializeField] private Sprite usedShell;
    [Range(1f, 2f)] [SerializeField] private float verticalOffset = 1f;  // How high the ammo tracker should appear above the player's head.
    [SerializeField] private float shootForce = 700f;  // Amount of force exerted when the player shoots.
    [SerializeField] private float shotgunForceMultiplier = 2f;
    [SerializeField] private int pistolMaxAmmo = 4;
    [SerializeField] private int shotgunMaxAmmo = 2;


    private bool usingPistol;
    public bool shotgunEnabled;
    public Animator animator;

    bool isShooting = false;

    // These values update on shoot or on reload.
    int remainingPistolAmmo;
    int remainingShotgunAmmo;

    void Start()
    {
        // Player starts with a pistol
        usingPistol = true;
        remainingPistolAmmo = pistolMaxAmmo;
        remainingShotgunAmmo = shotgunMaxAmmo;

        // Make the shotgun shells invisible
        for (int i = pistolMaxAmmo; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void Update()
    {
        // Move ammo tracker with the player
        transform.position = (Vector2) player.position + new Vector2(0, verticalOffset);

        int currentAmmo = usingPistol ? remainingPistolAmmo : remainingShotgunAmmo;

        if (Input.GetButtonDown("Fire1") && GameManager.InputEnabled())
        {
            isShooting = currentAmmo > 0;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D playerToMouse = new Ray2D(player.position, mousePosition - (Vector2) player.position);

            if (controller.IsGrounded() && playerToMouse.direction.y > -0.2)
            {
                // Do nothing
                return;
            }
            else
            {
                currentAmmo--;

                if (currentAmmo < 0)
                {
                    currentAmmo = 0;
                }

                // Save how much ammo is left in the current weapon, and change bullet sprites to their used version
                if (usingPistol)
                {
                    remainingPistolAmmo = currentAmmo;
                    transform.GetChild(currentAmmo).GetComponent<SpriteRenderer>().sprite = usedBullet;
                }
                else
                {
                    remainingShotgunAmmo = currentAmmo;
                    transform.GetChild(currentAmmo + pistolMaxAmmo).GetComponent<SpriteRenderer>().sprite = usedShell;
                }
            }
        }

        if (Input.GetButtonDown("Fire2") && GameManager.InputEnabled() && shotgunEnabled)
        {
            SwitchWeapons();
        }
    }

    public void OnLanding()
    {
        Reload();
    }

    private void FixedUpdate()
    {
        if (isShooting)
        {
            float forceMultipler = (usingPistol) ? 1 : shotgunForceMultiplier;
            controller.Shoot(shootForce * forceMultipler);
            isShooting = false;
        }
    }

    private void Reload()
    {
        remainingPistolAmmo = pistolMaxAmmo;
        remainingShotgunAmmo = shotgunMaxAmmo;

        for (int i = 0; i < pistolMaxAmmo; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = readyBullet;
        }

        for (int i = pistolMaxAmmo; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = readyShell;
        }
    }

    private void SwitchWeapons()
    {
        if (usingPistol)
        {
            // Make the shotgun shells visible
            for (int i = pistolMaxAmmo; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            }

            // Make the pistol bullets invisible
            for (int i = 0; i < pistolMaxAmmo; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }
            animator.SetBool("ShotgunEnabled", true);
        }
        else
        {
            animator.SetBool("ShotgunEnabled", false);
            // Make the shotgun shells invisible
            for (int i = pistolMaxAmmo; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }

            // Make the pistol bullets visible
            for (int i = 0; i < pistolMaxAmmo; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        // Switch weapons
        usingPistol = !usingPistol;
    }

    public void MakeInvisible()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void MakeVisible()
    {
        if (!usingPistol)
        {
            // Make the shotgun shells visible
            for (int i = pistolMaxAmmo; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            }

            // Make the pistol bullets invisible
            for (int i = 0; i < pistolMaxAmmo; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            // Make the shotgun shells invisible
            for (int i = pistolMaxAmmo; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }

            // Make the pistol bullets visible
            for (int i = 0; i < pistolMaxAmmo; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    public bool UsingPistol()
    {
        return usingPistol;
    }
}
