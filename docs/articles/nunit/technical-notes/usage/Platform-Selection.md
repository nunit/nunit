If your tests are compiled x86 or x64, NUnit will run each assembly
by default in a process targeted at the appropriate platform as well as
the targeted runtime. If you run multiple assemblies in the same process,
you may not mix x86 and x64 assemblies.

On an x64 machine, if your test assembly is compiled x86, you **must not**
use **/process=Single** on the command line. This is because NUnit is already
running in a 64-bit process and will  fail when it tries to load your assembly
into the same process.

If your test assembly references any x86 or x64 assemblies, it should be built
for the same platform. Consequently, you may not mix x86 and x64 references.

