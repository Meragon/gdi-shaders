namespace GdiShaders
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using GdiShaders.Core;
    using GdiShaders.Examples;

    public partial class Form1 : Form
    {
        public object[] shaders = 
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
            new SampleGdiShader23(),
            new SampleGdiShader24(),
            new SampleGdiShader25(),
            new SampleGdiShader26(),
            new SampleGdiShader27(),
            new SampleGdiShader28(),
            new SampleGdiShader29(),
            new SampleGdiShader30(),
            new SampleGdiShader31(),
            new SampleGdiShader32(),
            new SampleGdiShader33(),
            new SampleGdiShader34(),
            new SampleGdiShader35(),
            new SampleGdiShader36(),
        };

        public Form1()
        {
            InitializeComponent();

            listBox1.Items.AddRange(shaders);
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            ShaderRenderer.Stop();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
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
        private void shaderRenderer1_OnStop(object sender, EventArgs e)
        {
            shaderRenderer1.OnStop -= shaderRenderer1_OnStop;
            
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)(() => shaderRenderer1.shader = listBox1.SelectedItem as GdiShader));
            }
            else
            {
                shaderRenderer1.shader = listBox1.SelectedItem as GdiShader;
            }
            shaderRenderer1.Start();
        }
        private void checkBoxFixedStep_CheckedChanged(object sender, EventArgs e)
        {
            shaderRenderer1.fixedStep = checkBoxFixedStep.Checked;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            shaderRenderer1.scale = (float)numericUpDown1.Value;
            shaderRenderer1.Refresh();
        }
    }
}
