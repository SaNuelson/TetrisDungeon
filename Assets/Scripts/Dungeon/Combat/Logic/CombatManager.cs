using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{

    private static CombatManager instance = null;
    public static CombatManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("CombatManager instance accessed while outside a fight");
                return null;
            }
            return instance;
        }
    }

    public static void Initialize()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        GameObject combatManGo = new GameObject("CombatManager");
        CombatManager combatManager = combatManGo.AddComponent<CombatManager>();
        instance = combatManager;
    }

    [SerializeField] private Combatable[] PlayerTeam;
    [SerializeField] private Combatable[] EnemyTeam;


}
