---
uid: nunitagent
---

The nunit-agent.exe program is used by other runners when the tests are being
run in a separate process. It is not intended for direct execution by users.

NUnit runs tests in a separate process by default.

When running under the Gui, NUnit will continue to use the same
process to reload tests so that it is not normally necessary to
re-attach to a new process. However, if the settings are changed
in a way that requires a different process - for example, by changing
the version of the runtime that is being used - the old process will
be terminated and a new one created. In that case, it's necessary
to re-attach to the new process.

#### Debugging

When debugging tests that are run in a separate process, it is
not possible to do so by simply running the console or gui runner
under the debugger. Rather, it is necessary to attach the debugger
to the nunit-agent process after the tests have been loaded.

