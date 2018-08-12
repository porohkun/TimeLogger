using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for TagsControl.xaml
    /// </summary>
    public partial class TagsControl : UserControl
    {
        public static readonly DependencyProperty TagsProperty = DependencyProperty.Register("Tags",
              typeof(ObservableCollection<Label>),
              typeof(TagsControl),
              new FrameworkPropertyMetadata(new ObservableCollection<Label>()) { BindsTwoWayByDefault = true });

        public ObservableCollection<Label> Tags
        {
            get => (ObservableCollection<Label>)GetValue(TagsProperty);
            set => SetValue(TagsProperty, value);
        }

        public event Action<Label> TagAdded;
        public event Action<Label> TagRemoved;

        public TagsControl()
        {
            InitializeComponent();
        }

        private void RefreshTags()
        {
            if (newTagBox.Editor != null)
                newTagBox.Editor.Text = "";
        }

        //private void SetTagToButton(Button button, Label label)
        //{
        //    button.Content = label.Name;
        //    button.Tag = label;
        //}

        //private Button NewButton(Label label)
        //{
        //    var button = new Button();
        //    button.Margin = new Thickness(3);
        //    button.Loaded += Button_Loaded;
        //    button.Click += Button_Click;
        //    SetTagToButton(button, label);
        //    button.ContextMenu = Resources["popup"] as ContextMenu;
        //    return button;
        //}

        //private void Button_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var button = (Button)sender;
        //    ((Border)button.Template.FindName("Part_Border", button)).CornerRadius = new CornerRadius(4);
        //    ((ContentPresenter)button.Template.FindName("Part_ContentPresenter", button)).Margin = new Thickness(3, 0, 3, 0);
        //}

        private void ButtonPopup_Click(object sender, RoutedEventArgs e)
        {
            var lab = (((sender as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag as Label;
            Tags.Remove(lab);
            TagRemoved?.Invoke(lab);
            RefreshTags();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var popup = (sender as Button).ContextMenu;
            popup.PlacementTarget = (UIElement)sender;
            popup.Visibility = Visibility.Visible;
            popup.IsOpen = true;
        }

        private void newTagBox_Loaded(object sender, RoutedEventArgs e)
        {
            newTagBox.ApplyTemplate();
            newTagBox.Editor.TextChanged += Editor_TextChanged;
            newTagBox.Editor.KeyDown += Editor_KeyDown;
        }

        private void ProcessTags(string text)
        {
            var tags = text.Split(',', ';', ':', ' ', '/', '\\', '|').Distinct();
            foreach (var tag in tags)
                if (!string.IsNullOrEmpty(tag))
                {
                    var lab = Label.GetLabelByName(tag);
                    if (!Tags.Any(l => l.Name == lab.Name))
                    {
                        Tags.Add(lab);
                        TagAdded?.Invoke(lab);
                    }
                }
            Label.Labels.Sort();
            RefreshTags();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tags != null)
            {
                var text = ((TextBox)sender).Text;
                if (Regex.IsMatch(text, "[,;:/\\| ]", RegexOptions.IgnoreCase))
                {
                    ProcessTags(text);
                }
            }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessTags(((TextBox)sender).Text);
            }
        }

        private void newTagBox_SelectionAdapterCommit()
        {
            ProcessTags(newTagBox.Editor.Text);
        }
    }
}
