using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.TagHelpers
{

    [HtmlTargetElement("div",Attributes ="page-model")]
    public class PageLinkTagHelper :TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PagingInfo PageModel { get; set; }
        public String  PageAction{ get; set; }
        public bool PageClassEnabled { get; set; }
        public String PageClass { get; set; }
        public String PageClassNormal { get; set; }
        public String PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper IUrlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i= 1 ;i <= PageModel.TotalPage; i++)
                {
                TagBuilder tag = new TagBuilder("a");
                String url = PageModel.urlParam.Replace(":" , i.ToString());
                tag.Attributes["href"] = url;
                if (PageClassEnabled) 
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i==PageModel.CurrentPage? PageClassSelected :PageClassNormal); 
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
