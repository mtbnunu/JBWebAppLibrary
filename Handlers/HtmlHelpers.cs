using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JBWebappLibrary
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            var builder = new TagBuilder("li")
            {
                InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName).ToHtmlString()
            };

            if (controllerName == currentController && actionName == currentAction)
                builder.AddCssClass("active");

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString bsEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null, bool showLabel = false,
            SelectList selectList = null)
        {
            var data = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);


            var dtype = data.DataTypeName;
            if (dtype == "EmailAddress")
                dtype = "email";

            var dclass = "form-control";

            TagBuilder obj;

            if (selectList != null)
            {
                obj = new TagBuilder("select");
                var selected = false;
                foreach (var item in selectList)
                {
                    TagBuilder selItem = new TagBuilder("option");
                    selItem.Attributes.Add("value", item.Value);
                    selItem.InnerHtml = item.Text;
                    if (data.Model != null && item.Value.ToString() == data.Model.ToString())
                    {
                        selItem.Attributes.Add("selected", "selected");
                        selected = true;
                    }
                    obj.InnerHtml += selItem.ToString();
                }
                obj.InnerHtml = "<option disabled "+(selected?"":"selected")+">--Please Select--</option> " + obj.InnerHtml;
            }
            else if (dtype == "MultilineText")
            {
                obj = new TagBuilder("textarea");
                obj.Attributes.Add("placeholder", data.ShortDisplayName);
                if (data.Model != null)
                {
                    obj.InnerHtml = data.Model.ToString();
                }
            }
            else
            {
                obj = new TagBuilder("input");
                obj.Attributes.Add("type", dtype);
                obj.Attributes.Add("placeholder", data.ShortDisplayName);
                if (data.Model != null)
                {
                    obj.Attributes.Add("value", data.Model.ToString());
                }
            }

            obj.Attributes.Add("id", data.PropertyName);
            obj.Attributes.Add("name", data.PropertyName);
            if (data.ModelType.Name.Equals("Boolean"))
            {
                obj.Attributes["type"] = "checkbox";
                obj.Attributes.Add("onChange", "$(this).prop('value',$(this).prop('checked'))");

                if (data.Model != null && data.Model.Equals(true))
                {
                    obj.Attributes.Add("checked", "checked");
                }
            }
            else if (data.ModelType.Name.Equals("Byte[]"))
            {
                obj.Attributes["type"] = "file";
                if (!string.IsNullOrEmpty(data.Watermark))
                {
                    obj.Attributes["accept"] = data.Watermark;
                }
                obj.Attributes.Add("class", dclass);
            }
            else
            {
                obj.Attributes.Add("class", dclass);
            }

            if (data.IsRequired && !data.ModelType.Name.Equals("Boolean"))
            {
                obj.Attributes.Add("required","required");
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                obj.MergeAttributes(attributes);
            }

            TagBuilder lbl = new TagBuilder("label");
            lbl.Attributes.Add("for", data.PropertyName);
            lbl.InnerHtml = data.ShortDisplayName;
            if (!string.IsNullOrEmpty(data.Description))
            {
                lbl.InnerHtml += " <small class='text-muted'>" + data.Description + "</small>";
            }

            return new MvcHtmlString("<div class='form-group'>" + (showLabel ? lbl.ToString() : "") + obj.ToString() + "</label></div>");
        }

        public static MvcHtmlString bsValidationSummary(this HtmlHelper helper, string validationMessage = "")
        {
            string retVal = "";
            if (helper.ViewData.ModelState.IsValid)
                return new MvcHtmlString("");
            retVal += "<div class='alert alert-danger alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button><strong>";
            if (!String.IsNullOrEmpty(validationMessage))
                retVal += helper.Encode(validationMessage);
            retVal += "</strong>";
            retVal += "<ul>";
            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors)
                    retVal += "<li>" + helper.Encode(err.ErrorMessage) + "</li>";
            }
            retVal += "</ul></div>";
            return new MvcHtmlString(retVal);
        }

        public static MvcHtmlString DumpModel(this HtmlHelper htmlHelper, IEnumerable<Object> o, object htmlAttributes = null)
        {
            TagBuilder table = new TagBuilder("table");
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                table.MergeAttributes(attributes);
            }

            StringBuilder sb = new StringBuilder();
            var props = o.First().GetType().GetProperties();
            sb.Append("<thead><tr>");
            foreach (var prop in props)
            {
                sb.Append("<td>");
                sb.Append(prop.Name);
                sb.Append("</td>");
            }
            sb.Append("</tr></thead><tbody>");
            foreach (var item in o)
            {
                sb.Append("<tr>");
                foreach (var prop in props)
                {
                    sb.Append("<td>");
                    sb.Append(prop.GetValue(item));
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("tbody");
            table.InnerHtml = sb.ToString();
            return new MvcHtmlString(table.ToString());
        }
    }
}

