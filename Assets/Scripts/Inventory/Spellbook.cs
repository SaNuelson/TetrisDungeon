using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spellbook : MonoBehaviour
{
    public TetrisManager Tetris;

    public TileFactoryPreset TileFactoryPreset;
    private TileFactory TileFactory;

    public List<Spell> Spells;
    public Spell CurrentSpell;

    public Combatable Combatable;

    public UnityEvent SpellPrepared = new UnityEvent();
    public UnityEvent SpellCast = new UnityEvent();

    void Awake()
    {
        if (TileFactoryPreset == null)
            TileFactory = TileFactoryPreset.GetDefault();

        if (TileFactory == null)
            TileFactory = TileFactoryPreset.GetFactory();

        if (Combatable == null)
            Combatable = transform.parent.GetComponent<Combatable>();

        if (Tetris == null)
        {
            var candidates = FindObjectsOfType<TetrisManager>();
            if (candidates.Length != 1)
            {
                Debug.LogError("Could not find a single unique instance of Tetris");
                return;
            }
            Tetris = candidates[0];
        }
    }


    void Update()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                PrepareSpell(i);
            }
        }
    }

    private void PrepareSpell(int index)
    {
        var spell = Spells[index];

        if (Combatable.CurrentMana < spell.ManaCost)
        {
            Debug.Log("CastSpell -- not enough mana");
            return;
        }

        Combatable.CurrentMana -= spell.ManaCost;

        var minoObject = new GameObject("Spell");
        minoObject.SetActive(false);

        var minoScript = minoObject.AddComponent<MinoScript>();
        minoScript.ConstructFromPreset(spell.MinoPreset, TileFactory);

        minoScript.BeforeDestroyed.AddListener(CastSpell);
        Tetris.EnqueueMino(minoScript);
    }

    private void CastSpell()
    {
        if(CurrentSpell == null)
        {
            Debug.LogError("Spellbook.CastSpell -- no active spell.");
            return;
        }
    }

}

[System.Serializable]
public class Spell
{
    public MinoShapePreset MinoPreset;
    public int ManaCost;
}