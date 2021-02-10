Thanks for having interest to contribute to Faultify!
This book goes into some technical implementation details of this library.
Feel free to join the Discord server if you have any questions.

# IL-Inspectors
Those tools can be used to inspect IL-code.

- https://github.com/icsharpcode/ILSpy
- https://github.com/dnSpy/dnSpy (allows manual editing of IL)

# Testing
All mutations are tested in [Faultify.Tests](https://github.com/Faultify/Faultify/tree/main/Faultify.Tests). 
Code that is to be mutated has to be compiled such that this compiled assembly can be read by `Mono.Cecil`.
There is a folder [TestSource](https://github.com/Faultify/Faultify/tree/main/Faultify.Tests/UnitTests/TestSource) that contains 
various targets whom are compiled at run time by the unit tests. 

For new mutations:
- Add a test target or update an existing one.
- Write a unit test that loads this target and mutates it (see the existing unit tests)
