using System;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.Services;
using Orchard.Tags.Models;

namespace NogginBox.Tagged.SortCriteria
{
	[OrchardFeature("NogginBox.TagsMatchContentSortCriteria")]
	public class TagsMatchContentSortCriteria : ISortCriterionProvider
	{
		public Localizer T { get; set; }

		private readonly IWorkContextAccessor _workContextAccessor;

		public TagsMatchContentSortCriteria(IWorkContextAccessor workContextAccessor)
		{
			_workContextAccessor = workContextAccessor;
		}


		public void Describe(DescribeSortCriterionContext describe)
		{
			describe.For("General", T("General"), T("General sort criteria"))
				.Element("TagsMatch", T("Tags match"), T("Sorts the results by number of tag matches."),
						 context => ApplySortCriterion(context),
						 context => T("Tags match order")
				);
		}

		public void ApplySortCriterion(SortCriterionContext context)
		{
			var workContext = _workContextAccessor.GetContext();
			var tagParts = workContext.GetTaggedContentForCurrentContent();
			if (tagParts == null || ! tagParts.Any()) return;
			var tagIds = tagParts.SelectMany(t => t.CurrentTags).Select(t => t.Id);


			// Stuck : http://stackoverflow.com/questions/18317286/creating-an-orchard-query-sort-criteria-with-an-agregate-function

			// 
			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<TagsPartRecord>().Property("Tags", "tags").Property("TagRecord", "tagRecord");
			Action<IHqlSortFactory> filter = x => x.Asc("Id");

			// apply sort
			context.Query = context.Query.OrderBy(selector, filter);
		}
	}
}