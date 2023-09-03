using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimeLogger.Domain.Data;

namespace TimeLogger.Controls
{
    /// <summary>
    /// Interaction logic for TagsControl.xaml
    /// </summary>
    public partial class TagsControl : UserControl
    {
        [GeneratedRegex("[,;:/\\| ]", RegexOptions.IgnoreCase, "ru-RU")]
        private static partial Regex SeparatorRegex();

        public static readonly DependencyProperty TagsProperty = DependencyProperty.Register(nameof(Tags),
              typeof(ObservableCollection<Tag>),
              typeof(TagsControl),
              new FrameworkPropertyMetadata(new ObservableCollection<Tag>()) { BindsTwoWayByDefault = true });

        public ObservableCollection<Tag> Tags
        {
            get => (ObservableCollection<Tag>)GetValue(TagsProperty);
            set => SetValue(TagsProperty, value);
        }

        public event Action<Tag>? TagAdded;
        public event Action<Tag>? TagRemoved;

        public TagsControl()
        {
            InitializeComponent();
        }

        private void RefreshTags()
        {
            //if (newTagBox.Editor != null)
            //    newTagBox.Editor.Text = "";
        }

        private void ButtonPopup_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (menuItem.Parent is not ContextMenu menu) return;
            if (menu.PlacementTarget is not Button button) return;
            if (button.Tag is not Tag label) return;

            Tags.Remove(label);
            TagRemoved?.Invoke(label);
            RefreshTags();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            var popup = button.ContextMenu;
            if (popup == null) return;

            popup.PlacementTarget = button;
            popup.Visibility = Visibility.Visible;
            popup.IsOpen = true;
        }

        private void newTagBox_Loaded(object sender, RoutedEventArgs e)
        {
            //newTagBox.ApplyTemplate();
            //newTagBox.Editor.TextChanged += Editor_TextChanged;
            //newTagBox.Editor.KeyDown += Editor_KeyDown;
        }

        private void ProcessTags(string text)
        {
            //var tags = text.Split(',', ';', ':', ' ', '/', '\\', '|').Distinct();
            //foreach (var tag in tags)
            //    if (!string.IsNullOrEmpty(tag))
            //    {
            //        var lab = Tag.GetLabelByName(tag);
            //        if (!Tags.Any(l => l.Name == lab.Name))
            //        {
            //            Tags.Add(lab);
            //            TagAdded?.Invoke(lab);
            //        }
            //    }
            //Tag.Tags.Sort();
            //RefreshTags();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            if (SeparatorRegex().IsMatch(textBox.Text))
            {
                ProcessTags(textBox.Text);
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
            //ProcessTags(newTagBox.Editor.Text);
        }

    }
}
