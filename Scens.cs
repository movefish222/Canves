using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Canves.Core;

namespace Canves{
    public class Scene : GObject{
        private MultiwayTree tree;
        public ComboBox comboBox;
        public Scene(ComboBox comboBox) {
            tree = new MultiwayTree(this);
            this.comboBox = comboBox;
        }
        public void Render(Graphics g){
            // 从场景根递归渲染整棵子树，世界坐标 = 各级父节点 position 之和。
            // 这样即便对象被 Addchild 重新挂到别的节点下，依旧会被绘制。
            foreach(GObject child in Children){
                RenderNode(g, child, position);
            }
        }
        private void RenderNode(Graphics g, GObject node, Vector2 parentWorld){
            Vector2 world = node.position + parentWorld;
            if(node is CanvObject co && co.visal){
                co.Render(g, world);
            }
            foreach(GObject child in node.Children){
                RenderNode(g, child, world);
            }
        }
        public void Add(GObject obj){
            Add(tree, obj);
            comboBox.Items.Add(obj.ToString() + obj.id);
        }
        public void Add(GObject[] objs){
            foreach (var obj in objs){
                Add(tree, obj);
                comboBox.Items.Add(obj.ToString() + obj.id);
            }
        }
        public void Add(List<GObject> objs){
            foreach (var obj in objs){
                Add(tree, obj);
                comboBox.Items.Add(obj.ToString() + obj.id);
            }
        }
        public bool Contains(GObject obj){
            return Children.Contains(obj);
        }
        public GObject Find(int id){
            foreach(var i in Children){
                if(i.id == id) return i;
            }
            return this;
        }
        public void Addchild(GObject parent, GObject child){
            if(!parent.ContainsChild(child)){
                parent.Add(tree, child);
            }
        }
        public void Clear(){
            Children.Clear();
        }
        public void Sort(){ //TODO:Tabnine
            Children.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        }
    }
}