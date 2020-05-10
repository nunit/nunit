# Contributions

This page contains guidelines to follow when evaluating and accepting contributions in form of source code from other developers.

## Shape

All proposed changes to the code should come in the form of GitHub pull requests. Pull requests are built automatically on the build server and they make it easy to track the origin of any change.

## Quality

When appropriate, pull requests should contain enough unit tests to provide coverage for the changes they introduce. Because pull requests are built automatically on the build server it is important to check that the changes pass all tests.

Ideally pull requests should contain only few commits which address directly the issue or feature.

## Integrating

There are several ways a pull request can be integrated into NUnit's repository:

* Pull requests can be merged automatically using GitHub's Web interface. This is an option only if they are  clean of useless commits which would only cause confusion and clutter the history for no good reason. Merging pull requests using GitHub Web interface creates a merge commit even when fast-forward would be possible.

* Pull requests can be merged manually once they've been checked out locally. Usually this would result in a fast-forward commit if no other changes appeared in the branch where the pull request is being integrated. In other cases you may still have the option to rebase the changes rather than merging. Although a linear history is nice, it's probably even better to force a merge instead, because it records more clearly where those changes came from and also because it automatically closes the pull request (although there may be GitHub hooks to close the pull request, perhaps mentioning its # in the commit message, which I'm not aware of).

Usually it is preferable to use GitHub's automatic merge, and if the pull request does not comply with the few commits rule ask the contributor to sanitize it before doing it yourself.

Avoid committing contributions by repeating the changes in the pull request manually, because this loses track of the origin of changes.

## Special reviews

Anyone submitting a PR can ask for extra review or review by a
particular person. Just say so in the PR indicating what needs special
review. If you want someone particular to review it, use @ notation.

By default, any PR is eligible to merge after it is reviewed as OK. If
anyone wants to create a PR early to get feedback on the code, then
he should say so right at the top of the PR.

## Cleaning up

After a pull request has been integrated remember to close associated issues, when appropriate and if such issues exist.

If a pull request has been integrated manually remember to close the pull request afterwards.

The branch of a pull request, in case it belongs to the NUnit's repository might be deleted as well. GitHub provides the option to do so in the Web user interface. Be careful before deleting a branch that the author may have intended the branch to be long-lived.

## Notes

For contributions from within the team, using branches on the NUnit repository rather than clones, don't forget to delete the branches once merged unless they need to hang around for long-running work
