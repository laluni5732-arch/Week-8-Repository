using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;

namespace OperationsHub;

/// <summary>
/// MainWindow coordinates task management, quick links, focus timing,
/// artwork selection, and text-file reporting.
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableCollection<string> _items = new();
    private readonly ObservableCollection<QuickLink> _links = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private TimeSpan _remaining = TimeSpan.FromMinutes(25);
    private bool _timerRunning;
    private string? _artworkPath;

    private string DataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OperationsHub");
    private string ItemsFile => Path.Combine(DataDirectory, "items.json");
    private string LinksFile => Path.Combine(DataDirectory, "quick-links.json");

    public MainWindow()
    {
        InitializeComponent();
        lstItems.ItemsSource = _items;
        lstLinks.ItemsSource = _links;
        _timer.Tick += Timer_Tick;
        LoadItems();
        LoadLinks();
        UpdateTimerDisplay();
        txtStatus.Text = "Ready. Add a task, save a quick link, or start a focus session.";
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        string message = AddItem(txtEntry.Text);
        txtStatus.Text = message;
        if (message.StartsWith("Added:", StringComparison.Ordinal))
            txtEntry.Clear();
        txtEntry.Focus();
    }

    private string AddItem(string? rawInput)
    {
        string entry = (rawInput ?? string.Empty).Trim();
        if (entry.Length == 0)
            return "Nothing added. Enter a task or inventory item first.";

        _items.Add(entry);
        SaveItems();
        return $"Added: {entry}";
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (lstItems.SelectedItem is not string selected)
        {
            txtStatus.Text = "Select an item in the task board first.";
            return;
        }

        bool removed = RemoveItem(lstItems.SelectedIndex);
        txtStatus.Text = removed ? $"Removed: {selected}" : "That item was not found.";
    }

    private bool RemoveItem(int index)
    {
        // Reject no selection or a position outside the collection.
        if (index < 0 || index >= _items.Count)
            return false;

        // Remove this specific row, even when task names are duplicated.
        _items.RemoveAt(index);

        SaveItems();
        return true;
    }

    private void AddLink_Click(object sender, RoutedEventArgs e)
    {
        string name = txtLinkName.Text.Trim();
        string rawUrl = txtLinkUrl.Text.Trim();

        if (name.Length == 0)
        {
            txtStatus.Text = "Enter a name for the quick link.";
            txtLinkName.Focus();
            return;
        }

        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            txtStatus.Text = "Enter a complete website address beginning with https:// or http://.";
            txtLinkUrl.Focus();
            return;
        }

        if (_links.Any(link => string.Equals(link.Url, uri.AbsoluteUri, StringComparison.OrdinalIgnoreCase)))
        {
            txtStatus.Text = "That website is already in your quick links.";
            return;
        }

        var link = new QuickLink { Name = name, Url = uri.AbsoluteUri };
        _links.Add(link);
        SaveLinks();
        lstLinks.SelectedItem = link;
        txtLinkName.Clear();
        txtLinkUrl.Clear();
        txtStatus.Text = $"Saved quick link: {name}";
    }

    private void OpenLink_Click(object sender, RoutedEventArgs e)
    {
        if (lstLinks.SelectedItem is not QuickLink link)
        {
            txtStatus.Text = "Select a quick link to open.";
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(link.Url) { UseShellExecute = true });
            txtStatus.Text = $"Opened {link.Name} in your default browser.";
        }
        catch (Exception ex)
        {
            ShowError($"Could not open {link.Name}. Check that your default browser is configured.", ex);
        }
    }

    private void RemoveLink_Click(object sender, RoutedEventArgs e)
    {
        if (lstLinks.SelectedItem is not QuickLink link)
        {
            txtStatus.Text = "Select a quick link to remove.";
            return;
        }

        _links.Remove(link);
        SaveLinks();
        txtStatus.Text = $"Removed quick link: {link.Name}";
    }

    private void LinkSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstLinks.SelectedItem is QuickLink link)
            txtStatus.Text = $"Selected {link.Name} — {link.Url}";
    }

    private void Timer_Click(object sender, RoutedEventArgs e)
    {
        if (_timerRunning)
        {
            _timer.Stop();
            _timerRunning = false;
            txtStatus.Text = "Focus timer paused.";
            return;
        }

        if (_remaining <= TimeSpan.Zero)
            _remaining = TimeSpan.FromMinutes(25);

        _timer.Start();
        _timerRunning = true;
        txtStatus.Text = "Focus timer running. One step at a time.";
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_remaining > TimeSpan.Zero)
            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

        UpdateTimerDisplay();
        if (_remaining > TimeSpan.Zero)
            return;

        _timer.Stop();
        _timerRunning = false;
        txtStatus.Text = "Focus session complete. Take a short break if you need one.";
        MessageBox.Show(this, "Your 25-minute focus session is complete.", "Operations Hub",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void UpdateTimerDisplay() => txtTimer.Text = _remaining.ToString(@"mm\:ss");

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _timerRunning = false;
        _remaining = TimeSpan.FromMinutes(25);
        UpdateTimerDisplay();
        txtStatus.Text = "Timer reset to 25:00.";
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export work report",
            Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|All files (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = "operations-hub-report.txt",
            AddExtension = true
        };
        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            File.WriteAllText(dialog.FileName, BuildReport());
            txtStatus.Text = $"Report exported to {dialog.FileName}";
        }
        catch (Exception ex)
        {
            ShowError("Could not export the report.", ex);
        }
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Reopen a work report",
            Filter = "Text and Markdown files (*.txt;*.md)|*.txt;*.md|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            txtDocument.Text = File.ReadAllText(dialog.FileName);
            txtStatus.Text = $"Opened text from {dialog.FileName}";
        }
        catch (Exception ex)
        {
            ShowError("Could not open that file.", ex);
        }
    }

    private void Image_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose artwork",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff)|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();

            imgArtwork.Source = bitmap;
            imgButton.Source = bitmap;
            _artworkPath = dialog.FileName;
            txtStatus.Text = $"Artwork loaded: {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex)
        {
            ShowError("Could not load that image. Try a supported PNG, JPG, BMP, GIF, or TIFF file.", ex);
        }
    }

    private string BuildReport()
    {
        var lines = new System.Collections.Generic.List<string>
        {
            "OPERATIONS HUB — WORK REPORT",
            $"Created: {DateTime.Now:yyyy-MM-dd HH:mm}",
            string.Empty,
            "TASKS / INVENTORY"
        };

        if (_items.Count == 0)
            lines.Add("(No items currently in the list.)");
        else
        {
            int number = 1;
            foreach (string item in _items)
                lines.Add($"{number++}. {item}");
        }

        lines.AddRange(new[]
        {
            string.Empty, "QUICK LINKS"
        });
        if (_links.Count == 0)
            lines.Add("(No quick links saved.)");
        else
            foreach (QuickLink link in _links)
                lines.Add($"- {link.Name}: {link.Url}");

        lines.AddRange(new[]
        {
            string.Empty, "TEAM CONTRACT OR WORK REPORT", txtDocument.Text,
            string.Empty, "ARTWORK", _artworkPath ?? "(No artwork selected.)",
            string.Empty, "FOCUS TIMER", _remaining.ToString(@"mm\:ss")
        });
        return string.Join(Environment.NewLine, lines);
    }

    private void LoadItems()
    {
        try
        {
            if (!File.Exists(ItemsFile))
                return;

            string json = File.ReadAllText(ItemsFile);
            string[]? saved = JsonSerializer.Deserialize<string[]>(json);
            if (saved is null)
                return;

            foreach (string item in saved)
                if (!string.IsNullOrWhiteSpace(item))
                    _items.Add(item);
        }
        catch (Exception ex)
        {
            txtStatus.Text = $"Could not load saved items: {ex.Message}";
        }
    }

    private void SaveItems()
    {
        try
        {
            Directory.CreateDirectory(DataDirectory);
            File.WriteAllText(ItemsFile, JsonSerializer.Serialize(_items));
        }
        catch (Exception ex)
        {
            ShowError("Could not save the item list.", ex);
        }
    }

    private void LoadLinks()
    {
        try
        {
            if (File.Exists(LinksFile))
            {
                string json = File.ReadAllText(LinksFile);
                var saved = JsonSerializer.Deserialize<QuickLink[]>(json);
                if (saved is not null)
                {
                    foreach (QuickLink link in saved)
                    {
                        if (!string.IsNullOrWhiteSpace(link.Name) && IsWebUrl(link.Url))
                            _links.Add(link);
                    }
                }
            }

            // Provide a useful first link on a new install; users can remove it or add their own.
            if (_links.Count == 0)
            {
                _links.Add(new QuickLink
                {
                    Name = "Canvas / SBCCD",
                    Url = "https://sbccd.instructure.com/"
                });
                SaveLinks();
            }
        }
        catch (Exception ex)
        {
            txtStatus.Text = $"Could not load quick links: {ex.Message}";
        }
    }

    private static bool IsWebUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private void SaveLinks()
    {
        try
        {
            Directory.CreateDirectory(DataDirectory);
            File.WriteAllText(LinksFile, JsonSerializer.Serialize(_links));
        }
        catch (Exception ex)
        {
            ShowError("Could not save quick links.", ex);
        }
    }

    private void ShowError(string message, Exception ex)
    {
        txtStatus.Text = message;
        MessageBox.Show(this, $"{message}\n\n{ex.Message}", "Operations Hub",
            MessageBoxButton.OK, MessageBoxImage.Error);
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        SaveItems();
        SaveLinks();
        base.OnClosed(e);
    }
}

public sealed class QuickLink
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
