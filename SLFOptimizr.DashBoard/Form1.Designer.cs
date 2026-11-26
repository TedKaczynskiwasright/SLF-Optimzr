namespace SLFOptimizr.DashBoard
{
    partial class Optimizr
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            guna2ControlBox3 = new Guna.UI2.WinForms.Guna2ControlBox();
            lblFps = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(components);
            SuspendLayout();
            // 
            // guna2ControlBox1
            // 
            guna2ControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            guna2ControlBox1.BackColor = Color.FloralWhite;
            guna2ControlBox1.CustomizableEdges = customizableEdges1;
            guna2ControlBox1.FillColor = Color.Red;
            guna2ControlBox1.IconColor = Color.White;
            guna2ControlBox1.Location = new Point(137, 12);
            guna2ControlBox1.Name = "guna2ControlBox1";
            guna2ControlBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2ControlBox1.Size = new Size(45, 29);
            guna2ControlBox1.TabIndex = 0;
            // 
            // guna2ControlBox2
            // 
            guna2ControlBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            guna2ControlBox2.BackColor = Color.FloralWhite;
            guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            guna2ControlBox2.CustomizableEdges = customizableEdges3;
            guna2ControlBox2.FillColor = Color.FromArgb(255, 128, 0);
            guna2ControlBox2.IconColor = Color.White;
            guna2ControlBox2.Location = new Point(29, 12);
            guna2ControlBox2.Name = "guna2ControlBox2";
            guna2ControlBox2.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2ControlBox2.Size = new Size(45, 29);
            guna2ControlBox2.TabIndex = 1;
            // 
            // guna2ControlBox3
            // 
            guna2ControlBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            guna2ControlBox3.BackColor = Color.FloralWhite;
            guna2ControlBox3.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox;
            guna2ControlBox3.CustomizableEdges = customizableEdges5;
            guna2ControlBox3.FillColor = Color.Black;
            guna2ControlBox3.IconColor = Color.White;
            guna2ControlBox3.Location = new Point(80, 12);
            guna2ControlBox3.Name = "guna2ControlBox3";
            guna2ControlBox3.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2ControlBox3.Size = new Size(45, 29);
            guna2ControlBox3.TabIndex = 2;
            // 
            // lblFps
            // 
            lblFps.BackColor = Color.Transparent;
            lblFps.Font = new Font("Magic Sound", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFps.ForeColor = Color.White;
            lblFps.Location = new Point(29, 68);
            lblFps.Name = "lblFps";
            lblFps.Size = new Size(153, 45);
            lblFps.TabIndex = 3;
            lblFps.Text = "FPS: <b>0.0</b>";
            // 
            // guna2DragControl1
            // 
            guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            guna2DragControl1.UseTransparentDrag = true;
            // 
            // Optimizr
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(194, 125);
            Controls.Add(lblFps);
            Controls.Add(guna2ControlBox3);
            Controls.Add(guna2ControlBox2);
            Controls.Add(guna2ControlBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Optimizr";
            RightToLeft = RightToLeft.No;
            Text = "SLF-Optimzr";
            ResumeLayout(false);
            PerformLayout();
        }

        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox3;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFps;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
    }
}
