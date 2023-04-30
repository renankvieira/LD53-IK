using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
public static class Extensions {

    public static T DuplicateObjectByComponent<T>(this T originalComponent, bool deparent = false) where T : UnityEngine.Component
    {
        GameObject newGameObject = GameObject.Instantiate(originalComponent.gameObject);
        if (deparent)
            newGameObject.transform.parent = null;
        return newGameObject.GetComponent<T>();
    }

    public static Vector3 RandomInside(this Vector3 myVector, bool onlyPositive = false)
    {
        myVector.x = Random.Range(onlyPositive ? 0f : -myVector.x, myVector.x);
        myVector.y = Random.Range(onlyPositive ? 0f : -myVector.y, myVector.y);
        myVector.z = Random.Range(onlyPositive ? 0f : -myVector.z, myVector.z);
        return myVector;
    }


    public static void ToggleAll(this Behaviour[] behaviours, bool active)
    {
        foreach (Behaviour behaviour in behaviours)
            behaviour.enabled = active;
    }



    public static Rigidbody GetRb(this GameObject go)
    {
        return go.GetComponent<Rigidbody>();
    }

    public static T AddOrGetComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static int LastValidIndex<T>(this List<T> list)
    {
        if (list.Count == 0)
            Debug.LogWarning("Attention: The list is empty.");

        return list.Count - 1;
    }

    public static int LastValidIndex<T>(this T[] list)
    {
        if (list.Length == 0)
            Debug.LogWarning("Attention: The array is empty.");

        return list.Length - 1;
    }

    public static T GetLast<T>(this T[] list)
    {
        if (list.Length == 0)
            Debug.LogWarning("Empty list.");
        return list[list.Length - 1];
    }

    public static T GetLast<T>(this List<T> list)
    {
        if (list.Count == 0)
            Debug.LogWarning("Empty list.");
        return list[list.Count - 1];
    }

    public static void KillAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
            GameObject.Destroy(child.gameObject);
    }

    public static void SetRandomTrigger(this Animator anim, string[] triggers, bool preResettingAll = false)
    {
        if (preResettingAll)
            for (int i = 0; i < triggers.Length; i++)
                anim.ResetTrigger(triggers[i]);

        anim.SetTrigger(triggers.GetRandom());
    }


    //public static void SetActive(this GameObject go, bool active, float delay)
    //{
    //    CoroutineHolder.Instance.DelayedInvoke(() =>
    //    {
    //        go.SetActive(active);
    //    }, delay);
    //}


    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

