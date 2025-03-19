using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SCEIVag_Pack.Forms
{
    public partial class AddForm : Form
    {
        public static Main MainForm;
        public AddForm(Main f1)
        {
            MainForm = f1;
            InitializeComponent();

        }
        public class Duplicate : INotifyPropertyChanged
        {
           
            public class Folders : StringConverter
            {
                public static List<string> data 
                { 
                    get
                    {
                        return Enumerable.Range(0, MainForm.treeView1.Nodes.Count).Select(
                            x => MainForm.treeView1.Nodes[x].Text
                            ).ToList();
                    }
                    set { }
                }

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true; // Habilita a lista suspensa
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return true; // Restringe a escolha às opções da lista
                }

                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    return new StandardValuesCollection(data);
                }
            }
            public string Choosed = "Select one entry...";
            public IECS.Prog.Entry Selected;

            [Category("Choosed Entry")]
            [DisplayName("Entry")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.Prog.Entry ChoosedEntry
            {
                get
                {
                    OnPropertyChanged(nameof(ChoosedEntry));
                    return Selected;
                }
                set => Selected = value;
            }

            [   Category("Duplicate Entry"),
                Description("Choose one of these entries to duplicate."),
                TypeConverter(typeof(Folders))]
            public string Entries
            {
                get => Choosed;
                set
                {
                    Choosed = value;

                    // Adiciona uma nova propriedade quando "Item 1" for selecionado
                    if (Choosed != null)
                    {
                        Selected = MainForm.treeView1.Nodes.Cast<SCEINode>().Where
                            (x=> x.Text == Choosed).ToArray()[0].Entry;
                        
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


        }
        public class Entry 
        {
            [Category("Edit Program Entries"),
            DisplayName("Entry"),
                Description("Visualize and edit entries array."),
            TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.Prog.Entry[] Entries
            {
                get => MainForm.sceifile._Program.Entries;
                set => MainForm.sceifile._Program.Entries = value;
            }


        }
        public class DuplicateSSet : INotifyPropertyChanged
        {
            public class Folders : StringConverter
            {
                public static List<string> data
                {
                    get
                    {
                        return Enumerable.Range(0, MainForm.sceifile._SampleSets.SampleSet_Count).Select(
                            x => $"SampleSet_{x}"
                            ).ToList();
                    }
                    set { }
                }

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true; // Habilita a lista suspensa
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return true; // Restringe a escolha às opções da lista
                }

                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    return new StandardValuesCollection(data);
                }
            }
            public string Choosed = "Select one entry...";
            public IECS.Sset.SampleSet Selected;

            [Category("Choosed Entry")]
            [DisplayName("Entry")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.Sset.SampleSet ChoosedEntry
            {
                get
                {
                    OnPropertyChanged(nameof(ChoosedEntry));
                    return Selected;
                }
                set => Selected = value;
            }

            [Category("Duplicate SampleSet"),
                Description("Choose one of these entries to duplicate."),
                TypeConverter(typeof(Folders))]
            public string Entries
            {
                get => Choosed;
                set
                {
                    Choosed = value;

                    // Adiciona uma nova propriedade quando "Item 1" for selecionado
                    if (Choosed != null)
                    {
                        Selected = MainForm.sceifile._SampleSets.SampleSets[Convert.ToInt32(Choosed.Split(new string[] 
                        {"SampleSet_" }, StringSplitOptions.RemoveEmptyEntries)[0])];

                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class SSet
        {
            [Category("Edit SampleSets"),
            DisplayName("SampleSets"),
                Description("Visualize and edit SampleSets array."),
            TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.Sset.SampleSet[] SampleSets
            {
                get => MainForm.sceifile._SampleSets.SampleSets;
                set => MainForm.sceifile._SampleSets.SampleSets = value;
            }
        }
        public class DuplicateSample : INotifyPropertyChanged
        {
            public class Folders : StringConverter
            {
                public static List<string> data
                {
                    get
                    {
                        return Enumerable.Range(0, MainForm.sceifile._Samples.Samples_Count).Select(
                            x => $"Sample_{x}"
                            ).ToList();
                    }
                    set { }
                }

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
                {
                    return true; // Habilita a lista suspensa
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
                {
                    return true; // Restringe a escolha às opções da lista
                }

                public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    return new StandardValuesCollection(data);
                }
            }
            public string Choosed = "Select one entry...";
            public IECS.VagSamples.Sample Selected;

            [Category("Choosed Entry")]
            [DisplayName("Entry")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.VagSamples.Sample ChoosedEntry
            {
                get
                {
                    OnPropertyChanged(nameof(ChoosedEntry));
                    return Selected;
                }
                set => Selected = value;
            }

            [Category("Duplicate Sample"),
                Description("Choose one of these entries to duplicate."),
                TypeConverter(typeof(Folders))]
            public string Entries
            {
                get => Choosed;
                set
                {
                    Choosed = value;

                    // Adiciona uma nova propriedade quando "Item 1" for selecionado
                    if (Choosed != null)
                    {
                        Selected = MainForm.sceifile._Samples.VAG_Samples[Convert.ToInt32(Choosed.Split(new string[]
                        {"Sample_" }, StringSplitOptions.RemoveEmptyEntries)[0])];

                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class Sample
        {
            [Category("Edit Samples"),
            DisplayName("Samples"),
                Description("Visualize and edit Samples array."),
            TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.VagSamples.Sample[] Samples
            {
                get => MainForm.sceifile._Samples.VAG_Samples;
                set => MainForm.sceifile._Samples.VAG_Samples = value;
            }
        }
        public class VAG_Info
        {
            [Category("Edit Infos"),
            DisplayName("Vags"),
                Description("Visualize and edit VAGs audios array."),
            TypeConverter(typeof(ExpandableObjectConverter))]
            public IECS.VAGInfo.Information[] Infos
            {
                get => MainForm.sceifile._Infos.VAG_Infos;
                set => MainForm.sceifile._Infos.VAG_Infos = value;
            }
            
        }


        public Type_Add Selected_Add;
        public enum Type_Add : int
        {
            DuplicateEntry = 0,
            EditEntry = 1,
            DuplicateSampleSet = 2,
            EditSset = 3,
            DuplicateSample = 4,
            EditSample = 5,
            EditVAG = 6
        };

        #region Functions
        void HideGrid() { if (add_box.Visible == true) add_box.Visible = false; }
        void ShowGrid() { if (add_box.Visible == false) add_box.Visible = true; }
        void ClearGrid() { if(add_box.SelectedObject!=null)add_box.SelectedObject = null; }
        void PopulateGrid()
        {
            ShowGrid();
            ClearGrid();
            switch (Selected_Add)
            {
                case Type_Add.DuplicateEntry:
                    Duplicate dup = new Duplicate();
                    add_box.SelectedObject = dup;
                    break;

                case Type_Add.EditEntry:
                    Entry ent = new Entry();
                    add_box.SelectedObject = ent;
                    break;

                case Type_Add.DuplicateSampleSet:
                    DuplicateSSet duplicateSSet = new DuplicateSSet();
                    add_box.SelectedObject = duplicateSSet;
                    break;

                case Type_Add.EditSset:
                    SSet sset = new SSet();
                    add_box.SelectedObject = sset;
                    break;

                case Type_Add.DuplicateSample:
                    DuplicateSample duplicateSample = new DuplicateSample();
                    add_box.SelectedObject = duplicateSample;
                    break;
                case Type_Add.EditSample:
                    Sample sample = new Sample();
                    add_box.SelectedObject = sample;
                    break;
                case Type_Add.EditVAG:
                    VAG_Info vag = new VAG_Info();
                    add_box.SelectedObject = vag;
                    break;
            }
        }

        #endregion

        #region Buttons and Events
        private void AddForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void add_bt_Click(object sender, EventArgs e)
        {
            List<string> list_files = new List<string>();
            switch (Selected_Add)
            {
                case Type_Add.DuplicateEntry:
                    Duplicate dup = new Duplicate();
                    dup = add_box.SelectedObject as Duplicate;
                    MainForm.sceifile._Program.Add(dup.ChoosedEntry);
                    list_files = new List<string>();
                    list_files.AddRange(MainForm.FileList[MainForm.listadeIECS.listView1.SelectedItems[0].SubItems[1].Text]);
                    list_files.Add($"S_FOLDER_{list_files.Count}");
                    MainForm.FileList[MainForm.listadeIECS.listView1.SelectedItems[0].SubItems[1].Text] = list_files.ToArray();

                    break;

                case Type_Add.EditEntry:
                    Entry ent = new Entry();
                    ent = add_box.SelectedObject as Entry;
                    break;

                case Type_Add.DuplicateSampleSet:
                    DuplicateSSet duplicateSSet = new DuplicateSSet();
                    duplicateSSet = add_box.SelectedObject as DuplicateSSet;
                    MainForm.sceifile._SampleSets.Add(duplicateSSet.ChoosedEntry);
                    break;

                case Type_Add.EditSset:
                    SSet sset = new SSet();
                    sset = add_box.SelectedObject as SSet;
                    break;

                case Type_Add.DuplicateSample:
                    DuplicateSample duplicateSample = new DuplicateSample();
                    duplicateSample = add_box.SelectedObject as DuplicateSample;
                    MainForm.sceifile._Samples.Add(duplicateSample.ChoosedEntry);
                    break;
                case Type_Add.EditSample:
                    Sample sample = new Sample();
                    sample = add_box.SelectedObject as Sample;
                    break;
                case Type_Add.EditVAG:
                    VAG_Info vag = new VAG_Info();
                    vag = add_box.SelectedObject as VAG_Info;

                    break;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void canc_bt_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
   
        private void type_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected_Add = (Type_Add)type_select.SelectedIndex;
            PopulateGrid();
        }
        #endregion

        private void add_box_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
        }
    }
}
