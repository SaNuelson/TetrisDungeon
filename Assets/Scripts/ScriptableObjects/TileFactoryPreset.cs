using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TileFactoryPreset")]
public class TileFactoryPreset : ScriptableObject
{
    public string Name;
    public bool IsDefault = false;
    public TileType Type;
    public Sprite[] Sprites;


    private TileFactory factory = null;
    public TileFactory GetFactory()
    {
        if (factory == null)
        {
            factory = new TileFactory(Sprites, Type);
        }
        return factory;
    }

    private static TileFactoryPreset customDefaultFactoryPreset = null;
    private static TileFactory defaultFactory = null;
    public static TileFactory GetDefault()
    {
        if (customDefaultFactoryPreset != null)
            return customDefaultFactoryPreset.GetFactory();

        if (defaultFactory == null)
        {
            defaultFactory = TileFactory.GetDefault();
        }
        return defaultFactory;
    }

    public static UnityEvent DefaultChanged = new UnityEvent();
    private static void ChangeDefault(TileFactoryPreset newDefaultPreset)
    {
        if (customDefaultFactoryPreset != null)
        {
            Debug.LogWarning($"Overwriting old custom default factory ({customDefaultFactoryPreset.name}) with ({newDefaultPreset.name}).");
            customDefaultFactoryPreset.IsDefault = false;
        }
        customDefaultFactoryPreset = newDefaultPreset;

        DefaultChanged.Invoke();
    }

    private void OnValidate()
    {
        factory = new TileFactory(Sprites, Type);

        // Update default factory
        if (IsDefault)
        {
            if (customDefaultFactoryPreset != this)
            {
                ChangeDefault(this);
            }
        }
    }
}