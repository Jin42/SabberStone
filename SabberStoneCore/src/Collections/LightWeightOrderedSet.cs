﻿using SabberStoneCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SabberStoneCore.Collections
{
	/// <summary>
	/// Collection which stores unique items in insertion order.
	/// This collection does NOT support manipulations.
	/// </summary>
	/// <typeparam name="T">The type to be stored.</typeparam>
	/// <seealso cref="IReadOnlyOrderedSet{T}" />
	internal class LightWeightOrderedSet<T> : IReadOnlyOrderedSet<T>
	{
		/// <summary>An instance of LightWeightOrderedSet without any elements.</summary>
		public static readonly LightWeightOrderedSet<T> Empty = BuildEmpty();

		/// <summary>Contains the set items.</summary>
		private readonly T[] _internalContainer;

		/// <summary>The comparer function for T.</summary>
		private readonly IEqualityComparer<T> _comparer;

		T IReadOnlyList<T>.this[int index] => _internalContainer[index];

		int IReadOnlyCollection<T>.Count => _internalContainer.Length;

		#region CONSTRUCTORS

		private LightWeightOrderedSet(T[] container, IEqualityComparer<T> comparer)
		{
			_internalContainer = container ?? throw new ArgumentNullException("container is null!");
			_comparer = comparer ?? GetDefaultComparer();
		}

		/// <summary>Builds a lightweight (ReadOnly) OrderedSet from the provided data.</summary>
		/// <param name="data">The data.</param>
		/// <param name="comparer">The comparer.</param>
		/// <param name="throwOnConstraintViolation">If true throws an error if the unique item constraint is violated.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">data is null!</exception>
		public static LightWeightOrderedSet<T> Build(IEnumerable<T> data, IEqualityComparer<T> comparer = null, bool throwOnConstraintViolation = true)
		{
			if (data == null) throw new ArgumentNullException("data is null!");
			if (comparer == null) { comparer = GetDefaultComparer(); }

			// Filter out all duplicate values from enumerable.
			IEnumerable<T> filteredData = data.Distinct();
			T[] distinctElementArray = filteredData.ToArray();

			if (throwOnConstraintViolation)
			{
				if (data.Count() != distinctElementArray.Length)
				{
					throw new ConstraintViolationException("Duplicate items detected!");
				}
			}

			// Build and return object.
			return new LightWeightOrderedSet<T>(distinctElementArray, comparer);
		}

		/// <summary>Builds a lightweight (ReadOnly) OrderedSet from the provided data.</summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static LightWeightOrderedSet<T> Build(params T[] data)
		{
			return Build(data, GetDefaultComparer(), true);
		}

		/// <summary>Builds a lightweight (ReadOnly) OrderedSet without any data.</summary>
		/// <returns></returns>
		private static LightWeightOrderedSet<T> BuildEmpty()
		{
			return new LightWeightOrderedSet<T>(new T[0] { }, null);
		}

		#endregion

		void IReadOnlySet<T>.ForEach(Action<T> lambda)
		{
			for (int i = 0; i < _internalContainer.Length; ++i)
			{
				lambda(_internalContainer[i]);
			}
		}

		bool IReadOnlySet<T>.Exists(Func<T, bool> lambda)
		{
			for (int i = 0; i < _internalContainer.Length; ++i)
			{
				if (lambda(_internalContainer[i]))
				{
					return true;
				}
			}

			return false;
		}

		bool IReadOnlySet<T>.Contains(T item)
		{
			for (int i = 0; i < _internalContainer.Length; ++i)
			{
				if (_comparer.Equals(_internalContainer[i], item))
				{
					return true;
				}
			}

			return false;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)_internalContainer).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _internalContainer.GetEnumerator();
		}

		IOrderedEnumerable<T> IOrderedEnumerable<T>.CreateOrderedEnumerable<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			if (descending)
			{
				return _internalContainer.OrderByDescending(keySelector, comparer);
			}
			else
			{
				return _internalContainer.OrderBy(keySelector, comparer);
			}
		}

		private static IEqualityComparer<T> GetDefaultComparer()
		{
			return EqualityComparer<T>.Default;
		}
	}
}
