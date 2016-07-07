﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Transforms;

namespace Pipeline.DotNetFiddle.Impl.Transforms {
    public class DateDiffTransform : BaseTransform, ITransform {

        public static readonly Dictionary<string, Func<DateTime, DateTime, object>> Parts = new Dictionary<string, Func<DateTime, DateTime, object>>() {
            {"day", (x,y) => (y-x).TotalDays},
            {"date", (x, y) => new DateTime((y-x).Ticks)},
            {"hour", (x,y)=>(y-x).TotalHours},
            {"millisecond", (x,y)=>(y-x).TotalMilliseconds},
            {"minute",(x,y)=>(y-x).TotalMinutes},
            {"second",(x,y)=>(y-x).TotalSeconds},
            {"tick",(x,y)=>(y-x).Ticks},
            {"month",(x,y)=>(y-x).TotalDays / (365/12.0) },
            { "year",(x,y)=>(y-x).TotalDays / 365 }
        };

        public static readonly Dictionary<string, string> PartReturns = new Dictionary<string, string>() {
            {"day", "double"},
            {"date", "date"},
            {"hour", "double"},
            {"millisecond", "double"},
            {"minute","double"},
            {"second","double"},
            {"tick","long"},
            {"year","double" },
            {"month","double" }
        };

        private readonly Field _start;
        private readonly Field _end;
        private readonly Action<IRow> _transform;

        public DateDiffTransform(IContext context) : base(context) {
            var input = MultipleInput().TakeWhile(f=>f.Type.StartsWith("date")).ToArray();

            _start = input[0];

            if (Context.Transform.TimeComponent.In("year", "month")) {
                Context.Warn("datediff can not determine exact years or months.  For months, it returns (days / (365/12.0)).  For years, it returns (days / 365).");
            }

            if (PartReturns.ContainsKey(context.Transform.TimeComponent)) {
                if (input.Count() > 1) {
                    // comparing between two dates in pipeline
                    _end = input[1];
                    if (Context.Field.Type == PartReturns[context.Transform.TimeComponent]) {
                        _transform = row => row[context.Field] = Parts[context.Transform.TimeComponent]((DateTime)row[_start], (DateTime)row[_end]);
                    } else {
                        _transform = row => row[context.Field] = context.Field.Convert(Parts[context.Transform.TimeComponent]((DateTime)row[_start], (DateTime)row[_end]));
                    }
                } else {
                    // comparing between one date in pipeline and now (depending on time zone)
                    var fromTimeZone = Context.Transform.FromTimeZone == Constants.DefaultSetting ? "UTC" : Context.Transform.FromTimeZone;
                    var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone);
                    if (Context.Field.Type == PartReturns[context.Transform.TimeComponent]) {
                        _transform = row => row[context.Field] = Parts[context.Transform.TimeComponent](now, (DateTime)row[_start]);
                    } else {
                        _transform = row => row[context.Field] = context.Field.Convert(Parts[context.Transform.TimeComponent](now, (DateTime)row[_start]));
                    }
                }

            } else {
                Context.Warn($"datediff does not support time component {Context.Transform.TimeComponent}.");
                var defaults = Constants.TypeDefaults();
                _transform = row => row[Context.Field] = Context.Field.Default != Constants.DefaultSetting ? Context.Field.Convert(Context.Field.Default) : defaults[Context.Field.Type];
            }

        }

        public IRow Transform(IRow row) {
            _transform(row);
            Increment();
            return row;
        }
    }
}