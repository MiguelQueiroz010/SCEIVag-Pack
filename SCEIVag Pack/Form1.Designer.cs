namespace SCEIVag_Pack
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirIECToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iECToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarVAGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.containerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairTudoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recriarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vagnlabel = new System.Windows.Forms.Label();
            this.verslabel = new System.Windows.Forms.Label();
            this.filenamelabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.extrairuniccon = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem,
            this.iECToolStripMenuItem,
            this.containerToolStripMenuItem,
            this.sobreToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(412, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirBINToolStripMenuItem,
            this.abrirIECToolStripMenuItem,
            this.salvarToolStripMenuItem,
            this.fecharToolStripMenuItem,
            this.sairToolStripMenuItem});
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.arquivoToolStripMenuItem.Text = "Arquivo";
            // 
            // abrirBINToolStripMenuItem
            // 
            this.abrirBINToolStripMenuItem.Name = "abrirBINToolStripMenuItem";
            this.abrirBINToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.abrirBINToolStripMenuItem.Text = "Abrir BINContainer";
            this.abrirBINToolStripMenuItem.Click += new System.EventHandler(this.abrirBINToolStripMenuItem_Click);
            // 
            // abrirIECToolStripMenuItem
            // 
            this.abrirIECToolStripMenuItem.Name = "abrirIECToolStripMenuItem";
            this.abrirIECToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.abrirIECToolStripMenuItem.Text = "Abrir IEC";
            this.abrirIECToolStripMenuItem.Click += new System.EventHandler(this.abrirIECToolStripMenuItem_Click);
            // 
            // salvarToolStripMenuItem
            // 
            this.salvarToolStripMenuItem.Enabled = false;
            this.salvarToolStripMenuItem.Name = "salvarToolStripMenuItem";
            this.salvarToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.salvarToolStripMenuItem.Text = "Salvar";
            this.salvarToolStripMenuItem.Click += new System.EventHandler(this.salvarToolStripMenuItem_Click);
            // 
            // fecharToolStripMenuItem
            // 
            this.fecharToolStripMenuItem.Enabled = false;
            this.fecharToolStripMenuItem.Name = "fecharToolStripMenuItem";
            this.fecharToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.fecharToolStripMenuItem.Text = "Fechar";
            this.fecharToolStripMenuItem.Click += new System.EventHandler(this.fecharToolStripMenuItem_Click);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.sairToolStripMenuItem.Text = "Sair";
            this.sairToolStripMenuItem.Click += new System.EventHandler(this.sairToolStripMenuItem_Click);
            // 
            // iECToolStripMenuItem
            // 
            this.iECToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrairToolStripMenuItem,
            this.importarVAGToolStripMenuItem,
            this.extrairTodosToolStripMenuItem});
            this.iECToolStripMenuItem.Name = "iECToolStripMenuItem";
            this.iECToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.iECToolStripMenuItem.Text = "SCEI";
            // 
            // extrairToolStripMenuItem
            // 
            this.extrairToolStripMenuItem.Enabled = false;
            this.extrairToolStripMenuItem.Name = "extrairToolStripMenuItem";
            this.extrairToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.extrairToolStripMenuItem.Text = "Extrair VAG";
            this.extrairToolStripMenuItem.Click += new System.EventHandler(this.extrairToolStripMenuItem_Click);
            // 
            // importarVAGToolStripMenuItem
            // 
            this.importarVAGToolStripMenuItem.Enabled = false;
            this.importarVAGToolStripMenuItem.Name = "importarVAGToolStripMenuItem";
            this.importarVAGToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.importarVAGToolStripMenuItem.Text = "Importar VAG";
            this.importarVAGToolStripMenuItem.Click += new System.EventHandler(this.importarVAGToolStripMenuItem_Click);
            // 
            // extrairTodosToolStripMenuItem
            // 
            this.extrairTodosToolStripMenuItem.Enabled = false;
            this.extrairTodosToolStripMenuItem.Name = "extrairTodosToolStripMenuItem";
            this.extrairTodosToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.extrairTodosToolStripMenuItem.Text = "Extrair Todos";
            this.extrairTodosToolStripMenuItem.Click += new System.EventHandler(this.extrairTodosToolStripMenuItem_Click);
            // 
            // containerToolStripMenuItem
            // 
            this.containerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrairuniccon,
            this.extrairTudoToolStripMenuItem,
            this.fecharToolStripMenuItem1,
            this.recriarToolStripMenuItem});
            this.containerToolStripMenuItem.Name = "containerToolStripMenuItem";
            this.containerToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.containerToolStripMenuItem.Text = "Container";
            // 
            // extrairTudoToolStripMenuItem
            // 
            this.extrairTudoToolStripMenuItem.Enabled = false;
            this.extrairTudoToolStripMenuItem.Name = "extrairTudoToolStripMenuItem";
            this.extrairTudoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extrairTudoToolStripMenuItem.Text = "Extrair Tudo";
            this.extrairTudoToolStripMenuItem.Click += new System.EventHandler(this.extrairTudoToolStripMenuItem_Click);
            // 
            // recriarToolStripMenuItem
            // 
            this.recriarToolStripMenuItem.Name = "recriarToolStripMenuItem";
            this.recriarToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.recriarToolStripMenuItem.Text = "Recriar";
            this.recriarToolStripMenuItem.Click += new System.EventHandler(this.recriarToolStripMenuItem_Click);
            // 
            // sobreToolStripMenuItem
            // 
            this.sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            this.sobreToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.sobreToolStripMenuItem.Text = "Sobre";
            this.sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 204);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(360, 227);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Índice";
            this.columnHeader1.Width = 78;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Nome";
            this.columnHeader2.Width = 177;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Frequência(Hz)";
            this.columnHeader3.Width = 95;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vagnlabel);
            this.groupBox1.Controls.Add(this.verslabel);
            this.groupBox1.Controls.Add(this.filenamelabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 149);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arquivo";
            this.groupBox1.Visible = false;
            // 
            // vagnlabel
            // 
            this.vagnlabel.AutoSize = true;
            this.vagnlabel.Location = new System.Drawing.Point(33, 102);
            this.vagnlabel.Name = "vagnlabel";
            this.vagnlabel.Size = new System.Drawing.Size(97, 13);
            this.vagnlabel.TabIndex = 2;
            this.vagnlabel.Text = "Número de Áudios:";
            // 
            // verslabel
            // 
            this.verslabel.AutoSize = true;
            this.verslabel.Location = new System.Drawing.Point(59, 73);
            this.verslabel.Name = "verslabel";
            this.verslabel.Size = new System.Drawing.Size(71, 13);
            this.verslabel.TabIndex = 1;
            this.verslabel.Text = "Pack Versão:";
            // 
            // filenamelabel
            // 
            this.filenamelabel.AutoSize = true;
            this.filenamelabel.Location = new System.Drawing.Point(15, 31);
            this.filenamelabel.Name = "filenamelabel";
            this.filenamelabel.Size = new System.Drawing.Size(38, 13);
            this.filenamelabel.TabIndex = 0;
            this.filenamelabel.Text = "Nome:";
            // 
            // timer1
            // 
            this.timer1.Interval = 2600;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(12, 387);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(360, 66);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // extrairuniccon
            // 
            this.extrairuniccon.Enabled = false;
            this.extrairuniccon.Name = "extrairuniccon";
            this.extrairuniccon.Size = new System.Drawing.Size(180, 22);
            this.extrairuniccon.Text = "Extrair";
            this.extrairuniccon.Click += new System.EventHandler(this.extrairuniccon_Click);
            // 
            // fecharToolStripMenuItem1
            // 
            this.fecharToolStripMenuItem1.Enabled = false;
            this.fecharToolStripMenuItem1.Name = "fecharToolStripMenuItem1";
            this.fecharToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.fecharToolStripMenuItem1.Text = "Fechar";
            this.fecharToolStripMenuItem1.Click += new System.EventHandler(this.fecharToolStripMenuItem1_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SCEIVag_Pack.Properties.Resources.capa;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(412, 472);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "SCEI Audio Pack Tool";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirIECToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fecharToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirBINToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label vagnlabel;
        public System.Windows.Forms.Label verslabel;
        public System.Windows.Forms.Label filenamelabel;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStripMenuItem containerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recriarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iECToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importarVAGToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem extrairTudoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrairTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarToolStripMenuItem;
        public System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ToolStripMenuItem extrairuniccon;
        private System.Windows.Forms.ToolStripMenuItem fecharToolStripMenuItem1;
    }
}

