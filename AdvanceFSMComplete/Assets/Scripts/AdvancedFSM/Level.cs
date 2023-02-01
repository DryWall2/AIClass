using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int level;
    [Header("Class Randomness")]
    public int fastMax = 5;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
