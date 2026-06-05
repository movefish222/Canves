using System;
using System.Collections.Generic;

namespace Canves {
    public class MultiwayTree
    {
        private GObject root;

        public GObject Root => root;

        public MultiwayTree()
        {
            root = new GObject();
        }

        public MultiwayTree(GObject _root)
        {
            root = _root;
        }

        public void AddChild(GObject parent, GObject child)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (child == null)
                throw new ArgumentNullException(nameof(child));
            if (child == root)
                throw new InvalidOperationException("Cannot reparent root node.");
            if (IsAncestor(child, parent))
                throw new InvalidOperationException("Cannot make a node a child of its own descendant.");

            if (child.Parent != null && child.Parent.ContainsChild(child)) {
                child.Parent.Children.Remove(child);
            }
            parent.Children.Add(child);
            child.Parent = parent;
        }

        public void RemoveChild(GObject parent, GObject child)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (!parent.ContainsChild(child))
                throw new InvalidOperationException("Child not found under specified parent.");

            parent.Children.Remove(child);
            child.Parent = root;
        }

        public void RemoveSubtree(GObject node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (node == root)
                throw new InvalidOperationException("Cannot remove root node.");

            if (node.Parent != null && node.Parent.ContainsChild(node)) {
                node.Parent.Children.Remove(node);
            }
            node.Parent = root;
        }

        public void MoveChild(GObject child, GObject newParent)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));
            if (newParent == null)
                throw new ArgumentNullException(nameof(newParent));

            AddChild(newParent, child);
        }

        public GObject? FindById(int id)
        {
            return FindById(root, id);
        }

        private GObject? FindById(GObject node, int id)
        {
            if (node.id == id) return node;
            foreach (var child in node.Children) {
                var result = FindById(child, id);
                if (result != null) return result;
            }
            return null;
        }

        public GObject? Find(Predicate<GObject> predicate)
        {
            return Find(root, predicate);
        }

        private GObject? Find(GObject node, Predicate<GObject> predicate)
        {
            if (predicate(node)) return node;
            foreach (var child in node.Children) {
                var result = Find(child, predicate);
                if (result != null) return result;
            }
            return null;
        }

        public List<GObject> FindAll(Predicate<GObject> predicate)
        {
            var results = new List<GObject>();
            FindAll(root, predicate, results);
            return results;
        }

        private void FindAll(GObject node, Predicate<GObject> predicate, List<GObject> results)
        {
            if (predicate(node)) results.Add(node);
            foreach (var child in node.Children) {
                FindAll(child, predicate, results);
            }
        }

        public bool Contains(GObject node)
        {
            return Find(root, n => ReferenceEquals(n, node)) != null;
        }

        public int Count()
        {
            return Count(root);
        }

        private int Count(GObject node)
        {
            int c = 1;
            foreach (var child in node.Children) {
                c += Count(child);
            }
            return c;
        }

        public int Depth(GObject node)
        {
            int d = 0;
            var current = node;
            while (current != null && !ReferenceEquals(current, root)) {
                d++;
                current = current.Parent;
            }
            return d;
        }

        public void TraversePreOrder(Action<GObject> action)
        {
            TraversePreOrder(root, action);
        }

        private void TraversePreOrder(GObject node, Action<GObject> action)
        {
            action(node);
            foreach (var child in node.Children) {
                TraversePreOrder(child, action);
            }
        }

        public void TraversePostOrder(Action<GObject> action)
        {
            TraversePostOrder(root, action);
        }

        private void TraversePostOrder(GObject node, Action<GObject> action)
        {
            foreach (var child in node.Children) {
                TraversePostOrder(child, action);
            }
            action(node);
        }

        private bool IsAncestor(GObject potentialAncestor, GObject node)
        {
            var current = node;
            while (current != null) {
                if (ReferenceEquals(current, potentialAncestor)) return true;
                if (ReferenceEquals(current, root)) break;
                current = current.Parent;
            }
            return false;
        }
    }
}
