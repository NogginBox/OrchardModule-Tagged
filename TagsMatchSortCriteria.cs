using System;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Services;
using Orchard.Tags.Models;

namespace NogginBox.Tagged
{
	public class TagsMatchSortCriteria : ISortCriterionProvider
	{
		public Localizer T { get; set; }

		public void Describe(DescribeSortCriterionContext describe)
		{
			describe.For("General", T("General"), T("General sort criteria"))
				.Element("TagsMatch", T("Tags match"), T("Sorts the results by number of tag matches."),
						 context => ApplySortCriterion(context),
						 context => T("Tags match order")
				);
		}

		public void ApplySortCriterion(SortCriterionContext context) {
			bool ascending = (bool)context.State.Sort;

			// 
			Action<IAliasFactory> relationship = rel => rel.ContentItem();

			// apply sort
			context.Query = ascending
				? context.Query.OrderBy(relationship, x => x.Asc("Value"))
				: context.Query.OrderBy(relationship, x => x.Desc("Value"));
		}
	}
}