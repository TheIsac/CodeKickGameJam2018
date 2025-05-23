﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _20180713._Scripts;

public class ShipManager : MonoBehaviour
{
    public List<Base> Ships = new List<Base>();

    public Base GetClosestShip(Vector3 position)
    {
        Base closest = null;
        var closestDistance = -1f;
        foreach (var ship in Ships)
        {
            var distance = ship.GetDistanceToClosestBlock(position);
            if (distance < closestDistance || closestDistance < 0)
            {
                closestDistance = distance;
                closest = ship;
            }
        }

        return closest;
    }

    public bool IsCloseEnoughToSomeBase(Vector3 position, float minDistance)
    {
        return Ships.Any(ship => ship.IsCloseEnough(position, minDistance));
    }

    public Base GetClosestShipExcept(Vector3 position, params Base[] excludeShips)
    {
        Base closest = null;
        var closestDistance = -1f;
        foreach (var ship in Ships)
        {
            if (excludeShips.Contains(ship)) continue;

            var distance = ship.GetDistanceToClosestBlock(position);
            if (distance < closestDistance || closestDistance < 0)
            {
                closestDistance = distance;
                closest = ship;
            }
        }

        return closest;
    }
}