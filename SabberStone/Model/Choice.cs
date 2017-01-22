﻿using System.Collections.Generic;
using System.Text;
using HearthDb.Enums;
using log4net;

namespace SabberStone.Model
{
    public enum ChoiceAction
    {
        HAND, SUMMON, HEROPOWER, KAZAKUS, INVALID
    }

    public class Choice
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Choice(Controller controller)
        {
            Controller = controller;
        }

        public Controller Controller { get; set; }

        public ChoiceType ChoiceType { get; set; } = ChoiceType.INVALID;

        public ChoiceAction ChoiceAction { get; set; } = ChoiceAction.INVALID;

        public List<Card> Choices { get; set; }

        public void Stamp(Choice choice)
        {
            ChoiceType = choice.ChoiceType;
            ChoiceAction = choice.ChoiceAction;
            Choices = new List<Card>(choice.Choices);
        }

        public string FullPrint()
        {
            var str = new StringBuilder();
            str.Append($"{Controller.Name}[ChoiceType:{ChoiceType}][ChoiceAction:{ChoiceAction}][");
            Choices.ForEach(p => str.Append($"{p.Id}({p.Cost})"));
            return str.ToString();
        }
    }
}