# How to contribute

So you're thinking about contributing to NUnit? Great! Maintaining and enhancing NUnit is a big job, so **the community's help is really appreciated.**

Helping out isn't just writing code, it also includes submitting issues, helping confirm issues, working on the website, helping people on the [mailing lists](https://groups.google.com/forum/m/#!forum/nunit-discuss) and improving the documentation. 

## Submitting Issues

Requests for new features and bug reports keep the project moving forward.

### Before you submit an issue

- Ensure you are running the [latest version](https://github.com/nunit/nunit/releases) of NUnit.
- **Many 3rd party test runners do not support NUnit 3/4 yet.** They may appear to run NUnit tests, but they do not do so correctly.
- To ensure that the bug is in NUnit and not in the runner you are using, **test your bug using `dotnet test`, `nunit3-console` or `nunitlite`** first.
- **Search** the [issue list](https://github.com/nunit/nunit/issues?utf8=%E2%9C%93&q=is%3Aissue) (including closed issues) to make sure it hasn't already been reported.

### Submitting a good issue

- Give the issue a short, clear title that describes the bug or feature request
- Include what version of NUnit you are using, which runner you are using
- Tell us how you are running your tests including command line arguments or `runsettings`
- Include steps to reproduce the issue
- Include a short code example that reproduces the issue
- Use [markdown formatting](https://guides.github.com/features/mastering-markdown/) as appropriate to make the issue and code more readable.

## Confirming Issues

Before we work on issues, we must confirm them and be able to reproduce them. Confirming issues takes up a great deal of the team's time, so making that job easier is **really appreciated**.

Issues that need confirmation will have the **confirm** label or be unlabeled and have **no milestone**. You can help us to confirm issues by;

- Add steps to reproduce the issue
- Create unit tests to demonstrate the issue
- Test issues and provide feedback

If you’re getting ready to report a bug in NUnit, figuring out how to create a minimal repro is easier if you temporarily disable the debugger’s [Just My Code](https://docs.microsoft.com/en-us/visualstudio/debugger/just-my-code) setting. This allows you to step into NUnit's source code, set breakpoints, watch variables, etc.

## Documentation

Great documentation is essential for any open source project and NUnit is no exception. 

[Our documentation](https://docs.nunit.org/articles/nunit/intro.html) often lags behind the features that have been implemented or would benefit from better examples.

## Fixing Bugs and Adding Features 

We love pull requests, but would prefer that new contributors start with smaller issues and let us know before you contribute to prevent duplication of work.

Ensure you have an issue to connect your pull request to. If there isn't one, please open a new issue first, and state you have a fix before embarking on the pull request. 

To help new contributors get their feet wet, we have marked a number of issues with the `good first issue` label. These are great places to start.

It is also a good idea to add a comment to an issue that you are working on to let everyone know. If you stop working on it, also please let us know.

Please read through the [Developer Docs](https://docs.nunit.org/articles/developer-info/Team-Practices.html#technical-practices) before contributing to understand our coding standards and contribution guidelines.

When you are ready to contribute, instructions on how to build and run tests can be found in [BUILDING.md](https://github.com/nunit/nunit/blob/master/BUILDING.md)

## License

NUnit is under the [MIT license](https://github.com/nunit/nunit/blob/master/LICENSE.txt). By contributing to NUnit, you assert that:

* The contribution is your own original work.
* You have the right to assign the copyright for the work (it is not owned by your employer, or you have been given copyright assignment in writing).
