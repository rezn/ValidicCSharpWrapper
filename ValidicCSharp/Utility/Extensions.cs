﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValidicCSharp.Interfaces;
using ValidicCSharp.Model;
using ValidicCSharp.Request;
using SourceFilter = ValidicCSharp.Request.SourceFilter;

namespace ValidicCSharp.Utility
{
    public static class Extensions
    {
        public static ValidicResult<T> ToResult<T>(this string response, string fromString = null)
        {
            var parameter = typeof (T);

            if (fromString == null && parameter.Name == "List`1")
                fromString = parameter.GenericTypeArguments[0].Name.ToLower();

            var root = JObject.Parse(response);

            JObject summary = null;
            try
            {
                summary = JObject.FromObject(root["summary"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            string obj = null;
            try
            {
                var name = parameter.Name.ToLower();
                var element = root[name];
                if (element == null && fromString != null)
                    element = root[fromString];

                var aa = JObject.FromObject(element);
                obj = JsonConvert.SerializeObject(aa);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (obj == null && fromString != null)
            {
                try
                {
                    var element = root[fromString];
                    var array = JArray.FromObject(element);
                    obj = JsonConvert.SerializeObject(array);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            var tConverted = default(T);
            if (obj != null)
                tConverted = JsonConvert.DeserializeObject<T>(obj);

            var rootObject = new ValidicResult<T> {Object = tConverted};
            if (summary != null)
                rootObject.Summary = JsonConvert.DeserializeObject<Summary>(summary.ToString());
            ;
            return rootObject;
        }

        public static T Objectify<T>(this string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }

        public static T As<T>(this object o)
        {
            return (T) o;
        }

        public static Command GetInformationType(this Command command, CommandType type)
        {
            command.Type = type;
            return command;
        }

        public static Command AddSourceFilter(this Command command, string sourceToAdd)
        {
            command.Filters.AddSourceFilter(sourceToAdd);
            return command;
        }

        public static Command FromDate(this Command command, string date)
        {
            command.Filters.AddFromDateFilter(date);
            return command;
        }

        public static Command ToDate(this Command command, string date)
        {
            command.Filters.AddToDateFilter(date);
            return command;
        }

        public static List<ICommandFilter> AddSourceFilter(this List<ICommandFilter> filterList, string sourceToAdd)
        {
            var source = (SourceFilter) filterList.FirstOrDefault(a => a.Type == FilterType.Source);
            if (source == null)
            {
                source = new SourceFilter();
                filterList.Add(source);
            }
            source.AddSource(sourceToAdd);

            return filterList;
        }

        public static List<ICommandFilter> AddFromDateFilter(this List<ICommandFilter> filterList, string date)
        {
            var dateFilter = (FromDateFilter) filterList.FirstOrDefault(a => a.Type == FilterType.FromDate);

            if (dateFilter == null)
            {
                dateFilter = new FromDateFilter();
                filterList.Add(dateFilter);
            }
            dateFilter.AddDate(date);
            return filterList;
        }

        public static List<ICommandFilter> AddToDateFilter(this List<ICommandFilter> filterList, string date)
        {
            var dateFilter = (ToDateFilter) filterList.FirstOrDefault(a => a.Type == FilterType.ToDate);

            if (dateFilter == null)
            {
                dateFilter = new ToDateFilter();
                filterList.Add(dateFilter);
            }
            dateFilter.AddDate(date);
            return filterList;
        }
    }
}