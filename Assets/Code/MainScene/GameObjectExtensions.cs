using UnityEngine;
using System.Collections;

public static class GameObjectExtensions
{
    public static Coroutine StartCoroutine(this GameObject gameObject, IEnumerator routine)
    {
        MonoBehaviour component = gameObject.GetComponent<MonoBehaviour>();
        return component ? component.StartCoroutine(routine) : null;
    }
}