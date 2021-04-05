Extracted Name and Description from IMutationAnalyzer into the new interface IReportable.
Refactored IMutationGrouping to implement IReportable instead of local implementation.
Removed IGrouping interface from IMutationGrouping, as it was not used, and reimplemented IEnumerable
Moved MutationGrouping shorthand subclasses into MutationGrouping.cs
Changed signature of AnalyzeMutations (now GenerateMutations) for IMutationAnalyzer and all derivates
Updated TypeCollection: renamed to TypeChecker, enforced encapsulation and single responsability principle in affected classes.