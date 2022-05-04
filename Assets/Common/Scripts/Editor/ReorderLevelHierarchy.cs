using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class ReorderLevelHierarchy
{
    static GameObject parent = null;

    static GameObject[] SceneChildren() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

    static IEnumerable<GameObject> ParentChildren()
    {
        foreach (Transform child in parent.transform)
            yield return child.gameObject;
    }

    static IEnumerable<GameObject> Children() => parent != null ? ParentChildren() : SceneChildren();


    static GameObject RootFind(string name)
    {
        if (parent)
        {
            return parent.transform.Find(name)?.gameObject ?? null;
        }
        else
        {
            return SceneChildren().FirstOrDefault(go => go.name == name);
        }
    }

    static GameObject RootCreate(string name)
    {
        var go = new GameObject(name);
        if (parent)
            go.transform.SetParent(parent.transform);
        return go;
    }

    static GameObject RootEnsure(string name) => RootFind(name) ?? RootCreate(name);

    static GameObject[] RegularChildren(bool deep = false)
    {
        if (deep == false)
            return Children().Where(IsRegularChild).ToArray();

        var chilren = new List<GameObject>();
        var queue = new Queue<GameObject>(Children());

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (IsRegularChild(current))
                chilren.Add(current);

            // Skip Prefab
            if (PrefabUtility.IsAnyPrefabInstanceRoot(current))
                continue;

            foreach (Transform child in current.transform)
                queue.Enqueue(child.gameObject);
        }

        return chilren.ToArray();
    }

    static void Reparent(GameObject child, GameObject parent) => child.transform.SetParent(parent.transform);

    class RootParent
    {
        public string name;
        public System.Func<GameObject, bool> IsConcerned;
        public System.Func<GameObject, string> DefaultChildName;
        public GameObject gameObject;

        public bool ProcessChild(GameObject child)
        {
            if (IsConcerned(child))
            {
                Reparent(child, gameObject);
                return true;
            }
            return false;
        }

        void RenameChildren()
        {
            var names = new Register<string, GameObject>();

            var children = new List<GameObject>();
            foreach (Transform child in gameObject.transform)
                children.Add(child.gameObject);

            foreach (var child in children)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(child))
                {
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(child);
                    names.Add(prefab.name, child);
                }
                else
                {
                    if (DefaultChildName != null)
                    {
                        var name = DefaultChildName(child);
                        if (name != null)
                            names.Add(name, child);
                    }
                }
            }

            foreach (var (name, children2) in names.Entries())
            {
                if (children2.Count == 1)
                {
                    children2[0].name = name;
                }
                else
                {
                    int count = 0;
                    foreach (var child in children2)
                        child.name = $"{name} ({++count})";
                }
            }
        }

        public void Cleanup()
        {
            if (gameObject.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            RenameChildren();
        }
    }

    static RootParent[] rootParents = new RootParent[] {
        new RootParent {
            name = "Cameras",
            IsConcerned = go => go.GetComponent<VirtualCameraSwitcher>() != null,
        },
        new RootParent {
            name = "Blocks",
            IsConcerned = go => go.tag == "Block",
            DefaultChildName = go => "Block",
        },
        new RootParent {
            name = "InvisibleBlocks",
            IsConcerned = go => go.tag == "InvisibleBlock",
        },
        new RootParent {
            name = "Enemies",
            IsConcerned = go => go.tag == "Enemy",
        },
        new RootParent {
            name = "Collectables",
            IsConcerned = go => go.tag == "Collectable",
        },
        new RootParent {
            name = "Lava",
            IsConcerned = go => go.GetComponent<Destroyer>() != null,
        },
        new RootParent {
            name = "Triggers",
            IsConcerned = go => {
                if (go.TryGetComponent<Collider>(out var collider))
                    return collider.isTrigger;
                return false;
            },
        },
    };

    static bool IsRegularChild(GameObject go)
    {
        // Skip "RootParent".
        if (rootParents.Any(rp => rp.name == go.name))
            return false;

        return true;
    }

    [MenuItem("Tools/Reorder Level Hierarchy", false, 0)]
    static void Reorder()
    {
        parent = Selection.activeGameObject;

        foreach (var rp in rootParents)
            rp.gameObject = RootEnsure(rp.name);

        foreach (var child in RegularChildren(true))
        {
            foreach (var rp in rootParents)
            {
                if (rp.ProcessChild(child))
                    break;
            }
        }

        foreach (var rp in rootParents)
            rp.Cleanup();
    }
}
