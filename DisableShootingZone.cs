using System.Collections.Generic;
using UnityEngine;

public class DisableShootingZone : MonoBehaviour
{
    // List to keep track of tanks inside the collider
    private List<TankMovement> tanksInZone = new List<TankMovement>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the collider is a tank
        TankMovement tank = other.GetComponent<TankMovement>();
        if (tank != null) {
            // Add the tank to the list if it's not already in it
            if (!tanksInZone.Contains(tank)) {
                tanksInZone.Add(tank);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the tank from the list when it exits the collider
        TankMovement tank = other.GetComponent<TankMovement>();
        if (tank != null) {
            tanksInZone.Remove(tank);
        }
    }

    private void Update()
    {
        // Ensure canShoot is set to false for all tanks in the zone
        foreach (TankMovement tank in tanksInZone) {
            tank.canShoot = false;
        }
    }
}