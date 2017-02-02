using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class HtmlHelpers
    {
        public static string Truncate(this HtmlHelper helper, string input, int length)
        {
            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + "...";
            }
        }

        public static IHtmlString AssemblyVersion(this HtmlHelper helper)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return MvcHtmlString.Create(version);
        }

        public static MvcHtmlString DisplayWithBreaksFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var model = html.Encode(metadata.Model).Replace(Environment.NewLine, "<br />" + Environment.NewLine);

            if (String.IsNullOrEmpty(model))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }

        public static MvcHtmlString DisplayWithBreaks(this HtmlHelper helper, string input)
        {
            var model = helper.Encode(input).Replace(Environment.NewLine, "<br />");
            //model = model.Replace(Environment.NewLine, "<br />");

            if (String.IsNullOrEmpty(input))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }

        public static MvcHtmlString DisplayWithBreaksTruncated(this HtmlHelper helper, string input, int lines)
        {
            var model = helper.Encode(input).Replace(Environment.NewLine, "<br />");
            //model = model.Replace(Environment.NewLine, "<br />");
            //int start = 0;
            int ending = 0;
            for (int i = 0; i < lines; i++)
            {
                if (ending < model.Length)
                    ending = model.IndexOf("<br />", ending + 1);
            }

            if (ending < model.Length) model = model.Substring(0, ending);

            if (String.IsNullOrEmpty(input))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }
    }
}