#if UNITY_EDITOR
    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static T FindAssetByType<T>() where T : UnityEngine.Object
    {
        T asset = null;
        var guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        string firstGuid = string.Empty;

        if (guids.Length > 0)
        {
            firstGuid = guids[0];

            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(firstGuid);
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        return asset;
    }
#endif


    public static string GetFormattedString(this System.TimeSpan timeSpan, bool useDay = false)
    {
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;
        if (useDay)
        {
            int days = timeSpan.Days;
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", days, hours, minutes, seconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    //public static void Shuffle<T>(this T[] ts)
    //{
    //    var count = ts.Length;
    //    var last = count - 1;
    //    for (var i = 0; i < last; ++i)
    //    {
    //        var r = UnityEngine.Random.Range(i, count);
    //        var tmp = ts[i];
    //        ts[i] = ts[r];
    //        ts[r] = tmp;
    //    }
    //}

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static List<T> CloneList<T>(this List<T> ts)
    {
        List<T> newList = new List<T>();
        for (int i = 0; i < ts.Count; i++)
            newList.Add(ts[i]);
        return newList;
    }

    public static void SelectOnEditor(this GameObject go, bool logSelection = true)
    {
#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = go;
        if (logSelection)
            Debug.Log("Auto-selected: " + go.name, go);
#endif
    }

    public static void SetTriggerSafe(this Animator animatorComponent, string trigger)
    {
        if (!string.IsNullOrEmpty(trigger) && trigger != "-" && !trigger.StartsWith("#", System.StringComparison.CurrentCulture))
            animatorComponent.SetTrigger(trigger);
    }


    public static void FakeClick(this UnityEngine.UI.Button buttonToClick)
    {
        ExecuteEvents.Execute(buttonToClick.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    public static T FindClosest<T>(this List<T> objects, Vector3 origin) where T : MonoBehaviour
    {
        if (objects.Count == 0)
            return null;
        if (objects.Count == 1)
            return objects[0];

        int closestIndex = -1;
        float closestDistance = 0f;

        for (int i = 0; i < objects.Count; i++)
        {
            if (closestIndex == -1)
            {
                closestIndex = 0;
                closestDistance = Vector3.Distance(origin, objects[0].transform.position);
            }
            else
            {
                float distanceToCheck = Vector3.Distance(origin, objects[i].transform.position);
                if (distanceToCheck < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = Vector3.Distance(origin, objects[i].transform.position);
                }
            }
        }
        return objects[closestIndex];
    }

    public static GameObject CreateCube(this MonoBehaviour mb, Vector3? position = null)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (position != null)
            go.transform.position = position.Value;
        return go;
    }

    public static float Average(this float[] array, bool logWarningIfEmpty = false)
    {
        if (array.Length == 0)
        {
            if (logWarningIfEmpty)
                Debug.LogWarning("[Extensions] Trying to average empty array.");
            return 0f;
        }

        float total = 0f;
        foreach (float f in array)
            total += f;

        return total / array.Length;
    }


    public static bool IsInsideCameraViewPort(this Vector3 position, Camera cam)
    {
        Vector3 positionOnScreen = cam.WorldToViewportPoint(position);
        bool isInside = true;
        if ( !positionOnScreen.x.IsBetween(0f, 1f) || !positionOnScreen.y.IsBetween(0f, 1f) )
        {
            isInside = false;
        }
        return isInside;
    }

    public static bool IsBetween(this float number, float min, float max)
    {
        if (number >= min && number <= max)
        {
            return true;
        }
        return false;
    }

    public static Vector3 ClampToOne(this Vector3 vector)
    {
        if (vector.sqrMagnitude > 1f)
            return vector.normalized;
        return vector;
    }

    public static Vector3 ClampToMagnitude(this Vector3 vector, float magnitude)
    {
        if (vector.sqrMagnitude > (magnitude * magnitude))
            return vector.normalized * magnitude;
        return vector;
    }

    public static Quaternion ToQuaternion(this Vector3 testVector)
    {
        return Quaternion.Euler(testVector);
    }

    public static Vector2 GetNormalizedPosition(this Touch touch)
    {
        return new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
    }

    public static void ToggleAll(this GameObject[] gameObjects, bool activate)
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(activate);
        }
    }

    public static void ToggleAll(this List<GameObject> gameObjects, bool activate)
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(activate);
        }
    }

    public static void DelayedInvoke(this MonoBehaviour me, System.Action theDelegate, float time)
    {
        me.StartCoroutine(ExecuteAfterTime(theDelegate, time));
    }

    private static IEnumerator ExecuteAfterTime(System.Action theDelegate, float delay)
    {
        yield return new WaitForSeconds(delay);
        theDelegate();
    }

    public static void DelayOneFrameAndInvoke(this MonoBehaviour me, System.Action theDelegate)
    {
        me.StartCoroutine(ExecuteAfterOneFrame(theDelegate));
    }

    private static IEnumerator ExecuteAfterOneFrame(System.Action theDelegate)
    {
        yield return null;
        theDelegate();
    }

    public static void DelayToEndOfFrameAndInvoke(this MonoBehaviour me, System.Action theDelegate)
    {
        me.StartCoroutine(ExecuteOnEndOfFrame(theDelegate));
    }

    private static IEnumerator ExecuteOnEndOfFrame(System.Action theDelegate)
    {
        yield return new WaitForEndOfFrame();
        theDelegate();
    }



    //public static void ToggleAllGameObjects<T>(this List<T> list, bool activate) where T : MonoBehaviour
    //{
    //    foreach (T item in list)
    //    {
    //        item.gameObject.SetActive(activate);
    //    }
    //}

    public static void ToggleAllGameObjects<T>(this IReadOnlyList<T> list, bool activate) where T : MonoBehaviour
    {
        foreach (T item in list)
        {
            item.gameObject.SetActive(activate);
        }
    }

    //public static GameObject GetLinkedGameObjectByID(this List<UnlockableIDLinker> list, UnlockableID id)
    //{
    //    foreach (UnlockableIDLinker linker in list)
    //    {
    //        if (linker.id == id)
    //        {
    //            return linker.gameObject;
    //        }
    //    }
    //    return null;
    //}

    public static T GetRandom<T>(this List<T> collection, bool removingFromList = false)
    {
        if (collection == null)
        {
            Debug.LogWarning("Null collection: " + collection);
            return default;
        }
        if (collection.Count == 0)
        {
            Debug.LogWarning("Empty collection: " + collection);
            return default;
        }

        T randomItem = collection[Random.Range(0, collection.Count)];

        if (removingFromList)
            collection.Remove(randomItem);

        return randomItem;
    }

    public static T GetRandom<T>(this T[] collection)
    {
        if (collection == null)
        {
            Debug.LogWarning("Null collection: " + collection);
            return default;
        }
        if (collection.Length == 0)
        {
            Debug.LogWarning("Empty collection: " + collection);
            return default;
        }

        return collection[Random.Range(0, collection.Length)];
    }

    public static List<T> GetRandomSubList<T>(this List<T> collection, int subListSize, bool populateWithDiverseItems)
    {
        if (collection == null)
        {
            Debug.LogWarning("Null collection: " + collection);
            return default;
        }
        if (collection.Count == 0)
        {
            Debug.LogWarning("Empty collection: " + collection);
            return default;
        }
        if (collection.Count < subListSize && populateWithDiverseItems)
        {
            Debug.LogWarningFormat("Original list is smaller than intended sub-list: {0}, {1}, {2}", collection.Count, subListSize, collection);
            return default;
        }

        List<T> subList = new List<T>();

        while (subList.Count < subListSize)
        {
            T randomItem = collection[Random.Range(0, collection.Count)];

            if (populateWithDiverseItems)
            {
                if (!subList.Contains(randomItem))
                    subList.Add(randomItem);
            }
            else
            {
                subList.Add(randomItem);
            }
        }

        return subList;
    }



    public static int Invert01(this int n)
    {
        if (n == 0)
            return 1;
        if (n == 1)
            return 0;

        Debug.LogWarning("[Invert01] Not 0 or 1: " + n);
        return 0;
    }

    public static bool LowerThanRandomValue(this float f)
    {
        return Random.value < f;
    }




    //public abstract class BaseClass<TEnum>
    //where TEnum : struct, System.IComparable, System.IFormattable, System.IConvertible
    //{
    //    public abstract int GetEnum<TEnum>(TEnum value);
    //}

    //public abstract class TFindableByID { public abstract int GetEnumID(); }

    //public static T GenericFindItemByID<T>(this IReadOnlyList<T> list, int qweqwe) where T : TFindableByID
    //{
    //    foreach (T item in list)
    //    {
    //        if (item.GetEnumID() == (int)qweqwe)
    //        {
    //            return item;
    //        }
    //    }
    //    return null;
    //}
}

