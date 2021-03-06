﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ship
{
    namespace YT1300
    {
        public class HanSolo : YT1300
        {
            public HanSolo() : base()
            {
                PilotName = "Han Solo";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/han-solo.png";
                PilotSkill = 9;
                Cost = 46;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterGenerateAvailableActionEffectsList += HanSoloPilotAbility;
            }

            public void HanSoloPilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.HanSoloAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class HanSoloAction : ActionsList.GenericAction
    {
        public HanSoloAction()
        {
            Name = EffectName = "Han Solo's ability";
            IsReroll = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = callBack
            };
            diceRerollManager.Start();
            SelectAllRerolableDices();
            diceRerollManager.ConfirmReroll();
        }

        private static void SelectAllRerolableDices()
        {
            Combat.CurentDiceRoll.SelectBySides
            (
                new List<DieSide>(){
                    DieSide.Blank,
                    DieSide.Focus,
                    DieSide.Success,
                    DieSide.Crit
                },
                int.MaxValue
            );
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.NotRerolled > 0)
            {
                result = true;
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.NotRerolled > 0)
            {
                int focusToTurnIntoHit = 0;
                int focusToTurnIntoHitLeft = 0;
                if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    focusToTurnIntoHit = int.MaxValue;
                }
                else if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsOneFocusIntoSuccess) > 0)
                {
                    focusToTurnIntoHit = 1;
                }
                focusToTurnIntoHitLeft = focusToTurnIntoHit;

                int currentHits = 0;
                foreach (var die in Combat.DiceRollAttack.DiceList.Where(n => !n.IsRerolled))
                {
                    if (die.IsSuccess)
                    {
                        currentHits++;
                    }
                    else if (die.Side == DieSide.Focus)
                    {
                        if (focusToTurnIntoHitLeft > 0)
                        {
                            currentHits++;
                            focusToTurnIntoHitLeft--;
                        }
                    }
                }

                float averagePossibleHits = 0;
                if (focusToTurnIntoHit == int.MaxValue)
                {
                    averagePossibleHits = (6 / 8) * Combat.DiceRollAttack.NotRerolled;
                }
                else if (focusToTurnIntoHit == 1)
                {
                    if (Combat.DiceRollAttack.NotRerolled > 0)
                    {
                        averagePossibleHits = (6 / 8) + (4 / 8) * Combat.DiceRollAttack.NotRerolled-1;
                    }
                }
                else
                {
                    averagePossibleHits = (4 / 8) * Combat.DiceRollAttack.NotRerolled;
                }

                if (averagePossibleHits > currentHits) result = 85;
            }

            return result;
        }

    }
}
