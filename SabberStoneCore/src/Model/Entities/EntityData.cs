﻿using SabberStoneCore.Enums;
using SabberStoneCore.Loader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCore.Model.Entities
{
	/// <summary>
	/// Holds original entity data for a specific card.
	/// </summary>
	/// <seealso cref="System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{SabberStoneCore.Enums.EGameTag, System.Int32}}" />
	/// <autogeneratedoc />
	internal class EntityData : IEnumerable<KeyValuePair<EGameTag, int>>
    {
		/// <summary>The card which was used to create this instance.</summary>
		public readonly Card Card;

		/// <summary>The container with tags used to create this instance.</summary>
		/// TODO; This might be made private.
		public Dictionary<EGameTag, int> Tags;

		/// <summary>Gets or sets the value of a specific GameTag.</summary>
		/// <value></value>
		/// <param name="t">The GameTag.</param>
		/// <returns></returns>
		public int this[EGameTag t]
		{
			get { return Tags.ContainsKey(t) ? Tags[t] : (Card.Tags.ContainsKey(t) ? Card[t] : 0); }
			set { Tags[t] = value; }
		}

		/// <summary>Initializes a new instance of the <see cref="EntityData"/> class.</summary>
		/// <param name="card"></param>
		/// <param name="tags"></param>
		/// <autogeneratedoc />
		internal EntityData(Card card, Dictionary<EGameTag, int> tags)
        {
            Card = card;
            Tags = tags ?? new Dictionary<EGameTag, int>();
        }

		/// <summary>Resets all tags from the container.</summary>
		public void Reset()
		{
			//Tags = tags ?? new Dictionary<GameTag, int>(Enum.GetNames(typeof(GameTag)).Length);
			Tags.Remove(EGameTag.ATK);
			Tags.Remove(EGameTag.HEALTH);
			Tags.Remove(EGameTag.COST);
			Tags.Remove(EGameTag.DAMAGE);
			Tags.Remove(EGameTag.TAUNT);
			Tags.Remove(EGameTag.FROZEN);
			Tags.Remove(EGameTag.ENRAGED);
			Tags.Remove(EGameTag.CHARGE);
			Tags.Remove(EGameTag.WINDFURY);
			Tags.Remove(EGameTag.DIVINE_SHIELD);
			Tags.Remove(EGameTag.STEALTH);
			Tags.Remove(EGameTag.DEATHRATTLE);
			Tags.Remove(EGameTag.BATTLECRY);
			Tags.Remove(EGameTag.SILENCED);
		}

		/// <summary>Copies data from the other object into this one.</summary>
		/// <param name="data">The data.</param>
		public void Stamp(EntityData data)
        {
            Tags = new Dictionary<EGameTag, int>(data.Tags);
        }

		/// <summary>Returns a string uniquely identifying this object.</summary>
		/// <param name="ignore">The tags to ignore during hashing.</param>
		/// <returns>The hash string.</returns>
		public string Hash(params EGameTag[] ignore)
        {
            var hash = new StringBuilder();
            hash.Append("[");
            hash.Append(Card.Id);
            hash.Append("][GT:");
            var keys = Tags.Keys.ToList();
            keys.Sort();
            keys.ForEach(p =>
            {
                if (!ignore.Contains(p))
                {
                    hash.Append("{");
                    hash.Append(p.ToString());
                    hash.Append(",");
                    hash.Append(Tags[p].ToString());
                    hash.Append("}");
                }
            });
            hash.Append("]");
            return hash.ToString();
        }        

        public IEnumerator<KeyValuePair<EGameTag, int>> GetEnumerator()
        {
            // Entity ID
            var allTags = new Dictionary<EGameTag, int>(Card.Tags);

            // Entity tags override card tags
            foreach (KeyValuePair<EGameTag, int> tag in Tags)
            {
                allTags[tag.Key] = tag.Value;
            }

            return allTags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
			string s = Tags.Aggregate(Card.Name + " - ", (current, tag) => current + new Tag(tag.Key, tag.Value) + ", ");
            return s.Substring(0, s.Length - 2);
        }

        
    }
}