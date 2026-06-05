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
            GObject[] children = Children.ToArray();
            foreach(GObject child in children){
                RenderNode(g, child, position, transform.rotation, transform.scale);
            }
        }
        private void RenderNode(Graphics g, GObject node, Vector2 parentPos, float parentRot, float parentScale){
            float worldRot = parentRot + node.transform.rotation;
            float worldScale = parentScale * node.transform.scale;

            float rad = parentRot * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            float lx = node.transform.position.x * parentScale;
            float ly = node.transform.position.y * parentScale;
            Vector2 worldPos = parentPos + new Vector2(lx * cos - ly * sin, lx * sin + ly * cos);

            if(node is CanvObject co && co.visal){
                co.Render(g, worldPos, worldRot, worldScale);
            }
            GObject[] children = node.Children.ToArray();
            foreach(GObject child in children){
                RenderNode(g, child, worldPos, worldRot, worldScale);
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
        public void Sort(){
            Children.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        }
    }
}