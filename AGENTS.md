# AGENTS.md

## On every change

Before pushing or opening a PR, the agent MUST:

1. **Verify the .NET SDK is available** (see below). Install it if missing, or ask the user if elevation is required.
2. **Keep the change scoped** — only modify files related to the stated task. If the IDE, LSP, or package tooling touched unrelated files, revert those before committing. One topic per PR. No unnecessary refactoring.
3. **Build clean**: `dotnet build nunit.slnx -c Release`. No new warnings. Fix warnings in code you wrote — don't `#pragma warning disable`.
4. **Test**: `dotnet test nunit.slnx -c Release`.
5. Push and open the PR **only after build and tests pass**. Never push a failing build.

If a step fails, fix and re-run — don't bypass.

## Required CLI tool

The agent needs **.NET 10 SDK or newer** on `PATH` — check with `dotnet --list-sdks`. `git` is assumed; IDEs aren't required.

If the SDK isn't installed, try the platform package manager:

- **Windows**: `winget install Microsoft.DotNet.SDK.10`
- **macOS**: `brew install --cask dotnet-sdk`
- **Debian/Ubuntu**: `sudo apt-get install -y dotnet-sdk-10.0`

If the install needs elevation (sudo, admin) or the package manager itself is missing, **stop and ask the user**. For example:

> ".NET 10 SDK isn't installed and I don't have permission to install it. Please run `winget install Microsoft.DotNet.SDK.10` (or download from https://dotnet.microsoft.com/download) and confirm when done."

Don't invent workarounds — the build genuinely requires the SDK.

## Build & test quick reference

- **Solution**: `nunit.slnx` in repo root.
- **Target frameworks** in build output: `net8.0`, `net6.0`, `net462`. A full build produces binaries for each.
- **Test projects** (the ones to actually run): `nunit.framework.tests-*`, `nunit.framework.legacy.tests-*`, `nunitlite.tests-*`. Other test projects contain **intentionally-failing** fixtures used by integration tests — **IMPORTANT DONT "fix" their failures.**
- **Full packaging build** (only needed for releases, not per-change): `./build.ps1 --target=Test --configuration=Release` on Windows, `./build.sh --target=Test --configuration=Release` on Linux/Mac. See `BUILDING.md` for all Cake targets.

## Project conventions

- **Packages**: add `<PackageVersion>` to `Directory.Packages.props` and a version-less `<PackageReference>` to the `.csproj`. Central package management forbids `Version="..."` in `.csproj` — the build will fail.
- **Mono**: `#if NETFRAMEWORK` code runs on macOS/Linux via Mono, including on CI. Not Windows-only.
