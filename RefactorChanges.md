Extracted Name and Description from IMutationAnalyzer into the new interface IReportable.
Refactored IMutationGrouping to implement IReportable instead of local implementation.
Removed IGrouping interface from IMutationGrouping, as it was not used, and reimplemented IEnumerable
Moved MutationGrouping shorthand subclasses into MutationGrouping.cs
Changed signature of AnalyzeMutations (now GenerateMutations) for IMutationAnalyzer and all derivates
Updated TypeCollection: renamed to TypeChecker, enforced encapsulation and single responsability principle in affected classes.
Refactored RandomValueGenerator to be more consistent and extendable
Updated TypeCollection: renamed to TypeChecker, enforced encapsulation and single responsability principle in affected classes.
Restrucutred TestHostRunnerFactories to instead be one static factory for all the test host runners
Changed up the overuse of vars in the testproccess part of the code. Most of the vars have been turned into the proper defintion, but the obvious ones have been left as var
The previously ignored exceptions are now no longer ignored in CoverageRegistry.cs 
Renamed all XyzMutationAnalyzer to XyzAnalyzer
Moved all analyzers into their own shared namespace within Analyze
Added null pointer exception handling to isDynamicArray extension function
Added RandomValueGenerator instance to StringConstantAnalyzer, replacing the local randomizer implementation