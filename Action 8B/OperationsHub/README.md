# Operations Hub — Study Hub-inspired Edition

Operations Hub is a local Windows desktop workspace for CS110. This edition borrows Study Hub's student-dashboard personality: a dark navy canvas, indigo accents, clear numbered sections, concise guidance, and a calm “plan, focus, reflect” feel. It retains the original Operations Hub workflows and adds a customizable Quick Links launchpad.

## Run the app

1. Install the .NET 8 SDK and Visual Studio with the **.NET desktop development** workload.
2. Extract the ZIP.
3. Open `OperationsHub.csproj` in Visual Studio.
4. Restore the project if prompted, then press **F5**.

Or open a terminal in this folder and run:

```powershell
dotnet run
```

## Features

**Task board:** Add tasks or inventory items, reject blank input, remove the selected item, and persist the list between sessions.

**Quick links:** Save a name and website address, open the selected website in your default browser, or remove links. Links accept only `http://` and `https://` addresses. Canvas / SBCCD is supplied as the initial link; edit the list to suit your workflow. Quick links are saved locally between sessions and included in exported reports.

**Focus timer:** Start, pause, or reset a 25-minute focus session.

**Work log and reports:** Write a reflection, team contract, or progress report. Export a report as TXT or Markdown, including tasks, quick links, timer status, and the selected artwork path. Reopen a TXT or Markdown file in the editor.

**Artwork:** Load a local PNG, JPG, BMP, GIF, or TIFF image as a visual reference.

## Project files

`MainWindow.xaml` contains the window layout, styling, named controls, and button event connections.

`MainWindow.xaml.cs` contains the task list, quick-link validation and launch behavior, timer, artwork picker, report creation, file handling, and local persistence.

`App.xaml` and `App.xaml.cs` start the WPF application.

`OperationsHub.csproj` targets `net8.0-windows` and enables WPF.

`PATTERN.md` describes reusable implementation patterns.

`Evidence/` contains the flow explanation, test checklist, peer-review/AI record, Git checkpoint guide, and handoff receipt template.

## Local data and privacy

The task list and quick links are stored under:

`%LOCALAPPDATA%\\OperationsHub\\items.json`

`%LOCALAPPDATA%\\OperationsHub\\quick-links.json`

The app does not automatically upload your data to Canvas, GitHub, or a server. Reports are written only to the location you choose. The timer and work-log editor are not persisted automatically; export your report to keep a copy.

## Before handing in

Build and run the app on Windows. Complete the normal, invalid-input, and recovery tests in `Evidence/TEST-CHECKLIST.md`. Record actual AI and peer feedback in `Evidence/AI-PEER-REVIEW.md`, then fill out `Evidence/RECEIPT.md` with your real test and GitHub checkpoint details. Do not mark tests or reviews complete until you have performed them.


## Build troubleshooting

This package removes a `CharacterSpacing` attribute that is not supported by WPF's `TextBlock` and can prevent XAML compilation.

If the project still fails to build, in Visual Studio open **View > Output**, select **Build**, and copy the first error beginning with `error` (including its code, such as `MC####` or `CS####`). The first build error is usually the most useful one to diagnose.
