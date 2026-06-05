using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Canves.Core;

namespace Canves {
    public class GObject{
        public int id;
        protected GObject parent;
        public List<GObject> Children = new List<GObject>();
        public GTransform transform = new GTransform();
        public Vector2 position {
            get => transform.position;
            set => transform.position = value;
        }
        public GObject Parent{
            get{ return parent; }
            set{ if (!parent.Equals(value)) parent = value; }
        }
        public void Add(MultiwayTree tree, GObject obj) {
            tree.AddChild(this, obj);
        }
        public void Remove(GObject obj) {
            if(Children.Contains(obj)){
                obj.Parent = new GObject();
                Children.Remove(obj);
            }else{
                throw new Exception("Error: GObject not found");
            }
        }
        public bool ContainsChild(GObject obj){
            return Children.Contains(obj);
        }
    }
}