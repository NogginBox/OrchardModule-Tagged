using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Tags.Models;

namespace NogginBox.Tagged
{
	public static class WorkContextExtensionMethods
	{
		public static IList<TagsPart> GetTaggedContentForCurrentContent(this WorkContext workContext)
		{
			var contentItems = workContext.GetState<List<IContent>>("ContentItems");
			if (contentItems == null) return null;

			var taggedContent = contentItems.Where(c => c.As<TagsPart>() != null).ToList();

			return (taggedContent.Any())
				       ? taggedContent.Select(t => t.As<TagsPart>()).ToList()
				       : null;
		}
	}
}