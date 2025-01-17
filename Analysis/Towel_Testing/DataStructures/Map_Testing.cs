﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using Towel;
using Towel.DataStructures;

namespace Towel_Testing.DataStructures
{
	[TestClass]
	public class MapHashLinked_Testing
	{
		[TestMethod]
		public void Add_Testing()
		{
			{ // string, int
				const int count = 100000;
				IMap<string, int> map = new MapHashLinked<string, int>();
				Stepper.Iterate(count, i => map.Add(i, i.ToString()));
				map.Add(int.MinValue, int.MinValue.ToString());
				map.Add(int.MaxValue, int.MaxValue.ToString());

				// contains
				Stepper.Iterate(count, i => Assert.IsTrue(map.Contains(i)));
				Assert.IsTrue(map.Contains(int.MinValue));
				Assert.IsTrue(map.Contains(int.MaxValue));
				Assert.IsFalse(map.Contains(-1));
				Assert.IsFalse(map.Contains(count));

				// get
				Stepper.Iterate(count, i => Assert.IsTrue(map[i] == i.ToString()));
				Assert.IsTrue(map[int.MinValue] == int.MinValue.ToString());
				Assert.IsTrue(map[int.MaxValue] == int.MaxValue.ToString());

				Assert.ThrowsException<ArgumentException>(() => map.Add(0, 0.ToString()));
				Assert.ThrowsException<ArgumentException>(() => map.Add(int.MinValue, int.MinValue.ToString()));
				Assert.ThrowsException<ArgumentException>(() => map.Add(int.MaxValue, int.MaxValue.ToString()));
			}

			{ // int, string
				const int count = 100000;
				IMap<int, string> map = new MapHashLinked<int, string>();
				Stepper.Iterate(count, i => map.Add(i.ToString(), i));
				map.Add(int.MinValue.ToString(), int.MinValue);
				map.Add(int.MaxValue.ToString(), int.MaxValue);

				// contains
				Stepper.Iterate(count, i => Assert.IsTrue(map.Contains(i.ToString())));
				Assert.IsTrue(map.Contains(int.MinValue.ToString()));
				Assert.IsTrue(map.Contains(int.MaxValue.ToString()));
				Assert.IsFalse(map.Contains((-1).ToString()));
				Assert.IsFalse(map.Contains(count.ToString()));

				// get
				Stepper.Iterate(count, i => Assert.IsTrue(map[i.ToString()] == i));
				Assert.IsTrue(map[int.MinValue.ToString()] == int.MinValue);
				Assert.IsTrue(map[int.MaxValue.ToString()] == int.MaxValue);

				Assert.ThrowsException<ArgumentException>(() => map.Add(0.ToString(), 0));
				Assert.ThrowsException<ArgumentException>(() => map.Add(int.MinValue.ToString(), int.MinValue));
				Assert.ThrowsException<ArgumentException>(() => map.Add(int.MaxValue.ToString(), int.MaxValue));
			}
		}

		[TestMethod]
		public void Set_Testing()
		{
			{ // string, int
				const int count = 100000;
				IMap<string, int> map = new MapHashLinked<string, int>();
				Stepper.Iterate(count, i => map[i] = i.ToString());
				map[int.MinValue] = int.MinValue.ToString();
				map[int.MaxValue] = int.MaxValue.ToString();

				// contains
				Stepper.Iterate(count, i => Assert.IsTrue(map.Contains(i)));
				Assert.IsTrue(map.Contains(int.MinValue));
				Assert.IsTrue(map.Contains(int.MaxValue));
				Assert.IsFalse(map.Contains(-1));
				Assert.IsFalse(map.Contains(count));

				// get
				Stepper.Iterate(count, i => Assert.IsTrue(map[i] == i.ToString()));
				Assert.IsTrue(map[int.MinValue] == int.MinValue.ToString());
				Assert.IsTrue(map[int.MaxValue] == int.MaxValue.ToString());
			}

			{ // int, string
				const int count = 100000;
				IMap<int, string> map = new MapHashLinked<int, string>();
				Stepper.Iterate(count, i => map[i.ToString()] = i);
				map[int.MinValue.ToString()] = int.MinValue;
				map[int.MaxValue.ToString()] = int.MaxValue;

				// contains
				Stepper.Iterate(count, i => Assert.IsTrue(map.Contains(i.ToString())));
				Assert.IsTrue(map.Contains(int.MinValue.ToString()));
				Assert.IsTrue(map.Contains(int.MaxValue.ToString()));
				Assert.IsFalse(map.Contains((-1).ToString()));
				Assert.IsFalse(map.Contains(count.ToString()));

				// get
				Stepper.Iterate(count, i => Assert.IsTrue(map[i.ToString()] == i));
				Assert.IsTrue(map[int.MinValue.ToString()] == int.MinValue);
				Assert.IsTrue(map[int.MaxValue.ToString()] == int.MaxValue);
			}
		}

		[TestMethod]
		public void Remove_Testing()
		{
			{ // string, int
				const int count = 100000;
				IMap<string, int> map = new MapHashLinked<string, int>();
				Stepper.Iterate(count, i => map.Add(i, i.ToString()));
				for (int i = 0; i < count; i += 3)
				{
					map.Remove(i);
				}
				for (int i = 0; i < count; i++)
				{
					if (i % 3 == 0)
					{
						Assert.IsFalse(map.Contains(i));
					}
					else
					{
						Assert.IsTrue(map.Contains(i));
					}
				}
				Assert.IsFalse(map.Contains(-1));
				Assert.IsFalse(map.Contains(count));
			}

			{ // int, string
				const int count = 100000;
				IMap<int, string> map = new MapHashLinked<int, string>();
				Stepper.Iterate(count, i => map.Add(i.ToString(), i));
				for (int i = 0; i < count; i += 3)
				{
					map.Remove(i.ToString());
				}
				for (int i = 0; i < count; i++)
				{
					if (i % 3 == 0)
					{
						Assert.IsFalse(map.Contains(i.ToString()));
					}
					else
					{
						Assert.IsTrue(map.Contains(i.ToString()));
					}
				}
				Assert.IsFalse(map.Contains((-1).ToString()));
				Assert.IsFalse(map.Contains(count.ToString()));
			}
		}
	}
}
