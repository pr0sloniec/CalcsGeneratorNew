using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalcsGenerator.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChargeInputControl.xaml
    /// </summary>
    public partial class ChargeInputControl : UserControl
    {
        bool check = false;
        public bool Checked {
            get
            {
                return check;
            }
            set
            {
                if (!value)
                {
                    TextItem.Opacity = 0.5;
                    TextBoxItem.Opacity = 0.5;
                    CalcButton.Opacity = 0.5;
                    Value = 0;
                    TextBoxItem.Text = "0";
                    ValueChanged?.Invoke();
                    check = false;
                }
                else
                {
                    TextItem.Opacity = 1;
                    TextBoxItem.Opacity = 1;
                    CalcButton.Opacity = 1;
                    check = true;
                }
            }

        }

        public Action ValueChanged { get; set; }
        public Action ShowInfo { get; set; }

        public int Value { get; set; }
        public string PlaceHolder { set { TextItem.Text = value; } }

        public void LoadData(int value)
        {
            CheckBoxItem.IsChecked = true;
            check = true;
            TextItem.Opacity = 1;
            TextBoxItem.Opacity = 1;
            CalcButton.Opacity = 1;
            TextBoxItem.Text = value.ToString();
            Value = value;
        }

        public ChargeInputControl()
        {
            InitializeComponent();
            Checked = false;
            Value = 0;
        }

        private void CheckBoxItem_Click(object sender, RoutedEventArgs e)
        {
            Checked = !Checked;
        }

        private void ValidateButton(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("([1-9][0-9]?|100)");
            if (regex.IsMatch(TextBoxItem.Text))
            {
                Value = int.Parse(TextBoxItem.Text);
                ValueChanged.Invoke();
            }
            else
            {
                Interaction.MsgBox("Проверьте ввод!");
            }
        }

        private void OpenInfoDialog(object sender, MouseButtonEventArgs e)
        {
            ShowInfo?.Invoke();
        }
    }
}
