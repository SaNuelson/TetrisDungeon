﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Dungeon.Combat.Logic
{
    public enum ActionTarget
    {
        Self,
        FriendlyTarget, // single friendly
        FriendlyOthers, // all friendly but self
        FriendlyGroup, // all friendly incl. self
        EnemyTarget, // single enemy
        EnemyGroup // all enemy
    };

    public class Action : ScriptableObject
    {
        public string Name;
        public string Description;

        public Cost Cost;
        public Requirement Requirement;
        public Effect[] Effects;

        public bool IsPossible(Combatable actor)
        {
            if (!Cost.IsAvailable(actor))
                return false;

            if (!Requirement.IsMet(actor))
                return false;

            return true;
        }

        public void Act(Combatable actor)
        {
            if (!IsPossible(actor))
            {
                return;
            }

            Cost.Spend(actor);

            foreach(var eff in Effects)
            {
                //eff.Apply();
            }
        }
    }

    /// <summary>
    /// Resources required and spent during completing an action.
    /// </summary>
    [System.Serializable]
    public class Cost
    {
        public int Health;
        public int Mana;

        public bool IsAvailable(Combatable actor)
        {
            if (actor.CurrentHealth < Health)
                return false;

            if (actor.CurrentMana < Mana)
                return false;

            return true;
        }

        public void Spend(Combatable actor)
        {
            actor.CurrentHealth -= Health;
            actor.CurrentMana -= Mana;
        }
    }

    /// <summary>
    /// Resources / states required but not spent during completing an action.
    /// </summary>
    [System.Serializable]
    public class Requirement
    {
        public bool IsMet(Combatable actor)
        {
            return true;
        }
    }

    /// <summary>
    /// Effects applied to some group upon completing an action.
    /// </summary>
    [System.Serializable]
    public class Effect
    {
        public ActionTarget Target;


        public void Apply(Combatable actor)
        {
        }

        private void AffectOne(Combatable target)
        {

        }

        private void AffectGroup(Combatable[] targets)
        {
            foreach(var target in targets)
                AffectOne(target);
        }
    }
}
