﻿using System;

namespace DocaLabs.Http.Client.Mapping
{
    public static class UrlBuilder
    {
        public static Uri CreateUrl<TQuery>(Uri serviceUrl, TQuery query)
        {
            try
            {
                return new UriBuilder(serviceUrl)
                {
                    Query = TryMakeQueryString(query)
                }.Uri;
            }
            catch (Exception e)
            {
                throw new HttpClientException(string.Format(Resources.Text.failed_create_url, serviceUrl), e)
                {
                    DoNotRetry = true
                };
            }
        }

        static string TryMakeQueryString(object query)
        {
            if (query == null)
                return "";

            return query.GetType().GetCustomAttributes(typeof(QueryIgnoreAttribute), true).Length == 0
                ? QueryMapper.ToQueryString(query)
                : "";
        }
    }
}