using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Canves.Core;

namespace Canves {
    public partial class Form1 : Form {
        private Thread painting;
        private Thread updating;
        public Graphics g;
        public Scene scene;
        public Form1() {
            InitializeComponent();
        }
        private void MUpdate() {
            while (true) {
                Painting.Update();
            }
        }
        private void Draw() {
            while (true) {
                // if(isAbort) {
                //     continue;
                // }
                Painting.Draw();
                //Thread.Sleep(20);
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            Plot.graphics = this.CreateGraphics();
            scene = new Scene(this.comboBox1);
            Painting.scene = scene;
            Painting._Start();       
            Painting.Start();
            updating = new Thread(new ThreadStart(MUpdate));
            painting = new Thread(new ThreadStart(Draw));
            updating.Start();
            Thread.Sleep(100);
            painting.Start();
        }
        private void button2_Click(object sender, EventArgs e) {
            if(painting != null) {
                painting.Abort();
                updating.Abort();
                //this.label1.Text = (new Vector2(RandF.FloRandArray(0, 800, 2))).drection.length.ToString();
            }
            // if(painting != null && !isAbort) {
            //     isAbort = true;
            // }
        }
        private void Form1_Load(object sender, EventArgs e) {
            Plot._mainForm = this;
        }
    }
}
