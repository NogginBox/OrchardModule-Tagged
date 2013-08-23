using System;
using System.Linq;
using NogginBox.Tagged.Forms;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using Orchard.Tags.Models;

namespace NogginBox.Tagged.Filters
{
	[OrchardFeature("NogginBox.TagsInQuerystringFilter")]
	public class TagsInQuerystringFilter : IFilterProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TagsInQuerystringFilter(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Localizer T { get; set; }

		public void Describe(DescribeFilterContext describe) {
			describe.For("Tags", T("Tags"), T("Tags"))
				.Element("TagsInQuerystring", T("Has tags in querystring"), T("Dynamically match tags from the querystring. Usage: ?tags=x1,x2"),
					ApplyFilter,
					context => T("Tags match querystring tags"),
					TagMatchTypeForm.FormId
				);
		}

		public void ApplyFilter(FilterContext context)
		{
			var queryStringParams = _httpContextAccessor.Current().Request.QueryString;
			var tags = queryStringParams["Tags"];
			if (tags == null) return;

			var tagNames = tags.Split(',');
			if (!tagNames.Any()) return;
			
			Action<IAliasFactory> selector;
			Action<IHqlExpressionFactory> filter;

			// Match some or all of the querystring tags
			int op = Convert.ToInt32(context.State.Operator);
			switch (op)
			{
				case 0:
					// is one of
					selector = alias => alias.ContentPartRecord<TagsPartRecord>().Property("Tags", "tags").Property("TagRecord", "tagRecord");
					filter = x => x.InG("TagName", tagNames);
					context.Query.Where(selector, filter);
					break;
				case 1:
					// is all of
					foreach (var tag in tagNames)
					{
						var tagName = tag;
						selector = alias => alias.ContentPartRecord<TagsPartRecord>().Property("Tags", "tags"+tagName);
						filter = x => x.Eq("TagRecord.TagName", tagName);
						context.Query.Where(selector, filter);
					}
					break;
			}
		}
	}
}