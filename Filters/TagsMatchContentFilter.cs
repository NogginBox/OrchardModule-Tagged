using System;
using System.Linq;
using NogginBox.Tagged.Forms;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using Orchard.Tags.Models;

namespace NogginBox.Tagged.Filters
{
	[OrchardFeature("NogginBox.TagsMatchContentFilter")]
	public class TagsMatchContentFilter : IFilterProvider
	{
		private readonly IWorkContextAccessor _workContextAccessor;

		public TagsMatchContentFilter(IWorkContextAccessor workContextAccessor) {
			_workContextAccessor = workContextAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("Tags", T("Tags"), T("Tags"))
				.Element("TagsMatch", T("TagsMatch"), T("Tagged content items"),
					ApplyFilter,
					context => T("Tags match order"),
					TagMatchTypeForm.FormId
				);
		}

		public void ApplyFilter(FilterContext context) {
			var workContext = _workContextAccessor.GetContext();
			var tagParts = workContext.GetTaggedContentForCurrentContent();
			if (tagParts == null || ! tagParts.Any()) return;
			var tagIds = tagParts.SelectMany(t => t.CurrentTags).Select(t => t.Id).ToList();

			if (!tagIds.Any()) return;
				
			Action<IAliasFactory> selector = alias => alias.ContentPartRecord<TagsPartRecord>().Property("Tags", "tags").Property("TagRecord", "tagRecord");
			Action<IHqlExpressionFactory> filter = x => x.InG("Id", tagIds);
			context.Query.Where(selector, filter);
		}
	}
}