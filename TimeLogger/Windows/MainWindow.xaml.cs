using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static MainWindow _instance;
		public static MainWindow Instance => _instance;
		private DispatcherTimer _timer;
		private DispatcherTimer _updateTimer;
		private Task _currentTask;
		private TaskPeriod _currentPeriod;
		private DateTime _lastSave = DateTime.Now;
		private bool _trueClosing = false;
		private bool _ticking = false;
		private readonly bool _hiddenStart = false;

		#region Actions

		void SelectTask()
		{
			var win = new SelectTaskWindow();
			var dialogResult = win.ShowDialog();
			if (dialogResult.HasValue && dialogResult.Value && !string.IsNullOrWhiteSpace(win.TaskId))
			{
				_currentTask = Task.GetById(win.TaskId);
				if (string.IsNullOrWhiteSpace(_currentTask.Name))
				{
					var win2 = new SetNameWindow();
					win2.CurrentTask = _currentTask;
					win2.ShowDialog();
				}
				startButton.IsEnabled = true;
				taskBlock.Text = string.Format("({0}) {1}", _currentTask.ID, _currentTask.Name);
				_timer_Tick(null, null);
			}
		}

		void StartCurrentTask()
		{
			if (!_ticking)
			{
				_currentPeriod = TaskPeriod.GetNew(_currentTask);
				_currentPeriod.Begin();
				selectButton.IsEnabled = false;
				startButton.IsEnabled = false;
				pauseButton.IsEnabled = true;
				SaveAll();
				_ticking = true;
			}
		}

		void PauseCurrentTask()
		{
			if (_ticking)
			{
				_currentPeriod.Stop();
				_currentPeriod = null;
				selectButton.IsEnabled = true;
				startButton.IsEnabled = true;
				pauseButton.IsEnabled = false;
				SaveAll();
				_ticking = false;
			}
		}

		private void BeforeExit()
		{
			PauseCurrentTask();
			SaveAll();
		}

		public static void SaveAll()
		{
			Task.SaveAll();
			Instance._lastSave = DateTime.Now;
		}

		#endregion

		public MainWindow(bool hidden = false)
		{
			_instance = this;
			InitializeComponent();

			MakeIcon();
			Task.LoadAll();
			_timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 2)};
			_timer.Tick += _timer_Tick;
			_timer.Start();

			_updateTimer = new DispatcherTimer {Interval = new TimeSpan(2, 0, 0)};
			_updateTimer.Tick += _updateTimer_Tick;
			_updateTimer.Start();

			_hiddenStart = hidden;

			AsyncManager.Push(new AppUpdateTask());
		}

		private void _updateTimer_Tick(object sender, EventArgs e)
		{
			AsyncManager.Push(new AppUpdateTask());
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			dayTimeBlock.Text = new TimeSpan(TaskPeriod.GetToday().Sum(p => Math.Min((p.End - p.Start).Ticks, (p.End - DateTime.Now.Date).Ticks))).ToJira();
			_notifyIcon.Text = "Day total: " + dayTimeBlock.Text;
			taskTimeBlock.Text = TaskPeriod.TaskDuration(_currentTask).ToJira();
			if (_currentPeriod != null)
				stageTimeBlock.Text = _currentPeriod.DurationString;
			if (DateTime.Now - _lastSave > new TimeSpan(0, 10, 0))
				SaveAll();
		}

		void HideWindow()
		{
			Hide();
			_showHideMenuItem.Text = "Show";
		}

		void ShowWindow()
		{
			Show();
			bool tm = Topmost;
			Topmost = true;
			Topmost = tm;
			_showHideMenuItem.Text = "Hide";
		}

		#region notify icon

		System.Windows.Forms.NotifyIcon _notifyIcon;
		System.Windows.Forms.ToolStripMenuItem _showHideMenuItem;
		void MakeIcon()
		{

			_notifyIcon = new System.Windows.Forms.NotifyIcon();
			_notifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/TimeLogger;component/clock.ico")).Stream);
			_notifyIcon.Visible = true;
			//notifyIcon.DoubleClick += showHideMenu_Click;
			_notifyIcon.MouseClick += notifyIcon_MouseClick;
			_notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip() {
				Size = new System.Drawing.Size(120, 126)
			};
			_notifyIcon.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
			{
				new System.Windows.Forms.ToolStripMenuItem() { Size = new System.Drawing.Size(119, 22), Text = "Hide" },
				new System.Windows.Forms.ToolStripSeparator() { Size = new System.Drawing.Size(116, 6) },
				new System.Windows.Forms.ToolStripMenuItem() { Size = new System.Drawing.Size(119, 22), Text = "Select Task" },
				new System.Windows.Forms.ToolStripMenuItem() { Size = new System.Drawing.Size(119, 22), Text = "Start" },
				new System.Windows.Forms.ToolStripMenuItem() { Size = new System.Drawing.Size(119, 22), Text = "Stop" },
				new System.Windows.Forms.ToolStripSeparator() { Size = new System.Drawing.Size(116, 6) },
				new System.Windows.Forms.ToolStripMenuItem() { Size = new System.Drawing.Size(119, 22), Text = "Exit" }
			});
			_notifyIcon.ContextMenuStrip.Items[0].Click += showHideMenu_Click;
			_notifyIcon.ContextMenuStrip.Items[2].Click += selectMenu_Click;
			_notifyIcon.ContextMenuStrip.Items[3].Click += startMenu_Click;
			_notifyIcon.ContextMenuStrip.Items[4].Click += stopMenu_Click;
			_notifyIcon.ContextMenuStrip.Items[6].Click += exitMenu_Click;
			_showHideMenuItem = _notifyIcon.ContextMenuStrip.Items[0] as System.Windows.Forms.ToolStripMenuItem;
		}

		#endregion

		#region form buttons

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
			SelectTask();
		}

		private void pauseButton_Click(object sender, RoutedEventArgs e)
		{
			PauseCurrentTask();
		}

		private void startButton_Click(object sender, RoutedEventArgs e)
		{
			StartCurrentTask();
		}

		private void ontopButton_Click(object sender, RoutedEventArgs e)
		{
			Topmost = !Topmost;
		}

		private void infoButton_Click(object sender, RoutedEventArgs e)
		{
			var win = new InfoWindow();
			win.Show();
		}

		private void settingsButton_Click(object sender, RoutedEventArgs e)
		{
			var win = new SettingsWindow();
			win.ShowDialog();
		}

		private void jiraButton_Click(object sender, RoutedEventArgs e)
		{
			var win = new JiraWindow();
			win.Show();
		}

		#endregion

		#region notify icon menu buttons

		private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
				ShowWindow();
		}

		private void showHideMenu_Click(object sender, EventArgs e)
		{
			if (IsVisible)
				HideWindow();
			else
				ShowWindow();
		}

		private void selectMenu_Click(object sender, EventArgs e)
		{
			SelectTask();
		}

		private void startMenu_Click(object sender, EventArgs e)
		{
			StartCurrentTask();
		}

		private void stopMenu_Click(object sender, EventArgs e)
		{
			PauseCurrentTask();
		}

		private void exitMenu_Click(object sender, EventArgs e)
		{
			BeforeExit();
			_trueClosing = true;
			Close();
		}

		#endregion

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!_trueClosing)
			{
				e.Cancel = true;
				HideWindow();
			}
			else
				BeforeExit();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			if(_hiddenStart)
				Hide();
		}
	}
}
