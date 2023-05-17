using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int startingHealth = 100;

    private int CurrentHealth;
    private Vector3 StartPosition;
    public void GetShot(int damage, ShootingAgent shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, ShootingAgent shooter)
    {
        CurrentHealth -= damage;

        if(CurrentHealth <= 0)
        {
            Die(shooter);
        }
    }

    private void Die(ShootingAgent shooter)
    {
        Debug.Log("I Died!!");
        shooter.RegisterKill();
        Respawn();
    }

    private void Respawn()
    {
        CurrentHealth = startingHealth;
        transform.position = StartPosition;
    }

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        StartPosition = transform.position;
        CurrentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    private void OnMouseDown(ShootingAgent shooter)
    {
        GetShot(startingHealth, shooter);
    }
}
