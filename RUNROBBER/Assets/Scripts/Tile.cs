using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Possible Hound Tiles")]
    public List<GameObject> tiles = new List<GameObject>();

    [Space]
    [Header("Possible Hare Tiles")]
    public List<GameObject> tiles2 = new List<GameObject>();
}
