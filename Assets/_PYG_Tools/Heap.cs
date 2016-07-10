using UnityEngine;
using System.Collections;

/// <summary>
/// Heap. This is an optimized class which can replace List<T> when optimization is needed.
/// The class T must implement the interface IHeapContent<T>.
/// </summary>
public class Heap<T> where T : IHeapContent<T>
{
	//Storage
	T[] content;
	//Length
	int currentContentLength;

	/// <summary>
	/// Returns the length of this Heap List.
	/// </summary>
	/// <value>The count.</value>
	public int Count { get { return currentContentLength; } }

	/// <summary>
	/// Initializes a new instance of Heap using a length.
	/// Heap<T> varName = new Heap<T> (length);
	/// </summary>
	/// <param name="newLength">New length.</param>
	public Heap (int newLength)
	{
		content = new T[newLength];
	}

	/// <summary>
	/// Adds a new item to the Heap List, at the end of it.
	/// </summary>
	/// <param name="newObject">New object.</param>
	public void Add (T newObject)
	{
		newObject.heapIndex = currentContentLength;
		content [currentContentLength] = newObject;
		SortUp (newObject);
		currentContentLength++;
	}

	/// <summary>
	/// Removes the first item of the Heap List.
	/// </summary>
	/// <returns>The first.</returns>
	public T RemoveFirst ()
	{
		T firstObject = content [0];
		currentContentLength--;
		content [0] = content [currentContentLength];
		content [0].heapIndex = 0;

		SortDown (content [0]);

		return firstObject;
	}

	/// <summary>
	/// Updates the position of an item based on its heapindex.
	/// </summary>
	/// <param name="newObject">New object.</param>
	public void UpdateObject (T newObject)
	{
		SortUp (newObject);
	}

	/// <summary>
	/// Returns true if the specified list contains the object passed in.
	/// </summary>
	/// <param name="newObject">New object.</param>
	public bool Contains (T newObject)
	{
		return Equals (content [newObject.heapIndex], newObject);
	}

	void SortDown (T newObject)
	{
		while (true) {
			int childIndexL = newObject.heapIndex * 2 + 1;
			int childIndexR = newObject.heapIndex * 2 + 2;

			int swapIndex = 0;

			if (childIndexL < currentContentLength) {
				swapIndex = childIndexL;

				if (childIndexR < currentContentLength) {
					if (content [childIndexL].CompareTo (content [childIndexR]) < 0)
						swapIndex = childIndexR;
				}

				if (newObject.CompareTo (content [swapIndex]) < 0)
					Swap (newObject, content [swapIndex]);
				else
					return;
			} else
				return;
		}
	}

	void SortUp (T newObject)
	{
		int parentIndex = (newObject.heapIndex - 1) / 2;

		while (true) {
			T parentObject = content [parentIndex];

			if (newObject.CompareTo (parentObject) > 0)
				Swap (newObject, parentObject);
			else
				break;

			parentIndex = (newObject.heapIndex - 1) / 2;
		}
	}

	/// <summary>
	/// Manually swaps two items in the Heap List.
	/// </summary>
	/// <param name="objectA">Object a.</param>
	/// <param name="objectB">Object b.</param>
	void Swap (T objectA, T objectB)
	{
		content [objectA.heapIndex] = objectB;
		content [objectB.heapIndex] = objectA;

		int tmpIndex = objectA.heapIndex;
		objectA.heapIndex = objectB.heapIndex;
		objectB.heapIndex = tmpIndex;
	}
}