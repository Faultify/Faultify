Extracted Name and Description from IMutationAnalyzer into the new interface IReportable.
Refactored IMutationGrouping to implement IReportable instead of local implementation.
Removed IGrouping interface from IMutationGrouping, as it was not used, and reimplemented IEnumerable
Moved MutationGrouping shorthand subclasses into MutationGrouping.cs
