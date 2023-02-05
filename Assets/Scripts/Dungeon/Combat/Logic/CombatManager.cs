using Assets.Scripts.Tetris;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public GridManager tetris;
    public Combatable player;
    public Combatable enemy;

    private void Start()
    {
        if (tetris == null)
            tetris = GetComponentInChildren<GridManager>();

        if (player == null)
            player = GetComponentsInChildren<Combatable>()[0];

        if (enemy == null)
            enemy = GetComponentsInChildren<Combatable>()[1];

        tetris.LineCleared.AddListener(() => player.InflictDamage(enemy));
    }
}
