using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// MiniCharacter.xaml の相互作用ロジック
    /// </summary>
    public partial class MiniCharacter : UserControl
    {
        public ImageSource Source
        {
            get => CharacterImage.Source;
            set => CharacterImage.Source = value;
        }

        public string CharacterName
        {
            get => (string)NameLabel.Content;
            set => NameLabel.Content = value;
        }

        public Brush TextForeground
        {
            get => NameLabel.Foreground;
            set => NameLabel.Foreground = value;
        }

        public Brush TextBackground
        {
            get => BackTextLabel.Foreground;
            set => BackTextLabel.Foreground = value;
        }

        public MiniCharacter()
        {
            InitializeComponent();
        }
    }
}
