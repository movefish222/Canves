using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Canves.Core;

namespace Canves {
    class Effect{
        
    }
    public class GObject{
        public int id;
        protected GObject parent;
        public List<GObject> Children = new List<GObject>();
        public GObject Parent{ 
            get{ return parent; }
            set{ if (!parent.Equals(value)) parent = value; }
        }
        public Vector2 position = new Vector2();
        public void Add(MultiwayTree tree, GObject obj) {
            tree.AddChild(this, obj);
        }
        public void Remove(GObject obj) {
            Children.Remove(obj);
        }
        public bool ContainsChild(GObject obj){
            return Children.Contains(obj);
        }
    }
    public class CanvObject : GObject{
        public List<Color> colors = new List<Color>();
        public bool visal = true;
        virtual public void Render(Graphics g, Vector2 position){
        }
        public CanvObject(){
            parent = new GObject();
        }
    }
    public class Scene : GObject{
        private MultiwayTree tree;
        public ComboBox comboBox;
        public Scene(ComboBox comboBox) {
            tree = new MultiwayTree(this);
            this.comboBox = comboBox;
        }
        public void Render(Graphics g){
            foreach(CanvObject obj in Children){
                if(obj.visal){
                    obj.Render(g, obj.position + obj.Parent.position);
                }
            }
        }
        public void Add(GObject obj){
            Add(tree, obj);
            comboBox.Items.Add(obj.ToString() + obj.id);
        }
        public void Add(GObject[] objs){
            foreach (var obj in objs){
                Add(tree, obj);
                comboBox.Items.Add(obj);
            }
        }
        public void Add(List<GObject> objs){
            foreach (var obj in objs){
                Add(tree, obj);
                comboBox.Items.Add(obj);
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