using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Tags.Models;
using Orchard.Widgets.Services;

namespace NogginBox.Tagged
{
	public class WithTagsRuleProvider : IRuleProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWorkContextAccessor _workContextAccessor;

		public WithTagsRuleProvider(IHttpContextAccessor httpContextAccessor, IWorkContextAccessor workContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_workContextAccessor = workContextAccessor;
		}

		public void Process(RuleContext ruleContext)
		{
			if (!String.Equals(ruleContext.FunctionName, "tagged", StringComparison.OrdinalIgnoreCase))
				return;

			var tag = Convert.ToString(ruleContext.Arguments[0]);
			var workContext = _workContextAccessor.GetContext();
			var contentItems = workContext.GetState<List<IContent>>("ContentItems");

			if (contentItems != null)
			{
				var taggedContent = contentItems.Where(c => c.As<TagsPart>() != null);

				if (taggedContent.Any(c => c.As<TagsPart>().CurrentTags.Any(t => t.TagName == tag)))
				{
					ruleContext.Result = true;
					return;
				}
			}

			ruleContext.Result = false;
		}
	}
}