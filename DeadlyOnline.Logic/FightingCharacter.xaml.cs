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
    /// FightingCharactor.xaml の相互作用ロジック
    /// </summary>
    public partial class FightingCharacter : UserControl
    {
        private static readonly Type _typeofThis = typeof(FightingCharacter);

        public static readonly DependencyProperty CharacterProperty =
            DependencyProperty.Register(nameof(Character), typeof(CharaBaseData), _typeofThis,
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnCharacterChanged));


        private static void OnCharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FightingCharacter fc = d as FightingCharacter;
            Grid grid = fc.FightCharacterGrid;

            if (e.NewValue is CharaBaseData chara)
            {
                double hpRatio = (double)chara.HP / (double)chara.MaxHP;
                double spdGageRatio = (double)chara.SPDGage / (double)Constants.SPDGageMax;

                fc.LifeMThresholdTrans.Offset = fc.LifeMThresholdColored.Offset = hpRatio;
                fc.SPDMThresholdTrans.Offset = fc.SPDMThresholdColored.Offset = 1 - spdGageRatio;

                if (!grid.IsVisible)
                {
                    grid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (grid.IsVisible)
                {
                    grid.Visibility = Visibility.Hidden;
                }
            }
        }

        public CharaBaseData Character
        {
            get => (CharaBaseData)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        public FightingCharacter()
        {
            InitializeComponent();
            FightCharacterGrid.Visibility = Visibility.Hidden;
        }

        public void RefrectCharacterChange()
        {
            var tmpChara = Character;
            Character = null;
            Character = tmpChara;
        }
    }
}
