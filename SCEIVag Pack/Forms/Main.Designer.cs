﻿namespace SCEIVag_Pack
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirIECToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarComoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iECToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarVAGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.treevmenuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigEndianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.littleEndianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.containerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairuniccon = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairTudoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.recriarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirListaDeArquivosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selecionarPlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ativadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctx_Iecs = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.scei_layout = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.animpanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.animlbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.ctx_Iecs.SuspendLayout();
            this.scei_layout.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.animpanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem,
            this.iECToolStripMenuItem,
            this.containerToolStripMenuItem,
            this.sobreToolStripMenuItem,
            this.selecionarPlayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(644, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirBINToolStripMenuItem,
            this.abrirIECToolStripMenuItem,
            this.salvarToolStripMenuItem,
            this.salvarComoToolStripMenuItem,
            this.fecharToolStripMenuItem,
            this.sairToolStripMenuItem});
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.arquivoToolStripMenuItem.Text = "File";
            // 
            // abrirBINToolStripMenuItem
            // 
            this.abrirBINToolStripMenuItem.Name = "abrirBINToolStripMenuItem";
            this.abrirBINToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.abrirBINToolStripMenuItem.Text = "Open Container";
            this.abrirBINToolStripMenuItem.Click += new System.EventHandler(this.abrirBINToolStripMenuItem_Click);
            // 
            // abrirIECToolStripMenuItem
            // 
            this.abrirIECToolStripMenuItem.Name = "abrirIECToolStripMenuItem";
            this.abrirIECToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.abrirIECToolStripMenuItem.Text = "Open BHD";
            this.abrirIECToolStripMenuItem.Click += new System.EventHandler(this.abrirIECToolStripMenuItem_Click);
            // 
            // salvarToolStripMenuItem
            // 
            this.salvarToolStripMenuItem.Enabled = false;
            this.salvarToolStripMenuItem.Name = "salvarToolStripMenuItem";
            this.salvarToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salvarToolStripMenuItem.Text = "Save";
            this.salvarToolStripMenuItem.Click += new System.EventHandler(this.salvarToolStripMenuItem_Click);
            // 
            // salvarComoToolStripMenuItem
            // 
            this.salvarComoToolStripMenuItem.Enabled = false;
            this.salvarComoToolStripMenuItem.Name = "salvarComoToolStripMenuItem";
            this.salvarComoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salvarComoToolStripMenuItem.Text = "Save As";
            this.salvarComoToolStripMenuItem.Click += new System.EventHandler(this.salvarComoToolStripMenuItem_Click);
            // 
            // fecharToolStripMenuItem
            // 
            this.fecharToolStripMenuItem.Enabled = false;
            this.fecharToolStripMenuItem.Name = "fecharToolStripMenuItem";
            this.fecharToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fecharToolStripMenuItem.Text = "Close";
            this.fecharToolStripMenuItem.Click += new System.EventHandler(this.fecharToolStripMenuItem_Click);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sairToolStripMenuItem.Text = "Exit";
            this.sairToolStripMenuItem.Click += new System.EventHandler(this.sairToolStripMenuItem_Click);
            // 
            // iECToolStripMenuItem
            // 
            this.iECToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrairToolStripMenuItem,
            this.importarVAGToolStripMenuItem,
            this.extrairTodosToolStripMenuItem,
            this.toolStripMenuItem3,
            this.treevmenuitem,
            this.addToolStripMenuItem});
            this.iECToolStripMenuItem.Name = "iECToolStripMenuItem";
            this.iECToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.iECToolStripMenuItem.Text = "SCEI";
            // 
            // extrairToolStripMenuItem
            // 
            this.extrairToolStripMenuItem.Enabled = false;
            this.extrairToolStripMenuItem.Name = "extrairToolStripMenuItem";
            this.extrairToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.extrairToolStripMenuItem.Text = "Extract VAG";
            this.extrairToolStripMenuItem.Click += new System.EventHandler(this.extrairToolStripMenuItem_Click);
            // 
            // importarVAGToolStripMenuItem
            // 
            this.importarVAGToolStripMenuItem.Enabled = false;
            this.importarVAGToolStripMenuItem.Name = "importarVAGToolStripMenuItem";
            this.importarVAGToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.importarVAGToolStripMenuItem.Text = "Import VAG";
            this.importarVAGToolStripMenuItem.Click += new System.EventHandler(this.importarVAGToolStripMenuItem_Click);
            // 
            // extrairTodosToolStripMenuItem
            // 
            this.extrairTodosToolStripMenuItem.Enabled = false;
            this.extrairTodosToolStripMenuItem.Name = "extrairTodosToolStripMenuItem";
            this.extrairTodosToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.extrairTodosToolStripMenuItem.Text = "Extract All";
            this.extrairTodosToolStripMenuItem.Click += new System.EventHandler(this.extrairTodosToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(159, 22);
            this.toolStripMenuItem3.Text = "VAG Endianess";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Checked = true;
            this.toolStripMenuItem4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(139, 22);
            this.toolStripMenuItem4.Text = "Big Endian";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(139, 22);
            this.toolStripMenuItem5.Text = "Little Endian";
            // 
            // treevmenuitem
            // 
            this.treevmenuitem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bigEndianToolStripMenuItem,
            this.littleEndianToolStripMenuItem});
            this.treevmenuitem.Enabled = false;
            this.treevmenuitem.Name = "treevmenuitem";
            this.treevmenuitem.Size = new System.Drawing.Size(159, 22);
            this.treevmenuitem.Text = "TreeView";
            // 
            // bigEndianToolStripMenuItem
            // 
            this.bigEndianToolStripMenuItem.Name = "bigEndianToolStripMenuItem";
            this.bigEndianToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.bigEndianToolStripMenuItem.Text = "Expand All";
            this.bigEndianToolStripMenuItem.Click += new System.EventHandler(this.Expand_Clicl);
            // 
            // littleEndianToolStripMenuItem
            // 
            this.littleEndianToolStripMenuItem.Name = "littleEndianToolStripMenuItem";
            this.littleEndianToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.littleEndianToolStripMenuItem.Text = "Collapse All";
            this.littleEndianToolStripMenuItem.Click += new System.EventHandler(this.Collapse_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Enabled = false;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addToolStripMenuItem.Text = "Add/Edit Entries";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // containerToolStripMenuItem
            // 
            this.containerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrairuniccon,
            this.extrairTudoToolStripMenuItem,
            this.fecharToolStripMenuItem1,
            this.recriarToolStripMenuItem,
            this.abrirListaDeArquivosToolStripMenuItem});
            this.containerToolStripMenuItem.Name = "containerToolStripMenuItem";
            this.containerToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.containerToolStripMenuItem.Text = "Container";
            // 
            // extrairuniccon
            // 
            this.extrairuniccon.Enabled = false;
            this.extrairuniccon.Name = "extrairuniccon";
            this.extrairuniccon.Size = new System.Drawing.Size(142, 22);
            this.extrairuniccon.Text = "Extract";
            this.extrairuniccon.Click += new System.EventHandler(this.extrairuniccon_Click);
            // 
            // extrairTudoToolStripMenuItem
            // 
            this.extrairTudoToolStripMenuItem.Enabled = false;
            this.extrairTudoToolStripMenuItem.Name = "extrairTudoToolStripMenuItem";
            this.extrairTudoToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.extrairTudoToolStripMenuItem.Text = "Extract All";
            this.extrairTudoToolStripMenuItem.Click += new System.EventHandler(this.extrairTudoToolStripMenuItem_Click);
            // 
            // fecharToolStripMenuItem1
            // 
            this.fecharToolStripMenuItem1.Enabled = false;
            this.fecharToolStripMenuItem1.Name = "fecharToolStripMenuItem1";
            this.fecharToolStripMenuItem1.Size = new System.Drawing.Size(142, 22);
            this.fecharToolStripMenuItem1.Text = "Close";
            this.fecharToolStripMenuItem1.Click += new System.EventHandler(this.fecharToolStripMenuItem1_Click);
            // 
            // recriarToolStripMenuItem
            // 
            this.recriarToolStripMenuItem.Name = "recriarToolStripMenuItem";
            this.recriarToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.recriarToolStripMenuItem.Text = "Rebuild";
            this.recriarToolStripMenuItem.Click += new System.EventHandler(this.recriarToolStripMenuItem_Click);
            // 
            // abrirListaDeArquivosToolStripMenuItem
            // 
            this.abrirListaDeArquivosToolStripMenuItem.Enabled = false;
            this.abrirListaDeArquivosToolStripMenuItem.Name = "abrirListaDeArquivosToolStripMenuItem";
            this.abrirListaDeArquivosToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.abrirListaDeArquivosToolStripMenuItem.Text = "Open FileList";
            this.abrirListaDeArquivosToolStripMenuItem.Visible = false;
            this.abrirListaDeArquivosToolStripMenuItem.Click += new System.EventHandler(this.abrirListaDeArquivosToolStripMenuItem_Click);
            // 
            // sobreToolStripMenuItem
            // 
            this.sobreToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            this.sobreToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.sobreToolStripMenuItem.Text = "About";
            this.sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // selecionarPlayToolStripMenuItem
            // 
            this.selecionarPlayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ativadoToolStripMenuItem});
            this.selecionarPlayToolStripMenuItem.Name = "selecionarPlayToolStripMenuItem";
            this.selecionarPlayToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.selecionarPlayToolStripMenuItem.Text = "Options";
            this.selecionarPlayToolStripMenuItem.Click += new System.EventHandler(this.selecionarPlayToolStripMenuItem_Click);
            // 
            // ativadoToolStripMenuItem
            // 
            this.ativadoToolStripMenuItem.Checked = true;
            this.ativadoToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ativadoToolStripMenuItem.Name = "ativadoToolStripMenuItem";
            this.ativadoToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.ativadoToolStripMenuItem.Text = "Auto Play";
            this.ativadoToolStripMenuItem.Click += new System.EventHandler(this.ativadoToolStripMenuItem_Click);
            // 
            // ctx_Iecs
            // 
            this.ctx_Iecs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.ctx_Iecs.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.ctx_Iecs.Name = "ctx_Iecs";
            this.ctx_Iecs.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ctx_Iecs.ShowItemToolTips = false;
            this.ctx_Iecs.Size = new System.Drawing.Size(136, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem1.Text = "Extract VAG";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.extrairToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuItem2.Text = "Import VAG";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.importarVAGToolStripMenuItem_Click);
            // 
            // scei_layout
            // 
            this.scei_layout.ColumnCount = 2;
            this.scei_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.62733F));
            this.scei_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.37267F));
            this.scei_layout.Controls.Add(this.treeView1, 0, 0);
            this.scei_layout.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.scei_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scei_layout.Location = new System.Drawing.Point(0, 24);
            this.scei_layout.Name = "scei_layout";
            this.scei_layout.RowCount = 1;
            this.scei_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.scei_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 434F));
            this.scei_layout.Size = new System.Drawing.Size(644, 434);
            this.scei_layout.TabIndex = 1;
            this.scei_layout.Visible = false;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(378, 428);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.propertyGrid1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(387, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84.11215F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.88785F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(254, 428);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(248, 354);
            this.propertyGrid1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 363);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 62);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " ";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(100, 16);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(55, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // animpanel
            // 
            this.animpanel.Controls.Add(this.tableLayoutPanel1);
            this.animpanel.Location = new System.Drawing.Point(0, 24);
            this.animpanel.Name = "animpanel";
            this.animpanel.Size = new System.Drawing.Size(644, 434);
            this.animpanel.TabIndex = 2;
            this.animpanel.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.animlbl, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.646302F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.3537F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(644, 434);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // animlbl
            // 
            this.animlbl.AutoSize = true;
            this.animlbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.animlbl.Font = new System.Drawing.Font("Leelawadee", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.animlbl.Location = new System.Drawing.Point(3, 311);
            this.animlbl.Name = "animlbl";
            this.animlbl.Size = new System.Drawing.Size(638, 56);
            this.animlbl.TabIndex = 2;
            this.animlbl.Text = "Carregando as entradas...";
            this.animlbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::SCEIVag_Pack.Properties.Resources.FMvz;
            this.pictureBox1.Location = new System.Drawing.Point(3, 33);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(638, 275);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SCEIVag_Pack.Properties.Resources.scei_vag_toolnewart;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(644, 458);
            this.Controls.Add(this.animpanel);
            this.Controls.Add(this.scei_layout);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "SCEI Audio Pack Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ctx_Iecs.ResumeLayout(false);
            this.scei_layout.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.animpanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem containerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recriarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iECToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem extrairTudoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrairTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem extrairuniccon;
        private System.Windows.Forms.ToolStripMenuItem fecharToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem abrirListaDeArquivosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarComoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selecionarPlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ativadoToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem extrairToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem importarVAGToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip ctx_Iecs;
        private System.Windows.Forms.ToolStripMenuItem bigEndianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem littleEndianToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel animpanel;
        public System.Windows.Forms.TreeView treeView1;
        public System.Windows.Forms.PropertyGrid propertyGrid1;
        public System.Windows.Forms.LinkLabel linkLabel1;
        public System.Windows.Forms.TableLayoutPanel scei_layout;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        public System.Windows.Forms.ToolStripMenuItem treevmenuitem;
        public System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Label animlbl;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

