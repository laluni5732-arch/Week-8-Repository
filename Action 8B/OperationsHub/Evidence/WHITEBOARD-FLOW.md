# Action 8A — Whiteboard flow and explanation

## Context and attention first

**User:** a CS110 student or other individual maintaining a short task/inventory list.

**Need:** add a valid text item and immediately see it in the list, with clear feedback if the input is blank.

**Assumptions:** Windows desktop; one local user; simple text entries; no network connection required. Task entries are saved locally.

**SEEK / AAA class notes:** Fill in the instructor's exact course definitions and show how they informed the first action. Do not guess at acronym expansions if they were defined differently in lecture.

## Whiteboard prototype

```text
[User types into txtEntry]
          |
          v
[Clicks Add button in MainWindow.xaml]
          |
          v
[WPF raises Click -> Add_Click]
          |
          v
[AddItem(txtEntry.Text)]
          |
          +-- trim input --> empty? -- yes --> return validation message
          |                              |             |
          |                              |             v
          |                              |       [txtStatus updates]
          |                              |
          no                             |
          |                              |
          v                              |
[_items.Add(entry)]                     |
          |                              |
          v                              |
[SaveItems -> local JSON file]           |
          |                              |
          v                              v
[ObservableCollection notifies ListBox / status shows result]
```

## Screen sketch

Left panel: label, text-entry field, task list, Add and Remove selected buttons.

Right panel: artwork preview, focus timer, and work-report text area.

Bottom area: Choose artwork, Export contract, Reopen text, and a status line.

## Explain it aloud

The XAML defines the Add button and names its event handler `Add_Click`. WPF calls that C# handler when the button is clicked. The handler passes the text to `AddItem`. That method trims the text and returns early with a message if the input is blank. For valid text, it adds the item to the `ObservableCollection`, saves the list, and returns a success message. Since the ListBox's `ItemsSource` is the collection, the visible list updates automatically; the status text also changes.

## Peer check

Peer name/date: ______________________________

What the peer observed: _______________________

Suggested revision: ___________________________

Revision made (or reason not made): ____________


## Additional flow: Quick Links

User enters a link name and URL -> clicks **Save link** -> `AddLink_Click` trims and validates both values -> a `QuickLink` object is added to `_links` (`ObservableCollection<QuickLink>`) -> `lstLinks` visibly updates through `ItemsSource` binding -> `SaveLinks` writes the list to `quick-links.json`. To open it, the user selects an item and clicks **Open selected** -> `OpenLink_Click` uses `Process.Start` to hand the HTTP(S) address to the default browser -> the status label confirms the attempt or displays an error.
