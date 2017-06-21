using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GdiShaders
{
    public partial class Form1 : Form
    {
        public GdiShader[] shaders = new GdiShader[]
        {
            new SampleGdiShader(),
            new SampleGdiShader2(),
            new SampleGdiShader3(),
            new SampleGdiShader4(),
            new SampleGdiShader5(),
            new SampleGdiShader6(),
            new SampleGdiShader7(),
            new SampleGdiShader8(),
            new SampleGdiShader9(),
            new SampleGdiShader10(),
            new SampleGdiShader11(),
            new SampleGdiShader12(),
            new SampleGdiShader13(),
            new SampleGdiShader14(),
            new SampleGdiShader15(),
            new SampleGdiShader16(),
            new SampleGdiShader17(),
            new SampleGdiShader18(),
            new SampleGdiShader19(),
            new SampleGdiShader20(),
            new SampleGdiShader21(),
            new SampleGdiShader22(),
        };

        public Form1()
        {
            InitializeComponent();

            listBox1.Items.AddRange(shaders);
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            if (shaderRenderer1.shader != null)
            {
                shaderRenderer1.OnStop += shaderRenderer1_OnStop;
                ShaderRenderer.Stop();
            }
            else
                shaderRenderer1_OnStop(sender, e);
        }

        void shaderRenderer1_OnStop(object sender, EventArgs e)
        {
            shaderRenderer1.OnStop -= shaderRenderer1_OnStop;
            if (listBox1.InvokeRequired)
                shaderRenderer1.shader = (GdiShader)listBox1.Invoke(new Func<GdiShader>(() => { return listBox1.SelectedItem as GdiShader; }));
            else
                shaderRenderer1.shader = listBox1.SelectedItem as GdiShader;
            shaderRenderer1.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            ShaderRenderer.Stop();
        }
    }
}
