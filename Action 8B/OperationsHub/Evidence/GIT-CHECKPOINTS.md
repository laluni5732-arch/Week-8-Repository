# Git and GitHub checkpoint guide

Open a terminal in the `OperationsHub` project folder.

## Inspect changes

```powershell
git status
git diff
```

## First checkpoint (Action 8A)

If this folder is already inside your course repository, use that repository's existing remote and branch. Stage only the intended project files, then commit:

```powershell
git add .
git commit -m "Build Operations Hub first working action"
```

## Revised checkpoint (Action 8B)

After you test, revise the code, and fill out your evidence honestly:

```powershell
git status
git add .
git commit -m "Complete and test Operations Hub Week 8"
git push
```

If Git says there is no remote or upstream, check the course's instructions and configured repository with `git remote -v` and `git branch --show-current`. Do not create a new public repository or publish course work somewhere else unless your instructor's directions call for it. Never put passwords, tokens, or private credentials in the repository.

**Commit** = save a local checkpoint. **Push** = upload local commits to the configured remote. A commit can exist without a push.
