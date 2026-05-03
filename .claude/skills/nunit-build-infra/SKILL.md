---
name: nunit-build-infra
description: Use when editing `.csproj`, `.nuspec`, `Directory.Build.props`, `Directory.Build.targets`, `Directory.Packages.props`, the Cake build (`build.cake`, `build.ps1`, `build.sh`), or GitHub Actions workflows. Covers central package management, MinVer-driven versioning, the props-vs-targets split, and nuspec/csproj consistency.
---

# NUnit build infrastructure

The build is opinionated. Several patterns that look like ordinary csproj/nuspec editing in other repos are wrong here, and a few that look strange are load-bearing. When editing any build file, follow these conventions.

## Adding a package dependency

Two files always change, in lockstep:

1. Add the version to `src/NUnitFramework/Directory.Packages.props`:
```xml
   <PackageVersion Include="SomePackage" Version="1.2.3" />
```
2. Add a version-less reference in the consuming `.csproj`:
```xml
   <PackageReference Include="SomePackage" />
```

Never put `Version="..."` on a `<PackageReference>`. If you see one in a diff (often left by an IDE after a package update), delete the attribute.

## Adding a build-only or analyzer-only package

Mark with `PrivateAssets="all"` so it doesn't propagate to consumers, and place under the analyzers/build-tooling `ItemGroup` in `Directory.Build.props` rather than per-project:

```xml
<PackageReference Include="StyleCop.Analyzers" PrivateAssets="all" />
```

In the corresponding nuspec entry, mark such packages as `developmentDependency="true"` so consumers of the NUnit nuget don't pull them transitively.

## Versioning — don't touch it

Versioning is handled at build time by **MinVer**, which derives the version from git tags. Specifically:

- Never set `<AssemblyVersion>`, `<FileVersion>`, `<Version>`, or `<InformationalVersion>` in a `.csproj`.
- Never hand-write `[assembly: AssemblyVersion(...)]` or related attributes — `<GenerateAssemblyInfo>true</GenerateAssemblyInfo>` produces them.
- The deliberate convention is that `AssemblyVersion` stays at `Major.0.0.0` (so binding redirects don't churn on every release) while `AssemblyFileVersion` drifts. Don't "fix" this.

If you need to influence the version (e.g. for a release branch), do it via MinVer's environment variables in CI, not by editing project files.

## `Directory.Build.props` vs `Directory.Build.targets`

The split is enforced by convention, not by MSBuild:

- **`Directory.Build.props`**: properties (`<PropertyGroup>`), package references (`<ItemGroup>` with `PackageReference`), feature constants, build settings. Anything evaluated at project-load time.
- **`Directory.Build.targets`**: `<Target>` definitions — anything that runs as part of the build pipeline (custom build steps, post-compile work).

Don't put a `<Target>` in `.props` or a property in `.targets`. Reviewers will ask you to move it.

## `nuspec` and `.csproj` must agree

Each shipped NuGet has a `.nuspec` that lists its dependencies. Those must match the `<PackageReference>`s of the corresponding `.csproj` for each target framework.

This is asserted at test time by `NuspecDependenciesTests.cs` — drift fails the test suite during `dotnet test`. When you add a runtime dependency to a project that ships, update the nuspec in the same change. If the test reports drift, the nuspec is the file to update (not the test).

Build-only or analyzer-only packages should *not* appear in the nuspec, or should appear with `developmentDependency="true"` if they must.

## `Directory.Build.targets` and target authoring

When adding a target:

- Give it a clear, capitalised name that says what it does (`SetAssemblyVersion`, `CopyContractFiles`).
- Specify when it runs explicitly (`BeforeTargets="Build"` / `AfterTargets="..."`); don't rely on placement order.
- Targets that emit output for diagnostic purposes should respect MSBuild's terminal logger — `<Message Importance="High">` is necessary to be seen with the new logger.

## CI workflows (`.github/workflows`)

- Don't add a new TFM build matrix entry as part of an unrelated PR — adding a TFM has consequences for package payload and is reviewed separately.
- When adding a new dotnet step, install via `actions/setup-dotnet@v4` with an explicit `dotnet-version`. Don't depend on the runner's preinstalled SDK.
- Caches keyed on `Directory.Packages.props` and lock files — if you add a new file that influences restore, include it in the cache key.

## Common cleanup tasks while editing csproj

- IDE package-update tooling sometimes adds `Version="..."` to a `<PackageReference>` it just bumped. Delete the attribute and update `Directory.Packages.props` instead.
- IDE tooling sometimes inserts `<TargetFramework>` for a project that defines `<TargetFrameworks>` (plural) centrally. Remove the singular form unless the project genuinely targets one TFM only.
- Stale `<None>` / `<Compile>` items left behind after a file rename or move — clean these up; SDK-style projects use globs and don't need explicit entries for normal `.cs` files.
