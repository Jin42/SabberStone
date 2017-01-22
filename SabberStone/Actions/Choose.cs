﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using HearthDb.Enums;
using log4net;
using SabberStone.Model;
using SabberStone.Tasks.SimpleTasks;

namespace SabberStone.Actions
{
    public partial class Generic
    {
        public static Func<Controller, Card, bool> ChoicePick
            => delegate(Controller c, Card choice)
            {
                var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

                if (c.Choice.ChoiceType != ChoiceType.GENERAL)
                {
                    log.Info($"Choice failed, trying to pick in a non-pick choice!");
                    c.Game.PlayTaskLog.AppendLine($"Choice failed, trying to pick in a non-pick choice!");
                    return false;
                }

                if (!c.Choice.Choices.Contains(choice))
                {
                    log.Info($"Choice failed, trying to pick a card that doesn't exist in this choice!");
                    c.Game.PlayTaskLog.AppendLine($"Choice failed, trying to pick a card that doesn't exist in this choice!");
                    return false;
                }

                var playable = Entity.FromCard(c, choice);

                log.Info($"{c.Name} Picks {playable} as choice!");
                c.Game.PlayTaskLog.AppendLine($"{c.Name} Picks {playable} as choice!");

                switch (c.Choice.ChoiceAction)
                {
                    case ChoiceAction.HEROPOWER:
                        playable[GameTag.CREATOR] = c.Hero.Id;
                        c.Game.TaskQueue.Enqueue(new ReplaceHeroPower(playable as HeroPower)
                        {
                            Game = c.Game,
                            Controller = c,
                            Source = playable,
                            Target = playable
                        });
                        break;
                    case ChoiceAction.HAND:
                        c.Game.TaskQueue.Enqueue(new AddCardTo(playable, EntityType.HAND)
                        {
                            Game = c.Game,
                            Controller = c,
                            Source = playable,
                            Target = playable
                        });
                        break;
                    case ChoiceAction.KAZAKUS:
                        c.Setaside.Add(playable);
                        var kazakusPotions =
                            c.Setaside.GetAll.Where(p => p.Card.Id.StartsWith("CFM_621"))
                                .Select(p => p[GameTag.TAG_SCRIPT_DATA_NUM_1])
                                .ToList();
                        if (kazakusPotions.Any())
                        {
                            c.Game.TaskQueue.Enqueue(new PotionGenerating(kazakusPotions)
                            {
                                Game = c.Game,
                                Controller = c,
                                Source = playable,
                                Target = playable
                            });
                        }

                        break;
                }

                // reset choice it's done
                c.Choice = null;

                return true;
            };

        public static Func<Controller, List<Card>, bool> ChoiceMulligan
            => delegate(Controller c, List<Card> choices)
            {
                var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

                if (c.Choice.ChoiceType != ChoiceType.MULLIGAN)
                {
                    log.Info($"Choice failed, trying to mulligan in a non-mulligan choice!");
                    c.Game.PlayTaskLog.AppendLine($"Choice failed, trying to mulligan in a non-mulligan choice!");
                    return false;
                }

                if (!choices.TrueForAll(p => c.Choice.Choices.Contains(p)))
                {
                    log.Info($"Choice failed, trying to mulligan a card that doesn't exist in this choice!");
                    c.Game.PlayTaskLog.AppendLine($"Choice failed, trying to mulligan a card that doesn't exist in this choice!");
                    return false;
                }

                return true;
            };

        public static Func<Controller, ChoiceType, ChoiceAction, List<Card>, bool> CreateChoice
            => delegate (Controller c, ChoiceType type, ChoiceAction action, List<Card> choices)
            {
                var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                if (c.Choice != null)
                {
                    log.Info($"there is an unresolved choice, can't add a new one!");
                    return false;
                }

                c.Choice = new Choice(c)
                {
                    ChoiceType = type,
                    ChoiceAction = action,
                    Choices = choices
                };
                return true;
            };
    }
}