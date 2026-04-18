# AGENTS.md

Instructions for AI agents working in this repository.

## On every change

Before pushing or opening a PR, the agent MUST:

1. **Verify required CLI tools** (see below). Install what's missing, or ask the user if elevation is required.
2. **Keep the change scoped** — only modify files related to the stated task. If the IDE, LSP, or package tooling touched unrelated files, revert those before committing. One topic per PR.
3. **Build clean**: `dotnet build nunit.slnx -c Release`. No new warnings. Fix warnings in code you wrote — don't `#pragma warning disable`.
4. **Test**: `dotnet test nunit.slnx -c Release`.
5. Push and open the PR **only after build and tests pass**. Never push a failing build.

If a step fails, fix and re-run — don't bypass.

## Required CLI tools

IDEs are not required. The agent needs these on `PATH`:

| Tool | Why | Check |
|---|---|---|
| `dotnet` (.NET 10 SDK or newer) | Build and test | `dotnet --list-sdks` |
| `git` | Source control | `git --version` |
| `gh` | Read PR review comments, create PRs | `gh --version && gh auth status` |
| `mono` *(macOS / Linux only)* | Runs `#if NETFRAMEWORK` targets on non-Windows | `mono --version` |
| `pwsh` or `bash` | Runs `build.ps1` / `build.sh` (only needed for Cake packaging build) | `pwsh --version` / `bash --version` |

### If a tool is missing

Try the platform package manager first:

- **Windows**: `winget install Microsoft.DotNet.SDK.10` • `winget install GitHub.cli`
- **macOS**: `brew install --cask dotnet-sdk` • `brew install gh mono`
- **Debian/Ubuntu**: `sudo apt-get install -y dotnet-sdk-10.0 gh mono-devel`

If the install needs elevation (sudo, admin) or the package manager itself is missing, **stop and ask the user**. For example:

> ".NET 10 SDK isn't installed and I don't have permission to install it. Please run `winget install Microsoft.DotNet.SDK.10` (or download from https://dotnet.microsoft.com/download) and confirm when done."

Do not invent workarounds — the build genuinely requires the listed versions.

After `gh` is installed: `gh auth login` once to authenticate.

## Build & test quick reference

- **Solution**: `nunit.slnx` in repo root.
- **Target frameworks** in build output: `net8.0`, `net6.0`, `net462`. A full build produces binaries for each.
- **Test projects** (the ones to actually run): `nunit.framework.tests-*`, `nunit.framework.legacy.tests-*`, `nunitlite.tests-*`. Other test projects contain **intentionally-failing** fixtures used by integration tests — don't "fix" their failures.
- **Full packaging build** (only needed for releases, not per-change): `./build.ps1 --target=Test --configuration=Release` on Windows, `./build.sh --target=Test --configuration=Release` on Linux/Mac. See `BUILDING.md` for all Cake targets.

## Project conventions

- **Packages**: add `<PackageVersion>` to `Directory.Packages.props` and a version-less `<PackageReference>` to the `.csproj`. Central package management forbids `Version="..."` in `.csproj` — the build will fail.
- **Mono**: `#if NETFRAMEWORK` code runs on macOS/Linux via Mono, including on CI. Not Windows-only.
