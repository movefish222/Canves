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
        private volatile bool _paused = false;
        private readonly ManualResetEventSlim _pauseEvent = new(true);

        public Form1() {
            InitializeComponent();
        }
        private void MUpdate() {
            while (true) {
                _pauseEvent.Wait();
                Time.Tick();
                Painting.Update();
                Thread.Sleep(1);
            }
        }
        private void Draw() {
            while (true) {
                _pauseEvent.Wait();
                try {
                    Painting.Draw();
                } catch { }
                Thread.Sleep(1);
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            Plot.graphics = this.CreateGraphics();
            scene = new Scene();
            scene.position = new Vector2(this.ClientSize.Width / 2f, this.ClientSize.Height / 2f);
            Painting.scene = scene;
            Time.Start();
            Painting._Start();
            Painting.Start();
            updating = new Thread(new ThreadStart(MUpdate)) { IsBackground = true };
            painting = new Thread(new ThreadStart(Draw)) { IsBackground = true };
            updating.Start();
            Thread.Sleep(100);
            painting.Start();
        }
        private void button2_Click(object sender, EventArgs e) {
            if (painting == null) return;

            if (!_paused) {
                _pauseEvent.Reset();
                _paused = true;
                button2.Text = "继续";
            } else {
                _pauseEvent.Set();
                _paused = false;
                button2.Text = "暂停";
            }
        }
        private void Form1_Load(object sender, EventArgs e) {
            Plot._mainForm = this;
        }
    }
}
