# PATTERN.md — Reusable code patterns

## 1. XAML event to C# method to visible result

The Add button in `MainWindow.xaml` has `Click="Add_Click"`. WPF calls `Add_Click` when the user activates it. The handler passes the input text to `AddItem`, then writes the returned message to `txtStatus`. The method changes `_items`, and the ListBox shows the change through `ItemsSource`.

## 2. Fields, local variables, and scope

Fields such as `_items`, `_timer`, `_remaining`, and `_timerRunning` belong to `MainWindow` and persist for the window's lifetime. The local `entry` inside `AddItem` exists only during that method call. The constructor initializes controls and connects the timer event after `InitializeComponent()`.

## 3. Types and return values

`string` represents user-entered text and status messages. `bool` represents yes/no state, such as `_timerRunning` or whether an item was removed. `TimeSpan` represents timer duration. `AddItem` returns a `string` so the caller can display a result; `RemoveItem` returns a `bool`; event handlers return `void` because WPF invokes them for their side effects.

## 4. Collection and binding

An array has a fixed length after it is created. `ObservableCollection<string>` can grow and shrink and notifies a WPF control when its items change. The constructor assigns `_items` to `lstItems.ItemsSource`, so adding/removing items updates the ListBox without manually rebuilding the display.

## 5. Validation and early return

`AddItem` trims the incoming text before checking its length. Blank or whitespace-only text returns a helpful message before the collection is changed. `Remove_Click` also returns early when no list item is selected. These checks prevent invalid actions and give the user feedback.

## 6. File input/output and recovery

`SaveFileDialog` and `OpenFileDialog` let the user choose paths. `File.WriteAllText` exports the report; `File.ReadAllText` reopens it. File operations are inside `try/catch`, which lets the app report access, path, or read/write failures instead of silently failing.

## 7. Timer and report assembly

`DispatcherTimer.Tick` runs on the WPF UI dispatcher. The app tracks running state in `_timerRunning` and the remaining duration in `_remaining` (`TimeSpan`). `BuildReport` uses `foreach` to add every current task to the report lines before joining them into text.

## 8. Image loading and readable controls

The image picker supplies a user-selected file path. `BitmapImage` loads it and the same image source is assigned to the artwork preview and the `Image` inside the labelled Choose artwork button. The button still uses a normal `Click` handler. The dark background, light text, and gold buttons are chosen for contrast; peer feedback should still be used to check readability.

## 9. SEEK and AAA planning notes

Use your course's exact definitions of **SEEK** and **AAA** when presenting these concepts, because course acronyms can be instructor-specific. For this prototype, record the user, need, assumptions, and evidence in `Evidence/WHITEBOARD-FLOW.md`, then identify what was tested and what should change in the peer-review form.


## Quick links pattern

The Quick Links feature uses an `ObservableCollection<QuickLink>` as the window's in-memory list and assigns it to `lstLinks.ItemsSource`. The UI stays in sync when links are added or removed. `AddLink_Click` trims user input, checks that the name is present, and uses `Uri.TryCreate` to accept only absolute HTTP/HTTPS URLs. `OpenLink_Click` opens the selected address with `Process.Start` and `UseShellExecute = true`, which asks Windows to use the default browser. `LoadLinks` and `SaveLinks` serialize the collection to JSON in the user's local application data folder. This keeps the list between app launches without requiring a server.

Flow: text fields -> Add link click event -> validate name and URL -> add `QuickLink` object to collection -> ListBox updates -> JSON saves -> selected link opens in the default browser.
