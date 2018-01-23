# How to contribute

So you're thinking about contributing to NUnit? Great! Maintaining and enhancing NUnit is a big job, so **the community's help is really appreciated.**

Helping out isn't just writing code, it also includes submitting issues, helping confirm issues, working on the website, helping people on the [mailing lists](https://groups.google.com/forum/m/#!forum/nunit-discuss) and improving the documentation. 

## Submitting Issues

Requests for new features and bug reports keep the project moving forward.

### Before you submit an issue

- Ensure you are running the [latest version](https://github.com/nunit/nunit/releases) of NUnit.
- **Many 3rd party test runners do not support NUnit 3 yet.** They may appear to run NUnit tests, but they do not do so correctly.
- To ensure that the bug is in NUnit and not in the runner you are using, **test your bug using the nunit3-console.exe** first.
- **Search** the [issue list](https://github.com/nunit/nunit/issues?utf8=%E2%9C%93&q=is%3Aissue) (including closed issues) to make sure it hasn't already been reported.

### Submitting a good issue

- Give the issue a short, clear title that describes the bug or feature request
- Include what version of NUnit you are using
- Tell us how you are running your tests including command line arguments for the console runner
- Include steps to reproduce the issue
- If possible, include a short code example that reproduces the issue
- Use [markdown formatting](https://guides.github.com/features/mastering-markdown/) as appropriate to make the issue and code more readable.

## Confirming Issues

Before we work on issues, we must confirm them and be able to reproduce them. Confirming issues takes up a great deal of the team's time, so making that job easier is **really appreciated**.

Issues that need confirmation will have the **confirm** label or be unlabeled and have **no milestone**. You can help us to confirm issues by;

- Add steps to reproduce the issue
- Create unit tests to demonstrate the issue
- Test issues and provide feedback

As of version 3.10, the NUnit and NUnitLite NuGet packages support **debugger source-stepping** for each binary from the repository. Debuggers can step into the source code, set breakpoints, watch variables, etc. If you’re getting ready to report a bug in NUnit, figuring out how to create a minimal repro is much easier since you aren’t dealing with a black box!

To easily drop into NUnit code from your project any time, [follow these steps](https://github.com/nunit/docs/wiki/Debugger-Source-Stepping).

## Documentation

Great documentation is essential for any open source project and NUnit is no exception. [Our documentation](https://github.com/nunit/docs/wiki/NUnit-Documentation) often lags behind the features that have been implemented or would benefit from better examples.

A great place to start is by going through the [NUnit release notes](https://github.com/nunit/docs/wiki/Release-Notes) and ensuring that the documentation for new features is up to date.

## Fixing Bugs and Adding Features 

We love pull requests, but would prefer that new contributors start with smaller issues and let us know before you contribute to prevent duplication of work.

To help new contributors get their feet wet, we have marked a number of issues with the `easyfix` label. These are great places to start.

It is also a good idea to add a comment to an issue that you are working on to let everyone know. If you stop working on it, also please let us know.

Please read through the [developer docs](https://github.com/nunit/docs/wiki/Team-Practices#technical-practices) before contributing to understand our coding standards and contribution guidelines.

## License

NUnit is under the [MIT license](https://github.com/nunit/nunit/blob/master/LICENSE.txt). By contributing to NUnit, you assert that:

* The contribution is your own original work.
* You have the right to assign the copyright for the work (it is not owned by your employer, or
  you have been given copyright assignment in writing).
