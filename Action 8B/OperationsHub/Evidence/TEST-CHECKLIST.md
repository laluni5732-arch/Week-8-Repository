# Action 8B — Test checklist

Run these checks on the Windows machine where the project is built. Mark each result only after trying it.

| ID | Route | Steps | Expected result | Actual result / pass? |
|---|---|---|---|---|
| T1 | Normal add | Enter `Replace printer paper`; click Add | Item appears once; input clears; status says Added | Not run |
| T2 | Trim input | Enter `  Check supplies  `; click Add | Stored/displayed item has no leading/trailing spaces | Not run |
| T3 | Invalid add | Leave input blank; click Add | No blank item; useful status message | Not run |
| T4 | Invalid add | Enter spaces only; click Add | No blank item; useful status message | Not run |
| T5 | Remove without selection | Click Remove selected with nothing selected | No crash; status asks for a selection | Not run |
| T6 | Remove item | Select an existing item; click Remove selected | Selected item disappears and remains removed after restart | Not run |
| T7 | Persistence | Add item, close app, reopen app | Item is loaded from local JSON | Not run |
| T8 | Export report | Add task and notes; click Export contract; choose a `.txt` path | File is created with task(s), notes, timer, and artwork path | Not run |
| T9 | Reopen report | Click Reopen text and select the exported file | File contents appear in the notes/report text box | Not run |
| T10 | Cancel dialogs | Cancel export, reopen, and image dialogs | App remains usable; no crash | Not run |
| T11 | Valid image | Choose a valid PNG or JPG | Preview and button image update | Not run |
| T12 | Invalid/recovery image | Try a non-image file if selectable, or a damaged image | Helpful error feedback; app remains open | Not run |
| T13 | Timer | Start, pause, reset | Countdown changes, pauses, and returns to 25:00 | Not run |
| T14 | Build | Build with .NET 8 on Windows | Build succeeds without errors | Not run |

## Test summary (fill after testing)

Date and machine: ______________________________

Build result: __________________________________

Tests passed / attempted: _______________________

Bug found and reproduction steps: ______________

Fix or reason deferred: ________________________

Retest result: _________________________________


## Quick Links feature checks

- [ ] Add a link with a name and a complete `https://` address; confirm it appears in the list.
- [ ] Close and reopen the app; confirm the link remains.
- [ ] Select a link and choose **Open selected**; confirm it opens in the default browser.
- [ ] Try an empty name and confirm the status gives a useful explanation.
- [ ] Try an invalid URL or a non-HTTP(S) scheme and confirm it is rejected.
- [ ] Remove a selected link and confirm it stays removed after restarting.
- [ ] Confirm the default Canvas / SBCCD link is present on a fresh profile.
- [ ] Export a report and verify the saved quick links appear in its QUICK LINKS section.
