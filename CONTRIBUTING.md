# How to contribute

So you're thinking about contributing to NUnit? Great! Maintaining and enhancing NUnit is a big job, so **the community's help is really appreciated.**

Helping out isn't just writing code, it also includes submitting issues, helping confirm issues, working on the website, helping people in [nunit slack](https://nunit.slack.com) and improving the documentation.

## MENU

[Who can contribute](#who-can-contribute) Anyone can, but you need to fulfill certain simple requirements

[Use of AI and prevention of AI Slop](#use-of-ai-and-prevention-of-ai-slop)  AI is a very powerful tool, so yes you can use it, but we have certain guidelines you should follow.

[Submitting issues](#submitting-issues)  Requests for new features and bug reports keep the project moving forward.  

[Raising Pull Requests](#raising-pull-requests-for-bugs-and-new-features) Coding is always fun, but there is a process and guidelines you need to follow.

[Confirming issues](#confirming-issues) This is an important part. We do need repro projects to confirm issues.

[Documentation](#documentation) Our documentation does always need more work. If you love writing, please help here.

[Licensing](#license) Please read this so you are aware of the licensing.

> [!NOTE]
> Anyone who contributes issues or pull requests will be credited in the release notes for the version in which the fix ships. See [an example](https://docs.nunit.org/articles/nunit/release-notes/framework.html).

---------

## Who can contribute

Anyone can contribute! And we really want new contributors, so join in!

We'd love to know who you are, so please indulge us by making your GitHub profile a little informative.

Many people have "funny" account names, which is absolutely fine. But if you do, please add a name that tells us a little more about you. It doesn't have to be your full name, just something that helps us know who we're talking to.

Next is your location or time zone. This helps us know when you're likely to be active, and often gives us a clue about which language you speak. All of this makes it easier for us to communicate with you.

Contributors come with very different backgrounds. Some have been on GitHub for years, while others are just getting started.

If you've been around for a while, your GitHub profile already tells us quite a lot. We'll often have a look and quickly get an idea of what kind of developer you are—frontend, backend, testing, QA, or perhaps even a CEO who's tired of CEO-ing and wants to do some real work between meetings!

If you're new to GitHub, we'd really appreciate a little more information, either in your profile or in your first comment. A short introduction goes a long way. If you're a student, tell us! If you're experimenting with open source or just looking for something interesting to work on, tell us that too. If you're an experienced developer but this is your first time on GitHub, we'd love to know. It helps us meet you where you are and provide the right kind of guidance.

Finally, if you work for a company—and especially if you're contributing because of your company's needs—it's useful to mention that in your profile or introduction as well.

A good profile helps build trust.

Here are a few examples of good GitHub profiles (including some of the NUnit maintainers). They're all different, but hopefully they give you the general idea.

- [Manfred](https://github.com/manfred-brands)
- [Terje](https://github.com/osiristerje)
- [Steven](https://github.com/stevenaw)

PS: *We will never discriminate against anyone based on who you are or why you want to contribute.*

## Use of AI and Prevention of AI Slop

AI is here to stay, and yes, we know you'll use it — we do too. The important part is **how** you use it. AI is a powerful tool when used wisely, and it can make you both more productive and a better contributor.

> [!IMPORTANT]
> We don't accept [AI Slop](https://arxiv.org/html/2603.27249v1): large amounts of AI-generated content that has not been properly reviewed, understood, or validated by the contributor.
>
> Contributions generated and submitted entirely by AI agents, without meaningful human review and ownership, are not accepted.
>
> **You are responsible for everything you submit**, regardless of whether it was written by you or generated with the help of AI.  Please don't create unnecessary work for the maintainers by submitting content that hasn't been properly reviewed, understood, or tested.
>
> Repeated violations may result in you being blocked from contributing.

### These are easy wins

- Use AI to improve spelling, grammar, and sentence structure.
- Use AI to analyze existing code and explain how it works.
- Use AI to help you understand an issue or come up with a plan before you start coding.
- Use AI to review your own work before submitting it.

### But be more careful with coding and documentation

- Use AI to write code, but keep the context small and generate small, focused changes. **You** should know what you want to achieve and **you** should drive the AI—not the other way around.
- Use AI to write unit tests, but keep in mind that you must explicitly steer it to test the **problem**, not just verify the fix it suggested.
- Use AI to help write documentation, but always read it carefully yourself and verify that it is actually correct.
- Treat AI-generated code as a first draft. Review it just as carefully as you would review code from another contributor.
- Don't submit code that you can't explain, debug, or maintain yourself.
- Avoid large AI-generated rewrites or broad refactorings unless they've first been discussed with a maintainer.
- AI can confidently provide incorrect explanations. Always verify technical details before including them in code comments, documentation, issues, or pull requests.

### AI works best when you stay in control

The best AI-assisted contributions are usually those where the contributor already understands the problem and uses AI to speed up the work—not to replace their own thinking.

**Think of AI as an assistant, not as the contributor.** It can help you work faster, but it should never replace your understanding, judgment, or responsibility.

Small, well-understood, carefully reviewed contributions are almost always better than large AI-generated ones.

Remember that maintainer time is one of the project's most valuable resources. Please help us spend it improving NUnit rather than reviewing AI-generated mistakes.

### Further reading

- LLVM's excellent
  [AI Tool Use Policy](https://llvm.org/docs/AIToolPolicy.html),
  which follows many of the same principles as ours.

## Submitting Issues

Requests for new features and bug reports are what keep the project moving forward.

### Before you submit an issue

- Ensure you are running the [latest version](https://github.com/nunit/nunit/releases) of NUnit.
- To make sure the bug is in NUnit and not in the runner you're using, **try reproducing it** using `dotnet test` first.
- **Search** the [issue list](https://github.com/nunit/nunit/issues?utf8=%E2%9C%93&q=is%3Aissue) (including closed issues) to make sure it hasn't already been reported. If you're unsure, go ahead and raise it anyway. Duplicate issues can still be valuable—they often provide additional information or another perspective.

### Submitting a good issue

- Give the issue a short, clear title that describes the bug or feature request.
- Tell us which version of NUnit you're using, which runner (Visual Studio, VS Code, Rider, CI/`dotnet test`, etc.), and its version.
- Include the target .NET version you're testing against.
- If you're using NUnit3TestAdapter, include its version.
- Tell us how you're running your tests, including any command-line arguments or `runsettings`.
- Include clear steps to reproduce the issue.
- Include a short code example that reproduces the issue. You can also attach a ZIP file containing a minimal repro project, or submit a PR to the [NUnit.Issues](https://github.com/nunit/nunit.issues) repository. Create a folder with a name like `Issue1234`, matching the number of your GitHub issue.
- Use [Markdown formatting](https://guides.github.com/features/mastering-markdown/) where appropriate to make the issue and code easier to read.
- Screenshots are always appreciated, where applicable.

## Confirming Issues

Before we work on an issue, we first need to confirm it and be able to reproduce it. Confirming issues takes up a significant amount of the team's time, so anything you can do to make that process easier is **really appreciated**.

If the issue reporter has included a repro, the job is much simpler. If not, we'd be very happy to receive a working repro project in our [NUnit.Issues](https://github.com/nunit/nunit.issues) repository, in a subfolder named `IssueXXXX`.

Issues that need confirmation will either have the **confirm** label or be unlabeled and have **no milestone**. You can help us confirm issues by:

- Adding steps to reproduce the issue.
- Creating unit tests that demonstrate the issue.
- Testing reported issues and providing feedback.

If you're getting ready to report a bug in NUnit, creating a minimal repro is often easier if you temporarily disable the debugger's [Just My Code](https://docs.microsoft.com/en-us/visualstudio/debugger/just-my-code) setting. This allows you to step into NUnit's source code, set breakpoints, inspect variables, and generally see what's happening under the hood.

## Documentation

Great documentation is essential for any open source project, and NUnit is no exception.

[Our documentation](https://docs.nunit.org/articles/nunit/intro.html) sometimes lags behind newly implemented features, and many pages could benefit from better examples or additional explanations.

If you spot anything that's missing, incorrect, or could be improved, we'd love your help. Just raise a PR to the documentation repository. Unlike the framework itself, documentation changes don't require an issue first—although you're always welcome to create one if you'd like to discuss the change.

In the PR description, please explain what you're correcting or adding.

## Raising Pull Requests for Bugs and New Features

We love pull requests, but we do need them to follow a few guidelines.

- Every pull request **must** refer to an issue. We don't accept pull requests for bugs or new features from external contributors unless there's an issue first.
- In the issue, outline how you plan to fix it and discuss your approach with a maintainer to make sure you're heading in the right direction.
- Before you start coding, please get confirmation from a maintainer to go ahead with the PR, or ask to have the issue assigned to you.
- Always include unit tests with your PR. Make sure you test the problem itself, not just the specific fix you've implemented.
- To help new contributors get their feet wet, we've marked a number of issues with the `good first issue` label. These are great places to start and are intended as learning opportunities. We encourage you to use AI to help you understand the problem or the code, but not to do the work for you. The goal is to learn how NUnit works, not just to get the issue closed.
- It's also a good idea to leave a comment on the issue to let everyone know you're working on it. If you decide to stop, please let us know. Taking a break is perfectly normal, but it gives someone else the opportunity to pick up the work.

Feel free to read through the [Developer Docs](https://docs.nunit.org/articles/developer-info/Team-Practices.html#technical-practices) before contributing to get familiar with our coding standards and development practices.

Always build the project locally before submitting a PR. If you've broken one of our coding standards, you should see a warning in your IDE or an error if building from command line. You can build on the command line using [our build script](BUILDING.md#script-build) via `dotnet build`.

When you're ready to contribute, instructions for building the project and running the tests can be found in [BUILDING.md](https://github.com/nunit/nunit/blob/master/BUILDING.md).

If that's TL;DR, here's the short version:

| Action  | Command                |
| ------- | ---------------------- |
| Build   | `build`                 |
| Test    | `build --target=Test`   |
| Package | `build --target=Package` |

Of course, you can also do all of this from your IDE, whether that's Visual Studio, Visual Studio Code, or Rider.

## License

NUnit is under the [MIT license](https://github.com/nunit/nunit/blob/master/LICENSE.txt). By contributing to NUnit, you assert that:

* The contribution is your own original work.
* You have the right to assign the copyright for the work (it is not owned by your employer, or you have been given copyright assignment in writing).
