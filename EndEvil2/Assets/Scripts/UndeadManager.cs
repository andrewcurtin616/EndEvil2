using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Storage for information to and from the undead
/// Every undead has a reference to this
/// </summary>

public class UndeadManager
{
    public static UndeadManager instance = null;

    public static UndeadManager getInstance()
    {
        if (instance == null)
            instance = new UndeadManager();

        return instance;
    }



}
