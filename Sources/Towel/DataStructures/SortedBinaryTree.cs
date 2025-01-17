﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Towel.DataStructures
{
	/// <summary>A self-sorting binary tree based on the heights of each node.</summary>
	/// <typeparam name="T">The generic type of this data structure.</typeparam>
	public interface ISortedBinaryTree<T> : IDataStructure<T>,
		// Structure Properties
		DataStructure.IAddable<T>,
		DataStructure.IRemovable<T>,
		DataStructure.ICountable,
		DataStructure.IClearable,
		DataStructure.IComparing<T>,
		DataStructure.IAuditable<T>
	{
		#region Members

		/// <summary>Gets the current least value in the tree.</summary>
		T CurrentLeast { get; }
		/// <summary>Gets the current greated value in the tree.</summary>
		T CurrentGreatest { get; }

		/// <summary>Determines if the tree contains a value.</summary>
		/// <param name="compare">The compare delegate. This must match the compare delegate of the tree.</param>
		/// <returns>True if the value is in the tree or false if not.</returns>
		bool Contains(CompareToKnownValue<T> compare);
		/// <summary>Tries to get a value.</summary>
		/// <param name="compare">The compare delegate. This must match the compare delegate of the tree.</param>
		/// <param name="value">The value if found or default.</param>
		/// <param name="exception">The exception that occurred if the get failed.</param>
		/// <returns>True if the value was found or false if not.</returns>
		bool TryGet(CompareToKnownValue<T> compare, out T value, out Exception exception);
		/// <summary>Tries to remove a value.</summary>
		/// <param name="compare">The compare delegate. This must match the compare delegate of the tree.</param>
		/// <param name="exception">The exception that occurred if the remove failed.</param>
		/// <returns>True if the remove succeeded or false if not.</returns>
		bool TryRemove(CompareToKnownValue<T> compare, out Exception exception);
		/// <summary>Invokes a delegate for each entry in the data structure (left to right).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		void Stepper(StepRef<T> step);
		/// <summary>Invokes a delegate for each entry in the data structure (left to right).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		/// <returns>The resulting status of the iteration.</returns>
		StepStatus Stepper(StepRefBreak<T> step);
		/// <summary>Invokes a delegate for each entry in the data structure (right to left).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		void StepperReverse(Step<T> step);
		/// <summary>Invokes a delegate for each entry in the data structure (right to left).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		void StepperReverse(StepRef<T> step);
		/// <summary>Invokes a delegate for each entry in the data structure (right to left).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		/// <returns>The resulting status of the iteration.</returns>
		StepStatus StepperReverse(StepBreak<T> step);
		/// <summary>Invokes a delegate for each entry in the data structure (right to left).</summary>
		/// <param name="step">The delegate to invoke on each item in the structure.</param>
		/// <returns>The resulting status of the iteration.</returns>
		StepStatus StepperReverse(StepRefBreak<T> step);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		void Stepper(Step<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		void Stepper(StepRef<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The result status of the stepper function.</returns>
		StepStatus Stepper(StepBreak<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The result status of the stepper function.</returns>
		StepStatus Stepper(StepRefBreak<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		void StepperReverse(Step<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		void StepperReverse(StepRef<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The result status of the stepper function.</returns>
		StepStatus StepperReverse(StepBreak<T> step, T minimum, T maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <param name="step">The step function.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The result status of the stepper function.</returns>
		StepStatus StepperReverse(StepRefBreak<T> step, T minimum, T maximum);

		#endregion
	}

	/// <summary>Contains extensions methods for the SortedBinaryTree interface.</summary>
	public static class SortedBinaryTree
	{
		#region Extensions

		/// <summary>Gets a traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static Stepper<T> Stepper<T>(this ISortedBinaryTree<T> tree) => tree.Stepper;
		/// <summary>Gets a traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRef<T> StepperRef<T>(this ISortedBinaryTree<T> tree) => tree.Stepper;
		/// <summary>Gets a traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperBreak<T> StepperBreak<T>(this ISortedBinaryTree<T> tree) => tree.Stepper;
		/// <summary>Gets a traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRefBreak<T> StepperRefBreak<T>(this ISortedBinaryTree<T> tree) => tree.Stepper;
		/// <summary>Gets a reverse traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static Stepper<T> StepperReverse<T>(this ISortedBinaryTree<T> tree) => tree.StepperReverse;
		/// <summary>Gets a reverse traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRef<T> StepperRefReverse<T>(this ISortedBinaryTree<T> tree) => tree.StepperReverse;
		/// <summary>Gets a reverse traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperBreak<T> StepperBreakReverse<T>(this ISortedBinaryTree<T> tree) => tree.StepperReverse;
		/// <summary>Gets a reverse traversal stepper for the tree.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRefBreak<T> StepperRefBreakReverse<T>(this ISortedBinaryTree<T> tree) => tree.StepperReverse;
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static Stepper<T> Stepper<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.Stepper(y => x(y), minimum, maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRef<T> StepperRef<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.Stepper(y => x(ref y), minimum, maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperBreak<T> StepperBreak<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.Stepper(y => x(y), minimum, maximum);
		/// <summary>Does an optimized step function (left to right) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRefBreak<T> StepperRefBreak<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.Stepper(y => x(ref y), minimum, maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static Stepper<T> StepperReverse<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.StepperReverse(y => x(y), minimum, maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRef<T> StepperRefReverse<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.StepperReverse(y => x(ref y), minimum, maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperBreak<T> StepperBreakReverse<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.StepperReverse(y => x(y), minimum, maximum);
		/// <summary>Does an optimized step function (right to left) for sorted binary search trees.</summary>
		/// <typeparam name="T">The generic type of this data structure.</typeparam>
		/// <param name="tree">The tree to traverse.</param>
		/// <param name="minimum">The minimum step value.</param>
		/// <param name="maximum">The maximum step value.</param>
		/// <returns>The stepper of the traversal.</returns>
		public static StepperRefBreak<T> StepperRefBreakReverse<T>(this ISortedBinaryTree<T> tree, T minimum, T maximum) => x => tree.StepperReverse(y => x(ref y), minimum, maximum);

		/// <summary>Tries to get a value.</summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="tree">The tree to get the value from.</param>
		/// <param name="compare">The compare delegate. This must match the compare that the Red-Black tree is sorted with.</param>
		/// <param name="value">The value if it is found.</param>
		/// <returns>True if the value was found or false if not.</returns>
		public static bool TryGet<T>(this ISortedBinaryTree<T> tree, CompareToKnownValue<T> compare, out T value)
		{
			return tree.TryGet(compare, out value, out _);
		}

		/// <summary>Gets a value.</summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="tree">The tree to get the value from.</param>
		/// <param name="compare">The compare delegate. This must match the compare that the Red-Black tree is sorted with.</param>
		/// <returns>The value.</returns>
		public static T Get<T>(this ISortedBinaryTree<T> tree, CompareToKnownValue<T> compare)
		{
			if (!tree.TryGet(compare, out T value, out Exception exception))
			{
				throw exception;
			}
			return value;
		}

		/// <summary>Tries to remove a value.</summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="tree">The tree to remove the value from.</param>
		/// <param name="compare">The compare delegate.</param>
		/// <returns>True if the remove was successful or false if not.</returns>
		public static bool TryRemove<T>(this ISortedBinaryTree<T> tree, CompareToKnownValue<T> compare)
		{
			return tree.TryRemove(compare, out _);
		}

		/// <summary>Removes a value.</summary>
		/// <typeparam name="T">The type of value.</typeparam>
		/// <param name="tree">The tree to remove the value from.</param>
		/// <param name="compare">The compare delegate.</param>
		public static void Remove<T>(this ISortedBinaryTree<T> tree, CompareToKnownValue<T> compare)
		{
			if (!tree.TryRemove(compare, out Exception exception))
			{
				throw exception;
			}
		}

		#endregion
	}
}
