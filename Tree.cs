namespace Canves {
// 多叉树类
    public class MultiwayTree
    {
        private GObject root;
        public MultiwayTree()
        {
            root = new GObject();
        }
        public MultiwayTree(GObject _root)
        {
            root = _root;
        }
        // 添加根节点
        public void AddRoot()
        {
            root = new GObject();
        }
        // 为指定节点添加子节点
        public void AddChild(GObject parent, GObject value)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "Parent node cannot be null.");
            }
            if(value.Parent != null && value.Parent.ContainsChild(value)){
                value.Parent.Children.Remove(value);
            }
            parent.Children.Add(value);
            value.Parent = parent;
        }
        // 前序遍历多叉树
        // public void PreOrderTraversal(GObject node)
        // {
        //     if (node == null)
        //     {
        //         return;
        //     }
        //     Console.Write(node.Value + " ");
        //     foreach (GObject child in node.Children)
        //     {
        //         PreOrderTraversal(child);
        //     }
        // }
        // // 对外暴露的前序遍历方法，从根节点开始
        // public void PreOrderTraversal()
        // {
        //     PreOrderTraversal(root);
        // }
    }
}